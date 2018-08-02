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
	public Dictionary<int, TerrainCell> activeCells;


	public List<Actor> activeActors;

	public Dictionary<int, TerrainCellData> dormantCells;
	public Dictionary<int, ItemData> dormantItems;
	public Dictionary<int, ActorData> dormantActors;

	public const int WorldWidth = 50;
	public const int WorldHeight = 50;
	public const int CellSize = 50; // x, y, z length in TerrainBlocks.

	private int nextActorId, nextItemId; // auto-incremented ids.


	public Overworld(){
		treadmills = new List<Treadmill>();
		activeActors = new List<Actor>();

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
		Treadmill treadmill = new Treadmill(-1, this, actor, new Vector2(), 1);
		treadmills.Add(treadmill);
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
		debugCam = new Camera();
		AddChild(debugCam);
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

	// Keep track of externally-created actor.
	public void RegisterActiveActor(Actor actor){
		activeActors.Add(actor);
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
		GD.Print("Overworld.RequestCell not implemented");
		if(coords == null){
			return null;
		}
		int id = PositionToCellId(coords);
		
		
		if(dormantCells.ContainsKey(id)){
			TerrainCellData dormant = dormantCells[id];
			dormantCells.Remove(id);

			TerrainCell active = new TerrainCell(dormant);
			activeCells.Add(id, active);
			return active;
		}
		if(activeCells.ContainsKey(id)){
			return activeCells[id];
		}


		return null;
	}

	/* Check if cell is still used, and if not store or queue for storage. */
	public void ReleaseCell(Vector2 coords){
		GD.Print("Overworld.ReleaseCell not implemented");
	}

}