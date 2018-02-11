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
  }
  
  
  public void Move(int x, int y){
      if(debug){ GD.Print("Actor: Moving[" + x + "," + y + "]"); }
      Translate(new Vector3(x, 0, -y));
      //MoveAndCollide(new Vector3(0, 0, 0));
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
  public static Actor ActorFactory(Brains b = Brains.Player1){
    PackedScene actorPs = (PackedScene)GD.Load("res://Scenes/Prefabs/Actor.tscn");
    Node actorInstance = actorPs.Instance();
    
    if(b == Brains.Player1){
      PackedScene eyesPs = (PackedScene)GD.Load("res://Scenes/Prefabs/Eyes.tscn");
      Node eyesInstance = eyesPs.Instance();
      actorInstance.AddChild(eyesInstance);
      
      Vector3 eyesPos = new Vector3(0, 2, 0);
      Spatial eyesSpatial = (Spatial)eyesInstance;
      eyesSpatial.Translate(eyesPos);
    }
    
    
    
    Actor actor = (Actor)actorInstance;
    actor.Init(b);
    return actor;
  }
}
