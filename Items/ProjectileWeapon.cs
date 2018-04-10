using Godot;
using System;

public class ProjectileWeapon : Item, IUse, IWeapon {
  
  const int BaseDamage = 10;
  const float ProjectileOffset = 3f;
  const float ImpulseStrength = 10f;
  
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
    
    Vector3 projectilePosition = ProjectilePosition();
    Vector3 globalPosition = this.ToGlobal(projectilePosition);
    
    Spatial gameNode = (Spatial)Session.GameNode();
    
    Vector3 gamePosition = gameNode.ToLocal(globalPosition);
    projectile.Translation = gamePosition;
    gameNode.AddChild(projectile);

      
    Vector3 impulse = -this.Transform.basis.z;
    
    // TODO: Find a better way to increase magnitude (Multiplying by a scalar doesn't work)
    impulse *= impulse;
    
    projectile.ApplyImpulse(new Vector3(0, 0, 0), impulse);
  }
  
  private Vector3 ProjectilePosition(){
    Vector3 current = Translation; 
    Vector3 forward = -Transform.basis.z;
    forward *= ProjectileOffset;
    return current + forward;
  }
  
}
