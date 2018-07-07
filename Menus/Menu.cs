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
      case Menus.HUD: ret = NewHUDMenu(); break;
      case Menus.Pause: ret = NewPauseMenu(); break;
      case Menus.Main: ret = NewMainMenu(); break;
      case Menus.Multiplayer: ret = NewMultiplayerMenu(); break;
      case Menus.Lobby: ret = NewLobbyMenu(); break;
      case Menus.Inventory: ret = NewInventoryMenu(); break;
      case Menus.Settings: ret = NewSettingsMenu(); break;
    }
    return ret;
  }
  

  public static Node NewMainMenu(){
    Node menuInstance = new MainMenu();
    MainMenu menu = (MainMenu)menuInstance;
    Session.session.AddChild(menu);
    menu.Init();
    return menuInstance;
  }
  
  public static Node NewMultiplayerMenu(){
    Node menuInstance = new MultiplayerMenu();
    MultiplayerMenu menu = (MultiplayerMenu)menuInstance;
    Session.session.AddChild(menu);
    menu.Init();
    return menuInstance;
  }
  
  public static Node NewLobbyMenu(){
    Node menuInstance = new LobbyMenu();
    LobbyMenu menu = (LobbyMenu)menuInstance;
    Session.session.AddChild(menu);
    menu.Init();
    return menuInstance;
  }
  
  public static Node NewPauseMenu(){
    Node menuInstance = new PauseMenu();
    PauseMenu menu = (PauseMenu)menuInstance;
    Session.session.AddChild(menu);
    menu.Init();
    return menuInstance;
  }
  
  public static Node NewHUDMenu(){
    Node menuInstance = new HUDMenu();
    HUDMenu menu = (HUDMenu)menuInstance;
    Session.session.AddChild(menu);
    menu.Init();
    return menuInstance;
  }

  public static Node NewInventoryMenu(){
    Node menuInstance = new InventoryMenu();
    menuInstance.Name = "InventoryMenu";
    InventoryMenu menu = (InventoryMenu)menuInstance;
    Session.session.AddChild(menu);
    menu.Init();
    return menuInstance; 
  }

  public static Node NewSettingsMenu(){
    Node menuInstance = new SettingsMenu();
    menuInstance.Name = "SettingsMenu";
    SettingsMenu menu = (SettingsMenu)menuInstance;
    Session.session.AddChild(menu);
    menu.Init();
    return menuInstance;  
  }
  
  public static void ScaleControl(Control control, float width, float height, float x, float y){
    if(control == null){ return; }
    control.SetSize(new Vector2(width, height)); 
    control.SetPosition(new Vector2(x, y)); 
  }
}
