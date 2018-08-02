using Godot;
using System;
using System.Collections.Generic;

public class TerrainCellData {
	public int id;
    public Vector2 coords; // Coords inside Overworld
	public List<TerrainBlock> blocks; // Non-empty blocks in this data

	public TerrainCellData(){
		blocks = new List<TerrainBlock>();
	}
}