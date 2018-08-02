/*
	A block represents a non-empty space in a GridMap.
	For now this class provides a convenient abstraction for loading and saving
	voxel terrain data, but any additional state will go here as well.
*/

using Godot;

public class TerrainBlock {
	public Vector3 orientation;
	public Vector3 grid_position; 
	public int meshId;

	public TerrainBlock(){
		orientation = new Vector3();
		grid_position = new Vector3();
	}

	public override string ToString(){
		string ret = "Block:[";
		int x = (int)grid_position.x;
		int y = (int)grid_position.y;
		int z = (int)grid_position.z;
		ret += x + "," + y + "," + z + "]";
		ret += "meshId: " + meshId;
		return ret;
	}
}