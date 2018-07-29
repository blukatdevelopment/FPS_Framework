/*
	A terrain cell is a GridMap used to represent one subsection of the overworld's terrain.
*/
using System;
using Godot;

public class TerrainCell : GridMap{
	public int id; // id according to overworld.
	public int x, y; // Coords on overworld.

	public TerrainCellData GetData(){
		GD.Print("TerrainCell.GetData not implemented");
		return null;
	}
	
	public void LoadData(TerrainCellData data){
		GD.Print("TerrainCell.LoadData not implemented");
	}
}