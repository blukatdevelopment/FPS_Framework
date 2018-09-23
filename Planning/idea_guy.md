This file primarily contains ideas that simply don't fit within the current scope of development. Additional content includes descriptions of a future video game made from this project. 



# Epics, Stories, Tasks
These ideas are broken up by level of granularity and scope into 
Epics- Group of inter-operating features that forms a system in itself. (eg menus, items)
Stories- Individual feature or mechanic that contains multiple tasks.
Tasks- A bite-sized piece of functionality that relies on other features or systems.


# Epic
## Story
### Task



### Kingdom
Port in the kingdom card game as a multiplayer minigame.

# Crafting
Craft new items from existing items using blueprints.


## Crafting menu
A menu would either require viwing recipes, or allow freeform 
combination of materials as in minecraft.

## Recipe config
Recipes could either be stored in an sqlite database or in a
JSON file.


# Building
Produce structures from connecting components using blueprints.


# Procedural Quests
Complete quests created procedurally according to the conditions and
wealth(for rewards) of the NPCs that offer them.


# Dialogue System
Traverse dialogue trees to interact with NPCs using speech.




# Followers
Unlock dialogue tree for NPCs that lets you give them orders to stay/follow
and act as pack mules.

# Survival
Player should be vulnerable to the elements and have certain needs
that can give stat penalties and worst death.

### Hunger/Thirst
Actors should need to eat and drink to survive.



# Open World
The game world should consist of interior and exterior cells
inter-navigable by doors.

## 2D map
A texture should be generated to reflect the overworld.


## Persistent NPCS/structures
NPCs and structures should not appear or vanish from thin air
in a player's absence(aside from initial spawn and Background NPC lifecycle).


## Background: NPC lifecycle
NPCs should be born, reproduce, and die accross the overworld according
to random events with odds influenced by the world and other NPCS.

# Multiplayer
Multiple players should be able to connect to an open world server
and interact with its world.

# Background: Cities
Cities should act as entities guided by policy and individual members.
Cities build buildings, establish trade routes, and can participate in
wars.

# Humanoid actor animations
The player and other humanoid NPCs would need
a set of animations.

### Aim
Actor models should bend at the spine programmatically

### Walking
Forward/backward/side/side animations

### Running
Forward/backward/side/side animations

### Crouch walking
Forward/backward/side/side animations

### Transition from crouch walking to crouch running
Forward/backward/side/side animations

### Jumping
Jump animation.

### One hand animations
Punch/slash/stab/hold

### Two hand animations
Swing/hold

# Feral actor animations

### Walking
Forward/backward animations

### Actions
Bite/Bark/lick animations


# Vehicles
Players should be able to pilot various kinds of vehicles.

## Mounted Animal
Actors should be able to ride on the backs of certain animals that let
them climb steeper slopes and move faster.

## Automobile
Actors should be able to drive wheeled vehicles that move quickly over
smooth surfaces such as roads.

## Airship/Chopper
Slow aircraft containing rooms, passenger seats, or cargo.
They are meant to take off and land vertically. 

## Gravjet/Airplane/Jetplane
Quick-maneuvering aircraft made for bombing runs and dog fights.

# Real-Time Tactics
RTT gameplay entails battlefields with humans and AIs issueing orders to
their own armies.  

## Squads
Squad leaders(NPC or player) should be able to issue movement and rules of engagement
commands to squadmades(NPC or player).


## Command map
A 2D map should allow a commander to select squads/groups and issue orders to their
squad leaders.


# Arena Gamemodes

## Team mutator
Players on the same team will be penalized or prevented from harming
teammates whilst scoring will be shared with all teammates. AI will
not attack teammates.

## Horde Mode
Defenders fight against incoming waves of enemy NPCs that grow more
powerful and numerous with each subsequent wave.


# Dual instance online multiplayer
Figure out how to share focus between two or more windows, probably by associating one
with keyboard/mouse and the others with gamepads.

# Splitscreen multiplayer
One or both players use gamepads to play on the same game instance.

# Dual Screen multiplayer
Utilize multiple displays for split screen



# Very near-scope features
Some features could be implemented tomorrow in under 10 hours.

## Containers
Item transfer menu allows player to transfer
items between their inventory and containers
such as chests and corpses that have their own
inventories.


## Trading
The trade menu provides fixed buy/sell prices to
exchange items between two Actor's inventories.

## Speech
Json files should be parsed into speech trees
consisting of speech nodes. A speech node should consist
of a list of body texts read to player through clicking, 
a list of options. Each option has a list of properties
used in rendering it as well as a list of actions performed
when said option is selected.

The other side is a menu in order to display a loaded
speech node and perform its actions when an option is selected.
Actions:
go <Speech node id>
give <item> <quantity>
set-property <speech id> <option id> <property> <value>
Properties:
visible: <bool> // If false, option does not appear.
evaluate-method: <method name> // method that decides whether effects take place.
action-evaluate: <action> // Corresponding actions for this option only apply when evaluation method returns true.


# Background stuff

## Logging system
Instead of GD.Print(), use a Session.Debug() line to drop text into one or more log files that can
be tailed and grepped for a better debugging experience. 


# Philosophy
The following section is mostly empty rhetoric about a non-existant finished product. 

**Pillars of gameplay**

Consquence - 
All actions should have reactions, human or AI.
A consistent explanation for events should exist, even if it is obscured
from the player.

