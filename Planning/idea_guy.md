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

# Modular Content System
As much as possible, the modular content system should be a set of interfaces that can be implemented to allow content to be added to the game in a modular fashion. This has two intended effects.

Firstly, the "core" code should function independently from any module's implementation by using these interfaces.

Secondly, these interfaces should define modes of interaction between entities (items, actors, terrain) that any class can take advantage of.


## Actor

### IStats.cs
Stat handler

bool HasStat(string name)

int GetStat(string name)

### IEdible.cs
food

### IThinkSlow.cs
turn-based AI

void Update(int timeUnits)

### IActor.cs
Executor of actions, most likely implemented by turn-based AI.

int GetActionPoints()
bool ConsumeActionPoints(int quantity, IAction action)

### IAction.cs
A discrete action to be used by an IActor.
First an IActor sets the parameters and checks the cost.
An IActor then attempts to perform the action, consuming the action points involved.
This should work well for turn-based AI as well as for abilities or spells.


void Init(IActor executor) // Set the executor. Action may attempt to cast executor to other interface types to perform side-effects on Execute or to calculate cost.


bool CanExecute()
bool Execute()
int Cost()

string GetName()
string GetDescription()


List<string> GetParametersNames()
Dictionary<string, string> GetParameterTypes()
Dictionary<string, string> GetParameterValues()
Dictionary<string, string> GetParameterDescriptions()

string GetParameterValue(string name)
string GetParameterType(string name)
string GetParameterDescription(string name)

bool SetValues(Dictionary<string, string> values) // Returns true on success
bool SetValue(string name, string value) // Returns true on success

### IThinkFast.cs
real-time AI


# Item

# Terrain
