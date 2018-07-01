# Planning

This folder is where all files relating to planning reside.

# State of development

At the time of this writing this game is being developed by a
one person assuming all roles. Godot 3.0 Mono is the engine selected 
for this project.

The first iteration of development is completed. This creates room for
two kinds of growth: refinement of existing features and the introduction
of new features. The former includes consists mostly of granular changes
as well as architectural review to reduce complexity. The second introduces
complexity. This balance must be attended to 


# Completed Milestones

**Version 0.0.1 Arena**
Minimalistic multiplayer arena shooter.

# Inventory
Inventory should allow storage of items limited only by weight.
By default, the player should have access to their hand. Any other items
should be stored in the inventory when picked up.

# Things that need refactoring or fixing before this branch gets merged:
- Menu factory needs to stop relying on scene files.
- Make more session methods static
- Make sure received ammo is deposited into inventory.
- De-couple Actor from Arena code, particularly for inventory init.


## DONE Design
Each role should have responsibilities laid out in terms of tasks and acceptance
criteria that can be marked complete or ready for review.

This should *feel* just like the inventory system in the 3D fallout games.

## Art

## Level Design

## Sound engineering

## Testing
- Each programming user story or task should be tested and approved before
the deliverable can be released.

## DONE Interface Programming

**DONE Inventory Menu**
Inventory menu should provide several components.

- DONE scrolling list of potentially infinite number of items.
- DONE Buttons at the top of the menu should filter list by category.
	- Weapons
	- Apparel
	- Aid
	- Misc
	- Ammo
- DONE Single left click should display item info.
- DONE Double-clicking item should cause it to be equipped, used, or not.
- DONE Right clicking an item should drop it.
- DONE Items should have number "Brick(5)" to indicate stack size.
- DONE Total weight should be displayed at the top of the menu.
- DONE Item info should be displayed near item list.

**ABORTED Quantity menu**
When dropping a stack of more than 5, quantity sub menu should render.

- Name of item should display at top
- Quantity and a slider for the quantity should display below
This should get pushed out a bit later, after a system to support an
arbitrary number of sub-menus and popups is available.

**DONE Update HUD: item in reach**
If ItemInReach is not null, display "Pick up " + itemName

## Network Programming

- DONE Sync initializing inventory.
- DONE Sync dropping items.
- DONE Sync eqipping items
- Sync picking up items.


##DONE Core Programming

**DONE Item category**
Items should have a category defaulting to Misc in order to organize them.
- Weapons
- Apparel
- Aid
- Misc
- Ammo

**DONE IInteract**
Interact(Actor actor = null, Item.Uses interaction)


## DONE Gameplay Programming

**DONE Actor.InitiateInteraction()**
Actor should be able to raycast forward in the center of their vision and return any Iinteract hit. 

**DONE Actor.InteractWith()**
Pressing E should interact with InteractionInReach() if possible.

**DONE Actor.DiscardItem(Item item)**
Inventory should call this to drop items onto the ground in front of the actor.

**ABORTED Actor.DropItem()**
Pressing Q should drop the equipped item.
Maybe later. Or not.

**DONE Item**
- Item should be an Iinteract, 
- Item.Push(int i = 1), Item.Pop(int i = 1) [Maybe implement IStack]

**DONE Equip/unequip**
When item is not eqipped, actor should default to unarmed melee.

**DONE Powerup**
Interacting with a powerup should cause it to work on the player as if collided with.

## AI programming
