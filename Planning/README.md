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
- Revisit item stacking.
- Revisit ItemQuery, maybe rename to ItemKey and apply kits before spawning actors in Arena
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


## Auth service
- Build auth service in other repo.
- Set up outward-facing nodejs server to offer this service.
- Build static Util method in this project to attempt to connect and use configured auth service endpoint.

## Save/Load options in Adventure mode pause menu
- Static method to figure out if in singleplayer adventure mode
- Add the button when this is true


## Load option in adventure menu
- Add the button to lead to the load menu

## Load menu
- Context-sensitive back button
- Check saves directory and list .adventure files in scrolling list
- Add load button that starts game using selected file

## Select adventure name in adventure menu
- Add text field 
- auto-validation with overwrite warning