using Godot;
using System;

public class ProjectileWeapon : Item, IUse, IWeapon {
  
  const int BaseDamage = 10;
  
  public void Init(){
    
  }
  
  public Damage GetBaseDamage(){
    return new Damage(BaseDamage);
  }
  
  public override void Use(Item.Uses use){
    switch(use){
      case Uses.A: Fire(); break;
      case Uses.B: GD.Print("Aim"); break;
      case Uses.D: GD.Print("Reload"); break;
    }
  }
  
  protected virtual void Fire(){
    Item projectile = Item.Factory(Item.Types.Bullet);
    
  }
}
