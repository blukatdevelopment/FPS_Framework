using Godot;

public class AmmoPowerUp : PowerUp {
  public int ammo = 50;
  public string ammoType = "bullet";
  
  public override void ApplyPowerUp(object obj){
    IHasAmmo receiver = obj as IHasAmmo;
    if(receiver == null){
      return;
    }
    
    receiver.StoreAmmo(ammoType, ammo);
    GD.Print("Ammo applied");
    this.QueueFree();
  }
}
