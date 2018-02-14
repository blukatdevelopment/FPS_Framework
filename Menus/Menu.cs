using Godot;
using System;

public class Menu{
  public enum Controls{ Button, TextBox }; 
  public enum Menus{ None, Main, Multiplayer, Lobby};
  
  /* Returns instances of desired control. */
  public static Control ControlFactory(Controls control){
    switch(control){
      case Controls.Button: return Button(); break;
      case Controls.TextBox: return TextBox(); break;
    }
    return null;
  }
  
  public static Control Button(string text = "", Action onClick = null){
    Node buttonInstance = Session.Instance("res://Scenes/Prefabs/Controls/Button.tscn");
    Button button = (Button)buttonInstance;
    if(text != ""){ button.SetText(text); }
    if(onClick != null){ button.SetOnClick(onClick); }
    return (Control)buttonInstance;
  }
  
  public static Control TextBox(string val = ""){
    Node textBoxInstance = Session.Instance("res://Scenes/Prefabs/Controls/TextBox.tscn");
    if(val != ""){
      TextEdit textBox = (Godot.TextEdit)textBoxInstance;
      textBox.SetText(val);
    }
    return (Control)textBoxInstance;
  }
  
  public static Node MenuFactory(Menus menu){
    Node ret = null;
    switch(menu){
      case Menus.None: return null; break;
      case Menus.Main: ret = MainMenu(); break;
      case Menus.Multiplayer: ret = MultiplayerMenu(); break;
      case Menus.Lobby: ret = LobbyMenu(); break;
    }
    return ret;
  }
  

  public static Node MainMenu(){
    Node menuInstance = Session.Instance("res://Scenes/Prefabs/Menus/MainMenu.tscn");
    MainMenu menu = (MainMenu)menuInstance;
    Session.session.AddChild(menu);
    menu.Init();
    return menuInstance;
  }
  
  public static Node MultiplayerMenu(){
    Node menuInstance = Session.Instance("res://Scenes/Prefabs/Menus/MultiplayerMenu.tscn");
    MultiplayerMenu menu = (MultiplayerMenu)menuInstance;
    Session.session.AddChild(menu);
    menu.Init();
    return menuInstance;
  }
  
  public static Node LobbyMenu(){
    Node menuInstance = Session.Instance("res://Scenes/Prefabs/Menus/LobbyMenu.tscn");
    LobbyMenu menu = (LobbyMenu)menuInstance;
    Session.session.AddChild(menu);
    menu.Init();
    return menuInstance;
  }
}
