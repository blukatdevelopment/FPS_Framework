using Godot;
using System;

public class Menu{
  public enum Controls{ Button }; 
  public enum Menus{ None, Main };
  
  /* Returns instances of desired control. */
  public static Control ControlFactory(Controls control){
    switch(control){
      case Controls.Button: return Button(); break;
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
  
  public static Node MenuFactory(Menus menu){
    Node ret = null;
    switch(menu){
      case Menus.None: return null; break;
      case Menus.Main: ret = MainMenu(); break;
    }
    return ret;
  }
  
  /* Returns a Node tree consisting of a camera and some controls. */
  public static Node MainMenu(){
    PackedScene menu_ps = (PackedScene)GD.Load("res://Scenes/Prefabs/Menus/MainMenu.tscn");
    Node menu_instance = menu_ps.Instance();
    MainMenu menu = (MainMenu)menu_instance;
    menu.SetStartButton((Godot.Button)Button(text : "Kill Menu.", onClick: menu.Start));
    return menu_instance;
  }
}
