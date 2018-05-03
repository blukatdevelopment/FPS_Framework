using Godot;
using System;

public class Ai : Brain
{
  Actor[] actorsInSight;
  
  float remainingDelay = 0f;
  const float Delay = 0.03f; 
  public Ai(Actor actor, Eyes eyes) : base (actor, eyes){
    actorsInSight = Actor[];
  }
  
  public override void Update(float delta){
    remainingDelay -= delta;
    if(remainingDelay <= 0f){
      remainingDelay = delay;
      See();
      Think();
    }
    
  }
  
  void See(){
    
  }
  
  void Think(){
    GD.Print("Thinking...");
  }
  
  
  
  

}
