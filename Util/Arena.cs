using Godot;
using System;
using System.Collections.Generic;

public class Arena : Spatial {
  bool singlePlayer;
  List<Actor> actors;
  Spatial terrain;
  
  public void Init(bool singlePlayer){
    this.singlePlayer = singlePlayer;
    actors = new List<Actor>();
    InitTerrain();
    SpawnItem(Item.Types.Rifle, new Vector3(0, 5, 0));
    SpawnActor(new Vector3(0, 5, 0));
    SpawnActor(new Vector3(0, 5, 5), Actor.Brains.Ai);
  }
  
  public void InitTerrain(){
    PackedScene ps = (PackedScene)GD.Load("res://Scenes/Prefabs/Terrain.tscn");
    Node instance = ps.Instance();
    AddChild(instance);
    terrain = (Spatial)instance;
  }
  
  public Item SpawnItem(Item.Types type, Vector3 pos){
    Item item = Item.Factory(type);
    item.Translation = pos;
    AddChild(item);
    return item;
  }
  
  public Actor SpawnActor(Vector3 pos, Actor.Brains brain = Actor.Brains.Player1){
    Actor actor = Actor.ActorFactory(brain);
    actors.Add(actor);
    actor.SetPos(pos);
    AddChild((Node)actor);
    return actor;
  }
  
  /* A factory to do all that node stuff in lieu of a constructor */ 
  public static Arena ArenaFactory(){
    PackedScene ps = (PackedScene)GD.Load("res://Scenes/Arena.tscn");
    Node instance = ps.Instance();
    return (Arena)instance;
  }

}
