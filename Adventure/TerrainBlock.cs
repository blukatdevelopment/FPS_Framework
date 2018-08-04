/*
	A block represents a non-empty space in a GridMap.
	For now this class provides a convenient abstraction for loading and saving
	voxel terrain data, but any additional state will go here as well.
*/

using Godot;

public class TerrainBlock {
	public enum Blocks{
    Air,
    Dirt = 2 // Hardcoded to full block
  };
  
	public Vector3 orientation;
	public Vector3 gridPosition; 
	public Blocks blockId;


	public TerrainBlock(){
		orientation = new Vector3();
		gridPosition = new Vector3();
	}

	public override string ToString(){
		string ret = "Block:[";
		int x = (int)gridPosition.x;
		int y = (int)gridPosition.y;
		int z = (int)gridPosition.z;
		ret += x + "," + y + "," + z + "]";
		ret += "blockId: " + blockId;
		return ret;
	}

	// TODO: Find a non-hardcoded way to configure this.
	public static MeshLibrary GetTheme(){
		MeshLibrary ret = ResourceLoader.Load("res://Terrain/Terrain.meshlib") as MeshLibrary;
		return ret;
	}

}