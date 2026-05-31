# Building from source

This document is for developers who want to build `ITRTGTextureOverrideMod.dll` from source.

Most users should download a ready-made release archive instead.

## Requirements

You need:

- Idling to Rule the Gods installed locally;
- BepInEx 5 installed into the game folder;
- a C# compiler;
- access to the game's local Unity assemblies.

The project references DLLs from two local folders:

```text
<GameDir>/BepInEx/core/
<GameDir>/itrtg_Data/Managed/
```

The game folder name may differ slightly depending on platform/build, but the important part is the Unity `*_Data/Managed` folder.

Do not commit or redistribute Unity game assemblies, `BepInEx.dll`, or `0Harmony.dll` in this repository.

## Linux / Ubuntu build

Install Mono:

```bash
sudo apt update
sudo apt install mono-complete
```

Check that `mcs` is available:

```bash
mcs --version
```

Build:

```bash
./build.sh "/path/to/Steam/steamapps/common/Idling to Rule the Gods"
```

Example:

```bash
./build.sh "$HOME/.steam/debian-installation/steamapps/common/Idling to Rule the Gods"
```

If successful, the output is:

```text
ModPackage/BepInEx/plugins/ITRTGTextureOverrideMod.dll
```

Install into the game folder:

```bash
./install_to_game.sh "/path/to/Steam/steamapps/common/Idling to Rule the Gods"
```

## Windows build

The repository currently includes a Linux-oriented `build.sh`, but the plugin can also be built on Windows with Visual Studio, Rider, or another C# compiler.

Create a Class Library project targeting a compatible .NET Framework profile used by BepInEx 5 plugins, then reference:

```text
<GameDir>/BepInEx/core/BepInEx.dll
<GameDir>/BepInEx/core/0Harmony.dll
<GameDir>/<GameDataFolder>/Managed/UnityEngine.dll
<GameDir>/<GameDataFolder>/Managed/UnityEngine.CoreModule.dll
<GameDir>/<GameDataFolder>/Managed/UnityEngine.ImageConversionModule.dll
```

Depending on the Unity version, you may need additional `UnityEngine.*.dll` references.

The source file is:

```text
src/ITRTGTextureOverrideMod.cs
```

Build the DLL and place it here:

```text
ModPackage/BepInEx/plugins/ITRTGTextureOverrideMod.dll
```

Then copy the contents of `ModPackage` into the game folder.

## Build script behavior

`build.sh` tries to detect:

- the Unity managed folder;
- `BepInEx.dll`;
- `0Harmony.dll`;
- `netstandard.dll`;
- all available `UnityEngine*.dll` assemblies.

It then compiles:

```text
src/ITRTGTextureOverrideMod.cs
```

into:

```text
ModPackage/BepInEx/plugins/ITRTGTextureOverrideMod.dll
```

## Common build errors

### `UnityEngine.MonoBehaviour is defined in an assembly that is not referenced`

The compiler did not receive enough Unity references. Make sure the build script can find the game's `Managed` folder and that `UnityEngine.dll` / `UnityEngine.CoreModule.dll` are present.

### `netstandard, Version=2.1.0.0 is not referenced`

Install `mono-complete`, or make sure the build script can find `netstandard.dll` either in the game `Managed` folder or in Mono's facade assemblies.

### `BepInEx.dll not found`

Install BepInEx into the game folder first. The build script expects:

```text
<GameDir>/BepInEx/core/BepInEx.dll
```

### `0Harmony.dll not found`

Install BepInEx into the game folder first. The build script expects:

```text
<GameDir>/BepInEx/core/0Harmony.dll
```

## After building

Start the game through BepInEx and check:

```text
BepInEx/LogOutput.log
```

Expected plugin log lines:

```text
ITRTG Vegan Texture Overrides loaded
Texture override entries loaded
```
