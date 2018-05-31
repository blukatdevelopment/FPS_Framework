using Godot;
using System;
using System.Collections.Generic;

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
    if(remainingDelay <= 0f && !actor.IsBusy()){
      remainingDelay = Delay;
      See();
      Act();
    }
    
  }
  
  /* Follow target blindly */
  void Pursue(){
    if(target == null){
      return;
    }
    // This is local to the parent, which is not a robust solution.
    Vector3 tPos = target.Translation;
    Transform lookingAt = host.Transform.LookingAt(tPos, host.Up());
    Vector3 hRot = host.GetRotationDegrees();
    Vector3 lRot = lookingAt.basis.GetEuler();
    lRot = Util.ToDegrees(lRot);
    Vector3 turnRot = (lRot - hRot).Normalized();
    host.Turn(turnRot.y, turnRot.x);
  }
  
  void AimAt(Vector3 point){
    
  }
  
  Actor RayCastForActor(Vector3 start, Vector3 end){
    PhysicsDirectSpaceState spaceState = host.GetWorld().DirectSpaceState as PhysicsDirectSpaceState;
    var result = spaceState.IntersectRay(start, end);
    
    if(!result.ContainsKey("collider")){
      return null;
    }
    object collider = result["collider"];
    Actor candidate = collider as Actor;
    if(candidate != host){
      return candidate;
    }
    return null;
  }
  
  /* 
    Can't boxcast? Gridcast! 
    This will create a grid of raycasts to mimick the functionality of a 
    boxcast that is abstructed by obstacles.
    scale decides how many layers of raycasts surround the center
    spacing decides how far apart each layer is
  */
  List<Actor> GridCastForActor(Vector3 start, Vector3 end, int scale = 3, float spacing = 0.5f){
    List<Actor> found = new List<Actor>();
    if(scale < 0){
      scale *= -1;
    }
    
    List<Vector3> starts = new List<Vector3>();
    List<Vector3> ends = new List<Vector3>();
    
    float offX, offY;
    
    for(int i = -scale; i < scale; i++){
      for(int j = -scale; j < scale; j++){
        Vector3 otherStart = start;
        Vector3 otherEnd = end;
        offX = spacing * j;
        offY = spacing * i;
        otherStart += new Vector3(offX, offY, 0f);
        otherEnd += new Vector3(offX, offY, 0f);
        Actor candidate = RayCastForActor(otherStart, otherEnd);
        if(candidate != null && !found.Contains(candidate)){
          found.Add(candidate);
        }
      }
    }
    
    return found;
  }
  
  void See(){
    if(target == null){
      AcquireTarget();
    }
  }
  
  void AcquireTarget(){
    float distance = 100f;
    Vector3 start = actor.HeadPosition();
    Vector3 end = actor.Forward();
    end *= distance; // Move distance
    end += start; // Add starting position
    
    List<Actor> targets = GridCastForActor(start, end);
    if(targets.Count > 0){
      target = targets[0];
      GD.Print("Target acquired! " + target);
    }
  }
  
  void Act(){
    if(target == null){
      return;
    }
    Pursue();
    return;
    actor.Use(Item.Uses.A);
    target = null;
    
    IHasAmmo ammoHaver = actor.PrimaryItem() as IHasAmmo;
    if(ammoHaver != null){
      ManageAmmo(ammoHaver);
    }
  }
  
  void ManageAmmo(IHasAmmo ammoHaver){
    string[] ammoTypes = ammoHaver.AmmoTypes();
    if(ammoTypes == null || ammoTypes.Length == 0){
      return;
    }
    
    string ammoType = ammoTypes[0];
    int haverCount = ammoHaver.CheckAmmo(ammoType, -1);
    int actorCount = actor.CheckAmmo(ammoType, -1);
    if(haverCount == 0 && actorCount > 0){
      actor.Use(Item.Uses.D);
    }
  }

}
