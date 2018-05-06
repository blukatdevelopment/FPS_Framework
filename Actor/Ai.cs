using Godot;
using System;

public class Ai : Brain
{
  Actor[] actorsInSight;
  Actor host;
  float remainingDelay = 0f;
  const float Delay = 0.03f; 
  public Ai(Actor actor, Eyes eyes) : base (actor, eyes){
    host = actor;
    actorsInSight = new Actor[0];
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
    //GD.Print(collider);
    return null;
  }
  
  void See(){
    RayCastForActor(new Vector3(0, -1, 0), new Vector3(0, 1, 0));
  }
  
  void Think(){
    //GD.Print("Thinking...");
  }

}
