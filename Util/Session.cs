/*
  This pseudo-singleton is a dumping place for session-specific data and methods.
  The Session should be the root of the scene in the game. If it's null, things simply
  won't work.

*/
using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;


public class Session : Node {
  public static Session session;
  private Node activeMenu;
  public Arena arena;
  public NetworkSession netSes;
   

  public override void _Ready() {
    EnforceSingleton();
    ChangeMenu(Menu.Menus.Main);
    //ShowMethods(typeof());
    //ShowProperties(typeof(KinematicCollision));
  }
  
  public void Quit(){
    GetTree().Quit();  
  }
  
  /* Convenience method for creating nodes. */
  public static Node Instance(string path){
    PackedScene packedScene = (PackedScene)GD.Load(path);
    if(packedScene == null){
      GD.Print("Path [" + path + "] is invalid." );
      return null;
    }
    return packedScene.Instance();
  }
  
  /* Remove game nodes/variables in order to return it to a menu. */
  public void ClearGame(){
    if(arena != null){
      arena.QueueFree();
      arena = null;
    }
  }
  
  public void QuitToMainMenu(){
    ClearGame();
    ChangeMenu(Menu.Menus.Main);
  }
  
  /* Kills everything and starts a single-player game session. */
  public void SinglePlayerGame(){
    GD.Print("SinglePlayerGame");
    Node arenaNode = Arena.ArenaFactory();
    arena = (Arena)arenaNode;
    arena.Init(true);
    AddChild(arenaNode);
    //ChangeMenu(Menu.Menus.HUD); Disabled until HUD menu is added in.
    ChangeMenu(Menu.Menus.None);
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
      GD.Print(prop.Name);
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
    }
  }
}
