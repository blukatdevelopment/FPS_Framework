using Godot;
using System;

public class Ai : Brain
{
  
  public Ai(Actor actor, Node eyes) : base (actor, eyes){}
  
  /* Method STUB */
  public override void Update(float delta){
    GD.Print("AI update");
  }

}
