public class PowerUp : Item {
  
  public override void DoOnCollide(object body){
    ApplyPowerUp(body);
  }
  
  public virtual void ApplyPowerUp(object obj){
    
  }
  
}