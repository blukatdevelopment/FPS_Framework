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
  
  public enum Types{ // For mapping to classes in the factory
    None,
    Hand,
    Rifle,
    Bullet,
    HealthPack,
    AmmoPack
  };


  public enum Categories { // Inventory categories.
    None,
    Weapons,
    Apparel,
    Aid,
    Misc,
    Ammo
  };
  
  
  public Types type;
  public string name;
  public string description;
  public int quantity;
  public int weight;
  public Godot.CollisionShape collider;
  private bool collisionDisabled = true;
  protected Speaker speaker;
  protected object wielder;
  protected Area area;

  public void BaseInit(string name, string description, int quantity = 1, bool allowCollision = true){
    this.name = name;
    this.description = description;
    this.quantity = quantity;
    this.Connect("body_entered", this, nameof(OnCollide));
    SetCollision(allowCollision);
    InitArea();
    speaker = Speaker.Instance();
    AddChild(speaker);
  }
  
  public virtual bool IsBusy(){
    return false;
  }
  
  public override void _Process(float delta){
    if(Session.IsServer()){
      SyncPosition();
    }
  }

  public virtual string GetInteractionText(Item.Uses interaction = Item.Uses.A){
    string ret = "Talk to " + name + ".";
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
    int overflow = acquirer.ReceiveItem(this);
    if(overflow == 0){
      this.QueueFree();
    }
    quantity = overflow;
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
    object[] areaShapeOwners = area.GetShapeOwners();
    for(int i = 0; i < areaShapeOwners.Length; i++){
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

  // Base Save/Load of ItemData to be used by subclasses
  public ItemData ItemGetData(){
    ItemData dat = new ItemData();

    // NON-LIST DATA
    dat.name = name;
    dat.description = description;
    dat.quantity = quantity;
    dat.type = this.type;
    dat.weight = this.weight;

    // FLOATS
    Vector3 position = GetTranslation();
    dat.floats.Add(position.x); 
    dat.floats.Add(position.y);
    dat.floats.Add(position.z);

    // STRINGS
    dat.strings.Add(name);
    dat.strings.Add(description);

    // INTS
    dat.ints.Add(quantity);
    dat.ints.Add(weight);

    return dat;
  }

  // Should remove every element added by ItemReadData
  public void ItemReadData(ItemData dat){
    // FLOATS
    float x = dat.floats[0];
    float y = dat.floats[1];
    float z = dat.floats[2];

    SetTranslation( new Vector3(x, y, z));
    dat.floats.RemoveRange(0, 3);

    // STRINGS
    name = dat.strings[0];
    description = dat.strings[1];
    dat.strings.RemoveRange(0, 2);

    // INTS
    quantity = dat.ints[0];
    weight = dat.ints[1];
    dat.ints.RemoveRange(0, 2);
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
    object[] owners = GetShapeOwners();
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
    object[] owners = GetShapeOwners();
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
  public static Item Factory(Types type, string name = ""){
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
    }
    ret.type = type;
    if(name != ""){
      Node retNode = ret as Node;
      retNode.Name = name;
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

  public int GetWeight(){
    return weight * quantity;
  }

  public static int TotalWeight(List<ItemData> items){
    int total = 0;
    foreach(ItemData item in items){
      total += item.GetWeight();
    }
    return total;
  }

}
