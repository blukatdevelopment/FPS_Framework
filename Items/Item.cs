/*
  Base class for all types of items. The inheritance tree is somewhat deep and wide,
  as it is built to be expanded with custom functionality easily.
*/
using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;


public class Item : RigidBody, IHasInfo, IUse, IEquip, ICollide, IInteract{
  public enum Uses{ A, B, C, D, E, F, G };
  
  public enum Types{
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
  private bool collisionDisabled = true;
  protected Speaker speaker;
  protected object wielder;
  protected Area area;
  protected bool stopColliding = false; // Stop applying OnCollide effect
  protected bool paused = false;
  protected MeshInstance meshInstance;
  protected CollisionShape collisionShape;
  public bool stackable;
  public List<int> stack;

  public void BaseInit(string name, string description, string meshPath, bool allowCollision = true){
    this.name = name;
    this.description = description;
    this.Connect("body_entered", this, nameof(OnCollide));
    
    this.stack = new List<int>();

    SetCollision(allowCollision);
    InitArea();
    speaker = new Speaker();
    AddChild(speaker);

    meshInstance = new MeshInstance();
    meshInstance.Mesh = ResourceLoader.Load(meshPath) as Mesh;
    AddChild(meshInstance);

    collisionShape = new CollisionShape();
    AddChild(collisionShape);
    collisionShape.MakeConvexFromBrothers();
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

  public void PickUp(IHasItem acquirer){
    if(!Session.NetActive()){
      if(acquirer.ReceiveItem(this)){
        this.QueueFree();
      }
      return;
    }
    Node acquirerNode = acquirer as Node;
    if(acquirerNode == null){
      return;
    }
    string acquirerPath = acquirerNode.GetPath().ToString();
    DeferredPickup(acquirerPath);
    Rpc(nameof(DeferredPickup), acquirerPath);
  }

  [Remote]
  public void DeferredPickup(string acquirerPath){
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

  public ItemData ItemGetData(){
    ItemData dat = new ItemData();

    dat.name = name;
    dat.description = description;
    dat.type = type;
    dat.weight = weight;
    dat.pos = GetTranslation();
    dat.stack = new List<int>(stack.ToArray());
    dat.stackable = stackable;

    return dat;
  }

  public void ItemReadData(ItemData dat){
    SetTranslation(dat.pos);

    name = dat.name;
    description = dat.description;
    type = dat.type;
    weight = dat.weight;
    Translation = dat.pos;
    stack = new List<int>(dat.stack.ToArray());
    stackable = dat.stackable;
  }

  public void OnCollide(object body){
    Node bodyNode = body as Node;
    if(bodyNode == null){
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
  
  public virtual void Use(Uses use, bool released = false){}
  
  public virtual string GetInfo(){
    return name;
  }
  
  public virtual string GetMoreInfo(){
    return description;
  }
  
  public virtual void DoOnCollide(object body){}
  
  public static Item FromData(ItemData data){
    Item item = Factory(data.type);
    if(item != null){
      item.ReadData(data);
    }
    return item;
  }

  public static Item Factory(Types type){
    Item ret = null;
    if(type == Types.None){
      return null;
    }
    
    ret = FromType(type);
    
    string[] archetype = ItemArchetype(type);
    ret.BaseInit(archetype[0], archetype[1], archetype[2]);
    ret.type = type;

    if(ret.name != ""){
      Node retNode = ret as Node;
      retNode.Name = ret.name;
    }

    ret.id = Session.NextItemId();

    return ret;
  }

  // Return an instance of the type's archetypal class.
  public static Item FromType(Types type){
    Item ret = null;
    
    switch(type){
      case Types.Hand: ret = new MeleeWeapon() as Item; break;
      case Types.Rifle: ret = new ProjectileWeapon() as Item; break;
      case Types.Bullet: 
        ret = new Projectile() as Item; 
        ret.stackable = true;
        break;
      case Types.HealthPack:  ret = new HealthPowerUp() as Item; break;
      case Types.AmmoPack: ret = new AmmoPowerUp() as Item; break;
      case Types.Ammo: 
        ret = new Item();
        ret.stackable = true; 
        break;
      case Types.AidHealthPack: ret = new HealthAid() as Item; break;
    }
    
    return ret;
  }
  
  /* Returns a list of items */
  public static List<Item> BulkFactory(Types type, int quantity = 1){
    List<Item> ret = new List<Item>();
    
    Item first = Factory(type);

    if(quantity > 1 && first.stackable){
      for(int i = 1; i < quantity; i++){
        first.Push(Session.NextItemId());
      }
    }
    else if(quantity > 1){
      for(int i = 1; i < quantity; i++){
        ret.Add(Factory(type));
      }
    }
    ret.Add(first);
    
    return ret;
  }

  public static List<ItemData> ConvertListToData(List<Item> items){
    List<ItemData> ret = new List<ItemData>();
    
    foreach(Item item in items){
      ret.Add(item.GetData());
      item.QueueFree();
    }

    return ret;
  }

  // Return an array of strings used to initialize an archetypal item of given type.
  // When making new item types, add an archetype here.
  // 0 name
  // 1 description
  // 2 mesh name
  public static string[] ItemArchetype(Types type){
    string[] ret = null;
    
    switch(type){
      case Types.Hand:
        ret = new string[]{"Hand", "Raised in anger, it is a fist!", "res://Models/Hand.obj"};
        break;
      case Types.Rifle:
        ret = new string[]{"Rifle", "There are many like it, but this one is yours.", "res://Models/Rifle.obj"};
        break;
      case Types.Bullet:
        ret = new string[]{"Bullet", "Comes out one end of the rifle. Be sure to know which.", "res://Models/Bullet.obj"};
        break;
      case Types.HealthPack:
        ret = new string[]{"HealthPack", "Heals what ails you.", "res://Models/Healthpack.obj"};
        break;
      case Types.AmmoPack:
        ret = new string[]{"AmmoPack", "Food for your rifle.", "res://Models/Ammopack.obj"};
        break;
      case Types.Ammo:
        ret = new string[]{"Bullet", "A casing full of powder capped with a bullet. No further info available.", "res://Models/Ammopack.obj"};
        break;
      case Types.AidHealthPack:
        ret = new string[]{"HealthPack", "Heals what ails you.", "res://Models/Healthpack.obj"};
        break;
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


  public List<int> GetStack(){
    return stack;
  }

  public bool Push(ItemData item){
    if(!stackable || item.type != this.type || item.name != this.name){
      return false;
    }

    stack.Add(item.id);
    stack.AddRange(item.GetStack());
    return true;
  }

  public bool Push(int id){
    if(!stackable){
      return false;
    }

    stack.Add(id);
    return true;
  }

  public ItemData Pop(int quantity = 1){
    if(stack.Count == 0 || quantity > stack.Count){
      return null;
    }

    ItemData ret = GetData();
    ret.stack = new List<int>();

    ret.id = stack[0];
    stack.RemoveAt(0);

    for(int i = 1; i < quantity; i++){
      ret.stack.Add(stack[0]);
      stack.RemoveAt(0);
    }

    return ret;
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
      total += item.GetWeight();
    }

    return total;
  }

  public int GetWeight(){
    int ret = weight;
    ret += weight * stack.Count;
    return ret;
  }

  public int GetQuantity(){
    return 1 + stack.Count;
  }
}
