# Windows installation guide

This guide is for users who downloaded a ready-made release archive of **ITRTG Vegan Texture Mod**.

You do not need to compile anything.

## 1. Find the game folder

In Steam:

```text
Library -> Idling to Rule the Gods -> Manage -> Browse local files
```

This opens the game folder.

## 2. Install BepInEx

Download **BepInEx 5 x64 for Windows** from the official BepInEx releases page.

Extract the contents of the BepInEx archive directly into the game folder.

After extraction, the game folder should contain files and folders similar to:

```text
BepInEx/
winhttp.dll
doorstop_config.ini
```

## 3. Start the game once

Start the game normally from Steam, then close it.

This allows BepInEx to initialize its folders.

After the first launch, you should see:

```text
BepInEx/LogOutput.log
BepInEx/plugins/
```

## 4. Install the mod

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

## 5. Start the game

Start the game normally from Steam.

If BepInEx is installed correctly, the mod should load automatically.

## 6. Verify the mod loaded

Open:

```text
BepInEx/LogOutput.log
```

Look for lines similar to:

```text
ITRTG Vegan Texture Overrides loaded
Texture override entries loaded
```

## Updating the mod

To update the mod, overwrite the old files with the new release archive.

Usually only these files change:

```text
BepInEx/plugins/ITRTGTextureOverrideMod.dll
BepInEx/plugins/ITRTGTextureOverrides/
```

## Uninstalling

Delete:

```text
BepInEx/plugins/ITRTGTextureOverrideMod.dll
BepInEx/plugins/ITRTGTextureOverrides/
```

## Temporarily disabling the mod

Rename:

```text
BepInEx/plugins/ITRTGTextureOverrideMod.dll
```

to:

```text
BepInEx/plugins/ITRTGTextureOverrideMod.dll.disabled
```

Rename it back to enable the mod again.

## Common problems

### The game starts, but images are unchanged

Check that the DLL is here:

```text
BepInEx/plugins/ITRTGTextureOverrideMod.dll
```

Also check `BepInEx/LogOutput.log`.

### There is no `LogOutput.log`

BepInEx probably did not load. Re-check the BepInEx installation and make sure the files were extracted into the same folder as the game executable.

### The mod was extracted into an extra folder

Wrong:

```text
Idling to Rule the Gods/ITRTG_Vegan_Texture_Mod/BepInEx/plugins/...
```

Correct:

```text
Idling to Rule the Gods/BepInEx/plugins/...
```
