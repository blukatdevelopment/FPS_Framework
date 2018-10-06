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
	public int id; // Should match client's peerId in multiplayer. -1 for offline play.
	public ActorData actorData; // Data to init actor with
	public Actor actor; // Actor to monitor.
	public TerrainCell center; // cell at center of treadmill.
	public int radius; // How many layers of Cells should be  modified.
	public Vector3 pos; // Central positon for this treadmill to move to.
	public bool recenterActive = false;
	public bool paused = false;

	/*
		id is -1 for offline play
	*/
	public Treadmill(int id, Overworld world, ActorData actorData, Vector2 coords, int radius, Vector3 pos){
		this.id = id;
		this.world = world;
		this.pos = pos;
		this.center = world.RequestCell(coords);
		this.center.SetPos(pos);
		this.radius = radius;
		this.actorData = actorData;

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
				//GD.Print("Treadmill: Arg is null: " + arg);
			}
			return;
		}
	}

	public void Pause(){
		paused = true;
	}

	public void Unpause(){
		paused = false;
	}

	public void Init(){
		PopulateCells();
		PlayerInit();
	}

	public void PlayerInit(){
		actorData.pos = center.GetPos() + new Vector3(0, 20f, 0); // TODO: Make this not hardcoded.
		actor = world.ActivateActorData(Actor.Brains.Player1, actorData);
		Session.ChangeMenu(Menu.Menus.HUD);
	}

	public void Update(float delta){
		if(paused){
			return;
		}
		timer += delta;
		if(timer >= timerMax){
			timer = 0;
			CheckPlayerPos();
		}
	}

	// Populates cells surrounding Center
	public void PopulateCells(){
		System.Collections.Generic.Dictionary<Vector2, Vector3> map = GetPopulatedCellMap();
		ApplyCellMap(map);
		PopulateBoundaries();
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
			//GD.Print("Treadmill.CheckPlayerPos: recenter active. Aborting.");
			return;
		}

		//GD.Print("Player is currently at " +  actor.Translation + "," +  PosToCoords(actor.Translation));

		if(actor == null){
			//GD.Print("Treadmill.CheckPlayerPos: Actor null");
			return;
		}
		if(center.InBounds(actor.Translation)){
			return;
		}
		else{
			Vector2 newCenter = PosToCoords(actor.Translation);
			//List<Vector2> newCoords =  UsedCoords(newCenter);
			//GD.Print("Player has escaped from " + center.coords + " to " + newCenter);
			Recenter(newCenter);
		}
	}

  public System.Collections.Generic.Dictionary<Vector2, Vector3> GetPopulatedCellMap(){
		// Vector2 centerCoords = center.coords;
		// Vector3 centerPos = pos;
		// List<Vector2> candidateCoords = Util.CoordsInRadius(centerCoords, radius);
		// float scale = center.GetWidth();
		// List<Vector2> finalCoords = new List<Vector2>();

		// foreach(Vector2 coords in candidateCoords){
		// 	if(world.RequestCell(coords) != null){
		// 		finalCoords.Add(coords);
		// 	}
		// }

	  //  return Util.GetCellMap(finalCoords, centerCoords, centerPos, scale);

  	// Get map of potential cells and their positions.
		Vector2 centerCoords = center.coords;
		Vector3 centerPos = center.GetPos();
		List<Vector2> candidateCoords = Util.CoordsInRadius(centerCoords, radius);
		float scale = center.GetWidth();
		System.Collections.Generic.Dictionary<Vector2, Vector3> candidateMap;
		candidateMap = Util.GetCellMap(candidateCoords, centerCoords, centerPos, scale);
		

		System.Collections.Generic.Dictionary<Vector2, Vector3> ret;
		ret = new System.Collections.Generic.Dictionary<Vector2, Vector3>();

		foreach(Vector2 key in candidateMap.Keys){
			Vector2 coords = key;
			Vector3 position = candidateMap[key];

			TerrainCell cell = world.RequestCellAtPos(coords, true, position);
			if(cell != null){
				ret.Add(coords, position);
			}
		}

		return ret;

	}

	// Reposition cells according to positions in the cell map
	public void ApplyCellMap(
		System.Collections.Generic.Dictionary<Vector2, Vector3> map
	){
		// Pack all cells.
		foreach(Vector2 key in map.Keys){
			TerrainCell cell = world.RequestCell(key);
			if(cell != null){
				cell.Pack();
			}
		}

		// Unpack all cells in new location.
		foreach(Vector2 key in map.Keys){
			TerrainCell cell = world.RequestCell(key);
			if(cell != null){
				cell.Unpack(map[key]);
			}
		}
	}

	public Vector2 CenterCoords(){
		return center.coords;
	}

	public void Recenter(Vector2 newCenterCoords){
		if(recenterActive){
			//GD.Print("Another treadmill recenter is in progress. Aborting.");
			return;
		}

		if(world.RequestCell(newCenterCoords) == null){
			//GD.Print("Couldn't recenter treadmill to invalid coordinates" + newCenterCoords);
			return;
		}

		if(center.coords == newCenterCoords){
			//GD.Print("Treadmill.Recenter: Will not recenter to current position. Aborting.");
			return;
		}

		recenterActive = true;

		ReleaseAbandonedCells(CenterCoords(), newCenterCoords);
		ClearBoundaries();

		Vector3 oldCenterPos = center.GetPos();

		center = world.RequestCell(newCenterCoords);

		Vector3 newCenterPos = center.GetPos();


		string debug = "Treadmill.Recenter\n";
		debug += "\t Treadmill recentered to " + center.coords + "\n";
		debug += "\t moving from " + oldCenterPos + " to " + newCenterPos + ", " + (newCenterPos - oldCenterPos) + "\n";
		//GD.Print(debug);

		System.Collections.Generic.Dictionary<Vector2, Vector3> map = GetPopulatedCellMap();
		ApplyCellMap(map);
		PopulateBoundaries();
		
		// if(CellsShared()){
		// 	GD.Print("Treadmill.Recenter: Do some fancy shared cell logic.");
		// }

		recenterActive = false;

	}

	/* Release any cells that will no longer be used after a recenter. */
	public void ReleaseAbandonedCells(Vector2 fromCenter, Vector2 toCenter){
		List<Vector2> fromCoords = Util.CoordsInRadius(fromCenter, radius);
		List<Vector2> toCoords = Util.CoordsInRadius(toCenter, radius);

		foreach(Vector2 fCoords in fromCoords){
			bool found = false;
			foreach(Vector2 tCoords in toCoords){
				if(fCoords.x == tCoords.x && fCoords.y == tCoords.y){
					found = true;
				}
			}
			if(!found){
				world.ReleaseCell(fCoords);
			}
		}
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
		//GD.Print("Treadmill.ClearBoundaries not implemented");
	}

	// Create invisible walls in invalid or unused grid positions surrounding treadmill
	public void PopulateBoundaries(){
		//GD.Print("Treadmill.PopulateBoundaries not implemented");
	}

	// Find grid coords based on position relative to center terrainCell
	public Vector2 PosToCoords(Vector3 pos){
		Vector3 centerPos = center.GetPos();
		Vector2 centerCoords = center.coords;
		float centerScale = center.GetWidth();

		return Util.PosToCoords(centerPos, centerCoords, centerScale, pos);
	}

	// All coordinates this treadmill cares about.
	public List<Vector2> UsedCoords(){
		return UsedCoords(center.coords);
	}

	// Return all coords in radius
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
		Vector3 centerPos = center.GetPos();
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
		//GD.Print("UsingItem not implemented.");
		return true;
	}

	// Returns true when actor's observed by or inside treadmill
	public bool UsingActor(Actor actor){
		//GD.Print("UsingActor not implemented.");
		return true;
	}

	// Returns true if cell is in radius
	public bool UsingCell(Vector2 coords){
		List<Vector2> usedCoords = UsedCoords();
		foreach(Vector2 used in usedCoords){
			if(used.x == coords.x && used.y == coords.y){
				return true;
			}
		}
		return false;
	}


	// Returns true if at least one cell is used by another treadmill
	public bool CellsShared(){
		List<Vector2> usedCoords = Util.CoordsInRadius(CenterCoords(), radius);
		foreach(Vector2 used in usedCoords){
			List<Treadmill> users = world.CellUsers(used);
			if(users.Count > 1){
				return true;
			}
		}
		return false;
	}
}