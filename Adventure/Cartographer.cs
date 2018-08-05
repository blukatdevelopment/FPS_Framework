/*
	The cartographer generates the terrain, actors, and items of an Overworld.
*/
using System;
using System.Collections.Generic;
using Godot;

public class Cartographer {
	
	Overworld world;

	public Dictionary<int, TerrainCellData> cells;
	public Dictionary<int, ItemData> items;
	public Dictionary<int, ActorData> actors;

	public int worldWidth, worldHeight, cellSize;

	public void GenerateWorld(Overworld world){
		this.world = world;
		worldWidth = world.GetWorldWidth();
		worldHeight = world.GetWorldWidth();
		cellSize = world.GetCellSize();
		GenerateTerrain();
		GenerateActors();
		GenerateItems();
	}

	// Fill out grid of world 
	public void GenerateTerrain(){
		cells = new Dictionary<int, TerrainCellData>();
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
		data.coords = coords;
		data.id = world.CoordsToCellId((int)coords.x, (int)coords.y);


		for(int i = 0; i < cellSize; i++){
			for(int j = 0; j < cellSize; j++){
				TerrainBlock block = new TerrainBlock();
				block.gridPosition = new Vector3(i, 0, j);
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
		GD.Print("Cartographer.GenerateItems not implemented");
		items = new Dictionary<int, ItemData>();
	}

	// Place an Item 
	public ItemData GenerateItem(TerrainCellData cell){
		GD.Print("Cartographer.GenerateItem not implemented");
		return null;
	}

	public void GenerateActors(){
		GD.Print("Cartographer.GenerateActors not implemented");
		actors = new Dictionary<int, ActorData>();
		ActorData dat = GenerateActor(GetEmptyPosition(0));
		actors.Add(dat.id, dat);
	}

	public Vector3 GetEmptyPosition(int terrainCellId){
		return new Vector3();
	}

	public ActorData GenerateActor(Vector3 pos){
		GD.Print("Cartographer.GenerateActor not implemented");
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