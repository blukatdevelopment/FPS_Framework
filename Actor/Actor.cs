using Godot;
using System;

public class Actor : Node
{
  
  public enum Brains{Player1, Ai}; // Possible brains to use.
  private Brain brain;
  private Node eyes;
  
  public void Init(Brains b = Brains.Player1, Node eyes = null){
    this.eyes = eyes;
    switch(b){
      case Brains.Player1: brain = (Brain)new ActorInputHandler(this, eyes); break;
      case Brains.Ai: brain = (Brain)new Ai(this, eyes); break;
    }
  }
    
  public override void _Process(float delta)
  {
      if(brain != null){ brain.Update(delta); }
      else{ GD.Print("Brain Null"); }
  }
  
  public void Move(int x, int y){
      GD.Print("Moving[" + x + "," + y + "]");
  }
  /* The goal of this factory is to set up an actor's node tree in script so it's version controllable. */
  public static Actor ActorFactory(Brains b = Brains.Player1){
    PackedScene eyes_ps = (PackedScene)GD.Load("res://Scenes/Prefabs/Eyes.tscn");
    Node eyes_instance = ps.Instance();
    
    PackedScene actor_ps = (PackedScene)GD.Load("res://Scenes/Prefabs/Actor.tscn");
    Node actor_instance = ps.Instance();
    
    Actor actor = (Actor)actor_st;
    actor.Init(b, eyes_instance);
    return actor;
  }
}
