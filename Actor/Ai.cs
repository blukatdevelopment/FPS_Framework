using Godot;
using System;

public class Ai : Brain
{
  
  public Ai(Actor actor) : base (actor){}
  
  /* Method STUB */
  public override void Update(float delta){
    GD.Print("AI update");
  }

}
