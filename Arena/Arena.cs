/*
  The Arena is a self-contained game mode.
  Actors score points by killing other actors
  over the course of a round's duration.
*/

using Godot;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class Arena : Spatial {
  public bool local;
  public List<Actor> actors;
  public Spatial terrain;
  public List<Vector3> actorSpawnPoints, itemSpawnPoints;
  public int nextId = -2147483648;
  public bool gameStarted = false;
  public ArenaSettings settings;
  const float ScoreDuration = 5f;
  public float roundTimeRemaining, secondCounter;
  public bool roundTimerActive = false;
  public bool scorePresented = false;
  public System.Collections.Generic.Dictionary<int, int> scores;
  public int playerWorldId = -1;

  int playersReady = 0;

  public void Init(bool local){
    settings = Session.session.arenaSettings;
    
    if(settings == null){
      GD.Print("Using default arena settings.");
      settings = new ArenaSettings();
    }
    
    this.local = local;
    actors = new List<Actor>();
    scores = new System.Collections.Generic.Dictionary<int, int>();

    InitTerrain();
    InitSpawnPoints();
    
    if(local){
      LocalInit();
    }
    else{
      OnlineInit();
    } 
  }

  public override void _Process(float delta){
    if(roundTimerActive){
      Timer(delta);
    }
  }

  public void Timer(float delta){
    secondCounter += delta;

    if(secondCounter >= 1.0f){
      roundTimeRemaining -= secondCounter;
      secondCounter = 0f;

      if(Session.IsServer()){
        Rpc(nameof(SyncTime), roundTimeRemaining);
      }

      if(roundTimeRemaining < 0 && !scorePresented){
        PresentScore();

        if(Session.IsServer()){
          Rpc(nameof(PresentScore));
        }
      }
      
      if(roundTimeRemaining < -ScoreDuration){
        
        if(Session.IsServer()){
          Rpc(nameof(RoundOver));
          RoundOver();
        }
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
    if(playerWorldId == -1){
      return "Player not initialized.";
    }
    
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
  
  [Remote]
  public void SyncTime(float newTime){
    roundTimeRemaining = newTime;
  }

  [Remote]
  public void PresentScore(){
    scorePresented = true;
    SetPause(true);
    Session.ChangeMenu(Menu.Menus.HUD);
  }
  
  [Remote]
  public void RoundOver(){
    roundTimerActive = false;

    if(this.local){
      Session.QuitToMainMenu();
    }
    else{
      Session.ChangeMenu(Menu.Menus.Lobby);
      Session.ClearGame(true);
    } 
  }

  public int NextId(){
    int ret = nextId;
    nextId++;
    return ret;
  }

  public string NextBotName(){
    string name = "Bot_" + nextId;
    nextId++;
    return name;
  }

  public string NextItemName(){
    string name = "Item_" + nextId;
    nextId++;
    return name;
  }

  public void LocalInit(){
    if(settings.usePowerups){
      for(int i = 0; i < 1; i++){
        SpawnItem(Item.Types.AmmoPack, 10);
        SpawnItem(Item.Types.HealthPack);  
      }
    }

    InitActor(Actor.Brains.Player1, NextId());
    for(int i = 0; i < settings.bots; i++){
      InitActor(Actor.Brains.Ai, NextId());
    }

    roundTimeRemaining = settings.duration * 60;

    roundTimerActive = true;
  }

  public void OnlineInit(){
    if(!Session.IsServer()){
      return;
    }

    for(int i = 0; i < settings.bots; i++){
      settings.botIds.Add(NextId());
    }

    string settingsJson = ArenaSettings.ToJson(Session.session.arenaSettings);
    
    DeferredOnlineInit(settingsJson);
    Rpc(nameof(DeferredOnlineInit), settingsJson);
  }

  [Remote]
  public void DeferredOnlineInit(string json){
    if(!Session.IsServer()){
      settings = JsonConvert.DeserializeObject<ArenaSettings>(json);
      Session.session.arenaSettings = settings;
    }
    
    NetworkSession netSes = Session.session.netSes;
    netSes.playersReady = 0;
    playerWorldId = netSes.selfPeerId;
    
    foreach(KeyValuePair<int, PlayerData> entry in netSes.playerData){
      int id = entry.Value.id;
      InitActor(Actor.Brains.Remote, id);
    }

    foreach(int id in settings.botIds){
      if(Session.IsServer()){
        InitActor(Actor.Brains.Ai, id);
      }
      else{
        InitActor(Actor.Brains.Remote, id);
      }
    }
    
    if(settings.usePowerups){
      for(int i = 0; i < 1; i++){
        SpawnItem(Item.Types.HealthPack);
        SpawnItem(Item.Types.AmmoPack);
      }
    }

    if(Session.IsServer()){
      roundTimeRemaining = settings.duration * 60;
      roundTimerActive = true;
    }
  }

  public Actor InitActor(Actor.Brains brain, int id){
    scores.Add(id, 0);

    if(!Session.NetActive()){
      SpawnActor(brain, id);
      
      if(brain == Actor.Brains.Player1){
        playerWorldId = id;
      }
      
      return null;
    }

    Actor ret = null;

    if(id == playerWorldId){
      ret = SpawnActor(Actor.Brains.Player1, id);
    }
    else {
      ret = SpawnActor(brain, id);
    }

    return ret; 
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
      return;
    }

    Node actorNode = GetNode(new NodePath(actors[0]));
    Actor actor = actorNode as Actor;
    Actor.Brains brain = actor.brainType;
    
    int id = actor.id;

    actorNode.Name = "Deadplayer" + id;
    actor.QueueFree();
    actor = SpawnActor(brain, id);
    
    if(actor.brainType == Actor.Brains.Player1){
      Session.ChangeMenu(Menu.Menus.HUD);
    }
    
    if(actors.Length < 2 || actors[1] == ""){
     return; 
    }

    Node killerNode = GetNode(new NodePath(actors[1]));
    Actor killer = killerNode as Actor;

    if(killer != null){
      scores[killer.id]++;
    }
  }

  public void SetPause(bool val){
    foreach(Actor actor in actors){
      if(actor.IsPaused() != val){
        actor.TogglePause();
      }
    }
  }
  
  public void TogglePause(){
    foreach(Actor actor in actors){
      actor.TogglePause();
    }
  }
  
  public void InitSpawnPoints(){
    SceneTree st = GetTree();
    Godot.Array actorSpawns = st.GetNodesInGroup("ActorSpawnPoint");
    
    this.actorSpawnPoints = new List<Vector3>();
    
    for(int i = 0; i < actorSpawns.Count; i++){
      Spatial spawnPoint = actorSpawns[i] as Spatial;
      
      if(spawnPoint != null){
        this.actorSpawnPoints.Add(spawnPoint.GetGlobalTransform().origin);
      }
    }
    
    Godot.Array itemSpawns = st.GetNodesInGroup("ItemSpawnPoint");
    this.itemSpawnPoints = new List<Vector3>();
    
    for(int i = 0; i < itemSpawns.Count; i++){
      Spatial spawnPoint = itemSpawns[i] as Spatial;
      
      if(spawnPoint != null){
        this.itemSpawnPoints.Add(spawnPoint.GetGlobalTransform().origin);
      }
    }
  }
  
  public void SpawnItem(Item.Types type, int quantity = 1){
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
      Rpc(nameof(DeferredSpawnItem), type, name, pos.x, pos.y, pos.z, quantity);
    }
  }

  [Remote]
  public Item DeferredSpawnItem(Item.Types type, string name, float x, float y, float z, int quantity){
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
    

    ActorData dat = new ActorData();

    dat.id = id;
    dat.pos = pos;
    dat.health = dat.healthMax = 100;
    if(settings.useKits){
      dat.inventory.ReceiveItem(Item.Factory(Item.Types.Rifle));
      List<Item> kitItems = Item.BulkFactory(Item.Types.Ammo, 100);

      dat.inventory.ReceiveItem(kitItems[0]);
    }

    Actor actor = Actor.Factory(brain, dat);
    actor.NameHand(actor.Name + "(Hand)");  
    
    actors.Add(actor);
    AddChild(actor);

    if(settings.useKits){
      EquipActor(actor, Item.Types.Rifle, "Rifle");
    }

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

  public void PlayerReady(){
    if(Session.NetActive()){
      Rpc(nameof(DeferredPlayerReady));
    }
  }

  public void EquipActor(Actor actor, Item.Types itemType, string itemName){
    int index = actor.IndexOf(itemType, itemName);
    if(index == -1){
      GD.Print("Actor doesn't have this weapon.");
    }
    else{
      GD.Print("Equipping Actor with " + itemType.ToString());
      actor.EquipItem(index);
    }
  }

  [Remote]
  public void DeferredPlayerReady(){
    if(!Session.IsServer()){
      GD.Print("DeferredPlayerReady: Online and not server. Aborting.");
      return;
    }
    
    NetworkSession netSes = Session.session.netSes; 
    netSes.playersReady++;
    if(netSes.playersReady != netSes.playerData.Count){
      GD.Print("Waiting for other players.");
      return;
    }
    gameStarted = true;
    // if(settings.useKits){
    //   foreach(Actor actor in actors){
    //     EquipActor(actor, Item.Types.Rifle, "Rifle");
    //   }
    // }
    
  }

}
