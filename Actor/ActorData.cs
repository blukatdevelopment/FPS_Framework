/*
	The dormant representation of an actor that can be serialized and stuffed into
	sqlite tables.
*/
using Godot;
using System;
using System.Collections.Generic;

public class ActorData {
	public Actor.Brains brain;
	public string name;
	public int id, health, healthMax;
	public Vector3 pos, rot;

	// Extra data
	public System.Collections.Generic.Dictionary<string, string> extra;

	public ActorData(){
		pos = new Vector3();
		rot = new Vector3();
		extra = new System.Collections.Generic.Dictionary<string, string>();
	}

	public override string ToString(){
		int x = (int)pos.x;
		int y = (int)pos.y;
		int z = (int)pos.z;
		string ret = "ActorData:[" + health + "/" + healthMax + "][" + x + "," + y + "," + z + "]";
		ret += "Name: " + name;
		return ret;
	}
}