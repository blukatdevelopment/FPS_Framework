/*
  Instantiates and fires projectiles, launching them forward. 
  Limited by a store of ammo wich must be reloaded with delay.
*/
using Godot;
using System;

public class ProjectileWeapon : Item, IWeapon, IHasAmmo, IEquip {
  
  const int BaseDamage = 10;
  const float ProjectileOffset = 0.1f;
  const float ImpulseStrength = 50f;
  string ammoType = "bullet";
  int ammo = 10;
  int maxAmmo = 10;
  
  float busyDelay = 0f;
  bool busy = false;
  delegate void OnBusyEnd();
  OnBusyEnd busyEndHandler;
  
  public void Init(){
    
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
    string ret = name + "[" + ammo + "/" + maxAmmo;
    if(wielder != null){
      IHasAmmo ammoHolder = wielder as IHasAmmo;
      if(ammoHolder != null){
        ret += "/" + ammoHolder.CheckAmmo(ammoType, -1);
      }
    }
    ret += "]";
    return ret; 
  }
  
  public Damage GetBaseDamage(){
    return new Damage(BaseDamage);
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
    if(ammoType != this.ammoType || busy){
      return max;
    }
    
    int amount = max + ammo;
    
    if(maxAmmo <= amount){
      StartReload(maxAmmo);
      amount -= maxAmmo;
    }
    else{
      int fullAmmo = ammo + amount;
      StartReload(fullAmmo);
      amount = 0;
    }
    
    return amount;
  }
  
  public string[] AmmoTypes(){
    
    return new string[]{ ammoType };
  }
  
  void StartReload(int finalAmmo){
    busy = true;
    busyDelay = 2f;
    OnBusyEnd loadAmmo = () => { 
      ammo = finalAmmo; 
    };
    busyEndHandler = loadAmmo;
  }
  
  public override bool IsBusy(){
    return busy;
  }
  
  public override void Use(Item.Uses use, bool released = false){
    switch(use){
      case Uses.A: Fire(); break;
      case Uses.B: break; // Aim logic goes here.
      case Uses.D: Reload(); break;
    }
  }
  
  /* Assumes GameNode is a spatial. TODO: Clean that up. */
  protected virtual void Fire(){
    if(ammo < 1 || (Session.NetActive() && !Session.IsServer() )){
      return;
    }
    
    string name = Session.NextItemId();

    if(name == ""){
      GD.Print("ProjectileWeapon.Fire: NextItemId was blank.");
    }

    if(Session.IsServer()){
      DeferredFire(name);
      Rpc(nameof(DeferredFire), name);
    }
    else{
      DeferredFire(name);
    }
  }

  [Remote]
  public void DeferredFire(string name){
    ammo--;
    speaker.PlayEffect(Sound.Effects.RifleShot);
    Item projectile = Item.Factory(Item.Types.Bullet, name);
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
  
  private void Reload(){
    int needed = maxAmmo - ammo;
    
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
    int receivedAmmo = ammoHolder.RequestAmmo(ammoType, needed);
    StoreAmmo(ammoType, receivedAmmo);
  }
  
  private Vector3 ProjectilePosition(){
    Vector3 current = Translation; 
    Vector3 forward = -Transform.basis.z;
    forward *= ProjectileOffset;
    return current + forward;
  }
  
}
