using Godot;
using System;

public class ProjectileWeapon : Item, IUse, IWeapon {
  
  const int BaseDamage = 10;
  const float ProjectileOffset = 0.1f;
  const float ImpulseStrength = 50f;
  
  public void Init(){
    
  }
  
  public Damage GetBaseDamage(){
    return new Damage(BaseDamage);
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
