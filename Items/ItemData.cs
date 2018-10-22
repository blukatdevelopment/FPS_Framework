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
	public Vector3 pos, rot;
	public bool held;

	// Store all custom data added by subclasses of Item
	public System.Collections.Generic.Dictionary<string, string> extra;

	public ItemData(){
		pos = new Vector3();
		rot = new Vector3();

		extra = new System.Collections.Generic.Dictionary<string, string>();
	}

	public string GetExtra(string key){
		if(!extra.ContainsKey(key)){
			return "";
		}
		return extra[key];
	}

	public void SetExtra(string key, string val){
		if(extra.ContainsKey(key)){
			extra[key] = val;
			return;
		}
		extra.Add(key, val);
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
		ret.pos = original.pos;
		ret.rot = original.rot;
		
		foreach(string key in original.extra.Keys){
			ret.SetExtra(key, original.extra[key]);
		}

		ret.description = original.description;

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

	// TODO: Re-implement, maybe check for stackable key
	public bool CanStack(ItemData other){
		return true;
	}


	public int GetWeight(){
		return weight * quantity;
	}

}