#!/usr/bin/env bash
set -euo pipefail

PROJECT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
GAME_DIR="${1:-${GAME_DIR:-}}"

if [[ -z "$GAME_DIR" ]]; then
  echo "Usage: ./build.sh /path/to/Idling\ to\ Rule\ the\ Gods" >&2
  echo "or:    GAME_DIR=/path/to/game ./build.sh" >&2
  exit 1
fi

if [[ ! -d "$GAME_DIR" ]]; then
  echo "Game directory not found: $GAME_DIR" >&2
  exit 1
fi

BEPINEX_CORE="$GAME_DIR/BepInEx/core"
if [[ ! -f "$BEPINEX_CORE/BepInEx.dll" || ! -f "$BEPINEX_CORE/0Harmony.dll" ]]; then
  echo "BepInEx core files not found in: $BEPINEX_CORE" >&2
  echo "Install BepInEx first and run the game once." >&2
  exit 1
fi

MANAGED_DIR=""
# Common names seen in Steam/Unity builds. Keep this list explicit first,
# then fall back to a generic *_Data/Managed search.
for candidate in \
  "$GAME_DIR/itrtg_Data/Managed" \
  "$GAME_DIR/Idling to Rule the Gods_Data/Managed" \
  "$GAME_DIR/IdlingToRuleTheGods_Data/Managed"
do
  if [[ -d "$candidate" ]]; then
    MANAGED_DIR="$candidate"
    break
  fi
done

if [[ -z "$MANAGED_DIR" ]]; then
  found="$(find "$GAME_DIR" -maxdepth 3 -type d -path '*_Data/Managed' | head -n 1 || true)"
  if [[ -n "$found" ]]; then
    MANAGED_DIR="$found"
  fi
fi

if [[ -z "$MANAGED_DIR" || ! -d "$MANAGED_DIR" ]]; then
  echo "Unity Managed directory not found under: $GAME_DIR" >&2
  exit 1
fi

REFS=(
  "-r:$BEPINEX_CORE/BepInEx.dll"
  "-r:$BEPINEX_CORE/0Harmony.dll"
)

# Important: some BepInEx/Unity combinations still need the old UnityEngine.dll
# facade, while newer Unity builds also need UnityEngine.CoreModule.dll and
# UnityEngine.ImageConversionModule.dll. Referencing all UnityEngine*.dll files
# is the most robust option for this small plugin.
shopt -s nullglob
UNITY_DLLS=("$MANAGED_DIR"/UnityEngine*.dll)
shopt -u nullglob

if [[ ${#UNITY_DLLS[@]} -eq 0 ]]; then
  echo "No UnityEngine*.dll files found in: $MANAGED_DIR" >&2
  exit 1
fi

for dll in "${UNITY_DLLS[@]}"; do
  REFS+=("-r:$dll")
done

# Newer Unity assemblies may target netstandard. mcs/csc then needs the local
# netstandard facade. Try several common Ubuntu/Mono locations.
NETSTANDARD_CANDIDATES=(
  "$MANAGED_DIR/netstandard.dll"
  "$MANAGED_DIR/Facades/netstandard.dll"
  "/usr/lib/mono/4.8-api/Facades/netstandard.dll"
  "/usr/lib/mono/4.7.2-api/Facades/netstandard.dll"
  "/usr/lib/mono/4.7.1-api/Facades/netstandard.dll"
  "/usr/lib/mono/4.7-api/Facades/netstandard.dll"
  "/usr/lib/mono/4.6.2-api/Facades/netstandard.dll"
  "/usr/lib/mono/4.6.1-api/Facades/netstandard.dll"
)

NETSTANDARD_DLL=""
for candidate in "${NETSTANDARD_CANDIDATES[@]}"; do
  if [[ -f "$candidate" ]]; then
    NETSTANDARD_DLL="$candidate"
    break
  fi
done

if [[ -z "$NETSTANDARD_DLL" && -d /usr/lib/mono ]]; then
  NETSTANDARD_DLL="$(find /usr/lib/mono -path '*/Facades/netstandard.dll' -print 2>/dev/null | sort -V | tail -n 1 || true)"
fi

if [[ -n "$NETSTANDARD_DLL" && -f "$NETSTANDARD_DLL" ]]; then
  REFS+=("-r:$NETSTANDARD_DLL")
else
  echo "WARNING: netstandard.dll was not found." >&2
  echo "If compilation fails with 'netstandard, Version=2.1.0.0', install Mono reference assemblies:" >&2
  echo "  sudo apt update && sudo apt install mono-complete" >&2
fi

COMPILER=""
if command -v mcs >/dev/null 2>&1; then
  COMPILER="mcs"
elif command -v csc >/dev/null 2>&1; then
  COMPILER="csc"
else
  echo "No C# compiler found. Install Mono first:" >&2
  echo "  sudo apt update && sudo apt install mono-complete" >&2
  exit 1
fi

OUT_DIR="$PROJECT_DIR/ModPackage/BepInEx/plugins"
OUT_DLL="$OUT_DIR/ITRTGTextureOverrideMod.dll"
mkdir -p "$OUT_DIR"

rm -f "$OUT_DLL"

echo "Project dir: $PROJECT_DIR"
echo "Game dir:    $GAME_DIR"
echo "Managed dir: $MANAGED_DIR"
echo "Compiler:    $COMPILER"
if [[ -n "$NETSTANDARD_DLL" ]]; then
  echo "netstandard: $NETSTANDARD_DLL"
fi
echo "Unity refs:  ${#UNITY_DLLS[@]} UnityEngine*.dll file(s)"

"$COMPILER" \
  -target:library \
  -langversion:latest \
  -out:"$OUT_DLL" \
  "${REFS[@]}" \
  "$PROJECT_DIR/src/ITRTGTextureOverrideMod.cs"

echo "Built: $OUT_DLL"
echo "Now install with: ./install_to_game.sh '$GAME_DIR'"
