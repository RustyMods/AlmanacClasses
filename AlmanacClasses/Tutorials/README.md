# Almanac Classes
Plugin adds a node based progression system that unlocks through experience gained by killing monsters and playing the game.

Still in development, please report bugs and issues to me at OdinPlus while I keep balancing system.

## Contributors
- Dealman
- JoshLaseter
- Koreshx

## 0.5.9 update patch notes
<details closed>

```
Alternative talents:
- Chain Shot [enable in configs, replaces Hunter]
    - When active, bows/crossbows can ricochet and hit other targets, damage reduced each ricochet
- Fireball [enable in configs, replaces Goblin Beam]
    - Shoots a fireball projectile
- Leech [enable in configs, replaces Shaman Regeneration]
    - Heals user whenever they deal damage

Talent Changes:
- Enlightened [added eitr regen]

Characteristics Overhaul:
- You now unlock characteristic points by purchasing characteristic talents
- Use the points at your discretion through the new UI panel

Performance Improvements:
- Experience bar updates when experience changes, rather than every second

Command Overhaul:
- All the commands have been moved under a single prefix [talents]
- Improved search functionality

UI Improvements:
- Spell bar, Experience bar, Passive bar are more responsive when dragging

New UI:
- Spell Inventory:
    - When purchasing a talent that requires to be casted (abilities) and the spell bar is full, it can be found in the inventory panel
    - You can now remove/add spells from your spell bar by interacting with the inventory
- Characteristic Panel:
    - Displays current benefits from characteristics
- Passive bar:
    - Some passive talents are added to this new bar to be enabled/disabled

Compendium Changes:
- Passives now display if they are on/off
- Added passives that are not active

Icon Updates:
- Dealman has been working on overhauling the icons displayed in the spell bar

Altar changes:
- Fixed materials
- Updated emissions
```

</details>

## Highlights
<details closed>

Unique talent abilities that are either passive or activated spells

Some noteworthy abilities:

- Monkey Wrench - Allows to wield two handed weapons in one hand (does not apply to all)
- Dual Wield - You can wield different types of weapons together
- Bleeding - Stacked bleeding damaging ability
- Creature Mask - Spawn a friendly creature - Automatically dies after a certain time
- Shaman Heal - Heal players or self
- Song of Healing - Heals nearby players over time

And many more abilities

</details>

## Prestige System

Class system has a prestige system. You can prestige after you've met the <span style="color: orange;">threshold</span> (configurable)
Prestige each talent individually to increase their utility

## Classes

<details closed>
<summary>List of classes, although the system is structured in a way where you can combined talents from each class to craft a unique hero</summary>

- Bard
- Shaman
- Sage
- Ranger
- Rogue
- Warrior

</details>

## Experience

<details closed>
<summary>
Experience system rewards player whenever a creature is killed in the surrounding area. It does not matter who kills creature, all nearby players will be rewarded.
</summary>

Players also gain experience by:
- Chopping Trees
- Chopping Logs
- Hitting Ore Deposits
- Taming Creatures
- Picking up Items

Creature reward different experience, you can configure this using the generated YML file found:
- BepinEx/config/AlmanacClasses/Experience/AlmanacExperienceMap.yml

If a creature is not registered to this file, then the experienced gained will be based on the biome the player is in.

</details>

## Characteristics

<details closed>

Just like the talents, you can use your talent points to unlock characteristics points which have different effects:
- Intelligence: Increased magic damage
- Strength: Increased physical damage and carry weight
- Dexterity: Increased attack speed and stamina
- Constitution: Increased health
- Wisdom: Increased eitr

</details>

## Configurations

<details closed>

Plugin is fully configurable using the cfg file generated in BepinEx/config directory:
- RustyMods.AlmanacClasses.cfg

</details>

## Server
AlmanacExperienceMap.yml is synced through server for admins to configure.

