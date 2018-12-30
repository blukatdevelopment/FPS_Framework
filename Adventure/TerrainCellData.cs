/*
  A record containing a TerrainCell's data in minimal form.

  Performance issues demanded List<TerrainBlock> Be reduced to Dictionary<int, int>
  to reduce bulk of data. Additional tweaks may also be needed. 
*/

using Godot;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[System.Serializable]
public class TerrainCellData {
	public int id, xCoord, yCoord;
  // Non-empty blocks in this TerrainCell
  public System.Collections.Generic.Dictionary<int, int> blockData;

	public TerrainCellData(){
		blockData = new System.Collections.Generic.Dictionary<int, int>();
	}

  public void StoreBlock(TerrainBlock block){
    Vector3 pos = new Vector3(block.gpx, block.gpy, block.gpz);
      int index = Util.CoordsTo3DIndex(pos, Overworld.GetCellSize());
      int blockId = (int)block.blockId;
      blockData.Add(index, blockId);
  }

  public void StoreBlocks(List<TerrainBlock> blocks){
    foreach(TerrainBlock block in blocks){
      Vector3 pos = new Vector3(block.gpx, block.gpy, block.gpz);
      int index = Util.CoordsTo3DIndex(pos, Overworld.GetCellSize());
      int blockId = (int)block.blockId;
      blockData.Add(index, blockId);
    }
  }

  public List<TerrainBlock> GetBlocks(){
    List<TerrainBlock> blocks = new List<TerrainBlock>();
    foreach(int id in blockData.Keys){
      Vector3 pos = Util.IndexTo3DCoords(id, Overworld.GetCellSize());
      TerrainBlock.Blocks blockId = (TerrainBlock.Blocks)blockData[id];
      blocks.Add(new TerrainBlock(pos, blockId));
    }
    return blocks;
  }

  public static TerrainCellData FromJson(string json){
    return JsonConvert.DeserializeObject<TerrainCellData>(json);
  }

  public string ToJson(){
    return JsonConvert.SerializeObject(this, Formatting.Indented);
  }

  public string ToString(){
    return "TerrainCell " + id + "[" + xCoord + "," + yCoord + "] " + blockData.Count  + " blocks";
  }
}