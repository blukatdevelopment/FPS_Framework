/* 
	A serializable form of an item for stashing in inventories or databses. 
	ItemData is a record to be written to by specific Item classes when they
	need to be stored as Json and then passed as an argument to Item.Factory()
	to then be handed off to the appropriate Item class to be read.

	At the same time, ItemData in an inventory holds the info necessary to
	interact with it in menus.
*/

public class ItemData : IHasInfo {
	

	// These should be used by classes like Inventory, but NOT for ReadData
	public Item.Types type;
	public string name;
	public string description;
	public int quantity;

	// These should be handled by ReadData/GetData
	public List<int> ints;
	public List<string> strings;
	public List<float>  floats;


	public string GetInfo(){
		return name;
	}

	public string GetMoreInfo(){
		return description;
	}
}