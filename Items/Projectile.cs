/*
  Disappears after colliding, damage given if possible.
*/
using System;
using Godot;

public class Projectile : Item {
  int healthDamage = 50;
  public string sender; // Actor that fired this projectile.
  
  [Remote]
  public override void DoOnCollide(object body){
    IReceiveDamage receiver = body as IReceiveDamage;
    if(receiver != null){;
      GiveDamage(receiver);
    }
    this.QueueFree();
  }

  
  void GiveDamage(IReceiveDamage receiver){
    Damage damage = new Damage();
    damage.health = healthDamage;
    damage.sender = sender;
    receiver.ReceiveDamage(damage);
  }
}
