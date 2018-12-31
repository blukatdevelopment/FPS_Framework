/*
  TODO: Update this description
  An Actor is a living entity rendered in the 3D world and controlled by a Brain,
  which can either be an AI, or an input handler listening to a device.
*/

using Godot;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class Actor : KinematicBody, IReceiveDamage, IUse, IHasItem, IHasInfo, IHasAmmo, ILook, IInteract {
  
  public enum Brains{
    Player1, // Local player leveraging keyboard input.
    Ai,      // Computer player
    Remote   // Remote player controlled via RPC calls
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

  // Child nodes
  public Speaker speaker;
  public MeshInstance meshInstance;
  public CollisionShape collisionShape;

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

  public int id;
  public string name;

  // These can change when detailed actor models with animations are added in.
  public const string ActorMeshPath = "res://Models/Actor.obj";
  public const int HumanoidHealthMax = 100;

  public Actor(){
    brainType = Brains.Ai;
    InitChildren();
    InitBrain(brainType);
    inventory = new Inventory();
    InitHand();
    id = -1;
  }

  public Actor(Brains b){
    brainType = b;
    InitChildren();
    InitBrain(b);
    inventory = new Inventory();
    InitHand();
    id = -1;
  }

  public void InitBrain(Brains b){
    this.brainType = b;
    GD.Print("eyes. " + eyes);
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
  }

  /* Construct node tree. */
  protected void InitChildren(){
    // MeshInstance
    string meshPath = ActorMeshPath;
    meshInstance = new MeshInstance();
    meshInstance.Mesh = ResourceLoader.Load(meshPath) as Mesh;
    AddChild(meshInstance);

    // CollisionShape
    collisionShape = new CollisionShape();
    AddChild(collisionShape);
    collisionShape.MakeConvexFromBrothers();

    // Eyes
    Spatial eyesInstance;

    if(brainType == Brains.Player1){
      Camera cam = new Camera();
      eyesInstance = (Spatial)cam;
    }
    else{
      eyesInstance = new Spatial();
    }

    eyesInstance.Name = "Eyes";
    AddChild(eyesInstance);
    Vector3 eyesPos = EyesPos();
    eyesInstance.TranslateObjectLocal(eyesPos);
    eyes = eyesInstance;
    eyes.SetRotationDegrees(new Vector3(0, 0, 0));

    // Speaker
    speaker = new Speaker();
    AddChild(speaker);
  }

  public void InitStats(){
    healthMax = HumanoidHealthMax;
    health = healthMax;
  }

  public void LoadData(ActorData dat){
    // FIXME add remaining fields to ActorData and this method
    health = dat.health;
    healthMax = dat.healthMax;
    Translation = dat.pos;
    name = dat.name;
    id = dat.id;
    inventory = dat.inventory;
  }

  public ActorData GetData(){
    ActorData data = new ActorData();
    data.pos = Translation;
    data.health = health;
    data.healthMax = healthMax;
    data.brain = brainType;
    data.id = id;
    data.name = name;
    data.inventory = inventory;
    
    return data;
  }
  
  public Item PrimaryItem(){
    return activeItem;
  }

  public void InitHand(){
    if(hand != null){
      GD.Print("Hand already initialized.");
    }
    hand = Item.Factory(Item.Types.Hand);
    EquipHand(); 
  }

  public void NameHand(string handName){
    hand.Name = handName;
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

  /* Returns quantity of specified ammo.
     If max is greater than zero, will return at most max.
  */
  public int CheckAmmo(string ammoType, int max = 0){
    int quantity = inventory.GetQuantity(Item.Types.Ammo, ammoType);
    
    if(max > 0 && max > quantity){
      return quantity;
    }
    else if(max <= 0){
      return quantity;
    }
    
    return max;
  }
  
  /* Return up to max ammo, removing that ammo from inventory. */
  public List<ItemData> RequestAmmo(string ammoType, int max){
    return inventory.RetrieveItems(Item.Types.Ammo, ammoType, max);
  }
  
  public List<ItemData> StoreAmmo(List<ItemData> items){
    foreach(ItemData item in items){
      inventory.StoreItemData(item);
    }
    
    return new List<ItemData>();
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
      return;
    }    

    interactor.Interact((object)this, interaction);
  }

  public void Interact(object interactor, Item.Uses interaction = Item.Uses.A){  }
  
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
    float speed = 5f;
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
      GD.Print("Item at index " + index + " does not exist.");
      return;
    }

    DeferredEquipItem(index);
    if(Session.NetActive() && Session.IsServer()){
      Rpc(nameof(DeferredEquipItem), index);
    }
    else if(Session.NetActive()){
      RpcId(1, nameof(ServerEquipItem), index);
    }
  }

  [Remote]
  public void ServerEquipItem(int caller,  int index){
    DeferredEquipItem(index);

    foreach(KeyValuePair<int, PlayerData> entry in Session.session.netSes.playerData){
      if(entry.Key != caller){
        RpcId(entry.Key, nameof(DeferredEquipItem), index);
      }
    }
  }

  [Remote]
  public void DeferredEquipItem(int index){
    if(unarmed){
      StashHand();
    }
    else{
      StashItem();
    }

    ItemData dat = inventory.RetrieveItem(index);
    
    if(dat == null){
      GD.Print("Actor.DeferredEquipItem: Item at index " + index + " was null.");
      return;
    }

    Item item = Item.FromData(dat);

    if(eyes == null){
      GD.Print("Actor.DeferredEquipItem: No eyes.");
      return;
    }
    
    eyes.AddChild(item);
    
    item.Mode = RigidBody.ModeEnum.Static;
    item.Translation = new Vector3(HandPosX, HandPosY, HandPosZ);
    activeItem = item;
    activeItem.Equip(this);
    unarmed = false;
    GD.Print("Successfully equipped item " + item.Name);
  }
  
  /* Removes activeItem from hands. */
  public void StashItem(){
    if(unarmed){
      return;
    }

    if(activeItem == null){
      return;
    }
    
    if(activeItem.GetParent() == null){
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
      GD.Print("Can't stash hand when it's not active.");
      return;
    }

    eyes.RemoveChild(activeItem);
    activeItem = null;
    unarmed = false;
  }

  /* Equip hand when unarmed.  */
  public void EquipHand(){
    if(activeItem == hand){
      return;
    }

    activeItem = hand;
    eyes.AddChild(activeItem);
    activeItem.Translation = new Vector3(HandPosX, HandPosY, HandPosZ);
    activeItem.Mode = RigidBody.ModeEnum.Static;
    activeItem.Equip(this);
    unarmed = true;
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
      return;
    }

    activeItem.Use(use);
  }

  public static Vector3 EyesPos(){
    return new Vector3(0, 2, 0);
  }
    
  public override void _Process(float delta){
      if(brainType != Brains.Remote){ 
        brain.Update(delta); 
        Gravity(delta);
      }
  }
  
  public void Gravity(float delta){ 
    float gravityForce = GravityAcceleration * delta;
    gravityVelocity += gravityForce;

    if(gravityVelocity < TerminalVelocity){
      gravityVelocity = TerminalVelocity;
    }
    
    Vector3 grav = new Vector3(0, gravityVelocity, 0);
    Move(grav, delta);
  }

  public void Move(Vector3 movement, float moveDelta = 1f){
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
    return path.ToString();
    
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
      return;
    }
    
    
    if(!Session.NetActive()){
      string itemName = Session.NextItemName();
      DeferredDiscardItem(index, itemName);
    }
    else{
      Rpc(nameof(ServerDiscardItem), index);
    }
  }

  [Remote]
  public void ServerDiscardItem(int index){
    if(Session.NetActive() && !Session.IsServer()){
      return;
    }

    string itemName = Session.NextItemName();
    Rpc(nameof(DeferredDiscardItem), index, itemName);
    DeferredDiscardItem(index, itemName);
  }

  [Remote]
  public void DeferredDiscardItem(int index, string itemName){

    ItemData data = inventory.RetrieveItem(index);
    if(data == null){
      return;
    }

    Item item = Item.FromData(data);
    item.Name = itemName;
    Session.GameNode().AddChild(item);

    Transform trans = item.GlobalTransform;
    trans.origin = ToGlobal(new Vector3(HandPosX, HandPosY, HandPosZ));
    item.GlobalTransform = trans;

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

  public bool ReceiveItem(Item item){
    return inventory.ReceiveItem(item);
  }

  public bool ReceiveItems(List<Item> items){
    foreach(Item item in items){
      if(!inventory.ReceiveItem(item)){
        return false;
      }
    }

    return false;
  }

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
  
  public static Actor Factory(ActorData data){
    return Factory(data.brain, data);
  }

  public static Actor Factory(Brains brain = Brains.Player1, ActorData data = null){
    
    Actor actor = new Actor(brain);

    if(data != null){
      actor.LoadData(data);
      if(actor.id != -1){
        actor.Name = "Actor" + actor.id;
      }
    }

    return actor;
  }
}
