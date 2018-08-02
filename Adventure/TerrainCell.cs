/*
	A terrain cell is a GridMap used to represent one subsection of the overworld's terrain.
*/
using System;
using Godot;

public class TerrainCell : GridMap{
	public int id; // id according to overworld.
	public Vector2 coords;

	public TerrainCell(){
		coords = new Vector2(-1, -1);
	}

	public TerrainCell(TerrainCellData data){
		LoadData(data);
	}

	public TerrainCellData GetData(){
		GD.Print("TerrainCell.GetData not implemented");
		coords = new Vector2();
		return null;
	}
	
	
	public void LoadData(TerrainCellData data){
		coords = data.coords;
		id = data.id;
		foreach(TerrainBlock block in data.blocks){
			GD.Print(block);
		}
		GD.Print("TerrainCell.LoadData not implemented");
	}
}