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
- BUG: Can pick up own hand

# Completed Milestones

**Version 0.0.1 Arena**
Minimalistic multiplayer arena shooter.

## Design
Each role should have responsibilities laid out in terms of tasks and acceptance
criteria that can be marked complete or ready for review.

## Art

## Level Design

## Sound engineering

## Testing

## Interface Programming

## Network Programming

AI should send out RPC calls for their actions when network is active.


## Core Programming
    Boxcast should get fixed up and added to util.

## Gameplay Programming

## AI programming

**Fix the raycasting**
Use util methods exclusively.

**Fix nulls**
AI should not hold null pointers if it can be avoided.
Else they should check for nulls when running.

**Netcode**
AI should work in multiplayer.

**Performance**
Play with the mix between performance and interaction.
Currently 16 AI causes a significant amount of lag.