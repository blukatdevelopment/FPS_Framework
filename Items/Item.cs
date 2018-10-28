/*
  Base class for all types of items. The inheritance tree is somewhat deep and wide,
  as it is built to be expanded with custom functionality easily.
*/
using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;


public class Item : RigidBody, IHasInfo, IUse, IEquip, ICollide, IInteract{
  public enum Uses{ 
    A, // Primary use (Left Mouse) 
    B, // Secondary Use(Right Mouse)
    C, // Third Use(Middle mouse)
    D, // Fourth Use(R)
    E, // Fifth Use(E)
    F, 
    G 
  };
  
  public enum Types{ // For mapping to classes in the factory and categories in inventorymenu
    None,
    Hand,
    Rifle,
    Bullet,
    HealthPack,
    AmmoPack,
    Ammo,
    AidHealthPack
  };


  public enum Categories { // Inventory categories.
    None,
    Weapons,
    Apparel,
    Aid,
    Misc,
    Ammo
  };
  
  public int id;
  public Types type;
  public string name;
  public string description;
  public int weight;
  public Godot.CollisionShape collider;
  private bool collisionDisabled = true;
  protected Speaker speaker;
  protected object wielder;
  protected Area area;
  protected bool stopColliding = false; // Stop applying OnCollide effect
  protected bool paused = false;


  public void BaseInit(string name, string description, bool allowCollision = true){
    this.name = name;
    this.description = description;
    this.Connect("body_entered", this, nameof(OnCollide));
    SetCollision(allowCollision);
    InitArea();
    speaker = new Speaker();
    AddChild(speaker);
  }
  
  public virtual bool IsBusy(){
    return false;
  }

  public virtual void Pause(){
    paused = true;
  }

  public virtual void Unpause(){
    paused = false;
  }
  
  public override void _Process(float delta){
    if(Session.IsServer()){
      SyncPosition();
    }
  }

  public virtual string GetInteractionText(Item.Uses interaction = Item.Uses.A){
    string ret = "Pick up " + name + ".";
    switch(interaction){
      case Item.Uses.A:
        ret = "Pick up " + name + ".";
        break;
    }
    return ret;
  }

  public virtual void Interact(object interactor, Item.Uses interaction = Item.Uses.A){
    IHasItem acquirer = interactor as IHasItem;
    if(acquirer != null){
      PickUp(acquirer);
    }
  }

  // client -> client
  public void PickUp(IHasItem acquirer){
    if(!Session.NetActive()){
      if(acquirer.ReceiveItem(this)){
        this.QueueFree();
      }
      return;
    }
    Node acquirerNode = acquirer as Node;
    if(acquirerNode == null){
      GD.Print("Null acquirerNode");
      return;
    }
    string acquirerPath = acquirerNode.GetPath().ToString();
    DeferredPickup(acquirerPath);
    Rpc(nameof(DeferredPickup), acquirerPath);
  }

  [Remote]
  public void DeferredPickup(string acquirerPath){
    GD.Print("DeferredPickup");
    if(acquirerPath == ""){
      GD.Print("acquirerPath is null");
    }
    NodePath path = new NodePath(acquirerPath);
    Node acquirerNode = Session.session.GetNode(path);
    IHasItem acquirer = acquirerNode as IHasItem;
    
    if(acquirer == null){
      GD.Print("No acquirer found!");
    }
  
    if(acquirer.ReceiveItem(this)){
      this.QueueFree();
    }

  }

  public void SyncPosition(){
    Vector3 position = GetTranslation();
    float x = position.x;
    float y = position.y;
    float z = position.z;
    RpcUnreliable(nameof(SetPosition), x, y, z);
  }

  [Remote]
  public void SetPosition(float x, float y, float z){
    Vector3 pos = new Vector3(x, y, z);
    SetTranslation(pos); 
  }

