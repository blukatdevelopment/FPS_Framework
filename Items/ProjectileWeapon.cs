using Godot;
using System;

public class ProjectileWeapon : Item, IUse, IWeapon, IHasAmmo, IEquip {
  
  const int BaseDamage = 10;
  const float ProjectileOffset = 0.1f;
  const float ImpulseStrength = 50f;
  string ammoType = "bullet";
  int ammo = 10;
  int maxAmmo = 10;
  
  public void Init(){
    
  }
  
  public Damage GetBaseDamage(){
    return new Damage(BaseDamage);
  }
  
  /* Show up to max ammo */
  public int CheckAmmo(string ammoType, int max){
    if(this.ammoType != ammoType){
      return 0;
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
    if(maxAmmo < amount){
      ammo = maxAmmo;
      amount -= ammo;
    }
    else{
      ammo += amount;
      amount = 0;
    }
    return amount;
  }
  
  public void Equip(object wielder){
    
  }
  
  public void Unequip(object wielder){
    
  }
  
  public override void Use(Item.Uses use){
    switch(use){
      case Uses.A: Fire(); break;
      case Uses.B: GD.Print("Aim"); break;
      case Uses.D: Reload(); break;
    }
  }
  
  /* Assumes GameNode is a spatial. TODO: Clean that up. */
  protected virtual void Fire(){
    GD.Print("Fire");
    speaker.PlayEffect(Sound.Effects.RifleShot);
    Item projectile = Item.Factory(Item.Types.Bullet);
    
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
    GD.Print("Reload");
    speaker.PlayEffect(Sound.Effects.RifleReload);
  }
  
  private Vector3 ProjectilePosition(){
    Vector3 current = Translation; 
    Vector3 forward = -Transform.basis.z;
    forward *= ProjectileOffset;
    return current + forward;
  }
  
}
