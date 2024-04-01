# Almanac Classes
Plugin adds a node based progression system that unlocks through experience gained by killing monsters and playing the game.

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

Class system has a prestige system. You can prestige after you've met the threshold (configurable) and it will increase the effects of all the talents

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
- Shooting Arrows
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
```

![](https://i.imgur.com/F1w6ijU.png)
![](https://i.imgur.com/5TkBQit.png)
![](https://i.imgur.com/fhGeDnA.png)
