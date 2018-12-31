/*
  Disappears after colliding, damage given if possible.
*/
using System;
using Godot;

public class Projectile : Item {
  int healthDamage = 50;
  public string sender; // Node path for actor that fired this projectile.
  
  [Remote]
  public override void DoOnCollide(object body){
    IReceiveDamage receiver = body as IReceiveDamage;
    if(receiver != null){;
      GiveDamage(receiver);
    }
    this.QueueFree();
  }

  void GiveDamage(IReceiveDamage receiver){
    if(stopColliding){
      return;
    }
    
    Damage damage = new Damage();
    damage.health = healthDamage;
    damage.sender = sender;
    receiver.ReceiveDamage(damage);
    stopColliding = true;
  }
}
