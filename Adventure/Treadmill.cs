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
	public Vector3 pos;

	public Treadmill(int id, Overworld world, ActorData actorData, Vector2 coords, int radius, Vector3 pos){
		this.id = id;
		this.world = world;
		this.pos = pos;
		this.center = world.RequestCell(coords);
		this.center.SetPos(pos, world.GetCellSize());


		actorData.pos = pos + new Vector3(0, 10f, 0); // TODO: Make this not hardcoded.
		this.actor = world.ActivateActorData(Actor.Brains.Player1, actorData);
		Session.ChangeMenu(Menu.Menus.HUD);
		

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