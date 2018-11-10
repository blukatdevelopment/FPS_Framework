using Godot;
using System;
using System.Collections.Generic;

[System.Serializable]
public class TerrainCellData {
	public int id, xCoord, yCoord;
	public List<TerrainBlock> blocks; // Non-empty blocks in this data

	public TerrainCellData(){
		blocks = new List<TerrainBlock>();
	}
}