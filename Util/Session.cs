/*
  This pseudo-singleton is a dumping place for session-specific data and methods.
  The Session should be the root of the scene in the game. If it's null, things simply
  won't work.

*/
using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;


public class Session : Node {
  public static Session session;
  private Node activeMenu;
  public Arena arena;
  public NetworkSession netSes;

  public JukeBox jukeBox;
  

  public Actor player;

  public static string NextItemId(){
    if(Session.session.arena != null){
      return Session.session.arena.NextItemName();
    }
    return "";
  }

  public override void _Ready() {
    EnforceSingleton();
    ChangeMenu(Menu.Menus.Main);
    //ShowMethods(typeof(Godot.RigidBody));
    //ShowProperties(typeof(Godot.CollisionShape));
    //ShowVariables(typeof(Godot.RigidBody));
  }
  
  public void Quit(){
    GetTree().Quit();  
  }
  
  public void InitJukeBox(){
    if(jukeBox != null){
      return;
    }
    jukeBox = (JukeBox)Instance("res://Scenes/JukeBox.tscn");
    AddChild(jukeBox);
  }
  
  public static System.Random GetRandom(){
    if(Session.session.netSes != null && Session.session.netSes.random != null){
      return Session.session.netSes.random;
    }
    return new System.Random();
  }

  public static bool NetActive(){
    if(session.netSes != null){
      return true;
    }
    return false;
  }

  /* Used for syncing items. */
  public static bool IsServer(){
    if(session.netSes != null){
      return session.netSes.isServer;
    }
    return false;
  }

  /* Convenience method for creating nodes. */
  public static Node Instance(string path){
    byte[] bytes = Encoding.Default.GetBytes(path);
    path = Encoding.UTF8.GetString(bytes);
    PackedScene packedScene = (PackedScene)GD.Load(path);
    if(packedScene == null){
      GD.Print("Path [" + path + "] is invalid." );
      return null;
    }
    return packedScene.Instance();
  }
  
  /* Remove game nodes/variables in order to return it to a menu. */
  public void ClearGame(bool keepNet = false){
    if(arena != null){
      arena.QueueFree();
      arena = null;
    }
    if(!keepNet && netSes != null){
      netSes.QueueFree();
      netSes = null;
    }
    Input.SetMouseMode(Input.MouseMode.Visible);
  }
  
  public static Node GameNode(){
    if(Session.session.arena != null){
      return Session.session.arena;
    }
    return Session.session;
  }
  
  public void QuitToMainMenu(){
    ChangeMenu(Menu.Menus.Main);
    ClearGame();
  }
  
  public void SinglePlayerGame(){
    ChangeMenu(Menu.Menus.None);
    ChangeMenu(Menu.Menus.HUD);
    Node arenaNode = Arena.ArenaFactory();
    arena = (Arena)arenaNode;
    AddChild(arenaNode);
    arena.Init(true);
    
  }

  public void MultiPlayerGame(){
    ChangeMenu(Menu.Menus.None);
    Node arenaNode = Arena.ArenaFactory();
    AddChild(arenaNode);
    arena = (Arena)arenaNode;
    arena.Init(false);
    if(netSes.isServer == false){
      ChangeMenu(Menu.Menus.HUD);
    }
  }

  public void ChangeMenu(Menu.Menus menu){
    if(activeMenu != null){
      activeMenu.QueueFree();
      activeMenu = null;
    }
    activeMenu = Menu.MenuFactory(menu);
  }
  
  private void EnforceSingleton(){
    if(Session.session == null){ Session.session = this; }
    else{ this.QueueFree(); }
  }
  
  // Use this to find methods for classes.
  // Because Godot lacks real C# documentation.
  public static void ShowMethods(Type type){
    foreach (var method in type.GetMethods()){
      string ret = "" + method.ReturnType +"," + method.Name;
      foreach( var parameter in method.GetParameters()){
        ret += ", " + parameter.ParameterType + " " + parameter.Name; 
      }
      GD.Print(ret);
      System.Threading.Thread.Sleep(100);
    }
  }
  
  // Use this to find variables for classes, because Godot
  // Lacks real C# documentation
  public static void ShowProperties(Type type){
    foreach(PropertyInfo prop in type.GetProperties()){
      GD.Print(prop.Name + " : " + prop.PropertyType );
    }
  }
  
  // Use this to print out class variables 
  public static void ShowVariables(Type type){
    BindingFlags bindingFlags = BindingFlags.Public |
                            BindingFlags.NonPublic |
                            BindingFlags.Instance |
                            BindingFlags.Static;

    foreach (FieldInfo field in type.GetFields(bindingFlags)) {
        GD.Print(field.Name);
        System.Threading.Thread.Sleep(100);
    }
  }
  
  public string GetObjectiveText(){
    if(arena != null){
      return arena.GetObjectiveText();
    }
    return "Fight the enemies.";
  }
  
  /* Trickle events down from the Session */
  public void HandleEvent(SessionEvent sessionEvent){
    if(arena != null){
      arena.HandleEvent(sessionEvent);
    }
  }
  
  // Static convenience method.
  public static void Event(SessionEvent sessionEvent){
    Session.session.HandleEvent(sessionEvent);
  }
}
