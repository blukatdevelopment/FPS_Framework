using Godot;
using System;

public class Menu{
  public enum Controls{ Button }; 
  public enum Menus{ Main };
  
  /* Returns instances of desired control. */
  public static Control ControlFactory(Controls control){
    switch(control){
      case Controls.Button: return Button(); break;
    }
    return null;
  }
  
  public static Control Button(){
    PackedScene button_ps = (PackedScene)GD.Load("res://Scenes/Prefabs/Controls/Button.tscn");
    Node button_instance = button_ps.Instance();
    return (Control)button_instance;
  }
  
  public static Node MenuFactory(Menus menu){
    Node ret = null;
    switch(menu){
      case Menus.Main: ret = MainMenu(); break;
    }
    return ret;
  }
  
  /* Returns a Node tree consisting of a camera and some controls. */
  public static Node MainMenu(){
    PackedScene menu_ps = (PackedScene)GD.Load("res://Scenes/Prefabs/Menus/MainMenu.tscn");
    Node menu_instance = menu_ps.Instance();
    menu_instance.AddChild(Button());
    return menu_instance;
  }
}
