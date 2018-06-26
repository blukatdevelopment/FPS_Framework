/*
	Manages an unordered list of ItemData
*/

using System;
using System.Collections.Generic;
using Godot;

public class Inventory {
	List<ItemData> items;

	public Inventory(){
		items = new List<ItemData>();
	}

	public void Init(List<ItemData> items){
		this.items = items;
	}

	// Doesn't change value.
	public ItemData GetItem(int index){
		if(index < 0 || index > items.Count){
			return null;
		}
		return items[index];
	}

	// Changes value.
	public ItemData RetrieveItem(int index, int quantity = 1){
		ItemData item = GetItem(index);
		if(item == null){
			return null;
		}
		return item.Remove(quantity);
	}
}