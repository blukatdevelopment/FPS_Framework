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
  public Overworld adventure; // Overworld manages adventure mode.
  public NetworkSession netSes;
  public Random random;
  public JukeBox jukeBox;

  // Settings
  public ArenaSettings arenaSettings; // Set up just before Arena game
  public AdventureSettings adventureSettings; // Set up for an adventure game
  public float masterVolume, sfxVolume, musicVolume;
  public string userName;
  public float mouseSensitivityX, mouseSensitivityY;
  

  public enum Gamemodes{
    None,
    Arena,
    Adventure
  };

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
    InitJukeBox();
    InitSettings();
    //ShowMethods(typeof(Godot.RigidBody));
    //ShowProperties(typeof(Godot.CollisionShape));
    //ShowVariables(typeof(Godot.RigidBody));
  }

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
  
  public void Quit(){
    GetTree().Quit(); 
  }
  
  public static void InitJukeBox(){
    if(Session.session.jukeBox != null){
      return;
    }
    Session.session.jukeBox = new JukeBox();
    Session.session.AddChild(Session.session.jukeBox);
  }
  
  public static System.Random GetRandom(){
    if(Session.session.netSes != null && Session.session.netSes.random != null){
      return Session.session.netSes.random;
    }
    if(Session.session.random != null){
      return Session.session.random;
    }
    Session.session.random = new System.Random();
    return Session.session.random;
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
  public static void ClearGame(bool keepNet = false){
    Session ses = Session.session;
    if(ses.arena != null){
      ses.arena.QueueFree();
      ses.arena = null;
    }
    if(ses.adventure != null){
      ses.adventure.QueueFree();
      ses.adventure = null;
    }
    if(!keepNet && ses.netSes != null){
      ses.netSes.QueueFree();
      ses.netSes = null;
    }
    Input.SetMouseMode(Input.MouseMode.Visible);
  }
  
  public static Node GameNode(){
    if(Session.session.arena != null){
      return Session.session.arena;
    }
    return Session.session;
  }
  
  public static void QuitToMainMenu(){
    Session.ChangeMenu(Menu.Menus.Main);
    Session.ClearGame();
  }
  
  public static void SinglePlayerArena(){
    ChangeMenu(Menu.Menus.None);
    ChangeMenu(Menu.Menus.HUD);
    Session ses = Session.session;
    Node arenaNode = Arena.ArenaFactory();
    ses.arena = (Arena)arenaNode;
    ses.AddChild(arenaNode);
    ses.arena.Init(true);
  }

  public static void SinglePlayerAdventure(){
    GD.Print("SinglePlayerAdventure");
    ChangeMenu(Menu.Menus.None);
    Session.session.adventure = new Overworld();
    Session.session.AddChild(Session.session.adventure);
  }

  public static void MultiplayerAdventure(){
    GD.Print("MultiplayerAdventure Server:" + Session.IsServer());
    ChangeMenu(Menu.Menus.None);
    Session.session.adventure = new Overworld();
    Session.session.AddChild(Session.session.adventure);
  }

  public static void MultiplayerArena(){
    Session ses = Session.session;
    ChangeMenu(Menu.Menus.None);
    Node arenaNode = Arena.ArenaFactory();
    ses.AddChild(arenaNode);
    ses.arena = (Arena)arenaNode;
    ses.arena.Init(false);
    if(ses.netSes.isServer == false){
      ChangeMenu(Menu.Menus.HUD);
    }
  }

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
  
  public static string GetObjectiveText(){
    Session ses = Session.session;
    if(ses.arena != null){
      return ses.arena.GetObjectiveText();
    }
    if(ses.adventure != null){
      return ses.adventure.GetObjectiveText();
    }
    return "Fight the enemies.";
  }
  
  /* Trickle events down from the Session */
  public void HandleEvent(SessionEvent sessionEvent){
    if(Session.NetActive() && sessionEvent.type == SessionEvent.Types.ItemDiscarded){
      HandleItemDiscardedEvent(sessionEvent); // Refresh inventory when deferred discard item.
    }
    if(arena != null){
      arena.HandleEvent(sessionEvent);
    }
    if(adventure != null){
      adventure.HandleEvent(sessionEvent);
    }
  }

  public void HandleItemDiscardedEvent(SessionEvent sessionEvent){
    GD.Print("handleItemDiscardEvent");
    if(player.NodePath().ToString() != sessionEvent.args[0]){
      GD.Print("Your InventoryMenu doesn't need to be updated. Returning.");
      return;
    }
    InventoryMenu invMenu = activeMenu as InventoryMenu;
    if(invMenu == null){
      GD.Print("InventoryMenu not active. Not refreshing");
      return;
    }
    invMenu.RefreshInventory();
  }
  
  // Static convenience method.
  public static void Event(SessionEvent sessionEvent){
    Session.session.HandleEvent(sessionEvent);
  }

  public static void InitKit(Actor actor){
    Arena arena = Session.session.arena;
    if(arena != null){
      arena.InitKit(actor);
    }
  }

  public static void PlayerReady(){
    Arena arena = Session.session.arena;
    if(arena != null){
      arena.PlayerReady();
    }
  }

  // TODO: Pass off this logic to Overworld or Arena.
  public static Vector3 WorldPosition(Item item){
    return item.Translation;
  }
}
