using Godot;
using System;
using System.Collections.Generic;


public class Session : Node {
  public static Session session;
  private List<Actor> actors;
  
  
  
  public override void _Ready() {
    EnforceSingleton();
  }
  
  private void EnforceSingleton(){
    if(Session.session == null){ Session.session = this; }
    else{ this.QueueFree(); }
  }
  
  private void InitActors(){
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
  
  // Use this to find methods for classes.
  public static void ShowMethods(Type type){
    foreach (var method in type.GetMethods()){
      string ret = "" + method.ReturnType +"," + method.Name;
      foreach( var parameter in method.GetParameters()){
        ret += ", " + parameter.ParameterType + " " + parameter.Name; 
      }
      GD.Print(ret);
    }
    
  }
}
