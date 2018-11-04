/*
	A record for an item request consisting of two parts:
	
	A multipart primary key for a specific type of item
	consisting of it's type and name that can be used to
	request items from an inventory.

	A quantity with a default value of 1.

*/
using Godot;

public class ItemQuery{
	public Item.Types type;
	public string name;
	public int quantity;

	public ItemQuery(Item.Types type, string name, int quantity = 1){
		this.type = type;
		this.name = name;
		this.quantity = quantity;
	}
	
}