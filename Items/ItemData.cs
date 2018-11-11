/* 
	A serializeable storage class for items.
*/
using System;
using System.Collections.Generic;
using Godot;

public class ItemData : IHasInfo {
	
	public int id;
	public Item.Types type;
	public string name;
	public string description;
	public int weight;
	public Vector3 pos, rot;
	public bool held;
	public System.Collections.Generic.Dictionary<string, string> extra;
	public List<int> stack;
	public bool stackable;

	public ItemData(){
		pos = new Vector3();
		rot = new Vector3();

		extra = new System.Collections.Generic.Dictionary<string, string>();
		stack = new List<int>();
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
		ret.pos = original.pos;
		ret.rot = original.rot;
		ret.stack = new List<int>(original.stack.ToArray());

		foreach(string key in original.extra.Keys){
			ret.SetExtra(key, original.extra[key]);
		}

		ret.description = original.description;

		return ret;
	}

	public string ToString(){
		string ret = name;
		return ret;
	}

	public List<int> GetStack(){
    return stack;
  }

  public bool Push(ItemData item){
  	if(!stackable || item.type != this.type){
      return false;
    }

    stack.Add(item.id);
    stack.AddRange(item.GetStack());
    return true;
  }

  public ItemData Pop(int quantity = 1){
    if(stack.Count == 0 || quantity > stack.Count){
      return null;
    }

    ItemData ret = Clone(this);
    ret.stack = new List<int>();

    ret.id = stack[0];
    stack.RemoveAt(0);

    for(int i = 1; i < quantity; i++){
      ret.stack.Add(stack[0]);
      stack.RemoveAt(0);
    }

    return ret;
  }
}