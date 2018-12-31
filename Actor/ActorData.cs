/*
	The dormant representation of an actor that can be serialized and stuffed into
	sqlite tables.
*/
using Godot;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class ActorData {
	public Actor.Brains brain;
	public string name;
	public int id, health, healthMax;
	public Vector3 pos, rot;
	public Inventory inventory;

	// Extra data
	public System.Collections.Generic.Dictionary<string, string> extra;

	public ActorData(){
		id = -1;
		health = healthMax = 0;
		inventory = new Inventory();
		pos = new Vector3();
		rot = new Vector3();
		extra = new System.Collections.Generic.Dictionary<string, string>();
	}

	public override string ToString(){
		int x = (int)pos.x;
		int y = (int)pos.y;
		int z = (int)pos.z;

		string ret = "Actor[" + id + "]:\n";
		
		ret += "\tname: " + name + "\n";
		ret += "\tbrain: " + brain + "\n";
		ret += "\tPos: [" + x + "," + y + "," + z + "] \n";

		return ret;
	}

	public static ActorData FromJson(string json){
		return JsonConvert.DeserializeObject<ActorData>(json);
	}

	public static string ToJson(ActorData dat){
		return JsonConvert.SerializeObject(dat, Formatting.Indented);
	}
}