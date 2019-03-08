# FPS_Framework

As the title states, this is a framework for first person shooter games. 
Ideally, a developer should be able to take a release of this framework and
expand upon it by adding game-specific tweaks, scripts, and assets. With an
MIT license for both this framework and the engine it relies upon, the hope is
to provide a FOSS starting point for indie devs, and to promote linux gaming.

What this framework **is**:
- A collection of Base Classes for in-game items, Actors, Terrain
- Session management for 
- Netcode
- FPS Arena
- Openworld FPS sandbox

What this framework **isn't**:
- A broad library of quality assets
- A finished game filled with content

## History 

This project is the latest evolution of a [previous project](https://github.com/justi1jc/FPS), which aimed to
recreate common FPS game mechanics using Unity3D. Originally the idea was to switch out Unity3D for Godot
because it was FOSS and had permissive licenses. The idea was to replicate the functionality of the other, 
albeit with an MIT license so that the project could be used as a starting point for others(including myself).

After putting some serious time into developing the game, I noticed I was putting an inordinate amount of time into
the functionality under the dash, and not spending any time polishing the look and feel of the game. 
I've since come to realize that what I was intending to work on was not a finished game, but a framework for one.
A finished game would offer artistic content (such as a rocket launcher that fires potatos), whereas my concern
with this project is to tackle the technical problems (such as providing a projectile weapon and explosions).

Thus arose the plan to use the framework as a starting point for a [new project](https://github.com/blukatstudios/ManafestArena). As of this update, I am wrapping up said project and have learned many lessons. Rather than directly jump back into this project, I have decided to turn my attention to something unrelated in order to clear my mind and return with a fresh start. The plan is to tear this project apart and rebuild it in a smarter way upon my return.

# Development 

See releases 1.0.1 and 1.1.0 for what this project's older state.

Version 2.0.0 will undergo development after I take a hiatus from the FPS genre to clear my head, gain more familiarity with Godot, and allow Godot's 3D and C# features to mature a bit.

The Planning/README.md contains the game plan for the major refactoring/rebuild.

![MainMenu](/Screenshots/main_menu.png)
![TwoAi](/Screenshots/two_ai.png)
![Lobbymenu](/Screenshots/lobby_menu.png)
![InventoryMenu](/Screenshots/inventory_menu.png)
![SettingsMenu](/Screenshots/settings_menu.png)
![ArenaConfig](/Screenshots/arena_config.png)