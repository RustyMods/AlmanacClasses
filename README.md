# Almanac Classes
Plugin adds a node based progression system that unlocks through experience gained by killing monsters and playing the game.

Still in development, please report bugs and issues to me at OdinPlus while I keep balancing system.

0.4.0 - Complete overhaul, delete old configs.

## Highlights
Unique talent abilities that are either passive or activated spells

Some noteworthy abilities:

- Monkey Wrench - Allows to wield two handed weapons in one hand (only applies to longswords, battleaxes and fireball staff)
- Dual Wield - You can wield different types of weapons together
- Bleeding - Stacked bleeding damaging ability
- Creature Mask - Spawn a friendly creature - Automatically dies after a certain time
- Shaman Heal - Heals nearby players
- Song of Healing - Heals nearby players over time

And many more abilities

## Prestige System

Class system has a prestige system. You can prestige after you've met the threshold (configurable)
Prestige each talent individually to increase their utility

## Classes
- Bard
- Shaman
- Sage
- Ranger
- Rogue
- Warrior

## Experience
Experience system rewards player whenever a creature is killed in the surrounding area. It does not matter who kills creature, all players will be rewarded.
Players also gain experience by:
- Chopping Trees
- Chopping Logs
- Hitting Ore Deposits
- Taming Creatures
- Picking up Items

Creature reward different experience, you can configure this using the generated YML file found:
- BepinEx/config/AlmanacClasses/Experience/AlmanacExperienceMap.yml

If a creature is not registered to this file, then the experienced gained will be based on the biome the player is in.

## Configurations
Plugin is fully configurable using the cfg file generated in BepinEx/config directory:
- RustyMods.AlmanacClasses.cfg

## Server
AlmanacExperienceMap.yml is synced through server for admins to configure.

## Roadmap
- Add custom animations

## Commands
- class_talents
- talents_write
- talents_test
Each command has a help function that details the available tools

## Changelog
```
0.2.0 - Beta release
0.2.1 - Changed air jump button to get game settings jump button instead of spacebar, fixed some localization
0.2.2 - Almanac can reward class experience
0.2.3 - Spell keys improvement
0.2.4 - Modified Quick Shot to be an increase in percentage of draw speed or reload time (fixed crossbow not being affected) - added config to turn off visual effects for some
0.2.5 - Config cooldowns go up to 1000 and cooldown grayscales icon with radial fill
0.2.6 - Added API for other mods to add experience, Tweaked experience monster kill, duration configs extended
0.2.7 - Added experience orbs and another tweak at making experience share multiplayer
0.2.8 - Shader/Material Fixes
0.2.9 - Added more API functionalities
0.2.10- Updated material replacer
0.2.11- Added Survivor and Battle Fury abilities (alternative for dual wield & monkey wrench)
0.2.12- Fixed Master Chef and tweaked materials of altar
0.3.0 - Battle Fury near range only and fixed quick shot
0.3.1 - Fixed prestige not updating characteristic values
0.3.2 - Updated thunderstore icon
0.3.3 - Ashland release
0.3.4 - piece manager update
0.3.5 - fixed ui issue already in spellbook
0.3.6 - fixed strength not adding carry weight and added max level config
0.3.7 - tweaked lightning spell and added better dual wield animations
0.3.8 - fixed duplicate spells in book
0.3.9 - Fixed spellbook bug
0.4.0 - prestige system overhaul and major changes all around
0.4.1 - Fixed damage reduction on dual wield and monkey wrench, tweaked icons, added ashland monsters to xp map
0.4.2 - Added speed animation modifier and talent prestige cap
```

![](https://i.imgur.com/F1w6ijU.png)
![](https://i.imgur.com/5TkBQit.png)
![](https://i.imgur.com/fhGeDnA.png)

## Contact information
For Questions or Comments, find <span style="color:orange">Rusty</span> in the Odin Plus Team Discord

[![https://i.imgur.com/XXP6HCU.png](https://i.imgur.com/XXP6HCU.png)](https://discord.gg/v89DHnpvwS)

Or come find me at the [Modding Corner](https://discord.gg/fB8aHSfA8B)

##
If you enjoy this mod and want to support me:
[PayPal](https://paypal.me/mpei)

<span>
<img src="https://i.imgur.com/rbNygUc.png" alt="" width="150">
<img src="https://i.imgur.com/VZfZR0k.png" alt="https://www.buymeacoffee.com/peimalcolm2" width="150">
</span>
