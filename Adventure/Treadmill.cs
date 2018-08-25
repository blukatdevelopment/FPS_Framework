/*
	Manages the logic of monitoring one actor and requesting/releasing TerrainCells
	as the actor moves across the Overworld. 
*/
using Godot;
using System;
using System.Collections.Generic;

public class Treadmill {

	public float timer = 0f;
	public float timerMax = 0.5f;

	public Overworld world;
	public int id; // Should match client's peerId in multiplayer.
	public Actor actor; // Actor to monitor.
	public TerrainCell center; // cell at center of treadmill.
	public int radius; // How many layers of Cells should be  modified.
	public Vector3 pos; // Central positon for this treadmill to move to.
	public bool recenterActive = false;

	public Treadmill(int id, Overworld world, ActorData actorData, Vector2 coords, int radius, Vector3 pos){
		this.id = id;
		this.world = world;
		this.pos = pos;
		this.center = world.RequestCell(coords);
		this.center.SetPos(pos);
		this.radius = radius;


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

	public void Init(){
		PopulateCells();
	}

	public void Update(float delta){
		timer += delta;
		if(timer >= timerMax){
			timer = 0;
			CheckPlayerPos();
		}
	}

	// Populates cells surrounding Center
	public void PopulateCells(){
		List<Vector2> usedCoords = UsedCoords();
		foreach(Vector2 used in usedCoords) {
			TerrainCell terrainCell = world.RequestCell(used);
			if(terrainCell == null){
				//GD.Print(used + " couldn't be requested");
				continue;
			}
			Vector3 position = GridToPos(used);
			terrainCell.SetPos(position);
			
			//GD.Print("Set");
		}
	}

	// Release cells surrounding arbitrary central coordinates
	public void DePopulateCells(Vector2 aCenter){
		List<Vector2> coordsToRelease = UsedCoords(aCenter);
		foreach(Vector2 coords in coordsToRelease){
			if(UsingCell(coords)){
				//GD.Print("Still using this cell. No need to ask overworld to release");
				continue;
			}
			world.ReleaseCell(coords);
		}
	}

	public void CheckPlayerPos(){
		if(recenterActive == true){
			GD.Print("Treadmill.CheckPlayerPos: recenter active. Aborting.");
			return;
		}

		if(actor == null){
			GD.Print("Treadmill.CheckPlayerPos: Actor null");
			return;
		}
		if(center.InBounds(actor.Translation)){
			return;
		}
		else{
			Vector2 newCenter = PosToGrid(actor.Translation);
			List<Vector2> newCoords =  UsedCoords(newCenter);
			GD.Print("Player has escaped from " + center.coords + " to " + newCenter);
			Recenter(newCenter);
		}
	}

	public void Recenter(Vector2 newCenterCoords){
		recenterActive = true;
		Vector2 oldCenterCoords = center.coords;
		GD.Print("Recentering from " + oldCenterCoords + " to " + newCenterCoords);
		
		TerrainCell newCenter = world.RequestCell(newCenterCoords);
		GD.Print("Starting at " + newCenter.GetCenterPos());
		if(newCenter == null){
			GD.Print("Couldn't request new center. Aborting recenter to " + newCenterCoords);	
			return;		
		}

		center = newCenter;
		ClearBoundaries();
		PopulateCells();
		DePopulateCells(oldCenterCoords);
		PopulateBoundaries();
		GD.Print("Recenter complete.");
		
		if(CellsShared()){
			GD.Print("Moving tiles not possible. Returning.");
			recenterActive = false;
			return;
		}
		MoveUsedCells(newCenter.Translation);
		GD.Print("ending at " + center.GetCenterPos());
		recenterActive = false;
	}




	// Move all used cells whilst keeping relative position
	public void MoveUsedCells(Vector3 end){
		Vector2 centerCoords = center.coords;
		float width = center.GetWidth();
		Vector3 centerPos = end;

		List<Vector2> usedCoords = UsedCoords();

		foreach(Vector2 coords in usedCoords){
			TerrainCell usedCell = world.RequestCell(coords);
			if(usedCell != null){
				Vector3 newPos = GridToPos(coords, centerCoords, width, centerPos);
				usedCell.SetPos(newPos);
			}	
		}

	}

	// Move used cells to this position.
	public void MoveCellsToPos(Vector3 position){
		center.SetPos(position);
		List<Vector2>  usedCoords = UsedCoords();
		foreach(Vector2 used in usedCoords){
			TerrainCell cell = world.RequestCell(used);
			if(cell != null){
				Vector3 cellPos = GridToPos(used);
				cell.SetPos(cellPos);
			}
		}
	}


	// Clear out all boundaries
	public void ClearBoundaries(){
		GD.Print("Treadmill.ClearBoundaries not implemented");
	}

	// Create invisible walls in invalid or unused grid positions surrounding treadmill
	public void PopulateBoundaries(){
		GD.Print("Treadmill.PopulateBoundaries not implemented");
	}

	// Find grid coords based on position relative to center terrainCell
	public Vector2 PosToGrid(Vector3 pos){
		if(center.InBounds(pos)){
			return center.coords;
		}
		Vector3 centerPos = center.GetCenterPos();
		Vector3 distance =  pos - centerPos;
		float width = center.GetWidth();

		// offsets in X and Y coords
		int xDiff = (int)distance.x/(int)width;
		int yDiff = (int)distance.z/(int)width;

		if(xDiff == 0 && distance.x > width/2f){
			xDiff = 1;
		}
		else if(xDiff == 0 && distance.x < width/-2f){
			xDiff = -1;
		}
		if(yDiff == 0 && distance.z > width/2f){
			yDiff = 1;
		}
		else if(yDiff == 0 && distance.z < width/-2f){
			yDiff = -1;
		}

		Vector2 diff = new Vector2(xDiff, yDiff);
		return center.coords + diff;
	}

	// All coordinates this treadmill cares about.
	public List<Vector2> UsedCoords(){
		List<Vector2> ret = UsedCoords(center.coords);
		//GD.Print("ret" + ret);
		return ret;
	}

	public List<Vector2> UsedCoords(Vector2 centerCoords){
		List<Vector2> ret = new List<Vector2>();
		for(int i = -radius; i <= radius; i++){
			for(int j = -radius; j <= radius; j++){
				Vector2 offset = new Vector2(i, j);
				Vector2 coord = centerCoords + offset;
				ret.Add(coord);
			}
		}
		return ret;
	}

	
	// Defaults to use current center.
	public Vector3 GridToPos(Vector2 coords){
		Vector2 centerCoords = center.coords;
		float width = center.GetWidth();
		Vector3 centerPos = center.GetCenterPos();
		return GridToPos(coords, centerCoords, width, centerPos);
	}

	// Find position for a given grid relative to center.
	public Vector3 GridToPos(Vector2 coords, Vector2 centerCoords, float width, Vector3 centerPos){
		Vector2 dCoords = coords - centerCoords; // Diff in coordinates
		Vector2 dUnits = dCoords * width; // diff in world space
		Vector3 start = centerPos;
		Vector3 end = start + new Vector3(dUnits.x, 0f, dUnits.y);
		return end;
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
		List<Vector2> usedCoords = UsedCoords();
		foreach(Vector2 used in usedCoords){
			if(used.x == coords.x && used.y == coords.y){
				//GD.Print("Found cell at " + coords);
				return true;
			}
		}
		//GD.Print("Didn't find cell at " + coords);
		return false;
	}


	// Returns true if at least one cell is used by another treadmill
	public bool CellsShared(){
		List<Vector2> usedCoords = UsedCoords();
		foreach(Vector2 used in usedCoords){
			List<Treadmill> users = world.CellUsers(used);
			if(users.Count > 1){
				return true;
			}
		}
		return false;
	}
}