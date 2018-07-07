using System;
interface IInteract{
	// Describe interaction
	string GetInteractionText(Item.Uses interaction);
	// Perform interaction
	void Interact(object interactor, Item.Uses interaction);
}