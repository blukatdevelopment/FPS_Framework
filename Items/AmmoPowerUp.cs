/*
  Supplies ammo when picked up.
*/
using Godot;
using System.Collections.Generic;

public class AmmoPowerUp : PowerUp {
  public int ammo = 50;
  public string ammoType = "Bullet";
  
  public override void ApplyPowerUp(object obj){
    IHasAmmo receiver = obj as IHasAmmo;
    if(receiver == null){
      return;
    }
    
    List<Item> ammoItems = Item.BulkFactory(Item.Types.Ammo, ammoType, "", ammo);

    receiver.StoreAmmo(Item.ConvertListToData(ammoItems));
    this.QueueFree();
  }
}
