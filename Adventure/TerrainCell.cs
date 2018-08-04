/*
	A terrain cell is a GridMap used to represent one subsection of the overworld's terrain.
*/
using System;
using Godot;

public class TerrainCell : GridMap{
	public int id; // id according to overworld.
	public Vector2 coords;

	public TerrainCell(){
		BaseInit();
	}

	public TerrainCell(TerrainCellData data){
		BaseInit();
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

	public void SetPos(Vector3 pos, int cellSize){
		float effectiveScale = cellSize * CellSize.x; //Assume cubeic grid
		Vector3 offset = new Vector3(effectiveScale/2, 0, effectiveScale/2);
		Translation = pos - offset;
	}

}