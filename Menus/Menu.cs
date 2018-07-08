using Godot;
using System;

public class Menu{
  public enum Controls{ Button, TextBox }; 
  public enum Menus{
    None, 
    Main, 
    Multiplayer, 
    Settings,
    Lobby, 
    Pause, 
    HUD, 
    Inventory};
  
  public static Button Button(string text = "", Action onClick = null){
    Button button = new Button();
    if(text != ""){ button.SetText(text); }
    if(onClick != null){ button.SetOnClick(onClick); }
    return button;
  }
  
  public static TextEdit TextBox(string val = "", bool readOnly = true){    
    TextEdit textBox = new TextEdit();
    textBox.SetText(val);
    textBox.Readonly = readOnly;
  
    return textBox;
  }

  public static HSlider HSlider(float min, float max, float val, float step){    
    HSlider slider = new HSlider();
    slider.MinValue = min;
    slider.MaxValue = max;
    slider.Value = val;
    slider.Step = step;
    
    return slider;
  }
  
  public static Node MenuFactory(Menus menu){
    Node ret = null;
    switch(menu){
      case Menus.None: 
        Sound.PauseSong();
        return null; 
        break;
      case Menus.HUD: 
        ret = new HUDMenu(); 
        ret.Name = "Pause";
        break;
      case Menus.Pause: 
        ret = new PauseMenu();
        ret.Name = "Pause"; 
      break;
      case Menus.Main: 
        ret = new MainMenu();
        ret.Name = "Main"; 
        break;
      case Menus.Multiplayer: 
        ret = new MultiplayerMenu(); 
        ret.Name = "Multiplayer";
        break;
      case Menus.Lobby: 
        ret = new LobbyMenu(); 
        ret.Name = "Lobby";
        break;
      case Menus.Inventory: 
        ret = new InventoryMenu(); 
        ret.Name = "Inventory";
        break;
      case Menus.Settings: 
        ret = new SettingsMenu(); 
        ret.Name = "Settings";
        break;
    }
    Session.session.AddChild(ret);
    IMenu menuInstance = ret as IMenu;
    if(menuInstance != null){
      menuInstance.Init(0, 0, 0, 0); // Assuiming these are not subMenus
    }
    return ret;
  }
  
  public static void ScaleControl(Control control, float width, float height, float x, float y){
    if(control == null){ return; }
    control.SetSize(new Vector2(width, height)); 
    control.SetPosition(new Vector2(x, y)); 
  }
}
