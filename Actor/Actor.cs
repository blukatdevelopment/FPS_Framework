/*
  TODO: Update this description
  The Actor is a living entity in the game world.
  Each actor is controlled by a Brain.
  Items are children of an actor's Eyes.
  Inventory is crude weapon-switching and ammo count like that in classic FPS games.

*/

using Godot;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class Actor : KinematicBody, IReceiveDamage, IUse, IHasItem, IHasInfo, IHasAmmo, ILook, IInteract {
  
  public enum Brains{
    Player1, // Local player leveraging keyboard input.
    Ai,      // Computer player
    Remote     // Remote player controlled via RPC calls
  };
  
  private Brain brain;
  public Brains brainType;
  private Spatial eyes;
  
  const int maxY = 90;
  const int minY = -40;
  const float GravityAcceleration = -9.81f;
  const float TerminalVelocity = -53;
  
  private bool grounded = false; //True when Actor is standing on surface.
  public bool sprinting = false;
  private float gravityVelocity = 0f;
  
  public bool menuActive = false;
  public bool paused = false;

  public Speaker speaker;

  private int health;
  private int healthMax = 100;
  
  // Inventory
  private Item activeItem;
  private Item hand; // Weapon for unarmed actors.
  private bool unarmed = true; 
  private Inventory inventory;
  
  // Handpos
  private float HandPosX = 0;
  private float HandPosY = 0;
  private float HandPosZ = -1.5f;

  // Network
  public int netId;

  // World info TODO: Create an NPCCharacter module for personality stuff.
  public int worldId;
  public string name; 
  
  // Delayed inventory init for netcode.
  float initTimer, initDelay;
  bool initActive = false;
  string initHand, initRifle;
  float readyTimer, readyDelay;
  bool readyActive = false;

  
  public void Init(Brains b = Brains.Player1, bool initHealth = true){
    InitChildren();
    this.brainType = b;
    switch(b){
      case Brains.Player1: 
        brain = (Brain)new ActorInputHandler(this, eyes); 
        Session.session.player = this;
        break;
      case Brains.Ai: 
        brain = (Brain)new Ai(this, eyes); 
        break;
      case Brains.Remote:
        brain = null;
        break;

    }
    if(eyes != null){
      eyes.SetRotationDegrees(new Vector3(0, 0, 0));  
    }
    speaker = new Speaker();
    AddChild(speaker);
    if(initHealth){
      health = 100;
    }
    InitInventory();
  }

  public void LoadData(ActorData dat){
    // FIXME add remaining fields to ActorData and this method
    health = dat.health;
    healthMax = dat.healthMax;
    Translation = dat.pos;
    name = dat.name;
    worldId = dat.id;
  }

  public ActorData GetData(){
    //GD.Print("Actor.GetData not implemented");
    return new ActorData();
  }
  
  public Item PrimaryItem(){
    return activeItem;
  }
  
  // TODO: Remove hardcoded equipping code
  void InitInventory(){
    inventory = new Inventory();
    if(Session.NetActive() && !Session.IsServer()){
      return;
    }
    
    string rifle = Session.NextItemId();
    string hand = Session.NextItemId();

    if(rifle == "" || hand == ""){
      //GD.Print("InitInventory: Rifle and hand strings are blank");
    }

    if(!Session.NetActive()){
      DeferredInitInventory(hand);
    }
    if(Session.IsServer()){
      initTimer = 0f;
      initDelay = 0.25f;
      initActive = true;
      initHand = hand;
    }
  }

  [Remote]
  void DeferredInitInventory(string handName){
    InitHand(handName);
    Session.InitKit(this);
    if(Session.NetActive()){
      readyTimer = 0;
      readyDelay = 1f;
      readyActive = true;
    }
    else{ // Forego delaying the equip of this inventory.
      Session.PlayerReady();
    }
  }

  public void InitHand(string handName){
    hand = Item.Factory(Item.Types.Hand, handName);
    if(unarmed && activeItem == null){
      EquipHand();  
    }
    else{
      //GD.Print("Not equipping hand because holding something.");
    }
    
  }

  /* Return global position of eyes(if available) or body */
  public Vector3 GlobalHeadPosition(){
    if(eyes != null){
      return eyes.GlobalTransform.origin;
    }
    return GlobalTransform.origin;
  }

  /* Returns global transform of eyes(if available) or body */
  public Transform GlobalHeadTransform(){
    if(eyes != null){
      return eyes.GlobalTransform;
    }
    return GlobalTransform;
  }

  /* Global space 
    Point at end of ray in looking direction.
  */
  public Vector3 Pointer(float distance = 100f){
    Vector3 start = GlobalHeadPosition();
    Transform headTrans = GlobalHeadTransform();
    Vector3 end = Util.TForward(headTrans);
    end *= distance;
    end += start;
    return end;
  }

  public object VisibleObject(){
    Vector3 start = GlobalHeadPosition();
    Vector3 end = Pointer();
    World world = GetWorld();
    return Util.RayCast(start, end, world);
  }



  /* Show up to max ammo */
  public int CheckAmmo(string ammoType, int max){
    int index = inventory.IndexOf(Item.Types.Ammo, ammoType);
    if(index == -1){
      return 0;
    }
    return inventory.GetItem(index).quantity;
  }
  

  /* Return up to max ammo, removing that ammo from inventory. */
  public int RequestAmmo(string ammoType, int max){
    int index = inventory.IndexOf(Item.Types.Ammo, ammoType);
    if(index == -1){
      return 0;
    }
    return inventory.RetrieveItem(index, max).quantity;

  }
  
  /* Store up to max ammo, returning overflow. */
  public int StoreAmmo(string ammoType, int max){
    if(Session.NetActive() && !Session.IsServer()){
      //GD.Print("Client won't store ammo.");
      return 0;
    }
    //GD.Print("Storing ammo " + ammoType + "(" + max + ")");
    Item ammo = Item.Factory(Item.Types.Ammo, "TemporaryName", ammoType, max);
    ReceiveItem(ammo);
    return 0;
  }
  
  public string[] AmmoTypes(){
    
    return new string[]{"Bullet"};
  }
  
  public string GetInfo(){
    switch(brainType){
      case Brains.Player1:
        return "You.";
        break;
      case Brains.Ai:
        return "AI";
        break;
      case Brains.Remote:
        return "Online player";
        break;
    }
    return "Actor";
  }

  /* Return which interaction is currently going to take place. */
  public Item.Uses GetActiveInteraction(){
    return Item.Uses.A;
  }

  public string GetInteractionText(Item.Uses interaction = Item.Uses.A){
    string ret = "Talk to " + GetInfo() + ".";
    switch(interaction){
      case Item.Uses.A:
        ret = "Talk to " + GetInfo() + ".";
        break;
      case Item.Uses.B:
        ret = "Pickpocket " + GetInfo() + ".";
        break;
    }
    return ret;
  }

  public void InitiateInteraction(){
    IInteract interactor = VisibleObject() as IInteract;
    if(interactor == null){
      //GD.Print("Nothing in range.");
      return;
    }
    Item.Uses interaction = GetActiveInteraction();

    Item item = interactor as Item;
    if(item != null && (item == hand || item == activeItem)){
      //GD.Print("Don't interact with your active item or hand. Returning.");
      return;
    }    

    interactor.Interact((object)this, interaction);
  }

  public void Interact(object interactor, Item.Uses interaction = Item.Uses.A){
    //GD.Print("Interacted with " + GetInfo() + ".");
  }
  
  public string GetMoreInfo(){
    return "A character in this game.";
  }
  
  public Vector3 HeadPosition(){
    if(eyes != null){
      return eyes.ToGlobal(eyes.Translation);
    }
    return this.ToGlobal(Translation);
  }
  
  public Vector3 Forward(){
    if(eyes != null){
      return Util.TForward(eyes.Transform);
    }
    return Util.TForward(this.Transform);
  }
  
  public Vector3 Up(){
    if(eyes != null){
      return Util.TUp(eyes.Transform);
    }
    return Util.TUp(this.Transform);
  }
  
  public Vector3 RotationDegrees(){
    if(eyes != null){
      return eyes.GetRotationDegrees();
    }
    return GetRotationDegrees();
  }
  
  public Transform GetLookingTransform(){
    if(eyes != null){
      return eyes.Transform;
    }
    return Transform;
  }
  
  public void SetSprint(bool val){
    sprinting = val;
  }
  
  public float GetMovementSpeed(){
    float speed = 50f;
    if(sprinting){
      speed *= 2f;
    }
    return speed;
  }
  
  public void ToggleInventory(){
    if(!menuActive){
      SetMenuActive(true);
      Session.ChangeMenu(Menu.Menus.Inventory);  
    }
    else{
      SetMenuActive(false);
    }
  }
  
  /* Equip item based on inventory index. */
  public void EquipItem(int index){
    if(inventory.GetItem(index) == null){
      return;
    }

    DeferredEquipItem(index);
    if(Session.NetActive() && Session.IsServer()){
      Rpc(nameof(DeferredEquipItem), index);
    }
    else if(Session.NetActive()){
      RpcId(1, nameof(ServerEquipItem), Session.session.netSes.selfPeerId, index);
    }
  }

  // Client -> Server -> Client
  [Remote]
  public void ServerEquipItem(int caller, int index){
    DeferredEquipItem(index);

    foreach(KeyValuePair<int, PlayerData> entry in Session.session.netSes.playerData){
      if(caller != entry.Key){
        RpcId(entry.Key, nameof(DeferredEquipItem), index);
      }
    }
  }

  [Remote]
  public void DeferredEquipItem(int index){
    //GD.Print("DeferredEquipItem: " + index);
    if(unarmed){
      StashHand();
    }
    else{
      StashItem();
    }

    ItemData dat = inventory.RetrieveItem(index);
    
    Item item = Item.FromData(dat);

    if(eyes == null){
      //GD.Print("Not equipping item because eyes don't exist.");
      return;
    }
    
    eyes.AddChild(item);
    
    item.Mode = RigidBody.ModeEnum.Static;
    item.Translation = new Vector3(HandPosX, HandPosY, HandPosZ);
    activeItem = item;
    activeItem.Equip(this);
    unarmed = false;
  }



  
  /* Removes activeItem from hands. */
  public void StashItem(){
    if(unarmed){
      //GD.Print("Unarmed. Not Stashing.");
      return;
    }

    if(activeItem == null){
      //GD.Print("No activeitem. Not stashing.");
      return;
    }
    
    if(activeItem.GetParent() == null){
      //GD.Print("activeItem is orphan. Not stashing");
      return;
    }
    DeferredStashItem();
    if(Session.NetActive() && Session.IsServer()){
      Rpc(nameof(DeferredStashItem));
    }
    else if(Session.NetActive()){
      RpcId(1, nameof(ServerStashItem), Session.session.netSes.selfPeerId);
    }
  }

  // Client -> Server -> Client
  [Remote]
  public void ServerStashItem(int caller){
    DeferredStashItem();

    foreach(KeyValuePair<int, PlayerData> entry in Session.session.netSes.playerData){
      if(caller != entry.Key){
        RpcId(entry.Key, nameof(DeferredStashItem));
      }
    }
  }

  [Remote]
  public void DeferredStashItem(){
    activeItem.Unequip();
    inventory.ReceiveItem(activeItem);
    eyes.RemoveChild(activeItem);

    activeItem.QueueFree();
    EquipHand();
  }

  /* Remove hand in preparation of equipping an item */
  public void StashHand(){
    if(activeItem != hand){
      //GD.Print("Hand not out. Not stashing hand.");
      return;
    }
    eyes.RemoveChild(activeItem);
    activeItem = null;
    //GD.Print("Stashed hand");
    unarmed = false;
  }

  /* Equip hand when unarmed.  */
  public void EquipHand(){
    if(activeItem == hand){
      //GD.Print("Hand already out. Not equipping hand.");
      return;
    }
    activeItem = hand;
    eyes.AddChild(activeItem);
    activeItem.Translation = new Vector3(HandPosX, HandPosY, HandPosZ);
    activeItem.Mode = RigidBody.ModeEnum.Static;
    activeItem.Equip(this);
    unarmed = true;
    //GD.Print("Hand equipped");
  }
  
  /* Finds any attached eyes if eys are not already set by factory.  */
  protected void InitChildren(){
    if(eyes != null){
      return;
    }
    foreach(Node child in this.GetChildren()){
      switch(child.GetName()){
        case "Eyes": eyes = child as Eyes; break;
      }
    }
    if(eyes == null){ GD.Print("Actor's eyes are null!"); }
  }
  
  public bool IsBusy(){
    if(activeItem == null){
      return false;
    }
    return activeItem.IsBusy();
  }
  
  
  public void Use(Item.Uses use, bool released = false){
    DeferredUse(use, released);
    if(Session.NetActive() && Session.IsServer()){
      Rpc(nameof(DeferredUse), use, released);
    }
    else if(Session.NetActive()){
      RpcId(1, nameof(ServerUse), Session.session.netSes.selfPeerId, use, released);
    }
  }

  // Client -> Server -> Client
  [Remote]
  public void ServerUse(int caller, Item.Uses use, bool released){
    DeferredUse(use, released);

    foreach(KeyValuePair<int, PlayerData> entry in Session.session.netSes.playerData){
      if(caller != entry.Key){
        RpcId(entry.Key, nameof(DeferredUse), use, released);
      }
    }
  }


  [Remote]
  public void DeferredUse(Item.Uses use, bool released = false){
    if(activeItem == null){
      //GD.Print("No item equipped.");
      return;
    }
    activeItem.Use(use);
  }

  
    
  public override void _Process(float delta){
      if(brainType != Brains.Remote){ 
        brain.Update(delta); 
        Gravity(delta);
      }
      if(initActive){
        initTimer += delta;
        if(initTimer >= initDelay){
          initActive = false;
          initTimer = 0;
          DeferredInitInventory(initHand);
          Rpc(nameof(DeferredInitInventory), initHand);
        }
      }
      if(readyActive){
        readyTimer += delta;
        if(readyTimer >= readyDelay){
          Session.PlayerReady();
          readyActive = false;
          readyTimer = 0;
        }
      }
  }
  
  public void Gravity(float delta){ 
    float gravityForce = GravityAcceleration * delta;
    gravityVelocity += gravityForce;
    if(gravityVelocity < TerminalVelocity){
      gravityVelocity = TerminalVelocity;
    }
    
    Vector3 grav = new Vector3(0, gravityVelocity, 0);
    //grav = this.ToLocal(grav);
    Move(grav, delta);
  }

  
  public void Move(Vector3 movement, float moveDelta = 1f){
      //Vector3 movement = new Vector3(x, y, -z) * moveDelta; 
      movement *= moveDelta;
      
      Transform current = GetTransform();
      Transform destination = current; 
      destination.Translated(movement);
      
      Vector3 delta = destination.origin - current.origin;
      KinematicCollision collision = MoveAndCollide(delta);
      if(collision != null && collision.Collider != null){
        ICollide collider = collision.Collider as ICollide;
        if(collider != null){
          Node colliderNode = collider as Node;
          collider.OnCollide(this as object);
        }
      }
      if(!grounded && collision != null && collision.Position.y < GetTranslation().y){
        if(gravityVelocity < 0){
          grounded = true;
          gravityVelocity = 0f;
        }
      }
  }
  
  public void Jump(){
    if(!grounded){ return; }
    float jumpForce = 10;
    gravityVelocity = jumpForce;
    grounded = false;
    
  }

  [Remote]
  public void RemoteReceiveDamage(string json){
    Damage dmg = JsonConvert.DeserializeObject<Damage>(json);
    if(dmg != null){
      ReceiveDamage(dmg);
    }
  }
  
  public void ReceiveDamage(Damage damage){
    //GD.Print("Received damage " + damage.health);
    if(health <= 0){
      return;
    }
    health -= damage.health;
    
    if(damage.health > 0 && health > 0){
      speaker.PlayEffect(Sound.Effects.ActorDamage);
    }
    if(health <= 0){
      health = 0;
      speaker.PlayEffect(Sound.Effects.ActorDeath);
      Die(damage.sender);
    }
    if(health > healthMax){
      health = healthMax;
    }
  }
  
  public void Die(string source = ""){
    
    Transform = Transform.Rotated(new Vector3(0, 0, 1), 1.5f);

    string path = NodePath();
    SessionEvent evt = SessionEvent.ActorDiedEvent(path, source);
    Session.Event(evt);
  }

  public string NodePath(){
    NodePath path = GetPath();
    return path.ToString(); // GetConcatenatedSubnames was returning ""
    
  }

  public int GetHealth(){
    return health;
  }
  
  public void SyncPosition(){
    Vector3 pos = GetTranslation();
    RpcUnreliable(nameof(SetPosition), pos.x, pos.y, pos.z);
  }

  public void SyncAim(){
    Vector3 headRot = RotationDegrees();
    Vector3 bodyRot = GetRotationDegrees();

    float x = bodyRot.y;
    float y = headRot.x;

    RpcUnreliable(nameof(SetRotation), x, y);
  }

  public void DiscardItem(int index){
    ItemData data = inventory.GetItem(index);
    if(data == null){
      //GD.Print("No item found, not discarding.");
      return;
    }
    
    
    if(!Session.NetActive()){ // Singleplayer should directly call DeferredDiscardItem
      //GD.Print("Discarding item because no net active");
      string itemName = Session.NextItemId();
      DeferredDiscardItem(index, itemName);
    }
    else{
      //GD.Print("RPC serverdiscarditem");
      Rpc(nameof(ServerDiscardItem), index);
    }
  }

  [Remote]
  public void ServerDiscardItem(int index){
    if(Session.NetActive() && !Session.IsServer()){
      //GD.Print("Returning because not server.");
      return;
    }
    //GD.Print("Server RPC deferreddiscarditem" + inventory.ToString());
    string itemName = Session.NextItemId();
    Rpc(nameof(DeferredDiscardItem), index, itemName);
    DeferredDiscardItem(index, itemName);
  }

  [Remote]
  public void DeferredDiscardItem(int index, string itemName){
    //GD.Print("Deferred discard " + index + " " + itemName);
    ItemData data = inventory.RetrieveItem(index);
    if(data == null){
      //GD.Print("No item data. Not discarding.");
      return;
    }

    Item item = Item.FromData(data);
    item.Name = itemName;
    Session.GameNode().AddChild(item);
    //GD.Print("Item:" + item);
    Transform trans = item.GlobalTransform;
    trans.origin = ToGlobal(new Vector3(HandPosX, HandPosY, HandPosZ));
    item.GlobalTransform = trans;

    // Notify session
    Session.Event(SessionEvent.ItemDiscardedEvent(NodePath().ToString()));
  }

  public int IndexOf(Item.Types type, string name){
    return inventory.IndexOf(type, name);
  }

  public ItemData RetrieveItem(int index){
    return inventory.RetrieveItem(index); 
  }

  [Remote]
  public void SetRotation(float x, float y){
    Vector3 bodyRot = this.GetRotationDegrees();
    bodyRot.y = x;
    this.SetRotationDegrees(bodyRot);

    Vector3 headRot = RotationDegrees();
    headRot.x = y;
    if(headRot.x < minY){
      headRot.x = minY;  
    }
    if(headRot.x > maxY){
      headRot.x = maxY;
    }
    eyes.SetRotationDegrees(headRot);
  }

  [Remote]
  public void SetPosition(float x, float y, float z){
    Vector3 pos = new Vector3(x, y, z);
    SetTranslation(pos);
  }

  public void Turn(float x, float y){
    Vector3 bodyRot = this.GetRotationDegrees();
    bodyRot.y += x;
    this.SetRotationDegrees(bodyRot);
    
    Vector3 headRot = eyes.GetRotationDegrees();
    headRot.x += y;
    if(headRot.x < minY){
      headRot.x = minY;
    }
    if(headRot.x > maxY){
      headRot.x = maxY;
    }
    eyes.SetRotationDegrees(headRot);
  }
  
  
  public void SetPos(Vector3 pos){
    SetTranslation(pos);
    
  }
  
  public bool HasItem(string item){
    return false;
  }
  
  public string ItemInfo(){
    if(activeItem != null){
      return activeItem.GetInfo();
    }
    return "Unequipped";
  }

  // Good for server->client
  public int ReceiveItem(Item item){
    if(!Session.NetActive()){
      //GD.Print("received " + item.quantity + " " + item);
      return inventory.ReceiveItem(item);  
    }
    else if(Session.IsServer()){
      string json = JsonConvert.SerializeObject(item.GetData(), Formatting.Indented);
      DeferredReceiveItem(json);
      Rpc(nameof(DeferredReceiveItem),json);
      return 0;
    }
    else{
      //GD.Print("Online and not server. Not receivingItem");
      return 0;
    }
  }

  

  // Good for server->client
  [Remote]
  public void DeferredReceiveItem(string json){
    ItemData dat = JsonConvert.DeserializeObject<ItemData>(json);
    Item item = Item.FromData(dat);
    inventory.ReceiveItem(item);
  }
  
  public bool IsPaused(){
    return menuActive;
  }

  public void SetMenuActive(bool val){
    menuActive = val;

    if(menuActive){
      Input.SetMouseMode(Input.MouseMode.Visible);
    }
    else{
      Session.ChangeMenu(Menu.Menus.HUD);
      Input.SetMouseMode(Input.MouseMode.Captured);

    }
    
  }

  public int ItemCount(){
    return inventory.ItemCount();
  }

  public List<ItemData> GetAllItems(){
    return inventory.GetAllItems();
  }

  public void TogglePause(){
    ActorInputHandler inputHandler = brain as ActorInputHandler;
    if(inputHandler == null){
      return;
    }
    
    menuActive = !menuActive;
    
    if(menuActive){
      Session.ChangeMenu(Menu.Menus.Pause);
      Input.SetMouseMode(Input.MouseMode.Visible);
    }
    else{
      Session.ChangeMenu(Menu.Menus.HUD);
      Input.SetMouseMode(Input.MouseMode.Captured);
    }
  }


  public void Pause(){
    paused = true;
  }

  public void Unpause(){
    paused = false;
  }
  
  /* The goal of this factory is to set up an actor's node tree in script so it's version controllable. */
  public static Actor ActorFactory(Brains brain = Brains.Player1, ActorData data = null){
    PackedScene actorPs = (PackedScene)GD.Load("res://Scenes/Prefabs/Actor.tscn");
    Node actorInstance = actorPs.Instance();
    
    Actor actor = (Actor)actorInstance;
    
    Vector3 eyesPos = new Vector3(0, 2, 0);
    
    switch(brain){
      case Brains.Player1: 
        Node eyeInstance = Session.Instance("res://Scenes/Prefabs/Eyes.tscn");
        eyeInstance.Name = "Eyes";
        actor.eyes = eyeInstance as Spatial;
        break;
      default:
        Spatial eyeSpat = new Spatial();
        Node eyesNode = eyeSpat as Node;
        eyesNode.Name = "Eyes";
        actor.eyes = eyeSpat;
        break;
    }
    
    actor.eyes.TranslateObjectLocal(eyesPos);
    actor.AddChild(actor.eyes);
    actor.Init(brain);
    if(data != null){
      actor.LoadData(data);
    }
    return actor;
  }
}
