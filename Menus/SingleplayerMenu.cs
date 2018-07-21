// Allow player to configure their singleplayer game before starting.
using Godot;

public class SingleplayerMenu : Container, IMenu {
  Godot.Button mainMenuButton;
  Godot.Button startButton;

  Session.Gamemodes activeMode = Session.Gamemodes.None;

  IMenu arenaConfig;
  IMenu adventureConfig;

  public void Init(float minX, float minY, float maxX, float maxY){
  	GD.Print("Init singleplayer menu");
    InitControls();
    ScaleControls();
  }
  
  public void Resize(float minX, float minY, float maxX, float maxY){
  	ScaleControls();
  }

  public bool IsSubMenu(){
    return true;
  }

  public void Clear(){
    this.QueueFree();
  }

  void InitControls(){
    startButton = Menu.Button("Start Game", StartGame);
    AddChild(startButton);

    mainMenuButton = Menu.Button("Main Menu", ReturnToMainMenu);
    AddChild(mainMenuButton);


    arenaConfig = Menu.SubMenuFactory(Menu.SubMenus.ArenaConfig) as IMenu;
    adventureConfig = Menu.SubMenuFactory(Menu.SubMenus.AdventureConfig) as IMenu;
  }


  public void SetMenu(Session.Gamemodes mode){
    if(mode == activeMode){
      GD.print("Already set to " + mode);
      return;
    }
    switch(activeNode){
      case Session.Gamemode.None:
        break;
      case Session.Gamemode.Arena:
        RemoveChild(arenaConfig as Node);
        break;
      case Session.Gamemode.Adventure:
        RemoveChild(adventureConfig as Node);
        break; 
    }
  }

  void ScaleControls(){
    Rect2 screen = this.GetViewportRect();
    float width = screen.Size.x;
    float height = screen.Size.y;
    float wu = width/10; // relative height and width units
    float hu = height/10;

    Menu.ScaleControl(startButton, 2 * wu, hu, 0, 0);
    Menu.ScaleControl(mainMenuButton, 2 * wu, hu, 0, height - hu);


    arenaConfig.Init(2 * wu, hu, width, height);
    adventureConfig.Init(2 * wu, hu, width, height);
  }

  public void StartGame(){
    switch(activeMode){
      case Session.Gamemodes.None:
        GD.Print("No gamemode selected");
        break;
      case Session.Gamemodes.Arena:
        Session.SinglePlayerArena();
        break;
      case Session.Gamemodes.Adventure:
        GD.Print("Play singleplayer adventure mode");
        break;
    }
  }

  public void ReturnToMainMenu(){
    Session.ChangeMenu(Menu.Menus.Main);
  }
}