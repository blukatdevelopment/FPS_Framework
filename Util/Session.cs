using Godot;
using System;
using System.Collections.Generic;


public class Session : Node {
  public static Session session;
  private List<Actor> actors;
  
  public override void _Ready() {
    if(Session.session == null){ Session.session = this; }
    else{ this.QueueFree(); }
    actors = new List<Actor>();
    SpawnActor();
  }
  
  /* Create a new actor and place it as a child node. */
  private void SpawnActor() {
    Actor a = Actor.ActorFactory();
    if(a == null){
      GD.Print("Actor was null.");
      return;
    }
    actors.Add(a);
    this.AddChild(a);
  }
  
}
