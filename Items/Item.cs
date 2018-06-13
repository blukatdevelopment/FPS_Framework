using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;


public class Item : RigidBody, IHasInfo, IUse, IEquip {
  public enum Uses{ 
    A, // Primary use (Left Mouse) 
    B, // Secondary Use(Right Mouse)
    C, // Third Use(Middle mouse)
    D, // Fourth Use(R)
    E,
    F, 
    G 
  };
  
  public enum Types{
    None,
    Hand,
    Rifle,
    Bullet,
    HealthPack,
    AmmoPack
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
      //GD.Print("Colliding because !netactive");
      DoOnCollide(body);
    }
    else if(Session.IsServer()){
      //GD.Print("Colliding because isserver");
      DoOnCollide(body);
      string path = bodyNode.GetPath().GetConcatenatedSubnames();
      Rpc(nameof(OnCollideWithPath), path);
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
    GD.Print("Item used " + use);
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
    }
    
    return ret;
  }
  
  public virtual void Equip(object wielder){
    this.wielder = wielder;
  }
  
  public virtual void Unequip(){
    this.wielder = null;
  }

}
