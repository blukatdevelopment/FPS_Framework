using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;


public class Item : RigidBody, IHasInfo, IUse {
	public enum Uses{ 
	  A, // Primary use (Left Mouse) 
	  B, // Secondary Use(Right Mouse)
	  C, // Third Use(Middle mouse)
	  D, // Fourth Use(R)
	  E,
	  F, 
	  G 
	};
	
  public enum Types{
    None,
    Hand,
    Rifle
  };
  
  
  public Types type;
  public string name;
  public string description;
  public int quantity;
  public int quantityMax;
  
  public void BaseInit(string name, string description, int quantity = 1, int quantityMax = 1){
    this.name = name;
    this.description = description;
    this.quantity = quantity;
    this.quantityMax = quantityMax;
  }
  
  public virtual void Use(Uses use){
    GD.Print("Item used " + use);
  }
  
  public virtual string GetInfo(){
    return name;
  }
  
  public virtual string GetMoreInfo(){
    return description;
  }
  
  
  /* Returns a base/simple item by it's name. */
	public static Item Factory(Types type){
		Item ret = null;
		if(type == Types.None){
		  return null;
		}
		string itemFile = ItemFiles(type);
		
		ret = (Item)Session.Instance(itemFile);
		
		switch(type){
		  case Types.Hand: 
		    ret.BaseInit("Hand", "Raised in anger, it is a fist!");  
		    break;
		  case Types.Rifle: 
		    ret.BaseInit("Rifle", "There are many like it, but this one is yours.");
		    break;
		}
		
		
    
		
		return ret;
	}
	
	public static string ItemFiles(Types type){
	  string ret = "";
	  
	  switch(type){
			case Types.Hand: ret = "res://Scenes/Prefabs/Items/Hand.tscn"; break;
			case Types.Rifle: ret = "res://Scenes/Prefabs/Items/Rifle.tscn"; break;
		}
		
		return ret;
	}

}