  void InitArea(){
    if(area != null){
      return;
    }
    List<CollisionShape> shapes = GetCollisionShapes();
    this.area = new Area();
    CollisionShape areaShape = new CollisionShape();
    area.AddChild(areaShape);
    Godot.Array areaShapeOwners = area.GetShapeOwners();
    for(int i = 0; i < areaShapeOwners.Count; i++){
      int ownerInt = (int)areaShapeOwners[i];
      for(int j = 0; j < shapes.Count; j++){
        area.ShapeOwnerAddShape(ownerInt, shapes[i].Shape);
      }
    }
    area.Connect("body_entered", this, nameof(OnCollide));
    AddChild(area);
  }

  public virtual ItemData GetData(){
    return ItemGetData();
    
  }

  public virtual void ReadData(ItemData dat){
    ItemReadData(dat);
  }

  // Saves all the basic Item fields
  public ItemData ItemGetData(){
    ItemData dat = new ItemData();

    dat.name = name;
    dat.description = description;
    dat.type = type;
    dat.weight = weight;
    dat.pos = GetTranslation();

    return dat;
  }

  public void ItemReadData(ItemData dat){

    SetTranslation( dat.pos);

    name = dat.name;
    description = dat.description;
    type = dat.type;
    weight = dat.weight;
    Translation = dat.pos;
  }

  public void OnCollide(object body){
    Node bodyNode = body as Node;
    if(bodyNode == null){
      GD.Print("Item.OnCollide: body was not a node.");
      return;
    }

    if(!Session.NetActive()){
      DoOnCollide(body);
    }
    else if(Session.IsServer()){
      string path = bodyNode.GetPath().ToString();
      Rpc(nameof(OnCollideWithPath), path);
      DoOnCollide(body);
    }
  }

  [Remote]
  public void OnCollideWithPath(string path){
    NodePath nodePath = new NodePath(path);
    Node node = GetNode(nodePath);
    object obj = node as object;
    DoOnCollide(obj);
  }

  
  List<CollisionShape> GetCollisionShapes(){
    List<CollisionShape> shapes = new List<CollisionShape>();
    Godot.Array owners = GetShapeOwners();
    foreach(object owner in owners){
      int ownerInt = (int)owner;
      CollisionShape cs = (CollisionShape)ShapeOwnerGetOwner(ownerInt);
      if(cs != null){
        shapes.Add(cs);
      }
    }
    return shapes;
  }
  
  /* Disable or enable collisions. Particularly useful for held items. */
  public void SetCollision(bool val){
    Godot.Array owners = GetShapeOwners();
    collisionDisabled = !val;
    if(area == null){
      InitArea();
    }
    foreach(object owner in area.GetShapeOwners()){
      int ownerInt = (int)owner;
      CollisionShape cs = (CollisionShape)area.ShapeOwnerGetOwner(ownerInt);
      if(cs != null){
        cs.Disabled = !val;
      }
    }

    foreach(object owner in owners){
      int ownerInt = (int)owner;
      CollisionShape cs = (CollisionShape)ShapeOwnerGetOwner(ownerInt);
      if(cs != null){
        cs.Disabled = !val;
      }
    }
    ContactMonitor = val;
    if(val){
      ContactsReported = 10;
    }
    else{
      ContactsReported = 0;
    }
  }
  
  public virtual void Use(Uses use, bool released = false){
  }
  
  public virtual string GetInfo(){
    return name;
  }
  
  public virtual string GetMoreInfo(){
    return description;
  }
  

  
  public virtual void DoOnCollide(object body){}
  
  /* Convert ItemData to Item */
  public static Item FromData(ItemData data){
    Item item = Factory(data.type, data.name);
    if(item != null){
      item.ReadData(data);
    }
    return item;
  }

