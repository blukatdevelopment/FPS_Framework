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

	public string saveFile;

	// Entities with graphics and physics rendering
	public System.Collections.Generic.Dictionary<int, TerrainCell> cells;
	public System.Collections.Generic.Dictionary<int, Actor> actors;
	public System.Collections.Generic.Dictionary<int, Item> items;
	public System.Collections.Generic.Dictionary<Vector2, Spatial> invisibleWalls;

	// Unrendered entities
	public System.Collections.Generic.Dictionary<int, TerrainCellData> cellsData;
	public System.Collections.Generic.Dictionary<int, ItemData> itemsData;
	public System.Collections.Generic.Dictionary<int, ActorData> actorsData;
	
	public bool paused = false;

	public const int WorldWidth = 50;
	public const int WorldHeight = 50;
	public const int CellSize = 50; // x, y, z length in TerrainBlocks.
	public const int DefaultTreadmillRadius = 1;

	public int nextActorId, nextItemId; // auto-incremented ids.


	public Overworld(){
		nextActorId = 1;
		nextItemId = 1;

		treadmills = new List<Treadmill>();
		actors = new System.Collections.Generic.Dictionary<int, Actor>();
		cells = new System.Collections.Generic.Dictionary<int, TerrainCell>();
		items = new System.Collections.Generic.Dictionary<int, Item>();
		invisibleWalls = new System.Collections.Generic.Dictionary<Vector2, Spatial>();

		actorsData = new System.Collections.Generic.Dictionary<int, ActorData>();
		cellsData = new System.Collections.Generic.Dictionary<int, TerrainCellData>();
		itemsData = new System.Collections.Generic.Dictionary<int, ItemData>();

		Name = "Adventure";
		if(Session.IsServer()){
			// Server setup
		}
		else if(Session.NetActive()){
			// Client setup
		}
		else{
			LocalInit();
		}
	}

	public override void _Process(float delta){
		if(!paused){
			UpdateTreadmills(delta);
		}
	}

	public void LocalInit(){
		string debug = "LocalInit\n";

		if(Session.session.adventureSettings != null){
			saveFile = Session.session.adventureSettings.fileName;
		}

		InitWorld();
		
		int playerId = GetActorByBrain(Actor.Brains.Player1);
		if(playerId == -1 || !actorsData.ContainsKey(playerId)){
			GD.Print("Player1 does not exist with id " + playerId + " Aborting.");
			return;
		}
		
		// Render cell the player's in.
		ActorData playerData = actorsData[playerId];
		float scale = GetCellScale();
		Vector2 startingCoords = Util.CoordsFromPosition(playerData.pos, scale);
		RequestCell(startingCoords);

		// Find the now existing actor
		if(!actors.ContainsKey(playerId)){
			GD.Print("Player1 was not rendered properly");
			return;
		}

		CreateTreadmill(actors[playerId]);
	}

	/* Asks the cartographer to make a new world */
	public void InitWorld(){
		if(Session.session.adventureSettings != null 
			 && Session.session.adventureSettings.load){
			
			AdventureDb db = new AdventureDb(saveFile);
			OverworldData dat = db.LoadData();
			
			actorsData = dat.actorsData;
			itemsData = dat.itemsData;
			return;
		}

		Cartographer cart = new Cartographer();
		
		cart.GenerateWorld(this);
		cellsData = cart.cells;
		itemsData = cart.items;
		actorsData = cart.actors;
	}

	public int NextActorId(){
		int ret = nextActorId;
		nextActorId++;
		return ret;
	}

	public int NextItemId(){
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
		
		return null;
	}

	public Treadmill CreateTreadmill(Actor actor){
		Treadmill treadmill = new Treadmill(this, DefaultTreadmillRadius, actor);
		treadmills.Add(treadmill);
		return treadmill;
	}

	public void DestroyTreadmill(Treadmill treadmill){
		treadmills.Remove(treadmill);
		treadmill.ReleaseAll();
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

	// Returns rendered cell, rendering if necessary. Returns null for invalid
	// coords. Request/Release are high-level methods used by treadmills.
	public TerrainCell RequestCell(Vector2 coords){
		int id = CoordsToCellId(coords);
		if(id != -1){
			HandleWallsOnCellRequest(coords);
		}
		return RenderCell(id);
	}

	// Notes that at least one treadmill is no longer using this cell.
	public void ReleaseCell(Vector2 coords){
		int id = CoordsToCellId(coords);
		UnrenderCell(id); // TODO: delay unrendering
		HandleWallsOnCellRelease(coords);
	}

	public void HandleWallsOnCellRequest(Vector2 coords){
		
		if(invisibleWalls.ContainsKey(coords)){
			BreakWall(coords);
		}

		List<Vector2> neighbors = Util.NeighborCoords(coords, WorldHeight);
		foreach(Vector2 neighbor in neighbors){
			int index = Util.CoordsToCellIndex(neighbor, WorldHeight);
			if(!cells.ContainsKey(index) && !invisibleWalls.ContainsKey(neighbor)){
				BuildWall(neighbor);
			}
		}
	}

	public void HandleWallsOnCellRelease(Vector2 coords){
		List<Vector2> neighbors = Util.NeighborCoords(coords, WorldHeight);
		bool needWall = false;

		foreach(Vector2 neighbor in neighbors){
			int index = Util.CoordsToCellIndex(neighbor, WorldHeight);
			if(cells.ContainsKey(index)){
				needWall = true;
			}
		}

		if(needWall){
			BuildWall(coords);
		}
	}

	public bool WallNeeded(Vector2 coords){
		return true;
	}

	public void BreakWall(Vector2 coords){
		if(invisibleWalls.ContainsKey(coords) && invisibleWalls[coords] != null){
			invisibleWalls[coords].QueueFree();
			invisibleWalls.Remove(coords);
		}
	}

	public void BuildWall(Vector2 coords){
		
		if(invisibleWalls.ContainsKey(coords) && invisibleWalls[coords] != null){
			return;
		}
		
		BoxShape bs = new BoxShape();
		float scale = GetCellScale();

		Vector3 extents = new Vector3(scale, scale, scale);
		extents /= 2f; // Make these into half-extents.
		bs.Extents = extents;

		CollisionShape cs = new CollisionShape();
		cs.Shape = (Shape) bs;


		StaticBody wall = new StaticBody();
		wall.AddChild(cs);

		AddChild(wall);

		wall.Translation = Util.CellPositionFromCoords(coords, scale);

		invisibleWalls.Add(coords, (Spatial)wall);
	}

	// Size of cell in blocks
	public int GetCellSize(){
		return CellSize;
	}
 
 	// Scale of cell in world units.
	public static float GetCellScale(){
			return CellSize * BaseBlockSize().x;
	}

	public List<ActorData> ActorsDataAtCoords(Vector2 coords){
		List<ActorData> ret = new List<ActorData>();
		float scale = GetCellScale();
		foreach(int id in actorsData.Keys){
			ActorData actorData = actorsData[id];
			Vector2 actualCoords = Util.CoordsFromPosition(actorData.pos, scale);
			if(coords == actualCoords){
				ret.Add(actorData);
			}
		}
		return ret;
	}

	public List<Actor> ActorsAtCoords(Vector2 coords){
		List<Actor> ret = new List<Actor>();
		float scale = GetCellScale();
		foreach(int id in actors.Keys){
			Actor actor = actors[id];
			Vector2 actualCoords = Util.CoordsFromPosition(actor.Translation, scale);
			if(coords == actualCoords){
				ret.Add(actor);
			}
		}
		return ret;
	}

	public List<ItemData> ItemDataAtCoords(Vector2 coords){
		List<ItemData> ret = new List<ItemData>();
		float scale = GetCellScale();
		foreach(int id in itemsData.Keys){
			ItemData itemData = itemsData[id];
			Vector2 actualCoords = Util.CoordsFromPosition(itemData.pos, scale);
			GD.Print("Item " + id + " at " + actualCoords + ", " + itemData.pos );
			if(coords == actualCoords){
				ret.Add(itemData);
			}
		}
		return ret;
	}	

	public List<Item> ItemsAtCoords(Vector2 coords){
		List<Item> ret = new List<Item>();
		float scale = GetCellScale();
		foreach(int id in items.Keys){
			Item item = items[id];
			Vector2 actualCoords = Util.CoordsFromPosition(item.Translation, scale);
			if(coords == actualCoords){
				ret.Add(item);
			}
		}
		return ret;
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
			return new List<Treadmill>();
		}
		if(!cells.ContainsKey(cellId)){
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
		float scale = GetCellScale();
		return Util.CoordsFromPosition(position, scale);
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
			return -1;
		}
		return y * WorldWidth + x;
	}

	//############################################################################
	//#								Rendering and Unrendering Entities                         #
	//# An entity should be in the rendered or unrendered form, but not both.	   #
	//############################################################################


	public void Save(){
		OverworldData data = new OverworldData(this);

		AdventureDb db = new AdventureDb(saveFile);

		db.SaveData(data);
	}

	public int GetActorByBrain(Actor.Brains brain){
		foreach(int key in actors.Keys){
			if(actors[key].brainType == brain){
				return key;
			}
		}

		foreach(int key in actorsData.Keys){
			if(actorsData[key].brain == brain){
				return key;
			}
		}

		return -1;
	}

	public bool ActorExists(int id){
		return actors.ContainsKey(id) || actorsData.ContainsKey(id);
	}

	public Actor RenderActor(int id){
		if(actors.ContainsKey(id)){
			return actors[id];
		}
		
		if(!actorsData.ContainsKey(id)){
			return null;
		}

		ActorData actorData = actorsData[id];
		actorsData.Remove(id);

		Actor actor = Actor.Factory(actorData);
		actors.Add(actor.id, actor);
		AddChild(actor);

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
		AddChild(item);

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
		if(!Util.ValidCellIndex(id, WorldWidth)){
			return null;
		}

		if(cells.ContainsKey(id)){
			return cells[id];
		}
		
		if(!cellsData.ContainsKey(id)){
			AdventureDb db = new AdventureDb(saveFile);
			TerrainCellData data = db.LoadCell(id);
			cellsData.Add(id, data);
		}

		TerrainCellData cellData = cellsData[id];
		cellsData.Remove(id);

		TerrainCell cell = new TerrainCell(cellData, CellSize, BaseBlockSize());
		Vector3 cellPos = Util.CellPositionFromCoords(cell.coords, GetCellScale());
		cell.SetCenteredPosition(cellPos);
		cells.Add(cell.id, cell);
		AddChild(cell);

		foreach(ActorData actor in ActorsDataAtCoords(cell.coords)){
			RenderActor(actor.id);
		}

		foreach(ItemData item in ItemDataAtCoords(cell.coords)){
			GD.Print("Item found at " + cell.coords);
			RenderItem(item.id);
		}

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

		foreach(Actor actor in ActorsAtCoords(cell.coords)){
			UnrenderActor(actor.id);
		}

		foreach(Item item in ItemsAtCoords(cell.coords)){
			UnrenderItem(item.id);
		}

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

  public void HandleActorDead(SessionEvent sessionEvent){}

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