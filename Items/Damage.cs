using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;


public class Damage{
    public int health; // HP loss, Negative values heal.
    
    public Damage(int health = 0){
      this.health = health;
    }

}
