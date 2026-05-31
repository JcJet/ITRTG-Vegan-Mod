# Linux / Ubuntu installation guide

This guide is for users who downloaded a ready-made release archive of **ITRTG Vegan Texture Mod**.

You do not need to compile anything.

## 1. Find the game folder

Common Steam locations:

```bash
~/.steam/debian-installation/steamapps/common/Idling\ to\ Rule\ the\ Gods
~/.local/share/Steam/steamapps/common/Idling\ to\ Rule\ the\ Gods
```

You can also use Steam:

```text
Library -> Idling to Rule the Gods -> Manage -> Browse local files
```

For the examples below, replace this path with your actual game folder:

```bash
GAME_DIR="$HOME/.steam/debian-installation/steamapps/common/Idling to Rule the Gods"
```

## 2. Install BepInEx

Download **BepInEx 5 Unix** from the official BepInEx releases page.

Extract the archive into the game folder.

After extraction, the folder should contain something like:

```text
BepInEx/
doorstop_config.ini
run_bepinex.sh
```

## 3. Make the launcher executable

```bash
cd "$GAME_DIR"
chmod +x run_bepinex.sh
```

## 4. Set the game executable name

Open `run_bepinex.sh` in a text editor:

```bash
nano "$GAME_DIR/run_bepinex.sh"
```

Find:

```bash
executable_name=""
```

Set it to the actual game executable.

To list files in the game folder:

```bash
ls "$GAME_DIR"
```

For Idling to Rule the Gods, it may be something like:

```bash
executable_name="itrtg.x86_64"
```

or:

```bash
executable_name="itrtg"
```

Save the file.

## 5. Configure Steam launch options

In Steam:

```text
Library -> Idling to Rule the Gods -> Properties -> Launch Options
```

Set:

```bash
./run_bepinex.sh %command%
```

This makes the normal Steam Play button launch the game through BepInEx.

## 6. Start the game once

Start the game from Steam, then close it.

This should create:

```text
BepInEx/LogOutput.log
BepInEx/plugins/
```

## 7. Install the mod

Extract the mod release archive into the game folder.

The final structure should be:

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

Command-line example:

```bash
cd "$GAME_DIR"
unzip ~/Downloads/ITRTG_Vegan_Texture_Mod.zip
```

## 8. Start the game

Start the game from Steam.

The mod should load automatically.

## 9. Verify the mod loaded

Open:

```bash
less "$GAME_DIR/BepInEx/LogOutput.log"
```

Look for lines similar to:

```text
ITRTG Vegan Texture Overrides loaded
Texture override entries loaded
```

## Updating the mod

Extract the new release archive into the game folder and overwrite existing files.

## Uninstalling

```bash
rm -f "$GAME_DIR/BepInEx/plugins/ITRTGTextureOverrideMod.dll"
rm -rf "$GAME_DIR/BepInEx/plugins/ITRTGTextureOverrides"
```

## Temporarily disabling the mod

```bash
mv "$GAME_DIR/BepInEx/plugins/ITRTGTextureOverrideMod.dll" \
   "$GAME_DIR/BepInEx/plugins/ITRTGTextureOverrideMod.dll.disabled"
```

To enable it again:

```bash
mv "$GAME_DIR/BepInEx/plugins/ITRTGTextureOverrideMod.dll.disabled" \
   "$GAME_DIR/BepInEx/plugins/ITRTGTextureOverrideMod.dll"
```

## Common problems

### The game starts, but images are unchanged

The game may not be launching through BepInEx. Check Steam Launch Options:

```bash
./run_bepinex.sh %command%
```

Also check:

```bash
ls "$GAME_DIR/BepInEx/plugins"
```

### There is no `BepInEx/LogOutput.log`

BepInEx did not load. Check:

- `run_bepinex.sh` is executable;
- `executable_name` is correct;
- Steam Launch Options are set correctly;
- BepInEx files were extracted into the game folder, not into a nested subfolder.

### The mod was extracted into an extra folder

Wrong:

```text
Idling to Rule the Gods/ITRTG_Vegan_Texture_Mod/BepInEx/plugins/...
```

Correct:

```text
Idling to Rule the Gods/BepInEx/plugins/...
```
