using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;


public class Item : Node {
	public enum Uses{ A, B, C, D, E, F, G };



	public static Node Factory(string itemName){
		Node ret = null;
		switch(itemName.ToLower()){
			case "none": ret = null; break;
		}
		return ret;
	}

}
