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

# Completed Milestones

**Version 0.0.1 Arena**
Minimalistic multiplayer arena shooter.


# Settings menu

The settings menu should provide some basic configuration for the player provided initial values on
first install using its own database file and database handler with related queries. The menu should
provide some basic options loaded by the Session upon startup.

## Design
Each role should have responsibilities laid out in terms of tasks and acceptance
criteria that can be marked complete or ready for review.

## Art

## Level Design

## Sound engineering

## Testing
	All features should be tested thoroughly to diagnose and resolve bugs.

## Interface Programming
**Settings Menu**
Make sure the following features exist
- Field validation if necessary
- Save button that gives confirmation upon success
- Prepopulate existing fields

Include the following fields for use
- mouse_sensitivity_x SLIDER 0, 1
- mouse_sensitivity_y SLIDER 0, 1
- username
- master volume SLIDER 0, 1
- sfx volume SLIDER 0, 1
- musc volume SLIDER 0, 1


## Network Programming

## Core Programming

**SettingsDb**
Settings Db should have the following features/methods
- Use separate db file.
- StoreSetting(name, value)
- string RetrieveSetting(name)
- First-time init

**Session init settings**
Session should initialize using settings provide by SettingsDb

**Volume**
-Speakers should use volume pulled from Session
-Jukebox should use volume pulled from Session

**Mouse sensitivity**
ActorInputHandler should pull these settings from Session
## Gameplay Programming

## AI programming
