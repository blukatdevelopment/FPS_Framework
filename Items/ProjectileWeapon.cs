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
  
  /* Assumes GameNode is a spatial. TODO: Clean that up. */
  protected virtual void Fire(){
    GD.Print("Fire");
    Item projectile = Item.Factory(Item.Types.Bullet);
    Vector3 globalPosition = this.ToGlobal(this.Translation);
    Spatial gameNode = (Spatial)Session.GameNode();
    Vector3 gamePosition = gameNode.ToLocal(globalPosition);
    projectile.Translation = gamePosition;
    gameNode.AddChild(projectile);
    projectile.ApplyImpulse(new Vector3(0, 0, 0), new Vector3(0, 10, 0));
  }
}
