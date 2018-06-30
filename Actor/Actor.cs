/*
  The Actor is a living entity in the game world.
  Each actor is controlled by a Brain.
  Items are children of an actor's Eyes.
  Inventory is crude weapon-switching and ammo count like that in classic FPS games.


  RPC calls go like so: (Notice AI is not currently supported)
  server->client: DeferredInitInventory, 
  client->others: DeferredUse, DeferredSwitchItem

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

  public Speaker speaker;

  private int health;
  private int healthMax = 100;
  
  // Inventory
  private Item activeItem;
  private bool unarmed = true; // True->Hand, False->Rifle
  private string ammoType = "bullet";
  private int ammo = 100;
  private int maxAmmo = 999;
  private List<Item> items;
  private Inventory inventory;
  
  // Handpos
  private float HandPosX = 0;
  private float HandPosY = 0;
  private float HandPosZ = -1.5f;


  // Network
  public int netId;

  // World info
  public int worldId;
  
  // Delayed inventory init.
  float initTimer, initDelay;
  bool initActive = false;
  string initHand, initRifle;

  
  public void Init(Brains b = Brains.Player1){
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
    speaker = Speaker.Instance();
    AddChild(speaker);
    health = 100;
    InitInventory();
  }
  
  public Item PrimaryItem(){
    return activeItem;
  }
  
  

  void InitInventory(){
    inventory = new Inventory();
    if(Session.NetActive() && !Session.IsServer()){
      return;
    }
    
    string rifle = Session.NextItemId();
    string hand = Session.NextItemId();

    if(rifle == "" || hand == ""){
      GD.Print("InitInventory: Rifle and hand strings are blank");
    }

    DeferredInitInventory(rifle, hand);

    if(Session.IsServer()){
      initTimer = 0f;
      initDelay = 0.25f;
      initActive = true;
      initHand = hand;
      initRifle = rifle;
    }
    
  }

  [Remote]
  void DeferredInitInventory(string rifleName, string handName){
    items = new List<Item>();
    ReceiveItem(Item.Factory(Item.Types.Hand, handName));
    ReceiveItem(Item.Factory(Item.Types.Rifle, rifleName));
    EquipItem(1);
    unarmed = false;
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

  /* Global space */
  public Vector3 Pointer(){
    float distance = 100f;
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
    if(this.ammoType != ammoType){
      return 0;
    }
    if(max < 0){
      return ammo;
    }
    if(max > ammo){
      return ammo;
    }
    return max;
  }
  
  /* Return up to max ammo, removing that ammo from inventory. */
  public int RequestAmmo(string ammoType, int max){
    int amount = CheckAmmo(ammoType, max);
    ammo -= amount;
    return amount;
  }
  
  /* Store up to max ammo, returning overflow. */
  public int StoreAmmo(string ammoType, int max){
    if(ammoType != this.ammoType){
      return max;
    }
    int amount = max + ammo;
    if(maxAmmo <= amount){
      ammo = maxAmmo;
      amount -= ammo;
    }
    else{
      ammo += amount;
      amount = 0;
    }
    return amount;
  }
  
  public string[] AmmoTypes(){
    
    return new string[]{"bullet"};
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
      GD.Print("Nothing in range.");
      return;
    }
    Item.Uses interaction = GetActiveInteraction();
    interactor.Interact((object)this, interaction);
  }

  public void Interact(object interactor, Item.Uses interaction = Item.Uses.A){
    GD.Print("Interacted with " + GetInfo() + ".");
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
    float speed = 5f;
    if(sprinting){
      speed *= 2f;
    }
    return speed;
  }
  
  public void SwitchItem(){
    DeferredSwitchItem();
    if(Session.NetActive()){
      Rpc(nameof(DeferredSwitchItem));
    }
  }

  [Remote]
  public void DeferredSwitchItem(){
    unarmed = !unarmed;
    if(unarmed){
      EquipItem(0);
    }
    else{
      EquipItem(1);
    }
  }
  
  /* Equip item based on index in items. */
  public void EquipItem(int index){
    if(index >= items.Count){
      GD.Print("Invalid index " + index);
      return;
    }
    
    StashItem();
    Item item = items[index];
    
    if(eyes == null){
      return;
    }
    
    eyes.AddChild(item);
    
    item.Mode = RigidBody.ModeEnum.Static;
    item.Translation = new Vector3(HandPosX, HandPosY, HandPosZ);
    activeItem = item;
    activeItem.Equip(this);
  }
  
  /* Removes activeItem from hands. */
  public void StashItem(){
    if(activeItem == null){
      GD.Print("No activeitem");
      return;
    }
    
    if(activeItem.GetParent() == null){
      GD.Print("activeItem is orphan");
      return;
    }
    
    eyes.RemoveChild(activeItem);
    
    activeItem = null;
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
    if(Session.NetActive()){
      Rpc(nameof(DeferredUse), use, released);
    }
  }


  [Remote]
  public void DeferredUse(Item.Uses use, bool released = false){
    if(activeItem == null){
      GD.Print("No item equipped.");
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
        initTimer+= delta;
        if(initTimer >= initDelay){
          initActive = false;
          initTimer = 0;
          Rpc(nameof(DeferredInitInventory), initRifle, initHand);
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
    Session.session.HandleEvent(evt);
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
  
  public int ReceiveItem(Item item){
    if(items.Count < 2){
      items.Add(item);
    }
    
    ItemData dat = item.GetData();
    GD.Print("ItemData: " + dat.ToString());

    return 0;
  }
  
  public bool IsPaused(){
    return menuActive;
  }

  public void TogglePause(){
    ActorInputHandler inputHandler = brain as ActorInputHandler;
    if(inputHandler == null){
      return;
    }
    
    menuActive = !menuActive;
    
    if(menuActive){
      Session.session.ChangeMenu(Menu.Menus.Pause);
      Input.SetMouseMode(Input.MouseMode.Visible);
    }
    else{
      Session.session.ChangeMenu(Menu.Menus.HUD);
      Input.SetMouseMode(Input.MouseMode.Captured);
    }
  }
  
  /* The goal of this factory is to set up an actor's node tree in script so it's version controllable. */
  public static Actor ActorFactory(Brains brain = Brains.Player1){
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
    return actor;
  }
}
