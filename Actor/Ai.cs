/*
  This is a primitive AI.
  It walks in spirals until finds another actor, at which point it pursues and attacks.
*/

using Godot;
using System;
using System.Collections.Generic;

public class Ai : Brain
{
  Actor target;
  Actor host;
  const float AimMargin = 0.5f; 
  
  float remainingDelay = 0f;
  const float Delay = 0.03f; 

  private float syncTimer = 0f;
  public const float syncRate = 0.05f;


  public Ai(Actor actor, Spatial eyes) : base (actor, eyes){
    host = actor;
    target = null;
  }
  
  public override void Update(float delta){
    remainingDelay -= delta;
    
    if(remainingDelay <= 0f && !actor.IsBusy()){
      remainingDelay = Delay;
      try{
        See();
        Act(delta);
      }
      catch(NullReferenceException ex){
        target = null;
        
      }
    }

    if(Session.IsServer() && Session.NetActive()){
      NetUpdate(delta);
    }
  }

  public void NetUpdate(float delta){
    syncTimer += delta;
    
    if(syncTimer >= syncRate){
      syncTimer = 0f;
      actor.SyncPosition();
      actor.SyncAim();
    }
  }
  
  void Wander(float delta){
    host.Turn(1f, 0f);
    host.Move(new Vector3(0, 0, -host.GetMovementSpeed()), delta);
  }
  
  /* Follow target blindly */
  void Pursue(float delta){
    Vector3 pos = host.Translation;
    Vector3 potentialPos = host.Pointer(1f);
    Vector3 targetPos = target.Translation;

    AimAt(targetPos);

    float currentDist = pos.DistanceTo(targetPos);
    float potentialDist = potentialPos.DistanceTo(targetPos);
    
    if(currentDist > potentialDist){
      host.Move(new Vector3(0, 0, -host.GetMovementSpeed()), delta);
    }
    
  }
  
  void AimAt(Vector3 point){
    // This is local to the parent, which is not an ideal solution.
    
    Transform hostTrans = host.Transform;    
    Transform lookingAt = hostTrans.LookingAt(point, host.Up());
    
    // Get horizontal rotation based on host body.
    Vector3 hostRot = host.GetRotationDegrees();
    
    // Get vertical Rotation based on head when possible.
    hostRot.x = host.RotationDegrees().x;
    
    Vector3 lookingRot = lookingAt.basis.GetEuler();
    lookingRot = Util.ToDegrees(lookingRot);
    Vector3 turnRot = (lookingRot - hostRot).Normalized();
    host.Turn(turnRot.y, turnRot.x);
  }
  
  bool AimingAt(Vector3 point){
    Vector3 hostRot = host.GetRotationDegrees();
    Transform aimedTrans = host.Transform.LookingAt(point, host.Up());
    
    Vector3 aimedRot = aimedTrans.basis.GetEuler();
    aimedRot = Util.ToDegrees(aimedRot);
    
    float aimAngle = hostRot.DistanceTo(aimedRot);
    
    return aimAngle < AimMargin;
  }
  
  void See(){
    if(target == null){
      AcquireTarget();
    }
  }
  
  void AcquireTarget(){
    float distance = 100f;
    Vector3 start = host.HeadPosition();
    
    Vector3 end = host.Forward();
    end *= distance; // Move distance
    end += start; // Add starting position
    
    List<Actor> targets = SeeActors();
    
    foreach(Actor targ in targets){
      if(targ != null && targ != host){
        target = targ;
        return;
      }
    }
  }

  List<Actor> SeeActors(){
    Vector3 start = host.GlobalHeadPosition();
    Vector3 end = host.Pointer();
    World world = host.GetWorld();

    List<object> objects = Util.GridCast(start, end, world, 3, 0.5f);

    List<Actor> ret = new List<Actor>();

    foreach(object obj in objects){
      Actor sighted = obj as Actor;
      
      if(sighted != null){
        ret.Add(sighted);
      }
    }

    return ret;
  }
  
  void Act(float delta){
    if(target == null){
      Wander(delta);
      return;
    }

    Vector3 targetPos = target.Translation;
    
    if(!AimingAt(targetPos)){
      Pursue(delta);
      return;
    }
    
    actor.Use(Item.Uses.A);
    
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
