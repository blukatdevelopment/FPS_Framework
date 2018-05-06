using Godot;
using System;

public class Ai : Brain
{
  Actor target;
  Actor host;
  
  float remainingDelay = 0f;
  const float Delay = 0.03f; 
  public Ai(Actor actor, Eyes eyes) : base (actor, eyes){
    host = actor;
    target = null;
  }
  
  public override void Update(float delta){
    remainingDelay -= delta;
    if(remainingDelay <= 0f){
      remainingDelay = Delay;
      See();
      Think();
    }
    
  }
  
  Actor RayCastForActor(Vector3 start, Vector3 end){
    PhysicsDirectSpaceState spaceState = host.GetWorld().DirectSpaceState as PhysicsDirectSpaceState;
    var result = spaceState.IntersectRay(start, end);
    
    if(!result.ContainsKey("collider")){
      return null;
    }
    object collider = result["collider"];
    return collider as Actor;
  }
  
  void See(){
    if(target != null){
      return;
    }
    float distance = 100f;
    Vector3 start = actor.HeadPosition();
    Vector3 end = actor.Forward();
    end *= distance; // Move distance
    end += start; // Add starting position
    
    target = RayCastForActor(start, end);
  }
  
  void Think(){
    if(actor.IsBusy()){
      return;
    }
    if(target != null){
      actor.Use(Item.Uses.A);
      target = null;
    }
    IHasAmmo ammoHaver = actor.PrimaryItem() as IHasAmmo;
    if(ammoHaver != null){
      actor.Use(Item.Uses.D);
      return;
    }
    
    
  }

}
