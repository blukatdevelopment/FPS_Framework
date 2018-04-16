using System;
using Godot;

public class Projectile : Item {
  int healthDamage = 50;
  
  public override void OnCollide(object body){
    IReceiveDamage receiver = body as IReceiveDamage;
    if(receiver != null){
      GiveDamage(receiver);
    }
  }
  
  void GiveDamage(IReceiveDamage receiver){
    Damage damage = new Damage();
    damage.health = healthDamage;
    receiver.ReceiveDamage(damage);
    this.QueueFree();
  }
}
