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
