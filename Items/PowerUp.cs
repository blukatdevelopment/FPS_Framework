using Godot;
public class PowerUp : Item {
  
  [Remote]
  public override void DoOnCollide(object body){
    ApplyPowerUp(body);
  }
  
  public virtual void ApplyPowerUp(object obj){
    
  }
  
}