# Planning

This folder is where all files relating to planning reside.

# Guidelines

## Commits

A commit message should look something like this. Note that the QA and Bug
sections are optional, but the type(scope): is not.

``` 
feat(AI): Installed morality core.

QA: Check to make sure testing chamber is not flooded with nerve toxins.
Bug: Fixes #1234
```

Acceptable commit types include:
- fix : A fix to existing functionality.
- feat : Adding new functionality.
- doc : Updating documentation.

# State of development

At the time of this writing this game is being developed by
one person assuming all roles. Godot 3.0.6 Mono is the engine selected 
for this project.

The first iteration of development created the Arena mode, wherein players and
AI fight one another in a static arena.

The ongoing second iteration of development is creating the Adventure mode, 
wherein players and AI interact in an open world sandbox.

# Areas lacking polish or needing refactor
- Menus use default skin and it is hideous.
- Actors lack humanoid models with animations.
- BUG: When a match ends and new match starts, arena is not initialized properly. (Will take significant effort to reproduce. Considering non-critical for the time being.)
- BUG: Cannot load adventure from pause menu

# Completed Milestones

**Version 1.0.1 Arena**
Minimalistic multiplayer arena shooter.


# Active stories


## Construct actor scene tree programmatically
