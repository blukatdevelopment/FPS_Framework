public class PowerUp : Item {
  
  public override void OnCollide(object body){
    ApplyPowerUp(body);
  }
  
  public virtual void ApplyPowerUp(object obj){
    
  }
  
}