/*
  Instantiates and fires projectiles, launching them forward. 
  Limited by a store of ammo wich must be reloaded with delay.
*/
using Godot;
using System.Collections.Generic;
using System;

public class ProjectileWeapon : Item, IWeapon, IHasAmmo, IEquip {
  
  const int BaseDamage = 10;
  const float ProjectileOffset = 0.1f;
  const float ImpulseStrength = 50f;
  public string ammoType = "Bullet";
  public Inventory inventory;
  public int maxAmmo = 10;
  public float busyDelay = 0f;
  public bool busy = false;
  public delegate void OnBusyEnd();
  public OnBusyEnd busyEndHandler;
  
  public ProjectileWeapon(){
    inventory = new Inventory();
  }
  
  public override void _Process(float delta){
    if(busy){
      busyDelay -= delta;
      
      if(busyDelay <= 0f){
        busy = false;
        busyDelay = 0f;
        busyEndHandler();
      }
    }  
  }
  
  public override ItemData GetData(){
    ItemData ret = ItemGetData();
    
    ret.description += "\nDamage: " + BaseDamage + "\n";
    ret.description += "Capacity: " + maxAmmo + "\n";
    ret.description += "Ammo Type: " + ammoType + "\n";
    ret.description += "Range: " + ImpulseStrength;
    
    return ret;
  }

  public override string GetInfo(){
    string ret = name + "[" + inventory.ItemCount() + "/" + maxAmmo;
    
    if(wielder != null){
      IHasAmmo ammoHolder = wielder as IHasAmmo;
      if(ammoHolder != null){
        ret += "/" + ammoHolder.CheckAmmo(ammoType, 0);
      }
    }
    
    ret += "]";
    return ret; 
  }
  
  public Damage GetBaseDamage(){
    return new Damage(BaseDamage);
  }
  
  public int CheckAmmo(string ammoType, int max = 0){
    int quantity = inventory.GetQuantity(Item.Types.Ammo, ammoType); 
    if(max > 0 && quantity > max){
      return max;
    }
    return quantity;
  }
  
  public List<ItemData> RequestAmmo(string ammoType, int max = 0){
    return inventory.RetrieveItems(Item.Types.Ammo, ammoType, max);
  }
  
  public List<ItemData> StoreAmmo(List<ItemData> ammo){
    if(busy){
      return ammo;
    }
    
    List<ItemData> ret = new List<ItemData>();
    
    foreach(ItemData data in ammo){
      if(inventory.ItemCount() < maxAmmo && data.type == Item.Types.Ammo && data.name == ammoType){
        inventory.StoreItemData(data);
      }
      else{
        ret.Add(data);
      }
    }
    
    return ret;
  }
  
  public string[] AmmoTypes(){
    return new string[]{ ammoType };
  }
  
  void StartReload(List<ItemData> newAmmo){
    busy = true;
    busyDelay = 2f;

    OnBusyEnd loadAmmo = () => { 
      foreach(ItemData ammo in newAmmo){
        inventory.StoreItemData(ammo);
      } 
    };

    busyEndHandler = loadAmmo;
  }
  
  public override bool IsBusy(){
    return busy;
  }
  
  public override void Use(Item.Uses use, bool released = false){
    if(busy){
      return;
    }

    switch(use){
      case Uses.A: Fire(); break;
      case Uses.B: break; // Aim logic goes here.
      case Uses.D: Reload(); break;
    }
  }
  
  protected virtual void Fire(){
    if(inventory.ItemCount() < 1 || (Session.NetActive() && !Session.IsServer() )){
      return;
    }
    
    string name = Session.NextItemName();

    if(Session.IsServer()){
      DeferredFire(name);
      Rpc(nameof(DeferredFire), name);
    }
    else{
      DeferredFire(name);
    }
  }

  public bool ExpendAmmo(){
    if(inventory.ItemCount() > 0){
      inventory.RetrieveItem(0, 1);
      return true;
    }
    
    return false;
  } 

  [Remote]
  public void DeferredFire(string name){
    if(!ExpendAmmo()){
      return;
    }
    
    speaker.PlayEffect(Sound.Effects.RifleShot);
    Item projectile = Item.Factory(Item.Types.Bullet);
    projectile.Name = name;
    Projectile proj = projectile as Projectile;
    Actor wielderActor = wielder as Actor;
    
    if(wielderActor != null){
      proj.sender = wielderActor.NodePath();
    }
    
    Vector3 projectilePosition = ProjectilePosition();
    Vector3 globalPosition = this.ToGlobal(projectilePosition);
    Spatial gameNode = (Spatial)Session.GameNode();
    
    Vector3 gamePosition = gameNode.ToLocal(globalPosition);
    projectile.Translation = gamePosition;
    gameNode.AddChild(projectile);

    Transform start = this.GetGlobalTransform();
    Transform destination = start;
    destination.Translated(new Vector3(0, 0, 1));
    
    Vector3 impulse = start.origin - destination.origin;
    projectile.SetAxisVelocity(impulse * ImpulseStrength);
  }

  public override void Equip(object wielder){
    ItemBaseEquip(wielder);
    IHasAmmo ammoHolder = wielder as IHasAmmo;
    
    if(ammoHolder != null){
      List<ItemData> newAmmo = ammoHolder.RequestAmmo(ammoType, maxAmmo);
      
      foreach(ItemData ammo in newAmmo){
        inventory.StoreItemData(ammo);
      }
    }
    
  }

  public override void Unequip(){
    IHasAmmo ammoHolder = wielder as IHasAmmo;
    
    if(ammoHolder != null){
      ammoHolder.StoreAmmo(inventory.RetrieveAllItems());
    }

    ItemBaseUnequip();
  }
  
  private void Reload(){
    int needed = maxAmmo - inventory.ItemCount();
    
    if(needed == 0 || wielder == null){
      return;
    }
    
    IHasAmmo ammoHolder = wielder as IHasAmmo;
    
    if(ammoHolder == null){
      return;
    }
    
    if(ammoHolder.CheckAmmo(ammoType, 1) == 0){
      return;
    }
    
    speaker.PlayEffect(Sound.Effects.RifleReload);
    List<ItemData> receivedAmmo = ammoHolder.RequestAmmo(ammoType, needed);
    
    foreach(ItemData ammo in receivedAmmo){
      inventory.StoreItemData(ammo);
    }
  }
  
  private Vector3 ProjectilePosition(){
    Vector3 current = Translation; 
    Vector3 forward = -Transform.basis.z;
    forward *= ProjectileOffset;
    
    return current + forward;
  }
  
}
