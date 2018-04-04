using Godot;
using System;

public class ProjectileWeapon : Item, IWeapon {
  
  const int Damage = 10;
  
  public void Init(){
    
  }
  
  public int GetBaseDamage(){
    return Damage;
  }
  
  public override void Use(Item.Uses use){
    switch(use){
      case Uses.A: GD.Print("Fire"); break;
      case Uses.B: GD.Print("Aim"); break;
      case Uses.D: GD.Print("Reload"); break;
    }
  }
}
