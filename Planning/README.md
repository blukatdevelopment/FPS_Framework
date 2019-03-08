# Hiatus
The plan is to work on another unrelated project of a different genre, get a bit more experience with godot, and then come back for a rebuild using lessons learned from Manafest: Arena.

## File structure

It's clear to me now that the top level of the file structure should be cleaned up to look like this:

assets/
    src/ - Source code
    textures/
    models/
    audio/
    data/ - Saves, preferences, level data, copy, etc
docs/
bin/
screenshots/

## Session/
Should contain Session.cs, which should primarily be the singleton god-object for the game.
It should primarilily provide static methods and maintain a global state consisting largely of components that implement interfaces such as IGameMode and IMenu that can be swapped out when convenient. Said interfaces should reside here.
Session should likewise receive events that are tricked down to its active IGameMode.

## Input/
There should be three layers of abstraction:
1. Detecting the current state of keys/buttons/axes
- should be updated as children of Session
- Should be handled based on configs. 
- Only listen to keys/buttons/axes for active mapping
2. Mapping this state to input events 
- type (eg press, combo press, held, double tap) 
- name (ie right hand, punch, moveForward, lookUp, crouch)
- static GetInputEvents(playerNumber)
- static SetInputMapping(playerNumber, mapping)
3. Mapping input events to in-game behaviors
- should be configurable in input menu
- One should exist for every player-controlled entity
- Should have Sensitivity for every pseudo-analog input
- Menu should provide "Press Punch" sort of setup to be joystick agnostic

## DB/
Should contain classes that abstract data stores and server communication away.
Some should do this asynchronously or with buffering due to retrieval speed, whilst others should fetch data immediately.

Examples:
SettingsDB - string-based key/value store for config.
CopyDB - Retrieve copy strings such as  "Hello, player!" using node strings such as "player.greeting" in order to avoid hardcoding this and allow language switching
AdventureDB - Save adventure games and maps
ArenaDB - Arena game saves
ItemDB - Item configs for use in ItemFactory
ActorDB - Stored premade and user-customized actors
DialogueDB - Dialogue trees
NameDB - Names with associated tags ie "Jonathan": ["first", "male", "biblical", "western"]

## Actor/
Actor should be an aggregation of components that each handle some aspectof an actor
StatsManager - RPG stats and calculations
ActorInputHandler - Control playable actors
ActorAnimationHandler - Should control blending animations of a rigged skeleton

## Item/
IItem
ItemFactory - Used in absence of 

### Item/Items/
Classes that implement IItem and use IItemBehaviors 
### Item/Behaviors/
Classes that implement IItemBehavior

## AI/
Should contain any artificial intelligence used in the game.

## Audio/
Jukebox - Play/loop music from folder, configured playlists, etc
Speaker - 3D Moving source of sound
OneShotAudio - Plays sound and then QueueFrees, static OneShotAudio.PlayAtPoint(point, sound)

## Dialogue/
Classes to construct dialogue trees that send events to the Session and make checks against a global EventHistory object.

## Inventory/
HotBar - Finite numbered slots meant to be cycled through and equipped
Inventory - Bottomless inventory of items
PaperDoll - Slots for passive equippable items such as clothing, charms, and armor

## Util/
Should contain miscellanious static engine helper methods in Util.cs, a simple test class in Test.cs, and any other misfits.

## Menu/
Should have a MenuFactory class that returns various classes that implement IMenu. Should also have a Menu class to provide convenience methods to build and configure various UI elements. One or more baseclasses for menus should allow allow extending the menus to be easier.

## Arena/
Should contain any Arena-specific classes 

## Adventure/
Should contain any Adventure-specific classes

## Network/
Should contain classes to further abstract away or simplify netcode and other online features.