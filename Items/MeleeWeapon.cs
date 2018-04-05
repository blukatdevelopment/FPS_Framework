using Godot;
using System;

public class MeleeWeapon : Item, IWeapon {
  
  const int HealthDamage = 10;
  
  public void Init(){
    
  }
  
  public Damage GetBaseDamage(){
    return new Damage(HealthDamage);
  }
  
  public override void Use(Item.Uses use){
    switch(use){
      case Uses.A: GD.Print("Swing"); break;
      case Uses.B: GD.Print("Guard"); break;
    }
  }
  
}
