using System;
using Godot;

public class Projectile : Item {
  int healthDamage = 50;
  
  [Remote]
  public override void DoOnCollide(object body){
    GD.Print("Projectile:DoOnCollide");
    IReceiveDamage receiver = body as IReceiveDamage;
    if(receiver != null){
      GD.Print("Giving damage");
      GiveDamage(receiver);
    }
    this.QueueFree();
  }

  
  void GiveDamage(IReceiveDamage receiver){
    Damage damage = new Damage();
    damage.health = healthDamage;
    receiver.ReceiveDamage(damage);
  }
}
