using Godot;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[System.Serializable]
public class TerrainCellData {
	public int id, xCoord, yCoord;
	public List<TerrainBlock> blocks; // Non-empty blocks in this data

	public TerrainCellData(){
		blocks = new List<TerrainBlock>();

	}

  public static TerrainCellData FromJson(string json){
    return JsonConvert.DeserializeObject<TerrainCellData>(json);
  }

  public string ToJson(){
    return JsonConvert.SerializeObject(this, Formatting.Indented);
  }

  public string ToString(){
    return "TerrainCell " + id + "[" + xCoord + "," + yCoord + "]";
  }
}