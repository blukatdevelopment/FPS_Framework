using Godot;
using System;
using System.Collections.Generic;


public class Session : Node {
  public static Session session;
  private List<Actor> actors;
  private Node activeMenu;
  private NetworkedMultiplayerENet peer;
  private const int Port = 8080;
  private const int MaxPlayers = 10;
  public const string DefaultServerAddress = "192.168.1.1";

  public override void _Ready() {
    EnforceSingleton();
    ChangeMenu(Menu.Menus.Main);
  }
  
  public void InitServer(){
    peer = new Godot.NetworkedMultiplayerENet();
    peer.CreateServer(Port, MaxPlayers);
    this.GetTree().SetNetworkPeer(peer);
  }

  public void InitClient(string address){
    peer = new Godot.NetworkedMultiplayerENet();
    peer.CreateClient(address, Port);
    this.GetTree().SetNetworkPeer(peer);
  }

  public void ChangeMenu(Menu.Menus menu){
    if(activeMenu != null){
      activeMenu.QueueFree();
      activeMenu = null;
    }
    activeMenu = Menu.MenuFactory(menu);
    if(activeMenu != null){ this.AddChild(activeMenu); }
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
