using Godot;
using System;

/*
  A brain is a base class for scripts that control an Actor's behavior.
*/

public class Brain {
  protected Actor actor;
  protected Spatial eyes;
  
  public Brain(Actor actor, Spatial eyes = null){
      this.actor = actor;
      this.eyes = eyes;
  }
  
  /* Called from update loop. Override this to control the actor.*/
  public virtual void Update(float delta){
    GD.Print("Please override this method:Update");
  }
  
}