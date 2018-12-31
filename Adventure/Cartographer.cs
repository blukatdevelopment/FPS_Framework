/*
	The cartographer generates a world's TerrainCells, Actors, and Items in
	unrendered form.
*/
using System;
using System.Collections.Generic;
using Godot;

public class Cartographer {
	
	Overworld world;

	public System.Collections.Generic.Dictionary<int, TerrainCellData> cells;
	public System.Collections.Generic.Dictionary<int, ItemData> items;
	public System.Collections.Generic.Dictionary<int, ActorData> actors;

	public int worldWidth, worldHeight, cellSize;
	public int nextItemId, nextActorId;

	public void GenerateWorld(Overworld world){
		this.world = world;
		worldWidth = world.GetWorldWidth();
		worldHeight = world.GetWorldWidth();
		cellSize = world.GetCellSize();
		nextItemId = 0;
		nextActorId = 0;
		GenerateTerrain();
		GenerateActors();
		GenerateItems();
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

	// Fill out grid of world 
	public void GenerateTerrain(){
		cells = new System.Collections.Generic.Dictionary<int, TerrainCellData>();
		for(int i = 0; i < worldHeight; i++){ // y
			for(int j = 0; j < worldWidth; j++){ // x
				TerrainCellData tcd = GenerateCell(new Vector2(j, i));
				cells.Add(tcd.id, tcd);
			}
		}
	}

	// Determine cell gen algo to use for these coords.
	public TerrainCellData GenerateCell(Vector2 coords){	
		return GenerateFlatCell(coords);
	}

	public TerrainCellData GenerateFlatCell(Vector2 coords){
		TerrainCellData data = new TerrainCellData();
		data.xCoord = (int)coords.x;
		data.yCoord = (int)coords.y;
		data.id = world.CoordsToCellId((int)coords.x, (int)coords.y);


		for(int i = 0; i < cellSize; i++){
			for(int j = 0; j < cellSize; j++){
				TerrainBlock block = new TerrainBlock();
				block.gpx = i;
				block.gpz = j;
				block.blockId = TerrainBlock.Blocks.Dirt;
				data.blocks.Add(block);
			}
		}

		// Add blocks to each corner
		data.blocks.Add(new TerrainBlock(new Vector3(1, 1, 1)));
		data.blocks.Add(new TerrainBlock(new Vector3(cellSize-2, 1, 1)));
		data.blocks.Add(new TerrainBlock(new Vector3(1, 1, cellSize-2)));
		data.blocks.Add(new TerrainBlock(new Vector3(cellSize-2, 1, cellSize-2)));
		
		return data;
	}

	public void GenerateItems(){
		items = new System.Collections.Generic.Dictionary<int, ItemData>();

		// Add a single rifle beside player 1
		ItemData item = GenerateItem(Item.Types.Rifle, new Vector3(0, 15f, 0));
		items.Add(item.id, item);
	}

	public ItemData GenerateItem(Item.Types type, Vector3 pos){
		Item item = Item.Factory(type);
		ItemData data = item.GetData();
		data.id = NextItemId();
		data.pos = pos;
		
		return data;
	}

	public void GenerateActors(){
		actors = new System.Collections.Generic.Dictionary<int, ActorData>();
		
		// Create player 1
		ActorData dat = GenerateActor(new Vector3(0f, 10f, 0f));
		dat.brain = Actor.Brains.Player1;
		actors.Add(dat.id, dat);
	}

	public Vector3 GetEmptyPosition(int terrainCellId){
		return new Vector3();
	}

	public ActorData GenerateActor(Vector3 pos){
		ActorData dat = world.CreateActor();
		dat.pos = pos;
		return dat;
	}

	public int CoordsToCellId(int x, int y){
		if(world == null){
			return -1;
		}
		return world.CoordsToCellId(x, y);
	}

}