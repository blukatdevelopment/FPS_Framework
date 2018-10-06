/*
	The overworld is responsible for managing the world's TerrainCells, 
	Actors, and Items as they convert between rendered and unrendered
	representations.

	Regardless of representation, only the behavior (influenced by updating in
	a real-time 3D or turn-based non-physics format) should change, leaving
	each terraincell in the same position for all game instances.
*/
using Godot;
using System;
using System.Collections.Generic;

public class Overworld : Spatial {
	public List<Treadmill> treadmills;
	public System.Collections.Generic.Dictionary<int, TerrainCell> activeCells;


	public List<Actor> activeActors;
	public List<Item> activeItems;

	public System.Collections.Generic.Dictionary<int, TerrainCellData> dormantCells;
	public System.Collections.Generic.Dictionary<int, ItemData> dormantItems;
	public System.Collections.Generic.Dictionary<int, ActorData> dormantActors;
	public bool paused = false;

	public const int WorldWidth = 50;
	public const int WorldHeight = 50;
	public const int CellSize = 50; // x, y, z length in TerrainBlocks.

	private int nextActorId, nextItemId; // auto-incremented ids.


	public Overworld(){
		treadmills = new List<Treadmill>();
		activeActors = new List<Actor>();
		activeCells = new System.Collections.Generic.Dictionary<int, TerrainCell>();
		activeItems = new List<Item>();

		Name = "Adventure";
		if(Session.IsServer()){
			ServerInit();
		}
		else if(Session.NetActive()){
			ClientInit();
		}
		else{
			SinglePlayerInit();
		}
	}

	public override void _Process(float delta){
		if(!paused){
			UpdateTreadmills(delta);
		}
	}

	public void ServerInit(){
		//GD.Print("Server init");
	}

	public void ClientInit(){
		//GD.Print("Client init");
	}

	public void SinglePlayerInit(){
		string debug = "SinglePlayerInit\n";

		InitWorld();
	}

	/* Asks the cartographer to make a new world
	 TODO: or loads an existing one */
	public void InitWorld(){
		Cartographer cart = new Cartographer();
		
		cart.GenerateWorld(this);
		dormantCells = cart.cells;
		dormantItems = cart.items;
		dormantActors = cart.actors;

		DebugInit();
	}

	// This can go away once player is moving around in world.
	Camera debugCam;
	public void DebugInit(){
		//debugCam = new Camera();
		//AddChild(debugCam);
		InspectWorld();
	}

	public void InspectWorld(){
		//GD.Print("Dormant cells: " + dormantCells.Count);
		TerrainCellData terrainDat = dormantCells[0];
		//GD.Print("Cell[0] blocks: " + terrainDat.blocks.Count);
		ActorData playerDat = dormantActors[0];
		//GD.Print("Dormant actor: " + playerDat);
	}

	private int NextActorId(){
		int ret = nextActorId;
		nextActorId++;
		return ret;
	}

	private int NextItemId(){
		int ret = nextItemId;
		nextItemId++;
		return ret;
	}

	/* Returns a new actor with next Id*/
	public ActorData CreateActor(){
		ActorData dat = new ActorData();
		dat.id = NextActorId();
		dat.healthMax = 100;
		dat.health = 100;

		return dat;
	}

	/* Return a new item with next id */
	public ItemData CreateItem(){
		int id = NextItemId();
		
		//GD.Print("Overworld.CreateItem not implemented");
		return null;
	}

	/* Returns id for given TerrainCell coordinates,
	 or -1 for invalid coordinates */
	public int CoordsToCellId(int x, int y){
		if(y < 0 || y >= WorldHeight || x < 0 || x >= WorldWidth){
			//GD.Print("Invalid coords:" + x + ", " + y);
			return -1;
		}
		return y * WorldWidth + x;
	}

	/* Returns id for given TerrainCell coordinates,
	 or -1 for invalid coordinates */
	public int CoordsToCellId(Vector2 coords){
		int x = (int)coords.x;
		int y = (int)coords.y;
		return CoordsToCellId(x, y);
	}

	/*
		Determine what cell a given position falls in.
	*/
	public int PositionToCellId(Vector2 pos){
		//GD.Print("Overworld.PositionToCellId not implemented");
		return -1;
	}

	public int GetWorldWidth(){
		return WorldWidth;
	}

	public int GetWorldHeight(){
		return WorldHeight;
	}

	public int GetCellSize(){
		return CellSize;
	}

  public Treadmill GetTreadmillById(int treadmillId){
  	foreach(Treadmill treadmill in treadmills){
  		if(treadmill.id == treadmillId){
  			return treadmill;
  		}
  	}
  	return null;
  }

	public void UpdateTreadmills(float delta){
		foreach(Treadmill treadmill in treadmills){
			treadmill.Update(delta);
		}
	}

	// Size of each block used in gridmaps
	public static Vector3 BaseBlockSize(){
		return new Vector3(6, 6, 6);
	}

	// width and length of each TerrainCell
	public static float CellWidth(){
		Vector3 baseBlockSize = BaseBlockSize();
		return baseBlockSize.x * CellSize;
	}

	public List<Treadmill> CellUsers(Vector2 coords){
		int id = CoordsToCellId(coords);
		return CellUsers(id);
	}

	public List<Treadmill> CellUsers(int cellId){
		if(cellId == -1){
			//GD.Print("Overworld.CellUsers: Invalid cellID:" + cellId);
			return new List<Treadmill>();
		}
		if(!activeCells.ContainsKey(cellId)){
			//GD.Print("Overworld.CellUsers: Cell not active:" + cellId);
			return new List<Treadmill>();	
		}
		TerrainCell cell = activeCells[cellId];
		Vector2 coords = cell.coords;

		List<Treadmill> users = new List<Treadmill>();
		foreach( Treadmill treadmill in treadmills){
			if(treadmill.UsingCell(coords)){
				users.Add(treadmill);
			}
		}
		return users;	
	}

	/*
		Instanciate item from an inventory. (Or projectiles from a weapon)
	*/
	public Item DropItem(ItemData data){
		return null;
	}

	public TerrainCell RequestCell(Vector2 coords){
		//TODO
		return null;
	}

	public void ReleaseCell(Vector2 coords){
		// TODO
	}

	public void StoreCell(int id){
		// TODO
	}

	/*
			########################################################################
														Session interactions
			########################################################################
	*/

	public void HandleEvent(SessionEvent sessionEvent){
    if(sessionEvent.type == SessionEvent.Types.ActorDied ){
      HandleActorDead(sessionEvent);
    }
    else if(sessionEvent.type == SessionEvent.Types.Pause){
     	TogglePause();
    }
  }

  public void HandleActorDead(SessionEvent sessionEvent){
  	//GD.Print("HandleActorDead not implemented");
  }

  public void TogglePause(){
    foreach(Actor actor in activeActors){
      actor.TogglePause();
    }
  }

  public void Pause(){
		paused = true;
	}

	public void Unpause(){
		paused = false;
	}

  public string GetObjectiveText(){
  	return "Take a look around.";
  }

}