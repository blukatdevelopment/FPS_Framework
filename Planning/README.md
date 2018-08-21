# Planning

This folder is where all files relating to planning reside.

# State of development

At the time of this writing this game is being developed by
one person assuming all roles. Godot 3.0.4 Mono is the engine selected 
for this project.

The first iteration of development is completed. This creates room for
two kinds of growth: refinement of existing features and the introduction
of new features. The former includes consists mostly of granular changes
as well as architectural review to reducOptimization will be a priority that will come only after things are feature complete.  However, that might be something worth noting. e complexity. The second introduces
complexity. This balance must be attended to 

# Areas lacking polish or needing refactor
- AI is currently not smart.
- There is only one placeholder Arena map.
- Items rely on Scenes for initialization.
- Menus use default skin and is hideous.
- Actors lack humanoid models with animations.
- Lack of item variety. (Currently no apparel or misc)
- BUG: When a match ends and new match starts, arena is not initialized properly. (Will take significant effort to reproduce. Considering non-critical for the time being.)
- Add <string, string> variable to ItemQuery and replace ItemData with it to simplify adding/removing data.

# Completed Milestones

**Version 1.0.1 Arena**
Minimalistic multiplayer arena shooter.


# Adventure Gamemode

Things that need to be done:
- Menus to access and configure adventure mode
- Overworld class to manage active Treadmills
- Treadmill class to render one portion of the map
- Terrain data storage
- Actor/Item storage
- placeholder world generation

The following steps are the tentative roadmap. WIth this much scope, things will likely change between steps.

## DONE Step 1: Access

Set up menus to navigate to adventure mode
- Multiplayer Server lobby
- Single player settings screen
- AdventureConfigMenu


## DONE Step 2: Prototypes

Build up some prototype classes with dummy methods and comments
- Overworld
- AdventureSettings
- Treadmill
- Cartographer - Generates the overworld's terrain.
- TerrainCell - Unit of overworld terrain controlling one GridMap
- TerrainCellData - Data from one Terrain Cell
- IAgent - 
- MonsterAgent

New classes discovered after setting up dummy code:
- OverworldData
- ItemQuery
- TerrainBlock

## Step 3: Hello, world!

Provide minimal implementation to render overworld with a single treadmill.
- Overworld
- AdventureSettings
- Treadmill
- Cartographer
- TerrainCell
- IAgent
- MonsterAgent

## Step 4: Netcode

Handle traditional problems Arena faces.
Give arbitrary user control over treadmill, transfer control when they leave.


## Step 5: MMO Netcode
Handle new problems 

Store SHA256 hash of password in user preferences.
Use username + password to login as player on server.
Allow dropping in and out of game (Spawn joining players at center of treadmill).

## Step 6: Multiple treadmills

Get multiple treadmills in one overworld and netcode.
Handle treadmills joining.
Handle treadmills separating.
Handle treadmills disappearing and appearing with players (Possibly with delay).


## Step 7: Databases 

Figure out saving/loading sections of the world using sqlite.
Create database handler for overworld that selects between multiple files depending on which adventure is loaded.
Determine a schema for terrain, actors(and their details)
Look into using UUID instead of auto-incremented IDs.
Make serializeable versions of actor. 
Look into options for loading database into memory and saving it to file. 


## Design
Each role should have responsibilities laid out in terms of tasks and acceptance
criteria that can be marked complete or ready for review.

This iteration should, in itself, contain a few steps(These are not necessarily sequential).

0. DONE Set up camera to view these steps.
1. DONE Generate single flat Gridmap (Perhaps with one cube poking up to show tile boundaries.)
2. DONE Generate map comprised of these Gridmaps in dormant form.
3. DONE Render a 3x3 grid of these Gridmaps.
4. DONE get treadmill to watch for player leaving center gridmap
5. DONE treadmills should determine which gridmaps will be released/requested.
6. recenter cells to treadmill position after recenter when possible
7. save/activate items and actors along with terrain cells (Handle via overworld)
8. Create invisible walls on unused boundaries of treadmill
9. Perform Populate and DePopulate in background or on another thread.
10. Populate items/Actors in active space.
11. Maintain dormant space separately from active space.

## Art

## Level Design

## Sound engineering

## Testing

## Interface Programming

## Network Programming

## Core Programming

## Gameplay Programming

## AI programming
