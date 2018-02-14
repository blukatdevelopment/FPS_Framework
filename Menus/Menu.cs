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
    PackedScene menuPs = (PackedScene)GD.Load("res://Scenes/Prefabs/Menus/MainMenu.tscn");
    Node menuInstance = menuPs.Instance();
    MainMenu menu = (MainMenu)menuInstance;
    menu.Init();
    return menuInstance;
  }
  
  public static Node MultiplayerMenu(){
    PackedScene menuPs = (PackedScene)GD.Load("res://Scenes/Prefabs/Menus/MultiplayerMenu.tscn");
    Node menuInstance = menuPs.Instance();
    MainMenu menu = (MainMenu)menuInstance;
    menu.Init();
    return menuInstance;
  }
  
  public static Node LobbyMenu(){
    PackedScene menuPs = (PackedScene)GD.Load("res://Scenes/Prefabs/Menus/LobbyMenu.tscn");
    Node menuInstance = menuPs.Instance();
    LobbyMenu menu = (LobbyMenu)menuInstance;
    menu.Init();
    return menuInstance;
  }
}