  /* Returns a base/simple item by it's name. */
  public static Item Factory(Types type, string name = "", string overrideName = ""){
    Item ret = null;
    if(type == Types.None){
      return null;
    }
    string itemFile = ItemFiles(type);
    
    ret = (Item)Session.Instance(itemFile);
    
    switch(type){
      case Types.Hand: 
        ret.BaseInit("Hand", "Raised in anger, it is a fist!");  
        break;
      case Types.Rifle: 
        ret.BaseInit("Rifle", "There are many like it, but this one is yours.");
        ret.weight = 8;
        break;
      case Types.Bullet:
        ret.BaseInit("Bullet", "Comes out one end of the rifle. Be sure to know which.");
        break;
      case Types.HealthPack:
        ret.BaseInit("HealthPack", "Heals what ails you.");
        break;
      case Types.AmmoPack:
        ret.BaseInit("AmmoPack", "Food for your rifle.");
        break;
      case Types.Ammo:
        ret.BaseInit("Bullet", "A casing full of powder capped with a bullet. No further info available.");
        break;
      case Types.AidHealthPack:
        ret.BaseInit("HealthPack", "Heals what ails you.");
        break;
    }
    ret.type = type;
    if(name != ""){
      Node retNode = ret as Node;
      retNode.Name = name;
    }
    if(overrideName != ""){
      ret.name = overrideName;
    }

    return ret;
  }
  
  /* Returns a list of items */
  public static List<Item> BulkFactory(Types type, string name = "", string overrideName = "", int quantity = 1){
    List<Item> ret = new List<Item>();
    for(int i = 0; i < quantity; i++){
      ret.Add(Factory(type, name, overrideName));
    }
    return ret;
  }

  /* Converts list of Items into list of ItemData */
  public static List<ItemData> ConvertListToData(List<Item> items){
    List<ItemData> ret = new List<ItemData>();
    foreach(Item item in items){
      ret.Add(item.GetData());
      item.QueueFree();
    }
    return ret;
  }

  public static string ItemFiles(Types type){
    string ret = "";
    
    switch(type){
      case Types.Hand: ret = "res://Scenes/Prefabs/Items/Hand.tscn"; break;
      case Types.Rifle: ret = "res://Scenes/Prefabs/Items/Rifle.tscn"; break;
      case Types.Bullet: ret = "res://Scenes/Prefabs/Items/Bullet.tscn"; break;
      case Types.HealthPack: ret = "res://Scenes/Prefabs/Items/HealthPack.tscn"; break;
      case Types.AmmoPack: ret = "res://Scenes/Prefabs/Items/AmmoPack.tscn"; break;
      case Types.Ammo: ret = "res://Scenes/Prefabs/Items/Ammo.tscn"; break;
      case Types.AidHealthPack: ret = "res://Scenes/Prefabs/Items/AidHealthPack.tscn"; break;
    }
    
    return ret;
  }
  
  public void ItemBaseEquip(object wielder){
    this.wielder = wielder;
    SetCollision(false);
  }

  public void ItemBaseUnequip(){
    this.wielder = null;
    SetCollision(true);
  }

  public virtual void Equip(object wielder){
    ItemBaseEquip(wielder);
  }
  
  public virtual void Unequip(){
    ItemBaseUnequip();
  }

  public static Categories TypeCategory(Types type){
    switch(type){
      case Types.Hand:
        return Categories.Weapons;
        break;
      case Types.Rifle:
        return Categories.Weapons;
        break;
      case Types.Bullet:
        return Categories.None;
        break;
      case Types.HealthPack:  // Powerups are not aid because they are used on contact
        return Categories.None;
        break;
      case Types.AmmoPack:
        return Categories.None;
        break;
      case Types.Ammo:
        return Categories.Ammo;
        break;
      case Types.AidHealthPack:
        return Categories.Aid;
        break;
    }
    return Categories.None;
  }

  public static List<ItemData> FilterByCategory(List<ItemData> unsorted, Categories category){
    List<ItemData> ret = new List<ItemData>();
    foreach(ItemData item in unsorted){
      if(TypeCategory(item.type) == category){
        ret.Add(item);
      }
    }

    return ret;
  }

  public static int TotalWeight(List<ItemData> items){
    int total = 0;
    foreach(ItemData item in items){
      total += item.weight;
    }
    return total;
  }

}
