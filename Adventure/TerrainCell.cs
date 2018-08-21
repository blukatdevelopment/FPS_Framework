/*
	A terrain cell is a GridMap used to represent one subsection of the overworld's terrain.
*/
using System;
using Godot;
using System.Collections.Generic;

public class TerrainCell : GridMap{
	public int id; // id according to overworld.
	public Vector2 coords;
	public int cellSize;
	public List<TerrainBlock> blockData;
	public Overworld world;

	public TerrainCell(Overworld world, int cellSize = 1){
		BaseInit(world);
		this.cellSize = cellSize;
		this.blockData = new List<TerrainBlock>();
	}

	public TerrainCell(Overworld world, TerrainCellData data, int cellSize = 1){
		BaseInit(world);
		this.cellSize = cellSize;	
		LoadData(data);
	}

	private void BaseInit(Overworld world){
		this.world = world;
		Theme = TerrainBlock.GetTheme();
		CellSize = new Vector3(1, 1, 1) * 6; // Hardcoded for this meshLibrary
	}


	// Return width of this  in world unitys
	public float GetWidth(){
		return CellSize.x * cellSize;
	}

	public bool InBounds(Vector3 pos){
		Vector3 min = GetMinBounds();
		Vector3 max = GetMaxBounds();
		
		if(pos.x < min.x || pos.y < min.y || pos.z < min.z){
			return false;
		}
		
		if(pos.x > max.x || pos.y > max.y || pos.z > max.z){
			return false;
		}

		return true;
	}

	public Vector3 GetMinBounds(){
		return Translation;
	}

	public Vector3 GetMaxBounds(){
		Vector3 ret = Translation;
		float scale = CellSize.x * cellSize;
		ret += new Vector3(scale, scale, scale);
		return ret;
	}


	public TerrainCellData GetData(){
		TerrainCellData data = new TerrainCellData();
		data.coords = coords;
		data.id = id;
		data.blocks = blockData;
		return data;
	}
	
	public void LoadData(TerrainCellData data){
		coords = data.coords;
		id = data.id;
		blockData = data.blocks;
		foreach(TerrainBlock block in data.blocks){
			int x = (int)block.gridPosition.x;
			int y = (int)block.gridPosition.y;
			int z = (int)block.gridPosition.z;
			int meshId = (int)block.blockId;
			SetCellItem(x, y, z, meshId);
		}
	}

	// Returns center of this terrain cell
	public Vector3 GetCenterPos(){
		float effectiveScale = cellSize * CellSize.x; //Assume cubeic grid
		Vector3 offset = new Vector3(effectiveScale/2, 0, effectiveScale/2);
		return Translation + offset;
	}

	// TODO: Relocate contained Actors and Items
	public void SetPos(Vector3 pos){
		Vector3 oldPos = Translation;
		float effectiveScale = cellSize * CellSize.x; //Assume cubeic grid
		Vector3 offset = new Vector3(effectiveScale/2, 0, effectiveScale/2);
		
		Vector3 newPos = pos - offset;
		//GD.Print( "Moving cell " + coords + "from " + oldPos + " to " + newPos);

		// 
		// foreach(Item item in items){
		// 	item.Translation = RecenterActivePos(oldPos, newPos, item.Translation);
		// }
		
		List<Actor> actors = world.ActiveActorsInBounds(this);
		foreach(Actor actor in actors){
			Vector3 start = actor.Translation;
			actor.Translation = RecenterActivePos(oldPos, newPos, actor.Translation);
			Vector3 end = actor.Translation;
			GD.Print("Actor moved from " + start + " to " + end);
		}


		Translation = newPos;
	}

	// Preserves position of activePos relative to newPos.
	public Vector3 RecenterActivePos(Vector3 oldPos, Vector3 newPos, Vector3 activePos){
		Vector3 diff = activePos - oldPos;
		Vector3 recenteredPos = newPos + diff;
		return recenteredPos;
	}

	public List<Item> ItemsInBounds(){
		return world.ActiveItemsInBounds(this);
	}

	public List<Actor> ActorsInBounds(){
		return world.ActiveActorsInBounds(this);
	}

}