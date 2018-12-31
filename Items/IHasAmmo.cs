// Interface for anything that contains an inventory of ammo.
using System.Collections.Generic;

interface IHasAmmo {
    
    /* Show up to max ammo */
    int CheckAmmo(string ammoType, int max);

    /* Return up to max ammo, removing that ammo from inventory. */
    List<ItemData> RequestAmmo(string ammoType, int max);
    
    /* Store up to max ammo, returning overflow. */
    List<ItemData> StoreAmmo(List<ItemData> ammo);
    
    /* Returns string representations of available ammo types. */
    string[] AmmoTypes();
}
