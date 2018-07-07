# Planning

This folder is where all files relating to planning reside.

# State of development

At the time of this writing this game is being developed by
one person assuming all roles. Godot 3.0.4 Mono is the engine selected 
for this project.

The first iteration of development is completed. This creates room for
two kinds of growth: refinement of existing features and the introduction
of new features. The former includes consists mostly of granular changes
as well as architectural review to reduce complexity. The second introduces
complexity. This balance must be attended to 

# Areas lacking polish
- AI needs to to rely on Util.RayCast and move AI.BoxCast to Util.BoxCast
- AI does not support netcode.
- AI is currently not smart.
- There's no settings menu to configure volume, mouse sensitivity, etc.
- There is only one placeholder Arena map.
- There are no options to configure single or mutliplayer games.
- Items rely on Scenes for initialization.
- Menus use default skin.
- Actors lack humanoid models with animations.
- Lack of item variety. (Currently no apparel, aid, or misc)
- Menu system might benefit from some inheritance.

# Completed Milestones

**Version 0.0.1 Arena**
Minimalistic multiplayer arena shooter.

# Arena Settings

Before starting a single player game and hosting a multiplayer game, a settings menu
should allow customizing how the game will be played. 

## Design
Each role should have responsibilities laid out in terms of tasks and acceptance
criteria that can be marked complete or ready for review.

## Art

## Level Design

## Sound engineering

## Testing

## Interface Programming
**Single player settings menu**
The following options should be available.
- Number of bots
- Starting kit?
- Spawn powerups?
- round duration

**Multiplayer settings menu**
The following options should be available.
- Number of bots (Not used at moment.)
- Starting kit?
- Spawn powerups?
- round duration

## Network Programming
Make sure the multiplayer settings menu changes only for the Server.

## Core Programming

**ArenaSettings**
Session.session should have an ArenaSettings object that contains all the 
configuration. This object should be initialized by the menus, then
attached to the Session.session instance for the Arena to draw upon when it inits.

**Sub Menu**
If convenient, figure out how to nest menus and apply that to the game settings menu
to prevent code duplication and overall menu complexity in the future.

## Gameplay Programming

Arena should load settings from Session.session.arenaSettings if it is available, 
and otherwise init with default values. These setting shsould influence the following
behaviors:
- Number of bots
- Starting kit?
- Spawn powerups?
- round duration

## AI programming
