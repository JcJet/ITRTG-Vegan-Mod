# Troubleshooting

## Images did not change

Most likely causes:

1. BepInEx is not installed.
2. The game is not being launched through BepInEx.
3. The mod was extracted into the wrong folder.
4. The plugin DLL is missing.
5. The texture override paths no longer match the current game version.

Check:

```text
BepInEx/LogOutput.log
```

If this file does not exist, BepInEx is not loading.

## `BepInEx/LogOutput.log` does not exist

BepInEx did not start.

### Windows

Make sure the BepInEx files were extracted into the same folder as the game executable.

Expected files:

```text
BepInEx/
winhttp.dll
doorstop_config.ini
```

### Linux

Make sure Steam launch options contain:

```bash
./run_bepinex.sh %command%
```

Also check:

```bash
chmod +x run_bepinex.sh
```

and verify that `executable_name` inside `run_bepinex.sh` is correct.

## The plugin does not appear in the log

Check that this file exists:

```text
BepInEx/plugins/ITRTGTextureOverrideMod.dll
```

Also check that it is not disabled:

```text
ITRTGTextureOverrideMod.dll.disabled
```

If it is disabled, rename it back to:

```text
ITRTGTextureOverrideMod.dll
```

## The plugin loads, but some images are still original

Open:

```text
BepInEx/plugins/ITRTGTextureOverrides/texture_overrides.tsv
```

The relevant asset path may be missing or renamed in the game.

If logging is enabled in the plugin, search `LogOutput.log` for resource paths related to the target asset names.

Then add another mapping line to `texture_overrides.tsv`.

Format:

```text
resource_path_or_asset_name<TAB>texture_file<TAB>pivot_x<TAB>pivot_y<TAB>pixels_per_unit
```

Example:

```text
Gui/pets/fsm	fsm.png	0.5022521	0.48583892	100
```

Use tabs, not spaces, between columns.

## The mod was extracted into an extra folder

Wrong:

```text
Idling to Rule the Gods/ITRTG_Vegan_Texture_Mod/BepInEx/plugins/...
```

Correct:

```text
Idling to Rule the Gods/BepInEx/plugins/...
```

Move the `BepInEx` folder from the extra nested folder into the game folder.

## The game updated and the mod stopped working

Try:

1. Start the game once without changing anything else.
2. Check `BepInEx/LogOutput.log`.
3. Reinstall the mod release ZIP.
4. Check whether the game still uses the same resource names.

The mod is designed to avoid direct dependency on obfuscated class names, but it still depends on resource paths or asset names.

## How to reset to the original game images

Delete:

```text
BepInEx/plugins/ITRTGTextureOverrideMod.dll
BepInEx/plugins/ITRTGTextureOverrides/
```

The original game asset files were not modified, so removing the plugin restores the original textures.
