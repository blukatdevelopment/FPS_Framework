/*
	A terrain cell is a GridMap used to represent one subsection of the overworld's terrain.
*/
using System;
using Godot;

public class TerrainCell : GridMap{
	public int id; // id according to overworld.
	public Vector2 coords;
	public int cellSize;

	public TerrainCell(int cellSize = 1){
		BaseInit();
		this.cellSize = cellSize;
	}

	public TerrainCell(TerrainCellData data, int cellSize = 1){
		BaseInit();
		this.cellSize = cellSize;	
		LoadData(data);
	}

	private void BaseInit(){
		Theme = TerrainBlock.GetTheme();
		CellSize = new Vector3(1, 1, 1) * 6; // Hardcoded for this meshLibrary
	}

	public TerrainCellData GetData(){
		GD.Print("TerrainCell.GetData not implemented");
		coords = new Vector2();
		return null;
	}

	// Return width of this  in world unitys
	public float Width(){
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
	
	public void LoadData(TerrainCellData data){
		GD.Print("TerrainCell.LoadData not implemented");
		coords = data.coords;
		id = data.id;
		foreach(TerrainBlock block in data.blocks){
			int x = (int)block.gridPosition.x;
			int y = (int)block.gridPosition.y;
			int z = (int)block.gridPosition.z;
			int meshId = (int)block.blockId;

			SetCellItem(x, y, z, meshId);
		}
	}

	public Vector3 GetCenterPos(){
		float effectiveScale = cellSize * CellSize.x; //Assume cubeic grid
		Vector3 offset = new Vector3(effectiveScale/2, 0, effectiveScale/2);
		return Translation + offset;
	}

	public void SetPos(Vector3 pos){
		float effectiveScale = cellSize * CellSize.x; //Assume cubeic grid
		Vector3 offset = new Vector3(effectiveScale/2, 0, effectiveScale/2);
		Translation = pos - offset;
	}

}