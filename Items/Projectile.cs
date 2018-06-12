using System;
using Godot;

public class Projectile : Item {
  int healthDamage = 50;
  public string sender; // Actor that fired this projectile.
  
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
    damage.sender = sender;
    receiver.ReceiveDamage(damage);
  }
}