Practive AI -
An AI should take action to stop its opponents, prepare a strategy to do so, 
and operate with limited information and resources.



# Mini Roadmap For Adventure mode

## RPG mechanics
-Perks
-Quests
-Leveling

## Core stuff
- World
- Multiplayer Coop

## Item stuff
- Items
- Looting
- Viewable inventory
- Varied weapons

## Combat stuff
- Combat
- Things to fight (not necessarily enemies)
- Intuitive Controls
- Enemies
- Looting

## Economy stuff
- Trading 
- Currency

# Modular Content System
The concept is relatively simple at a higher level. The world should be made of building blocks, self-contained modules that interact with one another only through clearly defined interfaces. The goal is that the last module of content added will not regress or force a rewrite of the first.


## NPCs
NPCs can be broken down into three constituents, AI, Agent, and Character. Any set of these components should be able to combine to form a distinct NPC. 

### Active AI (AI.cs)
The active AI is responsible for an active actor's real-time actions in 3D space.

### Dormant AI(Agent.cs)
The agent is responsible for a dormant actor's turn-based actions in the 2D dormant world.

**void Update(int timeUnits)**
Perform updating by spending presented timeUnits.

**int LocationAffinity(Location location)**
Assesses preference for said location, ranging from -100 to 100. This is to be used by cartographer
when placing randomly generated NPCs.

### Communications AI (Communicator.cs)
The communicator is responsible for parsing and interpreting incoming messages as well as forming outgoing messages, consulting other systems as necessary. 

**string[] GetDialogueOptions()**
Returns all available dialogue options.

**void Communicate(Actor speaker, int option)**
Communicates a particular option as defined above.

**void Communicate(Actor speaker, string message)**
Communicates a body of text for the AI to parse.

### Character (ActorStats.cs)
The character describes the abilities and limitations of the NPC as well as long-term state in the form of a dictionary of strings referred to as memories. While a dormant and active AI can store whatever it wants in its own memory footprint, the AI is unloaded from memory and only the memories of the character will remain to be accessed the next time the dormant or active AI is instantiated.

**int PerkRank(string perkName)**
Returns 0 if character lacks a perk of this name, or else its rank. This should be used by Actor.cs upon loading.

#### Stats
Stats are a dictionary of ints that track anything from condition (health/stamina/mana/hunger/thirst/fatigue/radiation levels), active effects(berzerk, invisible, poisoned, regen), to personality traits (impatient, brave, coward), to ICEPAWS attributes, to varying granularity of skills (unarmed, melee, swords, guns, ranged, bows, survival, fishing, rifles, pistols), and RGP values (perkPoints, skillPoints, experience, nextLevel) The environment, AI, and Actor will consult and interact with this interface, but the custom logic of the Character will decide how to handle these interactions and maintain its internal consistency.

**int GetStat(string statName)** 
Fetch arbitrary stat. 0 is default value.

**void SetStat(string statName, int statValue)**
Change a stat's value. If stat does not exist and is going to become non-zero, add it. (if possible)

**void ChangeStat(string statName, int delta)**
Add delta to stat's current value.

**string[] GetStatNames()** 
Returns all this character's non-zero stats.

**void ExperienceAction(string actionName, int multiplier)**
Apply xp for a given action.

#### Memories
Memories are a dictionary of strings used for long-term fact recall. 

**void ForgetMemory(string memoryName)**
Remove a memory if it exists.

**void StoreMemory(string memoryName, string memoryValue)**
Attempts to add or update a certain memory.

**string RecallMemory(string memoryName)**
Returns corresponding memoryValue if it exists.

**int ActorAffinity(Actor actor)**
Returns preference for this actor according the actor and existing memories.

## Items
Items inherit from Item.cs and so this not a true interface

**string[] Uses()**
Return names of all uses. Each index matches an enum value in Item.Uses.
Can be used to inform UI or AI about how to use an item.

**string[] GetProperties()**
Returns all of this item's non-zero properties. Examples of properties include range, damage, value, hasScope, capacity, usesAmmo, ammoType, etc. These may vary heavily from item to item

**int CheckProperty(string property)**
Returns value of this property, or zero.


## Locations (implementing Location.cs, stored in LocationData.cs record)
A location is used to generate Actors, Items, and Scenery on an arbitrary set of TerrainCells. The location's information can then be stored in a LocationData record for use by the Overworld. The goal of a implementations will likely be to provide content adhering to a particular theme (dungeon, village, ruins, forest), but with procedural variation to adapt to given terrain and to mitigate redundancy.

**Vector2[] UsedCells()**
Coordinates used by this location's terrain cells. 

**bool UsingCell(Vector3 cell)**
Returns true if this cell is being used.

**void ProvideCells(Vector2[] cells)**
Adds cell to UsedCells()

**int CellAffinity(Overworld world, Vector2 coords)**
Returns preference for these coords as a centerpoint on the overworld. 

**void GenerateActors()**
Creates specific actors on this location's used cells.

**void GenerateStructures()**
Creates structures and natural features for location.

**LocationData GetData()**
Returns the data for this location.

**string[] GetProperties()**
Returns all non-zero properties for this location. Examples include vegetation, minerals, airable, water, surfaceWater, monstersPresent, civilization, dungeon, urban, etc, etc

**int CheckProperty(string property)**
Returns a propert's value, or null.