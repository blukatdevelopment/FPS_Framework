using Godot;
using System;
using System.Collections.Generic;

public class Arena : Spatial {
  bool singlePlayer;
  List<Actor> actors;
  Spatial terrain;
  List<Vector3> actorSpawnPoints, itemSpawnPoints;
  
  public void Init(bool singlePlayer){
    this.singlePlayer = singlePlayer;
    actors = new List<Actor>();
    InitTerrain();
    InitSpawnPoints();
    SpawnItem(Item.Types.HealthPack);
    SpawnItem(Item.Types.AmmoPack);
    SpawnActor();
    SpawnActor(Actor.Brains.Ai);
  }
  
  public void InitTerrain(){
    PackedScene ps = (PackedScene)GD.Load("res://Scenes/Prefabs/Terrain.tscn");
    Node instance = ps.Instance();
    AddChild(instance);
    terrain = (Spatial)instance;
  }
  
  public void InitSpawnPoints(){
    GD.Print("Initializing item spawns");
    SceneTree st = GetTree();
    object[] actorSpawns = st.GetNodesInGroup("ActorSpawnPoint");
    this.actorSpawnPoints = new List<Vector3>();
    for(int i = 0; i < actorSpawns.Length; i++){
      Spatial spawnPoint = actorSpawns[i] as Spatial;
      if(spawnPoint != null){
        this.actorSpawnPoints.Add(spawnPoint.GetGlobalTransform().origin);
        GD.Print("Found actor spawn point" + spawnPoint.GetGlobalTransform().origin);
      }
    }
    
    object[] itemSpawns = st.GetNodesInGroup("ItemSpawnPoint");
    this.itemSpawnPoints = new List<Vector3>();
    for(int i = 0; i < itemSpawns.Length; i++){
      Spatial spawnPoint = itemSpawns[i] as Spatial;
      if(spawnPoint != null){
        this.itemSpawnPoints.Add(spawnPoint.GetGlobalTransform().origin);
        GD.Print("Found Item spawn point" + spawnPoint.GetGlobalTransform().origin);
      }
    }
  }
  
  public Item SpawnItem(Item.Types type){
    Vector3 pos = RandomItemSpawn();
    Item item = Item.Factory(type);
    item.Translation = pos;
    AddChild(item);
    return item;
  }
  
  public Vector3 RandomItemSpawn(){
    System.Random rand = new System.Random();
    int randInt = rand.Next(itemSpawnPoints.Count);
    return itemSpawnPoints[randInt];
  }
  
  public Actor SpawnActor(Actor.Brains brain = Actor.Brains.Player1){
    Vector3 pos = RandomActorSpawn();
    Actor actor = Actor.ActorFactory(brain);
    actors.Add(actor);
    actor.SetPos(pos);
    AddChild((Node)actor);
    return actor;
  }
  
  public Vector3 RandomActorSpawn(){
    System.Random rand = new System.Random();
    int randInt = rand.Next(itemSpawnPoints.Count);
    return actorSpawnPoints[randInt];
  }
  
  /* A factory to do all that node stuff in lieu of a constructor */ 
  public static Arena ArenaFactory(){
    PackedScene ps = (PackedScene)GD.Load("res://Scenes/Arena.tscn");
    Node instance = ps.Instance();
    return (Arena)instance;
  }

}
