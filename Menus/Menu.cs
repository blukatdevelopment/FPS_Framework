using Godot;
using System;

public class Menu{
  public enum Controls{ Button, TextBox }; 
  public enum Menus{ None, Main, Lobby};
  
  /* Returns instances of desired control. */
  public static Control ControlFactory(Controls control){
    switch(control){
      case Controls.Button: return Button(); break;
      case Controls.TextBox: return TextBox(); break;
    }
    return null;
  }
  
  public static Control Button(string text = "", Action onClick = null){
    PackedScene button_ps = (PackedScene)GD.Load("res://Scenes/Prefabs/Controls/Button.tscn");
    Node button_instance = button_ps.Instance();
    Button button = (Button)button_instance;
    if(text != ""){ button.SetText(text); }
    if(onClick != null){ button.SetOnClick(onClick); }
    return (Control)button_instance;
  }
  
  public static Control TextBox(string val = ""){
    PackedScene textBox_ps = (PackedScene)GD.Load("res://Scenes/Prefabs/Controls/TextBox.tscn");
    Node textBox_instance = textBox_ps.Instance();
    if(val != ""){
      TextEdit textBox = (Godot.TextEdit)textBox_instance;
      textBox.SetText(val);
    }
    return (Control)textBox_instance;
  }
  
  public static Node MenuFactory(Menus menu){
    Node ret = null;
    switch(menu){
      case Menus.None: return null; break;
      case Menus.Main: ret = MainMenu(); break;
      case Menus.Lobby: ret = LobbyMenu(); break;
    }
    return ret;
  }
  

  public static Node MainMenu(){
    PackedScene menu_ps = (PackedScene)GD.Load("res://Scenes/Prefabs/Menus/MainMenu.tscn");
    Node menu_instance = menu_ps.Instance();
    MainMenu menu = (MainMenu)menu_instance;
    menu.SetSinglePlayerButton((Godot.Button)Button(text : "Single Player", onClick: menu.SinglePlayerGame));
    menu.SetLobbyButton((Godot.Button)Button(text : "Multiplayer", onClick: menu.Lobby));
    menu.SetQuitButton((Godot.Button)Button(text : "Quit", onClick: menu.Quit));
    return menu_instance;
  }
  
  public static Node LobbyMenu(){
    PackedScene menu_ps = (PackedScene)GD.Load("res://Scenes/Prefabs/Menus/LobbyMenu.tscn");
    Node menu_instance = menu_ps.Instance();
    LobbyMenu menu = (LobbyMenu)menu_instance;
    menu.SetMainMenuButton((Godot.Button)Button(text : "MainMenu", onClick: menu.ReturnToMainMenu));
    menu.SetSendButton((Godot.Button)Button(text : "Send", onClick: menu.Send));
    menu.SetJoinButton((Godot.Button)Button(text : "Join", onClick: menu.Join));
    menu.SetHostButton((Godot.Button)Button(text : "Host", onClick: menu.Host));
    menu.SetComposeBox((Godot.TextEdit)TextBox());
    menu.SetMessageBox((Godot.TextEdit)TextBox());
    menu.SetAddressBox((Godot.TextEdit)TextBox(Session.DefaultServerAddress));
    menu.SetNameBox((Godot.TextEdit)TextBox("PlayerName"));
    return menu_instance;
  }
}
