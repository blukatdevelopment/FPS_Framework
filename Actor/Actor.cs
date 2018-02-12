using Godot;
using System;

public class Actor : KinematicBody
{
  
  public enum Brains{Player1, Ai}; // Possible brains to use.
  private Brain brain;
  private Eyes eyes;
  public bool debug = false;
  
  const int maxY = 90;
  const int minY = -90;
  
  private bool grounded = false; //True when Actor is standing on surface.
  
  public void Init(Brains b = Brains.Player1){
    InitChildren();
    switch(b){
      case Brains.Player1: brain = (Brain)new ActorInputHandler(this, eyes); break;
      case Brains.Ai: brain = (Brain)new Ai(this, eyes); break;
    }
    if(eyes != null){
      eyes.SetRotationDegrees(new Vector3(0, 0, 0));  
    }
  }
  
  
  /* Sets ChildNode references as appropriate. */
  protected void InitChildren(){
    foreach(Node child in this.GetChildren()){
      switch(child.GetName()){
        case "Eyes": eyes = child as Eyes; break;
      }
    }
    if(eyes == null){ GD.Print("Actor's eyes are null!"); }
  }
    
  public override void _Process(float delta)
  {
      if(brain != null){ brain.Update(delta); }
      else{ GD.Print("Brain Null"); }
      Gravity(delta);
  }
  
  public void Gravity(float delta){
    float gravForce = -9.81f;
    Vector3 grav = (new Vector3(0, gravForce, 0) * delta);
    Move(grav.x, grav.y, grav.z);
  }

  
  public void Move(float x, float y,  float z){
      if(debug){ GD.Print("Actor: Moving[" + x + "," + y + "," + z + "]"); }
      Vector3 movement = new Vector3(x, y, -z); 
      
      Transform current = GetTransform();
      Transform destination = current; 
      destination.Translated(movement);
      
      Vector3 delta = destination.origin - current.origin;
      KinematicCollision collision = MoveAndCollide(delta);
      if(!grounded && collision != null && collision.Position.y < GetTranslation().y){
        GD.Print("Landed!");
        grounded = true;
      }
  }
  
  public void Jump(){
    if(!grounded){ return; }
    float jumpHeight = 10;
    Move(0, jumpHeight, 0);
    grounded = false;
  }

  
  public void Turn(float x, float y){
    if(debug){GD.Print("Actor: Turning[" + x + "," + y + "]"); }
    Vector3 bodyRot = this.GetRotationDegrees();
    bodyRot.y += x;
    this.SetRotationDegrees(bodyRot);
    
    Vector3 headRot = eyes.GetRotationDegrees();
    headRot.x += y;
    if(headRot.x < minY){
      headRot.x = minY;  
    }
    if(headRot.x > maxY){
      headRot.x = maxY;
    }
    eyes.SetRotationDegrees(headRot);
  }
  
  
  public void SetPos(Vector3 pos){
    if(debug){ GD.Print("Actor: Set pos to " + pos); }
    SetTranslation(pos);
    
  }
  
  /* The goal of this factory is to set up an actor's node tree in script so it's version controllable. */
  public static Actor ActorFactory(Brains brain = Brains.Player1){
    PackedScene actorPs = (PackedScene)GD.Load("res://Scenes/Prefabs/Actor.tscn");
    Node actorInstance = actorPs.Instance();
    
    if(brain == Brains.Player1){
      PackedScene eyesPs = (PackedScene)GD.Load("res://Scenes/Prefabs/Eyes.tscn");
      Node eyesInstance = eyesPs.Instance();
      actorInstance.AddChild(eyesInstance);
      
      Vector3 eyesPos = new Vector3(0, 2, 0);
      Spatial eyesSpatial = (Spatial)eyesInstance;
      eyesSpatial.Translate(eyesPos);
    }
    
    
    
    Actor actor = (Actor)actorInstance;
    actor.Init(brain);
    return actor;
  }
}
