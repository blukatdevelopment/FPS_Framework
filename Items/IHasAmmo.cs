interface IHasAmmo {
    
	/* Show up to max ammo */
    int CheckAmmo(string AmmoType, int max);

    /* Return up to max ammo, removing that ammo from inventory. */
    int RequestAmmo(string AmmoType, int max);
    
    /* Store up to max ammo, returning overflow. */
    int StoreAmmo(string AmmoType, int max);
}