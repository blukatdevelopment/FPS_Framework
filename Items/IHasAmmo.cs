interface IHasAmmo {
    
    /* Show up to max ammo */
    int CheckAmmo(string ammoType, int max);

    /* Return up to max ammo, removing that ammo from inventory. */
    int RequestAmmo(string ammoType, int max);
    
    /* Store up to max ammo, returning overflow. */
    int StoreAmmo(string ammoType, int max);
    
    /* Returns string representations of available ammo types. */
    string[] AmmoTypes();
}
