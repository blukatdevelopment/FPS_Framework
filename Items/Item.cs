/*
  Base class for all types of items. The inheritance tree is somewhat deep and wide,
  as it is built to be expanded with custom functionality easily.
*/
using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;


public class Item : RigidBody, IHasInfo, IUse, IEquip, ICollide {
  public enum Uses{ 
    A, // Primary use (Left Mouse) 
    B, // Secondary Use(Right Mouse)
    C, // Third Use(Middle mouse)
    D, // Fourth Use(R)
    E,
    F, 
    G 
  };
  
  public enum Types{ // For use in factory
    None,
    Hand,
    Rifle,
    Bullet,
    HealthPack,
<<<<<<< HEAD
    AmmoPack
=======
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
>>>>>>> develop
  };
  
  
  public Types type;
  public string name;
  public string description;
  public int quantity;
  public int quantityMax;
  public Godot.CollisionShape collider;
  private bool collisionDisabled = true;
  protected Speaker speaker;
  protected object wielder;
<<<<<<< HEAD
=======
  protected Area area;
  protected bool stopColliding = false; // Stop applying OnCollide effect
>>>>>>> develop

  public void BaseInit(string name, string description, int quantity = 1, int quantityMax = 1, bool allowCollision = true){
    this.name = name;
    this.description = description;
    this.quantity = quantity;
    this.quantityMax = quantityMax;
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
    List<CollisionShape> shapes = GetCollisionShapes();
    Area area = new Area();
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
  
  public void SetCollision(bool val){
    object[] owners = GetShapeOwners();
    collisionDisabled = !val;
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
<<<<<<< HEAD
=======
      case Types.Ammo:
        ret.BaseInit("Bullet", "A casing full of powder capped with a bullet. No further info available.");
        break;
      case Types.AidHealthPack:
        ret.BaseInit("HealthPack", "Heals what ails you.");
        break;
>>>>>>> develop
    }
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
<<<<<<< HEAD
=======
      case Types.Ammo: ret = "res://Scenes/Prefabs/Items/Ammo.tscn"; break;
      case Types.AidHealthPack: ret = "res://Scenes/Prefabs/Items/AidHealthPack.tscn"; break;
>>>>>>> develop
    }
    
    return ret;
  }
  
  public virtual void Equip(object wielder){
    this.wielder = wielder;
  }
  
  public virtual void Unequip(){
<<<<<<< HEAD
    this.wielder = null;
=======
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

  public int GetWeight(){
    return weight * quantity;
  }

  public static int TotalWeight(List<ItemData> items){
    int total = 0;
    foreach(ItemData item in items){
      total += item.GetWeight();
    }
    return total;
>>>>>>> develop
  }

}