## Commands
```
talents help  - list of available commands
talents reset - resets all player almanac class data
talents clear - resets player talents without removing level, admin only
talents give  - [player name<string>] [amount<int>] gives player experience (nearby players only), admin only
talents size  - prints size of almanac class data saved on player file
talents save  - saves class data to player
```
## Changelog
<details closed>

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
0.4.3 - Made status effect timer more visible, added config to disable start effects
0.4.4 - fixed shaman heal, fixed server not loading talents, tweaked UI, added berzerk and sailor talents as alternatives
0.4.5 - fixed bard song triggering often, tweaked some effects and fixed battle fury - think this is last update for now - seems stable
0.4.6 - fixed server sync
0.4.7 - fixed characteristics being increased upon logout/login
0.4.8 - fixed forager talent, localized more text, set some default configs, fixed hunter talent
0.4.9 - group xp tweaks, more localized text, better tooltips, increased spellbook text font size, cooldown cannot be lower than length, open UI remotely key config
0.4.10- Tweaked shaman summon, fixed talents trying to apply to dead characters, talents don't consume cost if cannot use, animation toggle config added, fixed compatibility with CLLC - display exp issue, battle fury stamina gain configurable / upgrades with level
0.4.11- Tweaked UI to allow more text, Tweaked summons to not attack player pieces, Tweaked Master Chef for compatibility, Added more tooltips, fixed rogue backstab, shaman summon requires boss kills to unlock, fixed effects trying to apply to dead characters, added command for admins to give experience, removed spell skill modifier
0.4.12 - fixed raven fly away and air bender jumping more than allowable
0.4.13 - reverted bard animation to dance - added config to disable raven - fixed lightning effect not disappearing if creature killed before strike - added more redundancies to bard statuses
0.4.14 - Added check on RogueReflect if there is an attacker to reflect to - added config to cap characteristic talents
0.4.15 - overhauled experience map, toggle to lose experience on death, min/max level to get experience from creature, added alternative airbender which only requires eitr to use
0.5.0 - Added visual text effects and improved experience gain
0.5.1 - Added static experience map file to configure
0.5.1 - Fixed planting giving exp even if plant was not placed, added experience for fishing on hooked
0.5.2 - something
0.5.3 - Fixed EXP bar not updating, added Koreshx UI code to improve Spell bar
0.5.4 - Added additional conditionals for ability cooldown to make sure player is active
0.5.5 - Fixed not getting exp from veins and added method to get exp for foraging (add prefabs to static experience map)
0.5.6 - Added ability to change background and changed Call of Lightning FX
0.5.7 - Bog Witch update
0.5.8 - Added PR from Dealman and JoshLaseter
0.5.9 - Major update, read full description above
```
</details>

## Screenshots
<details closed>

![](https://i.imgur.com/F1w6ijU.png)
![](https://i.imgur.com/5TkBQit.png)
![](https://i.imgur.com/fhGeDnA.png)

</details>

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

## Talents

<details closed>

```yaml
Treasure Hunter: "Works like the wishbone to find treasure"
Sailor: "Always tailwind when sailing"
Rain Proof: "Prevents from getting wet"
Supplier: "Increases chop and pickaxe damage"
Airbender: "Allows to jump in the air"
Master Chef: "Increases food bonuses"
Pack Mule: "Increases carry weight"
Relax & Chill: "Passively adds comfort"
Creature Mask: "Spawns a friendly creature based on biome"
Forager: "Increases foraging"
Hunter: "Slows down nearby creatures\n(<color=orange>Creature affected is dependant on biome</color>)"
Lucky Shot: "Chance to not consume projectile"
Quick Shot: "Fires bows and crossbows faster when activated"
Trapped: "Sets a trap on the ground"
Call of Lightning: "Calls lightning down from the sky"
Meteor Strike: "Calls meteors down from the sky"
Boulder Strike: "Calls a boulder down upon your foes"
Nova Beam: "Cast a beam of fire and fury at your foes"
Ice Breath: "Triggers a furry of cold air towards your foes"
Heal: "Instantly heal your allies in a pinch\n(<color=orange>Hover over ally to select who to heal</color>)"
Shaman Protection: "Absorbs incoming damage"
Shaman Regeneration: "Increases stamina regeneration and eitr regeneration"
Ghastly Ambitions: "Spawns up to 3 friendly creatures for a short period of time"
Rooting: "Cast a furry of forest roots at your foes"
Song of Damage: "Increases damage output of nearby players"
Song of Healing: "Heals nearby players incrementally during the duration of effect"
Song of Vitality: "Increases nearby players health points"
Song of Speed: "Increases nearby players speed"
Song of Attrition: "Damages enemies around caster every second"
Quick Step: "Increases speed and reduces run stamina drain"
Swift: "Increases stamina and stamina regeneration"
Retaliation: "Reflects incoming damage"
Backstabber: "Chance to inflict backstab damage even when alerted"
Bleeding: "Inflicts bleed onto target\n(<color=orange>Multiple strikes increases damage stack</color>)"
Power: "Increases damage output and health regeneration"
Vitality: "Increases health and health regeneration"
Monkey Wrench: "Allows to equip two-handed weapons as one-handed weapons\n(<color=orange>May not apply to all weapons</color>)"
Fortification: "Reduces physical damage for a short period of time"
Dual Wield: "Allows to dual wield one-handed weapons\n(<color=orange>Stamina cost increased while dual wielding</color>)"
Battle Fury: "Chance to recover stamina from a kill"
Survivor: "Chance to heal instead of dying"
Enlightened: "Increase base eitr"
Looter: "Chance to get extra loot from backstab damage"
Builder: "Decrease cost of building"
Gipsy: "Allows to teleport non-teleportable materials based on talent level"
Berzerk: "Passively adds armor"
Airman: 'Allows to jump in the air whilst user has eitr'
Leech: 'Heals players on hit enemy'
Fireball: 'Fires fireball projectile'
Chain Shot: 'Arrows/Bolts can ricochet off targets to hit nearby enemies'
```

</details>