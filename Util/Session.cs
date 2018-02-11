using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;


public class Session : Node {
  public static Session session;
  private Node activeMenu;
  
  public NetworkedMultiplayerENet peer;
  public const int DefaultPort = 27182;
  private const int MaxPlayers = 10;
  public const string DefaultServerAddress = "127.0.0.1";
  public int selfPeerId;
  public Dictionary<int, List<string>> playerInfo;
  public Arena arena;

  public override void _Ready() {
    EnforceSingleton();
    ChangeMenu(Menu.Menus.Main);
    playerInfo = new Dictionary<int, List<string>>();
    //ShowMethods(typeof(Vector3));
  }
  
  public void Quit(){
    GetTree().Quit();  
  }
  
  public void InitServer(Godot.Object obj, string playerJoin, string playerLeave){
    peer = new Godot.NetworkedMultiplayerENet();
    this.GetTree().Connect("network_peer_connected", obj, playerJoin);
    this.GetTree().Connect("network_peer_disconnected", obj, playerLeave);
    peer.CreateServer(DefaultPort, MaxPlayers);
    this.GetTree().SetNetworkPeer(peer);
    selfPeerId = this.GetTree().GetNetworkUniqueId();
  }
  
  public void InitClient(string address, Godot.Object obj, string success, string fail){
    GetTree().Connect("connected_to_server", obj, success);
    GetTree().Connect("connection_failed", obj, fail);
    peer = new Godot.NetworkedMultiplayerENet();
    peer.CreateClient(address, DefaultPort);
    this.GetTree().SetNetworkPeer(peer);
    selfPeerId = this.GetTree().GetNetworkUniqueId();
  }
  
  /* Remove game nodes/variables in order to return it to a menu. */
  public void ClearGame(){
    if(arena != null){
      arena.QueueFree();
      arena = null;
    }
  }
  
  /* Kills everything and starts a single-player game session. */
  public void SinglePlayerGame(){
    GD.Print("SinglePlayerGame");
    Node arenaNode = Arena.ArenaFactory();
    arena = (Arena)arenaNode;
    arena.Init(true);
    AddChild(arenaNode);
    ChangeMenu(Menu.Menus.None);
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
