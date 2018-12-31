/*
	A terrain cell is a GridMap used to represent one subsection of the overworld's terrain.
*/
using System;
using Godot;
using System.Collections.Generic;

public class TerrainCell : GridMap{
	public int id; // Overworld ID
	public Vector2 coords;
	public int cellSize;
	public List<TerrainBlock> blockData;

	public List<Actor> packedActors;
	public List<Item> packedItems;

	public TerrainCell(TerrainCellData data, int cellSize, Vector3 baseBlockSize){
		Theme = TerrainBlock.GetTheme();
		this.cellSize = cellSize;
		CellSize = baseBlockSize;
		LoadData(data);
	}

	public float GetWidth(){
		return CellSize.x * cellSize;
	}

	public bool InBounds(Vector3 pos){
		Vector3 min = GetMinBounds();
		Vector3 max = GetMaxBounds();

		return Util.InBounds(min, max, pos);
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
		data.xCoord = (int)coords.x;
		data.yCoord = (int)coords.y;
		data.id = id;
		data.blocks = blockData;
		return data;
	}
	
	public void LoadData(TerrainCellData data){
		coords = new Vector2(data.xCoord, data.yCoord);
		id = data.id;
		blockData = data.blocks;
		foreach(TerrainBlock block in data.blocks){
			int x = block.gpx;
			int y = block.gpy;
			int z = block.gpz;
			int meshId = (int)block.blockId;
			SetCellItem(x, y, z, meshId);
		}
	}

	public void SetCenteredPosition(Vector3 pos){
		Vector3 centered = CenteredPos(pos);
		Translation = centered;
	}

	public Vector3 CenteredPos(Vector3 pos){
		float effectiveScale = GetWidth();
		Vector3 offset = new Vector3(effectiveScale/2f, 0, effectiveScale/2f);
		return pos - offset;
	}

	public Vector3 GetPos(){
		float effectiveScale = cellSize * CellSize.x; //Assume cubic grid
		float halfScale = effectiveScale/2f;
		Vector3 offset = new Vector3(halfScale, halfScale, halfScale);
		return Translation + offset;
	}

}