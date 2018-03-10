# Planning

This folder is where all files relating to planning reside.

# State of development

At the time of this writing this game is being developed by a
one person assuming all roles. Godot 3.0 Mono is the engine selected 
for this project, though the export templates will not be ready until
Godot 3.1 is released. 

With this in mind, planning will be restricted to the first 
iteration of development, after which planning for further 
development may be entertained. All out-of-scope ideas will be
contained within ideaguy.txt in order to avoid feature creep.


# CORE: Arena

Premise:
An arena game should ideally consist of the most essential features
needed by a multiplayer arena shooter. Each developmental role
or responsibility should be met minimally to achieve a functioning
deliverable.


## Design
Each role should have responsibilities laid out in terms of tasks and acceptance
criteria that can be marked complete or ready for review.

Combat should feel fast-paced due to movement speed, rate of fire, and quick
respawns. AI should be able to compete, but should not be overly difficult.

The arena gameplay should consist of spawning into a map with a rifle and 
finite ammo alongside other human or AI-controlled Actors. The player kills 
other Actors in order to score points, picking up ammo spawned at spawnpoints. 
A game should last 5 minutes.


## Art
The following models should be produced. No textures are necessary.

Actor
- Actor Body
- Actor Hand
- Rifle
- Bullet
- Ammo Pack
- Health Pack

Terrain
- Large platform
- Cube obstacle
- ramp


## Level Design
Three Arena maps should be designed with the following criteria:

- Falling out of the map should be difficult/impossible
- At least 20 spawnpoints that do not clip through terrain
- At least 5 item spawnpoints placed in accessible positions


## Sound engineering
The following sound effects should be produced in .ogg format.
- Firing rifle
- Swinging fist
- Taking damage

Three songs should be produced with the following criteria:
- At least two minutes in length
- Not excessively quiet or loud
- Loop seamlessly or with little syncopation


## Testing
- Each programming user story should be tested.

## Interface Programming

**Main Menu**
Should allow user to choose a game mode or exit.
Options
- Singleplayer - Start new game
- Multiplayer - Enter multiplayer menu
- Quit - exit game

**Multiplayer Menu**
Should allow user to select settings for either hosting or joining a game.

Options
- Host
- Join
- Return to main menu

Host settings fieldset
- Game name 
- Port 

Join settings fieldset
- server address
- Port 
- Name 

**Lobby Menu**
Should allow users to chat while they wait for the game to begin.

Options
- Return to main menu
- Send message
- Toggle start game countdown (If reaches 0, multiplayer game will begin.)

**Pause Menu**
Should allow users to quit the game.

Options
- Resume game
- Return to main menu
- Quit

**Round Scores screen**
Should list the names of all players in a game along with their scores.
After a specific amount of time, a new game should start.

Options-
Return to main menu at bottom of screen

**HUD**
Should display relevant info during gameplay.

Info shown
- Health
- Active item info
- Gamemode info (populated via Session)

## Network Programming
Networking architecture should be developed by modifying a single-player
gamemode.

- Players should be able to connect or host servers via the multiplayer menu
- Players should be able to chat or start the game in the lobby menu
- Players should also be able to fight one another in the multiplayer arena.

## Core Programming
The underlying architecture of the game should be modular and expandable to
simplify the development of content-specific programming.

- Transitioning between menus.
- Transition between gameplay and menus.
- Managing session variables.
- Instancing UI and in-game Scenes.
- Gather input from arbitrary device.


## Gameplay Programming

**FPS controls**
The core gameplay should consist of controlling an Actor using conventional
FPS mouse and keyboard controls.
- WASD movement
- Mouse aiming/turning
- Left Mouse to punch or shoot rifle
- R to reload rifle
- Tab to switch between rifle and hand.
- Escape to bring open or close pause menu, toggling gameplay input handling.

**Equipment**
Every Actor should spawn with a hand and a rifle. The rifle should equip upon 
init. 

When an item is equipped, its scene should be loaded in the actor's hand
position. An actor's input handler should direct certain inputs to the equipped
item.

When an item is unequipped, its scene should be removed from the Actor's node
and freed. Equipping one item should unequip the other.

**Health**
An Actor should have health ranging from 0 to 100. The Actor should have at least
one hitbox as a means for weapons to deliver damage. A dead Actor should rotate
on its side as to indicate it is dead. A dead actor's input handler should ignore
all non-menu input.


**Melee Weapon**
A melee weapon should trigger a swing it's cooldown is full.

Cooldown should be a float value that increases with _Process(delta) to a max
value and is set to 0 when a swing is triggered.

When a swing is active, _Process(delta) should increase swingTime for timing.
When swingTime exceeds damageStart for the first time, damageActive should be 
set true and the item should assume its forward position.
When swingTime exceeds damageEnd it should set damageActive to false and and
return to its normal position(The actor's hand position).

A melee weapon should receive collision signals from a hitbox. When damageActive,
and colliding with another actor (not itself), it should make the other actor 
ReceiveDamage and then set damageActive to false.


**Projectile**
A projectile should control a KinematicBody to move Forward(Vector3 velocity)
and have its velocity effected by gravity.

Upon being Fired, a projectile will begin moving according to its velocity
and update velocity with gravity.

Upon colliding, it should check if it collided with an actor, and if so, cause
them to ReceiveDamage. At this point the projectile should free itself.

**Ranged Weapon**
A ranged weapon should have a reserve of ammo that is depleted by firing 
projectiles and replenished by requesting ammo from its holder during a reload.

When firing, a ranged weapon creates a projectile scene and Fires it in the
direction the ranged weapon is facing.

When a reload (tertiary use) is attempted, it surveys stored and available ammo.
If ammo is not already full and its holder exists and has ammo, the ranged weapon
will trigger a reload.

When a reload is triggered it sets a reloadTime to 0 and increases it from 
_Process(delta). When reloadTime exceeds reloadDelay, the ranged weapon's 
reserve of ammo is increased by an amount successfully requested from its holder.

**Health/Ammo pickup**
ammoCount should be an integer ranging from 0 to 999. If not full(999), the player
should pick up any ammo it collides with.

When an ammo pack is picked up, the ammoCount rises to at most full(999) and the ammo
scene is freed.
The same applies to health packs, but with health instead of ammoCount.

## AI programming
AI should be as simple as possible and perform a few behaviors.

- Wander randomly, looking for targets.
- Attempt to pick up items when they are in sight.
- Select strategy according to inventory (weapons + ammo + item.HasAmmo())
- For ranged combat aim at target and fire until they die or leave vision.
- For melee combat aim at target and fire when within range until they die or leave
vision.
- Cease activity when dead.
