/*
  Provides health when picked up.
*/
using Godot;

public class HealthPowerUp : PowerUp {
  const int Health = 25;
  
  public override void ApplyPowerUp(object obj){
    IReceiveDamage receiver = obj as IReceiveDamage;
    
    if(receiver == null){
      return;
    }

    Damage damage = new Damage(-Health);
    receiver.ReceiveDamage(damage);
    this.QueueFree();
  }
}