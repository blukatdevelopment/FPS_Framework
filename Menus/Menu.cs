using Godot;
using System;

public class Menu{
  public enum Controls{ Button, TextBox }; 
<<<<<<< HEAD
  public enum Menus{ None, Main, Multiplayer, Lobby, Pause, HUD};
=======
  public enum Menus{ // Parented by Session.session
    None, 
    Main,
    Singleplayer,
    Multiplayer, 
    Settings,
    Lobby, 
    Pause, 
    HUD, 
    Inventory
  };

  public enum SubMenus{ // Parented by menu
    None,
    ArenaConfig
  }
>>>>>>> develop
  
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

  public static Label Label(string text = ""){
    Label label = new Label();
    label.Text = text;   
    return label;
  }
  
  public static Node MenuFactory(Menus menu){
    Node ret = null;
    switch(menu){
      case Menus.None: 
        Sound.PauseSong();
        return null; 
        break;
<<<<<<< HEAD
      case Menus.HUD: ret = HUDMenu(); break;
      case Menus.Pause: ret = PauseMenu(); break;
      case Menus.Main: ret = MainMenu(); break;
      case Menus.Multiplayer: ret = MultiplayerMenu(); break;
      case Menus.Lobby: ret = LobbyMenu(); break;
=======
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
      case Menus.Singleplayer: 
        ret = new SingleplayerMenu(); 
        ret.Name = "Singleplayer";
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
>>>>>>> develop
    }
    return ret;
  }

<<<<<<< HEAD
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
  
  public static Node PauseMenu(){
    Node menuInstance = Session.Instance("res://Scenes/Prefabs/Menus/PauseMenu.tscn");
    PauseMenu menu = (PauseMenu)menuInstance;
    Session.session.AddChild(menu);
    menu.Init();
    return menuInstance;
  }
  
  public static Node HUDMenu(){
    Node menuInstance = Session.Instance("res://Scenes/Prefabs/Menus/HUDMenu.tscn");
    HUDMenu menu = (HUDMenu)menuInstance;
    Session.session.AddChild(menu);
    menu.Init();
    return menuInstance;
=======
  public static Node SubMenuFactory(SubMenus menu){
    Node ret = null;
    switch(menu){
      case SubMenus.None: 
        return null;
        break;
      case SubMenus.ArenaConfig:
        ret = new ArenaConfigMenu();
        ret.Name = "ArenaConfig";
        break;
    }
    return ret;
>>>>>>> develop
  }
  
  public static void ScaleControl(Control control, float width, float height, float x, float y){
    if(control == null){ return; }
    control.SetSize(new Vector2(width, height)); 
    control.SetPosition(new Vector2(x, y)); 
  }
}
