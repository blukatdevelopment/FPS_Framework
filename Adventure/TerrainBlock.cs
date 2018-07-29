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
}