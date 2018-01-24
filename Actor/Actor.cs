using Godot;
using System;

public class Actor : Node
{
  
  public enum Brains{Player1, Ai}; // Possible brains to use.
  private Brain brain;
  
  public void Init(Brains b = Brains.Player1){
    switch(b){
      case Brains.Player1: brain = (Brain)new ActorInputHandler(this); break;
      case Brains.Ai: brain = (Brain)new Ai(this); break;
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

  public static Actor ActorFactory(Brains b = Brains.Player1){
    PackedScene ps = (PackedScene)GD.Load("res://Scenes/Prefabs/Actor.tscn");
    Node st = ps.Instance();
    Actor a = (Actor)st;
    a.Init(b);
    return a;
    
  }
}
