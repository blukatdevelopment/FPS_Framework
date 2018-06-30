// Provides a powerup effect on contact.
using Godot;
public class PowerUp : Item {
  
  [Remote]
  public override void DoOnCollide(object body){
    ApplyPowerUp(body);
  }
  
  public virtual void ApplyPowerUp(object obj){
    
  }

  public override void Interact(object interactor, Item.Uses interaction = Item.Uses.A){
	ApplyPowerUp(interactor);
  }
  
}