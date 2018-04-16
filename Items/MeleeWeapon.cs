using Godot;
using System;

public class MeleeWeapon : Item, IUse, IWeapon {
  
  const int HealthDamage = 10;
  
  public void Init(){
    
  }
  
  public Damage GetBaseDamage(){
    return new Damage(HealthDamage);
  }
  
  public override void Use(Item.Uses use){
    switch(use){
      case Uses.A: Swing(); break;
      case Uses.B: GD.Print("Guard"); break;
    }
  }
  
  private void Swing(){
    GD.Print("Swing");
    speaker.PlayEffect(Sound.Effects.FistSwing);
  }
  
}
