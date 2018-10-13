/* 
	A serializable form of an item for stashing in inventories or databses. 
	ItemData is a record to be written to by specific Item classes when they
	need to be stored as Json and then passed as an argument to Item.Factory()
	to then be handed off to the appropriate Item class to be read.

	At the same time, ItemData in an inventory holds the info necessary to
	interact with it in menus.
*/
using System;
using System.Collections.Generic;
using Godot;

public class ItemData : IHasInfo {
	

	// These should be used by classes like Inventory, but NOT for ReadData
	// When searching an inventory for an item, (type, name) are the primary key.
	public int id; // primary identifier
	public Item.Types type;
	public string name;
	public string description;
	public int quantity;
	public int weight;
	public Vector3 pos;


	// These should be handled by ReadData/GetData
	public List<int> ints;
	public List<string> strings;
	public List<float>  floats;


	public ItemData(){
		ints = new List<int>();
		strings = new List<string>();
		floats = new List<float>();
		pos = new Vector3();
	}

	public string GetInfo(){
		return name;
	}

	public string GetMoreInfo(){
		return description;
	}

	public static ItemData Clone(ItemData original){
		ItemData ret = new ItemData();
		ret.type = original.type;
		ret.name = original.name;
		ret.quantity = original.quantity;

		ret.description = original.description;
		ret.ints = new List<int>(original.ints as IEnumerable<int>);
		ret.strings = new List<string>(original.strings as IEnumerable<string>);
		ret.floats = new List<float>(original.floats as IEnumerable<float>);
		return ret;
	}

	public string ToString(){
		string ret = name + "(" + quantity + ")";
		return ret;
	}

	public ItemData Remove(int quant = 1){
		if(quant <= 0){
			return null;
		}

		ItemData ret = Clone(this);
		
		if(quant >= quantity){
			quantity = 0;
		}
		else{
			ret.quantity = quant;
			quantity -= quant;
		}
		return ret;
	}

	// Add stacking item and return overflow.
	public int Add(ItemData item){
		if(!CanStack(item)){
			return item.quantity;
		}

		quantity += item.quantity;
		return 0;
	}

	// Items of the same name and type are considered equivalent.
	public bool CanStack(ItemData other){
		if(type != other.type || name != other.name){
			return false;
		}
		if(ints.Count != other.ints.Count 
			|| strings.Count != other.strings.Count
			|| floats.Count != other.floats.Count){
			return false;
		}
		return true;
	}


	public int GetWeight(){
		return weight * quantity;
	}

}