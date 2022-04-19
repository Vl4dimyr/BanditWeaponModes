# BanditWeaponModes

## Description

This mod allows you to choose between 3 firing modes for the bandit's primary weapon: `Normal`, `Spam` and `DoubleDoubleTap`.
With `Spam` and `DoubleDoubleTap` mode you can just hold down the fire key and don´t have to spam it.

The modes can be selected using the number keys `1`, `2` and `3`, or cycled through using the `mouse wheel` or [DPad](https://en.wikipedia.org/wiki/D-pad).
The current mode is displayed above the primary skill icon.

## Modes

| Mode            | Key | Description | Screenshot |
|-----------------|-----|-------------|------------|
| Normal          |  1  | The default mode/behavior of the bandit's primary weapon. | ![normal](https://raw.githubusercontent.com/Vl4dimyr/BanditWeaponModes/master/images/sc_normal.jpg)
| Spam            |  2  | Automatically fires all shots as long as the fire key is pressed. | ![spam](https://raw.githubusercontent.com/Vl4dimyr/BanditWeaponModes/master/images/sc_spam.jpg)
| DoubleDoubleTap |  3  | Automatically double taps twice as long as the fire key is pressed. | ![double_double_tap](https://raw.githubusercontent.com/Vl4dimyr/BanditWeaponModes/master/images/sc_double_double_tap.jpg)

### Cycle through modes

| Direction | Actions                                 |
|-----------|-----------------------------------------|
| Forward   | Mouse Wheel Down, DPad Right, DPad Down |
| Backward  | Mouse Wheel Up, DPad Left, DPad Up      |

> Currently this is only tested with a xbox 360 controller (xbox one should work too).

## Config

### TL;DR

Use [Risk Of Options](https://thunderstore.io/package/Rune580/Risk_Of_Options/) for in-game settings!

![Risk Of Options Screenshot](https://raw.githubusercontent.com/Vl4dimyr/BanditWeaponModes/master/images/risk_of_options.jpg)

### Manual Config

The config file (`\BepInEx\config\de.userstorm.banditweaponmodes.cfg`) will be crated automatically when the mod is loaded.
You need to restart the game for changes to apply in game.

#### Example config

The example config keeps only mode selection with number keys enabled and sets the default mode to `DoubleDoubleTap`.

```ini
## Settings file was created by plugin BanditWeaponModes v1.0.0
## Plugin GUID: de.userstorm.banditweaponmodes

[Settings]

## The mode that is selected on game start. Modes: Normal, Spam, DoubleDoubleTap
# Setting type: String
# Default value: Normal
DefaultMode = DoubleDoubleTap

## When set to true modes can be selected using the number keys
# Setting type: Boolean
# Default value: true
EnableModeSelectionWithNumberKeys = true

## When set to true modes can be cycled through using the mouse wheel
# Setting type: Boolean
# Default value: true
EnableModeSelectionWithMouseWheel = false

## When set to true modes can be cycled through using the DPad (controller)
# Setting type: Boolean
# Default value: true
EnableModeSelectionWithDPad = false

## The delay (in milliseconds) between the 2nd and 3rd shot when using the DoubleDoubleTap Mode
# Setting type: Int32
# Default value: 400
DelayBetweenDoubleDoubleTaps = 400

## The delay (in milliseconds) between the shots
# Setting type: Int32
# Default value: 125
DelayBetweenShots = 125
```

## Manual Install

- Install [BepInEx](https://thunderstore.io/package/bbepis/BepInExPack/) and [R2API](https://thunderstore.io/package/tristanmcpherson/R2API/)
- Download the latest `BanditWeaponModes_x.y.z.zip` [here](https://thunderstore.io/package/Vl4dimyr/BanditWeaponModes/)
- Extract and move the `BanditWeaponModes.dll` into the `\BepInEx\plugins` folder
- (optional) Install [Risk Of Options](https://thunderstore.io/package/Rune580/Risk_Of_Options/)

## Changelog

The [Changelog](https://github.com/Vl4dimyr/BanditWeaponModes/blob/master/CHANGELOG.md) can be found on GitHub.

## Bugs/Feedback

For bugs or feedback please use [GitHub Issues](https://github.com/Vl4dimyr/BanditWeaponModes/issues).

## Help me out

[![Patreon](https://cdn.iconscout.com/icon/free/png-64/patreon-2752105-2284922.png)](https://www.patreon.com/vl4dimyr)

It is by no means required, but if you would like to support me head over to [my Patreon](https://www.patreon.com/vl4dimyr).
