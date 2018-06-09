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
    if(singlePlayer){
      SinglePlayerInit();
    }
    else{
      MultiplayerInit();
    }
  }

  public void SinglePlayerInit(){
    GD.Print("SinglePlayerInit");
    SpawnItem(Item.Types.HealthPack);
    SpawnItem(Item.Types.AmmoPack);
    SpawnActor(Actor.Brains.Player1);
    SpawnActor(Actor.Brains.Ai);
  }

  public void MultiplayerInit(){
    NetworkSession netSes = Session.session.netSes;

    int myId = netSes.selfPeerId;
    foreach(KeyValuePair<int, PlayerData> entry in netSes.playerData){
      int id = entry.Value.id;
      if(id == myId && !Session.session.netSes.isServer){
        
        SpawnActor(Actor.Brains.Player1, id);
      }
      else{
        SpawnActor(Actor.Brains.Remote, id);
      }
    }
    SpawnItem(Item.Types.HealthPack);
    SpawnItem(Item.Types.AmmoPack);
  }
  
  public void InitTerrain(){
    PackedScene ps = (PackedScene)GD.Load("res://Scenes/Prefabs/Terrain.tscn");
    Node instance = ps.Instance();
    AddChild(instance);
    terrain = (Spatial)instance;
  }
  
  public void HandleEvent(SessionEvent sessionEvent){
    if(sessionEvent.type == SessionEvent.Types.ActorDied ){
      GD.Print("Respawning player");
      Actor[] actors = sessionEvent.actors;
      if(actors != null && actors.Length > 0 && actors[0] != null){
        Actor.Brains brain = actors[0].brainType;
        actors[0].QueueFree();
        SpawnActor(brain);
      }
    }
    else if(sessionEvent.type == SessionEvent.Types.Pause){
      TogglePause();
    }
  }
  
  public void TogglePause(){
    foreach(Actor actor in actors){
      actor.Pause();
    }
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
        //GD.Print("Found actor spawn point" + spawnPoint.GetGlobalTransform().origin);
      }
    }
    
    object[] itemSpawns = st.GetNodesInGroup("ItemSpawnPoint");
    this.itemSpawnPoints = new List<Vector3>();
    for(int i = 0; i < itemSpawns.Length; i++){
      Spatial spawnPoint = itemSpawns[i] as Spatial;
      if(spawnPoint != null){
        this.itemSpawnPoints.Add(spawnPoint.GetGlobalTransform().origin);
        //GD.Print("Found Item spawn point" + spawnPoint.GetGlobalTransform().origin);
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
    System.Random rand = Session.GetRandom();
    int randInt = rand.Next(itemSpawnPoints.Count);
    return itemSpawnPoints[randInt];
  }
  
  public Actor SpawnActor(Actor.Brains brain = Actor.Brains.Player1, int id = 0){
    Vector3 pos = RandomActorSpawn();
    Actor actor = Actor.ActorFactory(brain);
    actors.Add(actor);
    actor.SetPos(pos);
    Node actorNode = actor as Node;
    if(id != 0){
      actorNode.Name = "Player" + id;
    }
    
    AddChild(actorNode);
    GD.Print(actorNode);
    return actor;
  }
  
  public Vector3 RandomActorSpawn(){
    System.Random rand = Session.GetRandom();
    int randInt = rand.Next(actorSpawnPoints.Count);
    return actorSpawnPoints[randInt];
  }
  
  /* A factory to do all that node stuff in lieu of a constructor */ 
  public static Arena ArenaFactory(){
    PackedScene ps = (PackedScene)GD.Load("res://Scenes/Arena.tscn");
    Node instance = ps.Instance();
    return (Arena)instance;
  }

}
