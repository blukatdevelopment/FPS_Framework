using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;


public class Item : RigidBody, IHasInfo {
	public enum Uses{ A, B, C, D, E, F, G };
  public enum Types{
    Hand,
    Rifle
  };
  
  public Types type;
  public string name;
  public string description;
  public int quantity;
  public int quantityMax;  
  
  public string GetInfo(){
    return name;
  }
  
  public string GetMoreInfo(){
    return description;
  }
  
  
	public static RigidBody Factory(string itemName){
		RigidBody ret = null;
		switch(itemName.ToLower()){
			case "none": ret = null; break;
		}
		return ret;
	}
	
	

}
