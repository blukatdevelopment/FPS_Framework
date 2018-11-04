/*
	A block represents a non-empty space in a GridMap.
	For now this class provides a convenient abstraction for loading and saving
	voxel terrain data, but any additional state will go here as well.
*/

using Godot;
using System;

[System.Serializable]
public class TerrainBlock {
	public enum Blocks{
    Air,
    Dirt = 2 // Hardcoded to full block
  };
  
  // Orientation
  public int orx, ory, orz;
  // Grid Position
  public int gpx, gpy, gpz;

	public Blocks blockId;


	public TerrainBlock(){
	}

	public TerrainBlock(Vector3 gridPosition, Vector3 orientation, Blocks blockId = Blocks.Dirt){
		orx = (int)orientation.x;
		ory = (int)orientation.y;
		orz = (int)orientation.z;

		gpx = (int)gridPosition.x;
		gpy = (int)gridPosition.y;
		gpz = (int)gridPosition.z;

		this.blockId = blockId;
	}

	// Because a Vector3 cannot be an optional argument
	public TerrainBlock(Vector3 gridPosition, Blocks blockId = Blocks.Dirt){
		orx = 0;
		ory = 0;
		orz = 0;

		gpx = (int)gridPosition.x;
		gpy = (int)gridPosition.y;
		gpz = (int)gridPosition.z;

		
		this.blockId = blockId;
	}

	public override string ToString(){
		string ret = "Block:[";
		ret += gpx + "," + gpy + "," + gpz + "]";
		ret += "blockId: " + blockId;
		return ret;
	}

	// TODO: Find a non-hardcoded way to configure this.
	public static MeshLibrary GetTheme(){
		MeshLibrary ret = ResourceLoader.Load("res://Terrain/Terrain.meshlib") as MeshLibrary;
		return ret;
	}

}