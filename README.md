# FPS_Project

FPS_Project is exactly what the name implies. The intent is to create a first-person multiplayer game
with which to experiment. The game engine used will be Godot 3.0 with C# as the scripting language of choice.

This project is a departure from a [previous project](https://github.com/justi1jc/FPS), which was an attempt
to recreate common FPS game mechanics using Unity3d.

Because Godot 3.0 and it's C# support is still new and buggy, development will be slow at first. As familiarity
with the engine, engine stability, and documentation improve, the rate of progress should as well.


# Development

Random feature branches will be the norm for the next few months.
At this early stage of development focus will be applied to laying out
simple solutions that can and will be refined later as more engine-specific
problems are solved.


feature/networked_game

Cosmetics
* Make lobby collect name and then hide buttons after connecting/hosting.
* Offer disconnect button.
* Display connected players.
* Store lobby info in Session. Clear it when leaving lobby.
* Create ready/start buttons.

Game
* Create one instance of each player in the lobby.
* Syncronize movement accross machines.
* Allow game to return to lobby.
