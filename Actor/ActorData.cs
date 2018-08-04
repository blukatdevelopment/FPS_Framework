/*
	The dormant representation of an actor that can be serialized and stuffed into
	sqlite tables.
*/
using Godot;
using System;

public class ActorData {
	public string name;
	public int id, health, healthMax;
	public Vector3 pos, rot;

	public ActorData(){
		pos = new Vector3();
		rot = new Vector3();
	}
	// TODO: Serialize inventory

	public override string ToString(){
		int x = (int)pos.x;
		int y = (int)pos.y;
		int z = (int)pos.z;
		string ret = "ActorData:[" + health + "/" + healthMax + "][" + x + "," + y + "," + z + "]";
		ret += "Name: " + name;
		return ret;
	}
}