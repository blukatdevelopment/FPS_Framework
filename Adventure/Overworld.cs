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

	// Entities with graphics and physics rendering
	public System.Collections.Generic.Dictionary<int, TerrainCell> cells;
	public System.Collections.Generic.Dictionary<int, Actor> actors;
	public System.Collections.Generic.Dictionary<int, Item> items;

	// Unrendered entities
	public System.Collections.Generic.Dictionary<int, TerrainCellData> cellsData;
	public System.Collections.Generic.Dictionary<int, ItemData> itemsData;
	public System.Collections.Generic.Dictionary<int, ActorData> actorsData;
	
	public bool paused = false;

	public const int WorldWidth = 50;
	public const int WorldHeight = 50;
	public const int CellSize = 50; // x, y, z length in TerrainBlocks.

	private int nextActorId, nextItemId; // auto-incremented ids.


	public Overworld(){
		nextActorId = 1;
		nextItemId = 1;

		treadmills = new List<Treadmill>();
		actors = new System.Collections.Generic.Dictionary<int, Actor>();
		cells = new System.Collections.Generic.Dictionary<int, TerrainCell>();
		items = new System.Collections.Generic.Dictionary<int, Item>();

		actorsData = new System.Collections.Generic.Dictionary<int, ActorData>();
		cellsData = new System.Collections.Generic.Dictionary<int, TerrainCellData>();
		itemsData = new System.Collections.Generic.Dictionary<int, ItemData>();

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

		int playerId = 1; // First actor created should be the player

	}

	/* Asks the cartographer to make a new world
	 TODO: or loads an existing one */
	public void InitWorld(){
		Cartographer cart = new Cartographer();
		
		cart.GenerateWorld(this);
		cellsData = cart.cells;
		itemsData = cart.items;
		actorsData = cart.actors;
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

	public int GetWorldWidth(){
		return WorldWidth;
	}

	public int GetWorldHeight(){
		return WorldHeight;
	}

	public void UpdateTreadmills(float delta){
		foreach(Treadmill treadmill in treadmills){
			treadmill.Update(delta);
		}
	}

	//############################################################################
	//#								Cell access and  management										             #
	//############################################################################	

	public TerrainCell RequestCell(Vector2 coords){
		return null;
	}

	public void ReleaseCell(Vector2 coords){

	}

	public int GetCellSize(){
		return CellSize;
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
		if(!cells.ContainsKey(cellId)){
			//GD.Print("Overworld.CellUsers: Cell not active:" + cellId);
			return new List<Treadmill>();	
		}
		TerrainCell cell = cells[cellId];
		Vector2 coords = cell.coords;

		List<Treadmill> users = new List<Treadmill>();
		foreach( Treadmill treadmill in treadmills){
			if(treadmill.UsingCell(coords)){
				users.Add(treadmill);
			}
		}
		return users;	
	}

	public Vector2 PositionToCoords(Vector3 position){
		return new Vector2(-1, -1);
	}

	public int PositionToCellId(Vector3 position){
		return -1;
	}

	public int CoordsToCellId(Vector2 coords){
		int x = (int)coords.x;
		int y = (int)coords.y;
		return CoordsToCellId(x, y);
	}

	public int CoordsToCellId(int x, int y){
		if(y < 0 || y >= WorldHeight || x < 0 || x >= WorldWidth){
			//GD.Print("Invalid coords:" + x + ", " + y);
			return -1;
		}
		return y * WorldWidth + x;
	}

	//############################################################################
	//#								Rendering and Unrendering Entities                         #
	//# An entity should be in the rendered or unrendered form, but not both.	   #
	//############################################################################

	public bool ActorExists(int id){
		return actors.ContainsKey(id) || actorsData.ContainsKey(id);
	}

	// Returns rendered actor(or null), actually rendering it if necessary
	public Actor RenderActor(int id){
		if(actors.ContainsKey(id)){
			return actors[id];
		}
		
		if(!actorsData.ContainsKey(id)){
			return null;
		}

		ActorData actorData = actorsData[id];
		actorsData.Remove(id);

		Actor actor = Actor.ActorFactory(actorData);
		actors.Add(actor.worldId, actor);

		return actor;
	}

	public ActorData UnrenderActor(int id){
		if(actorsData.ContainsKey(id)){
			return actorsData[id];
		}

		if(!actors.ContainsKey(id)){
			return null;
		}

		Actor actor = actors[id];
		ActorData actorData = actor.GetData();
		actors.Remove(id);
		actor.QueueFree();

		actorsData.Add(id, actorData);
		return actorData;
	}

	public bool ItemExists(int id){
		return items.ContainsKey(id) || itemsData.ContainsKey(id);
	}

	public Item RenderItem(int id){
		if(items.ContainsKey(id)){
			return items[id];
		}
		
		if(!itemsData.ContainsKey(id)){
			return null;
		}

		ItemData itemData = itemsData[id];
		itemsData.Remove(id);

		Item item = Item.FromData(itemData);
		items.Add(item.id, item);

		return item;
	}

	public ItemData UnrenderItem(int id){
		if(itemsData.ContainsKey(id)){
			return itemsData[id];
		}

		if(!items.ContainsKey(id)){
			return null;
		}

		Item item = items[id];
		ItemData itemData = item.GetData();
		items.Remove(id);
		item.QueueFree();

		itemsData.Add(id, itemData);
		return itemData;
	}

	public bool CellExists(int id){
		return items.ContainsKey(id) || itemsData.ContainsKey(id);
	}

	public TerrainCell RenderCell(int id){
		if(cells.ContainsKey(id)){
			return cells[id];
		}
		
		if(!cellsData.ContainsKey(id)){
			return null;
		}

		TerrainCellData cellData = cellsData[id];
		cellsData.Remove(id);

		TerrainCell cell = new TerrainCell(cellData);
		cells.Add(cell.id, cell);

		return cell;
	}

	public TerrainCellData UnrenderCell(int id){
		if(cellsData.ContainsKey(id)){
			return cellsData[id];
		}

		if(!cells.ContainsKey(id)){
			return null;
		}

		TerrainCell cell = cells[id];
		TerrainCellData cellData = cell.GetData();
		cells.Remove(id);
		cell.QueueFree();

		cellsData.Add(id, cellData);
		return cellData;
	}


	//############################################################################
	//#								Session interactions																			 #
	//############################################################################



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
    foreach(int id in actors.Keys){
      Actor actor = actors[id];
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