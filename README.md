# ITRTG Vegan Texture Mod

A small BepInEx plugin for **Idling to Rule the Gods** that replaces selected in-game images with vegan / raw-vegan themed alternatives.

The mod does not modify save files, gameplay balance, or the original game assets on disk. It replaces textures at runtime when the game loads them through Unity `Resources.Load`.

## What is changed

Current replacements:

| Original asset | New concept |
|---|---|
| `food_puny` | Apple and banana |
| `food_strong` | Raw vegan superfood bowl |
| `food_mighty` | Raw vegan energy bars |
| `rebirthbacon` | Magical Rebirth Root |
| `fsm` | Zucchini noodle monster |
| `fsm_evo` | Evolved zucchini noodle monster |
| `stick_fsm` | Sketch version of the zucchini noodle monster |

## For players: installing a ready-made release archive

If you downloaded a ready-made release ZIP, you do **not** need to build the project.

A ready-made release ZIP should contain a structure like this:

```text
BepInEx/
  plugins/
    ITRTGTextureOverrideMod.dll
    ITRTGTextureOverrides/
      texture_overrides.tsv
      Textures/
        food_puny.png
        food_strong.png
        food_mighty.png
        rebirthbacon.png
        fsm.png
        fsm_evo.png
        stick_fsm.png
```

You need to install BepInEx first, then extract the release ZIP into the game folder.

### Windows

Use this section if you run the native Windows version of the game.

1. Download **BepInEx 5 x64** for Windows from the BepInEx releases page. https://github.com/BepInEx/BepInEx/releases
2. Open the game folder in Steam:

   ```text
   Steam Library -> Idling to Rule the Gods -> Manage -> Browse local files
   ```

3. Extract BepInEx into the game folder.
4. Start the game once and close it. This should create:

   ```text
   BepInEx/LogOutput.log
   BepInEx/plugins/
   ```

5. Extract the `ITRTG_Vegan_Texture_Mod.zip` release archive into the game folder.
6. Start the game normally from Steam.

Expected final structure:

```text
Idling to Rule the Gods/
  BepInEx/
    plugins/
      ITRTGTextureOverrideMod.dll
      ITRTGTextureOverrides/
        texture_overrides.tsv
        Textures/
          food_puny.png
          food_strong.png
          food_mighty.png
          rebirthbacon.png
          fsm.png
          fsm_evo.png
          stick_fsm.png
```

More detailed Windows notes are in [INSTALL_WINDOWS.md](INSTALL_WINDOWS.md).

### Linux / Ubuntu

Use this section if you run the native Linux version of the game.

1. Download **BepInEx 5 Unix** from the BepInEx releases page.
2. Open the game folder. Common Steam locations are:

   ```bash
   ~/.steam/debian-installation/steamapps/common/Idling\ to\ Rule\ the\ Gods
   ~/.local/share/Steam/steamapps/common/Idling\ to\ Rule\ the\ Gods
   ```

3. Extract BepInEx into the game folder.
4. Make the BepInEx launcher executable:

   ```bash
   chmod +x run_bepinex.sh
   ```

5. Edit `run_bepinex.sh` and set `executable_name` to the real game executable. For example:

   ```bash
   executable_name="itrtg.x86_64"
   ```

6. In Steam launch options, set:

   ```bash
   ./run_bepinex.sh %command%
   ```

7. Start the game once and close it.
8. Extract the `ITRTG_Vegan_Texture_Mod.zip` release archive into the game folder.
9. Start the game from Steam.

More detailed Linux notes are in [INSTALL_LINUX.md](INSTALL_LINUX.md).

## Checking whether the mod loaded

Open:

```text
BepInEx/LogOutput.log
```

Look for lines similar to:

```text
ITRTG Vegan Texture Overrides loaded
Texture override entries loaded
```

If the log exists and these lines are present, the plugin loaded successfully.

## Uninstalling

Delete:

```text
BepInEx/plugins/ITRTGTextureOverrideMod.dll
BepInEx/plugins/ITRTGTextureOverrides/
```

To temporarily disable the plugin, rename:

```text
ITRTGTextureOverrideMod.dll
```

to:

```text
ITRTGTextureOverrideMod.dll.disabled
```

## For developers: building from source

The project is a single C# BepInEx plugin.

Basic Linux build:

```bash
./build.sh "/path/to/Steam/steamapps/common/Idling to Rule the Gods"
```

Then install into the game folder:

```bash
./install_to_game.sh "/path/to/Steam/steamapps/common/Idling to Rule the Gods"
```

See [BUILDING.md](BUILDING.md) for detailed build instructions and [PACKAGING.md](PACKAGING.md) for release packaging instructions.

## Repository layout

```text
src/
  ITRTGTextureOverrideMod.cs

ModPackage/
  BepInEx/
    plugins/
      ITRTGTextureOverrides/
        texture_overrides.tsv
        Textures/
          *.png

build.sh
install_to_game.sh
README.md
INSTALL_WINDOWS.md
INSTALL_LINUX.md
BUILDING.md
PACKAGING.md
TROUBLESHOOTING.md
```

## How the mod works

The plugin patches Unity resource loading and checks whether a loaded texture path or asset name has an override in `texture_overrides.tsv`.

If an override exists, the plugin loads the PNG from:

```text
BepInEx/plugins/ITRTGTextureOverrides/Textures/
```

and returns it instead of the original texture.

The game code is not modified. The game asset files are not repacked. This makes the mod less sensitive to C# obfuscation changes after game updates.

## Compatibility notes

This mod was designed around the Unity Mono / BepInEx 5 setup used by the tested build of Idling to Rule the Gods.

The mod may need updates if:

- the game switches to a different Unity runtime setup;
- resource paths are renamed;
- the target images are no longer loaded through Unity `Resources.Load`;
- BepInEx is not loading correctly on the user's platform.

## License and asset note

Add your preferred license before publishing the repository. The code and replacement images should be licensed separately if you want different terms for source code and art assets.
