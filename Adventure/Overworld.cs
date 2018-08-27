/*
	The overworld manages the active portions of the world
	contained in Treadmills (TerrainCells, Actors, Items)
	as well as the inactive portions of the world
	contained TerrainCellData and IAgents. 
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
		UpdateTreadmills(delta);
	}

	public void ServerInit(){
		GD.Print("Server init");
	}

	public void ClientInit(){
		GD.Print("Client init");
	}

	public void SinglePlayerInit(){
		GD.Print("SinglePlayerInit");
		InitWorld();
		ActorData actor = LoadDormantActor(-1);
		Treadmill treadmill = new Treadmill(-1, this, actor, new Vector2(), 1, new Vector3());
		treadmills.Add(treadmill);
		treadmill.Init();
	}

	/* Asks the cartographer to make a new world
	 or loads an existing one */
	public void InitWorld(){
		GD.Print("InitTerrain not implemented");
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
		GD.Print("Dormant cells: " + dormantCells.Count);
		TerrainCellData terrainDat = dormantCells[0];
		GD.Print("Cell[0] blocks: " + terrainDat.blocks.Count);
		ActorData playerDat = dormantActors[0];
		GD.Print("Dormant actor: " + playerDat);
	}

	/*
			########################################################################
														Dormant world manipulation
			########################################################################
	*/

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
		
		GD.Print("Overworld.CreateItem not implemented");
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
		GD.Print("Overworld.PositionToCellId not implemented");
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

	/*
			########################################################################
														Treadmill management
			########################################################################
	*/


	/* Remove dormant actor so it can be rendered. */
	public ActorData RequestActorData(int peerId, int actorId = -1){
		GD.Print("Overworld.RequestActor not implemented");
		return null;
	}

	// Add dormant ActorData to world
	public void StoreActorData(ActorData data){
		GD.Print("Overworld.StoreActorData not implemented");
	}

	public void UpdateTreadmills(float delta){
		foreach(Treadmill treadmill in treadmills){
			treadmill.Update(delta);
		}
	}

	public ActorData LoadDormantActor(int peerId, int actorId = -1){
		ActorData dat = null;
		if(actorId == -1){
			return GetADormantActor();
		}
		return dat;
	}

	// Returns a dormant actor without implied randomization nor order
	public ActorData GetADormantActor(){
		foreach(int key in dormantActors.Keys){
			return dormantActors[key];
		}
		return null;
	}

	// Converts active Actor to dormant ActorData
	public ActorData StoreActor(Actor actor){
		GD.Print("Overworld.StoreActor not implemented");
		return null;
	}

	// Creates Actor from ActorData
	public Actor ActivateActorData(Actor.Brains brain, ActorData data){
		Actor ret = Actor.ActorFactory(brain, data);
		AddChild(ret);
		activeActors.Add(ret);
		return ret;
	}

	// Keep track of externally-created actor.
	public void RegisterActiveActor(Actor actor){
		activeActors.Add(actor);
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
			GD.Print("Overworld.CellUsers: Cell not active:" + cellId);
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

	/*Request an Item from dormant*/
	public ItemData RequestItem(int treadmillId, int itemId){
		GD.Print("Overworld.RequestItem not implemented");	
		return null;
	}

	public void ReleaseItem(ItemData data){
		GD.Print("Overworld.ReleaseActor not implemented");
	}

	/* Locates or creates TerrainCell and returns it. */
	public TerrainCell RequestCell(Vector2 coords){
		if(coords == null){
			//GD.Print("RequestCell: Coords null");
			return null;
		}
		int id = CoordsToCellId(coords);
		
		//GD.Print("Coords: " + coords + " id" + id);

		if(activeCells.ContainsKey(id)){
			//GD.Print("Active Cell Found for " + coords);
			return activeCells[id];
		}
		if(dormantCells.ContainsKey(id)){
			//GD.Print("Dormant Cell Found for" + coords + "," + id);
			TerrainCellData dormant = dormantCells[id];
			dormantCells.Remove(id);

			TerrainCell active = new TerrainCell(this, dormant, GetCellSize());
			activeCells.Add(id, active);
			AddChild(active);
			//GD.Print("Returning new ");
			return active;
		}
	
		//GD.Print("Couldn't find dormant cell for " + coords + ", " + id);
		return null;
	}

	/* Check if cell is still used, and if not store or queue for storage. */
	public void ReleaseCell(Vector2 coords){
		foreach(Treadmill treadmill in treadmills){
			if(treadmill.UsingCell(coords)){
				//GD.Print("Cell at " + coords + " still in use.");
				return;
			}
		}
		int id = CoordsToCellId(coords);
		if(id == -1){
			//GD.Print(coords + " is invalid, ignoring." );
			return;
		}
		StoreCell(id); // TODO: Delay this action so player can be pursued.
	}

	public void StoreCell(int id){
		if(!activeCells.ContainsKey(id)){
			GD.Print("Cell " + id + " is not active, not storing.");
			return;
		}
		TerrainCell cell = activeCells[id];
		activeCells.Remove(id);
		TerrainCellData data = cell.GetData();
		// TODO: Store cell's items/NPCs here.
		cell.QueueFree();
		if(dormantCells.ContainsKey(data.id)){
			GD.Print("Cell " + data.id + " already stored, not storing.");
			return;
		}
		//GD.Print("Storing cell " + data.id);
		dormantCells.Add(data.id, data);
	}

	public List<Item> ActiveItemsInBounds(TerrainCell tc){
		List<Item> ret = new List<Item>();
		foreach(Item item in activeItems){
			if(tc.InBounds(item.Translation)){
				ret.Add(item);
			}
		}
		return ret;
	}

	public List<Actor> ActiveActorsInBounds(TerrainCell tc){
		List<Actor> ret = new List<Actor>();
		foreach(Actor actor in activeActors){
			if(tc.InBounds(actor.Translation)){
				ret.Add(actor);
			}
		}
		return ret;
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
  	GD.Print("HandleActorDead not implemented");
  }

  public void TogglePause(){
    foreach(Actor actor in activeActors){
      actor.TogglePause();
    }
  }

  public string GetObjectiveText(){
  	return "Take a look around.";
  }

}