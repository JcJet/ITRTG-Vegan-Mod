#!/usr/bin/env bash
set -euo pipefail

PROJECT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
GAME_DIR="${1:-${GAME_DIR:-}}"

if [[ -z "$GAME_DIR" ]]; then
  echo "Usage: ./install_to_game.sh /path/to/Idling\ to\ Rule\ the\ Gods" >&2
  exit 1
fi

if [[ ! -d "$GAME_DIR" ]]; then
  echo "Game directory not found: $GAME_DIR" >&2
  exit 1
fi

if [[ ! -f "$PROJECT_DIR/ModPackage/BepInEx/plugins/ITRTGTextureOverrideMod.dll" ]]; then
  echo "Plugin DLL not found. Run ./build.sh first." >&2
  exit 1
fi

mkdir -p "$GAME_DIR/BepInEx/plugins"
cp -r "$PROJECT_DIR/ModPackage/BepInEx/plugins/ITRTGTextureOverrideMod.dll" "$GAME_DIR/BepInEx/plugins/"
cp -r "$PROJECT_DIR/ModPackage/BepInEx/plugins/ITRTGTextureOverrides" "$GAME_DIR/BepInEx/plugins/"

echo "Installed plugin and textures into: $GAME_DIR/BepInEx/plugins"
echo "Start the game through BepInEx. Check BepInEx/LogOutput.log if something does not load."
