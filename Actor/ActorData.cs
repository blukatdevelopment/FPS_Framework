/*
	The dormant representation of an actor that can be serialized and stuffed into
	sqlite tables.
*/
using Godot;
using System;

public class ActorData {
	public int id, health, healthMax;
	public Vector3 pos, rot;

	public ActorData(){
		pos = new Vector3();
		rot = new Vector3();
	}
	// TODO: Serialize inventory
}