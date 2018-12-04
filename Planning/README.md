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
- docs : Updating documentation.

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

Title: Auth MVP
Components: Core, Networking
Body:
Every client should have a username and password when connecting to the server.
The server should store these (with password hashed) and check them again on
subsequent login. Failing auth should prevent send the client back to the
main menu.

Title: client/server overworld MVP
Components: Core, Networking
Body: 
A method should exist to send the terrain, items, and actors contained within
a cell from the server to client.

Another method should exist for clients to request the data above for a cell.

Instead of reading from disc, clients should request the cell data from the
server and leave it there. As such, clients should request cells in a 

A third method should exist so that the server can change the brain/type of
an actor when a client connects and takes over an actor (turning it Remote), 
or disconnects, abandoning it (turning it to Ai).

When a player's actor dies, they should respawn with a blank inventory at a 
random spawn point decided by the server.

Title: lobbyless multiplayer
Components: Core, Networking
Body:
The overworld server should start the game immediately, and clients should be
free to connect/disconnect whenever.

The menus should account for this change by disabling the lobby for 
online overworld clients and providing a start button for servers.