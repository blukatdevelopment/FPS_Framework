/*
	Manages an unordered list of ItemData
*/

using System;
using System.Collections.Generic;
using Godot;

public class Inventory : IHasItem {
	List<ItemData> items;

	public Inventory(){
		items = new List<ItemData>();
	}

	public void Init(List<ItemData> items){
		this.items = items;
	}

	/*
		Removes items with matching type and name.
		If max is greater than zero, will return up to that many items.
	*/
	public List<ItemData> RetrieveItems(Item.Types itemType, string itemName, int max = 0){
		List<ItemData> ret = new List<ItemData>();

		foreach(ItemData data in items){
			if(max > 0 && ret.Count >= max){
				break;
			}
			if(data.type != itemType || itemName != data.name){
				continue;
			}
			if(data.stackable){
				int startingQuantity = data.GetQuantity();
				ret.Add(data.Pop(max));
			}
			else{
				ret.Add(data);	
			}
		}

		foreach(ItemData data in ret){
			items.Remove(data);
		}
		return ret;
	}

	public int GetQuantity(Item.Types itemType, string itemName){
		int ret = 0;
		foreach(ItemData data in items){
			if(data.type == itemType && itemName == data.name){
				ret += data.GetQuantity();
			}
		}
		return ret;
	}

	public ItemData GetItem(int index){
		if(index < 0 || index >= items.Count){
			return null;
		}
		return ItemData.Clone(items[index]);
	}

	public ItemData RetrieveItem(int index, int quantity = 1){
		if(index >= items.Count || index < 0){
			return null;
		}

		ItemData ret;

		if(items[index].stackable){
			int startingQuantity = items[index].GetQuantity();

			ret = items[index].Pop(quantity);
			if(startingQuantity == quantity){
				items.RemoveAt(index);
			}
		}
		else{
			ret = items[index];
			items.Remove(ret);	
		}

		return ret;
	}

	public int IndexOf(Item.Types type, string name){
		for(int i = 0; i < items.Count; i++){
			ItemData item = items[i];
			if(item.type == type && item.name == name){
				return i;
			}
		}
		return -1;
	}

	public string ToString(){
		string ret = "Inventory:\n";
		foreach(ItemData item in items){
			ret += item.ToString() + "\n";
		}
		return ret;
	}

	public bool HasItem(string itemName){
		foreach(ItemData item in items){
			if(itemName == item.name){
				return true;
			}
		}
		return false;
	}

	public string ItemInfo(){
		return ToString();
	}

	public void StoreItemDataRange(List<ItemData> data){
		foreach(ItemData dat in data){
			StoreItemData(dat);
		}
	}

	public void StoreItemData(ItemData data){
		
		if(!data.stackable){
			items.Add(data);
			return;	
		}

		int index = StackableIndex(data);
		
		if(index == -1){
			items.Add(data);
			return;
		}

		items[index].Push(data);
	}

	public int StackableIndex(ItemData data){
		for(int i = 0; i < items.Count; i++){
			ItemData dat = items[i];
			if(dat.type == data.type && dat.name == data.name){
				return i;
			}
		}

		return -1;
	}

	public void ReceiveItemRange(List<Item> items){
		foreach(Item item in items){
			ReceiveItem(item);
		}
	}

	public bool ReceiveItem(Item item){
		ItemData data = item.GetData();
		StoreItemData(data);
		return true;
	}

	public Item PrimaryItem(){
		return null;
	}

	public int ItemCount(){
		int ret = 0;
		foreach(ItemData item in items){
			ret += item.GetQuantity();
		}
		return ret;
	}

	public List<ItemData> GetAllItems(){
		List<ItemData> ret = new List<ItemData>();
		foreach(ItemData dat in items){
			ret.Add(dat);
		}
		return ret;
	}

	public List<ItemData> RetrieveAllItems(){
		List<ItemData> ret = items;
		items = new List<ItemData>();
		return ret;
	}

}