/*
	A record for an overworld instance. 
  This is the vessel for a game's save data.
*/
using System;
using System.Collections.Generic;
using Godot;

public class OverworldData{
	public int nextActorId, nextItemId;
  public string saveFile;

  public System.Collections.Generic.Dictionary<int, TerrainCellData> cellsData;
  public System.Collections.Generic.Dictionary<int, ItemData> itemsData;
  public System.Collections.Generic.Dictionary<int, ActorData> actorsData;

  public OverworldData(){
    actorsData = new System.Collections.Generic.Dictionary<int, ActorData>();
    cellsData = new System.Collections.Generic.Dictionary<int, TerrainCellData>();
    itemsData = new System.Collections.Generic.Dictionary<int, ItemData>();    
  }

  public OverworldData(Overworld world){
    actorsData = new System.Collections.Generic.Dictionary<int, ActorData>();
    cellsData = new System.Collections.Generic.Dictionary<int, TerrainCellData>();
    itemsData = new System.Collections.Generic.Dictionary<int, ItemData>();

    ReadData(world);
  }

  public void ReadData(Overworld world){
    world.TogglePause(); // Keep things from moving around for the save.
    
    saveFile = world.saveFile;

    nextActorId = world.nextActorId;
    nextItemId = world.nextActorId;

    actorsData = world.actorsData;
    cellsData = world.cellsData;
    itemsData = world.itemsData;

    foreach(int id in world.actors.Keys){
      actorsData.Add(id, world.actors[id].GetData());
    }

    foreach(int id in world.cells.Keys){
      cellsData.Add(id, world.cells[id].GetData());
    }

    foreach(int id in world.items.Keys){
      itemsData.Add(id, world.items[id].GetData());
    }

    world.TogglePause();
  }
}