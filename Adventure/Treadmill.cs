/*
	Manages the logic of monitoring one actor and requesting/releasing TerrainCells
	as the actor moves across the Overworld. 
*/
using Godot;
using System;
using System.Collections.Generic;

public class Treadmill {
	public Overworld world;
	public int id; // Should match client's peerId in multiplayer.
	public Actor actor; // Actor to monitor.
	public TerrainCell center; // cell at center of treadmill.
	public int radius; // How many layers of Cells should be  modified.

	public Treadmill(int id, Overworld world, ActorData actor, Vector2 coords, int radius){
		this.id = id;
		this.world = world;
		this.actor = CreatePlayer(actor);
		this.center = world.RequestCell(coords);

		List<string> missingArgs = new List<string>();
		if(world == null){
			missingArgs.Add("world");
		}
		if(actor == null){
			missingArgs.Add("actor");
		}
		if(center == null){
			missingArgs.Add("center");
		}
		if(missingArgs.Count > 0){
			foreach(string arg in missingArgs){
				GD.Print("Treadmill: Arg is null: " + arg);
			}
			return;
		}


	}

	public Actor CreatePlayer(ActorData dat){
		return null;
	}

	// Returns true when item's position is inside treadmill.
	public bool UsingItem(Item item){
		GD.Print("UsingItem not implemented.");
		return true;
	}

	// Returns true when actor's observed by or inside treadmill
	public bool UsingActor(Actor actor){
		GD.Print("UsingActor not implemented.");
		return true;
	}

	// Returns true if cell is in 
	public bool UsingCell(Vector2 coords){
		GD.Print("UsingCell not implemented.");
		return true;
	}
}