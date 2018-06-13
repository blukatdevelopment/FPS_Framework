using Godot;
using System;
using System.Collections.Generic;

public class Arena : Spatial {
  bool singlePlayer;
  List<Actor> actors;
  Spatial terrain;
  List<Vector3> actorSpawnPoints, itemSpawnPoints;
  int nextId = -2147483648;
  
  const float RoundDuration = 10f;
  const float ScoreDuration = 5f;
  float roundTimeRemaining;
  bool roundTimerActive = false;
  bool scorePresented = false;
  Dictionary<int, int> scores;
  public int playerWorldId;

  public void Init(bool singlePlayer){
    this.singlePlayer = singlePlayer;
    actors = new List<Actor>();
    scores = new Dictionary<int, int>();
    InitTerrain();
    InitSpawnPoints();
    if(singlePlayer){
      SinglePlayerInit();
    }
    else{
      MultiplayerInit();
    }
    
  }

  public override void _Process(float delta){
    if(roundTimerActive){
      roundTimeRemaining -= delta;
      if(roundTimeRemaining < 0 && !scorePresented){
        PresentScore();
        scorePresented = true;
      }
      if(roundTimeRemaining < -ScoreDuration){
        roundTimerActive = false;
        RoundOver();
      }
    }
  }
  
  public bool PlayerWon(){
    int max = playerWorldId;
    foreach(KeyValuePair<int, int> key in scores){
      int score = key.Value;
      if(score > scores[max]){
        max = score;
      }
    }
    if(max == playerWorldId){
      return true;
    }
    return false;
  }

  public string GetObjectiveText(){
    

    if(scorePresented){
      return PlayerWon() ? "Victory!" : "Defeat!";
    }

    string ret = "Arena\n";
    string timeText = TimeFormat( (int)roundTimeRemaining);
    ret += "Time: " + timeText + "\n";
    ret += "Score: " + scores[playerWorldId];
    return ret;
  }
  
  public string TimeFormat(int timeSeconds){
    int minutes = timeSeconds / 60;
    int seconds = timeSeconds % 60;
    string minutesText = "" + minutes;
    if(minutes < 1){
      minutesText = "00";
    }
    string secondsText = "" + seconds;
    if(seconds < 1){
      secondsText = "00";
    }
    return minutesText + ":" + secondsText;
  }
  
  public void PresentScore(){
    TogglePause();
    Session.session.ChangeMenu(Menu.Menus.HUD);
    GD.Print("Presenting score");
  }
  
  public void RoundOver(){
    GD.Print("The round is over!");
    if(this.singlePlayer){
      Session.session.QuitToMainMenu();
    }
    else{
      Session.session.ChangeMenu(Menu.Menus.Lobby);
    } 
  }


  public int NextWorldId(){
    int ret = nextId;
    nextId++;
    return ret;
  }

  public string NextItemName(){
    string name = "Item_" + nextId;
    nextId++;
    return name;
  }

  public void SinglePlayerInit(){
    GD.Print("SinglePlayerInit");
    SpawnItem(Item.Types.HealthPack);
    SpawnItem(Item.Types.AmmoPack);

    InitActor(Actor.Brains.Player1, NextWorldId());
    InitActor(Actor.Brains.Ai, NextWorldId());
    roundTimeRemaining = RoundDuration;
    roundTimerActive = true;
  }

  public void InitActor(Actor.Brains brain, int id){
    SpawnActor(brain, id);
    scores.Add(id, 0);
    if(brain == Actor.Brains.Player1){
      playerWorldId = id;
    }
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
      HandleActorDead(sessionEvent);
    }
    else if(sessionEvent.type == SessionEvent.Types.Pause){
      TogglePause();
    }
  }

  public void HandleActorDead(SessionEvent sessionEvent){
    string[] actors = sessionEvent.args;  
    if(actors == null || actors.Length == 0 || actors[0] == ""){
      GD.Print("Arena.HandleActorDead: Insufficient args");
      return;
    }

    Node actorNode = GetNode(new NodePath(actors[0]));
    Actor actor = actorNode as Actor;
    Actor.Brains brain = actor.brainType;
    
    int id = actor.worldId;
    GD.Print("Respawning player id " + id);

    actorNode.Name = "Deadplayer" + id;
    actor.QueueFree();
    SpawnActor(brain, id);
    
    if(actors.Length < 2 || actors[1] == ""){
     return; 
    }

    Node killerNode = GetNode(new NodePath(actors[1]));
    Actor killer = killerNode as Actor;

    scores[killer.worldId]++;
    GD.Print("Killer has " + scores[killer.worldId]);

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
      }
    }
    
    object[] itemSpawns = st.GetNodesInGroup("ItemSpawnPoint");
    this.itemSpawnPoints = new List<Vector3>();
    for(int i = 0; i < itemSpawns.Length; i++){
      Spatial spawnPoint = itemSpawns[i] as Spatial;
      if(spawnPoint != null){
        this.itemSpawnPoints.Add(spawnPoint.GetGlobalTransform().origin);
      }
    }
  }
  
  public void SpawnItem(Item.Types type){
    if(Session.NetActive() && !Session.IsServer()){
      return;
    }
    Vector3 pos = RandomItemSpawn();
    Item item = Item.Factory(type);
    item.Translation = pos;
    AddChild(item);


    if(Session.IsServer()){
      string name = NextItemName();
      Node itemNode = item as Node;
      itemNode.Name = name;
      Rpc(nameof(DeferredSpawnItem), type, name, pos.x, pos.y, pos.z);
    }
  }

  [Remote]
  public Item DeferredSpawnItem(Item.Types type, string name, float x, float y, float z){
    Vector3 pos = new Vector3(x, y, z);
    Item item = Item.Factory(type);
    item.Translation = pos;

    Node itemNode = item as Node;
    itemNode.Name = name;

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
    actor.netId = id;
    actor.worldId = id;
    actors.Add(actor);
    actor.SetPos(pos);
    Node actorNode = actor as Node;
    if(id != 0){
      actorNode.Name = "Player" + id;
    }
    
    AddChild(actorNode);
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
