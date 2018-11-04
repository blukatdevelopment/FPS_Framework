using Godot;
using System;
using System.Collections.Generic;

[System.Serializable]
public class TerrainCellData {
	public int id;
	public List<TerrainBlock> blocks; // Non-empty blocks in this data

  public int xCoord;
  public int yCoord;


	public TerrainCellData(){
		blocks = new List<TerrainBlock>();
	}
}