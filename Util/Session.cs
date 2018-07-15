/*
  This pseudo-singleton is the focal point for the active session's state.
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

<<<<<<< HEAD
  public JukeBox jukeBox;
=======
  // Settings
  public ArenaSettings arenaSettings; // Set up just before Arena game
  public float masterVolume, sfxVolume, musicVolume;
  public string userName;
  public float mouseSensitivityX, mouseSensitivityY;
>>>>>>> develop
  

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
<<<<<<< HEAD
=======

  public void InitSettings(){
    GD.Print("InitSettings");
    SettingsDb db = SettingsDb.Init();
    masterVolume = Util.ToFloat(db.SelectSetting("master_volume"));
    sfxVolume = Util.ToFloat(db.SelectSetting("sfx_volume"));
    musicVolume = Util.ToFloat(db.SelectSetting("music_volume"));
    userName = db.SelectSetting("username");
    mouseSensitivityX = Util.ToFloat(db.SelectSetting("mouse_sensitivity_x"));
    mouseSensitivityY = Util.ToFloat(db.SelectSetting("mouse_sensitivity_y"));
    db.Close();
    Sound.RefreshVolume();
  }

  public static void SaveSettings(){
    SettingsDb db = SettingsDb.Init();
    db.StoreSetting("master_volume", "" + Session.session.masterVolume);
    db.StoreSetting("sfx_volume", "" + Session.session.sfxVolume);
    db.StoreSetting("music_volume", "" + Session.session.musicVolume);
    db.StoreSetting("mouse_sensitivity_x", "" + Session.session.mouseSensitivityX);
    db.StoreSetting("mouse_sensitivity_y", "" + Session.session.mouseSensitivityY);
    db.StoreSetting("username", Session.session.userName);
    db.Close();
    Sound.RefreshVolume();
  }
>>>>>>> develop
  
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
  
<<<<<<< HEAD
  public void SinglePlayerGame(){
=======
  public static void SinglePlayerArena(){
>>>>>>> develop
    ChangeMenu(Menu.Menus.None);
    ChangeMenu(Menu.Menus.HUD);
    Node arenaNode = Arena.ArenaFactory();
    arena = (Arena)arenaNode;
    AddChild(arenaNode);
    arena.Init(true);
    
  }

<<<<<<< HEAD
  public void MultiPlayerGame(){
=======
  public static void MultiplayerArena(){
    Session ses = Session.session;
>>>>>>> develop
    ChangeMenu(Menu.Menus.None);
    Node arenaNode = Arena.ArenaFactory();
    AddChild(arenaNode);
    arena = (Arena)arenaNode;
    arena.Init(false);
    if(netSes.isServer == false){
      ChangeMenu(Menu.Menus.HUD);
    }
  }

<<<<<<< HEAD
  public void ChangeMenu(Menu.Menus menu){
    if(activeMenu != null){
      activeMenu.QueueFree();
      activeMenu = null;
    }
    activeMenu = Menu.MenuFactory(menu);
=======
  public static void ChangeMenu(Menu.Menus menu){
    Session ses = Session.session;
    if(ses.activeMenu != null){
      IMenu menuInstance = ses.activeMenu as IMenu;
      if(menuInstance != null){
        menuInstance.Clear();
      }
      else{
        ses.activeMenu.QueueFree();
      }
      ses.activeMenu = null;
    }

    ses.activeMenu = Menu.MenuFactory(menu);
>>>>>>> develop
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
