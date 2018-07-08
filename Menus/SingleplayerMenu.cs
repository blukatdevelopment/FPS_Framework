// Allow player to configure their singleplayer game before starting.
using Godot;

public class SingleplayerMenu : Container, IMenu {
  Godot.Button mainMenuButton;
  Godot.Button startButton;

  IMenu arenaConfig;

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


    Node arenaConfigNode = Menu.SubMenuFactory(Menu.SubMenus.ArenaConfig);
    AddChild(arenaConfigNode);
    arenaConfig = arenaConfigNode as IMenu;
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
  }

  public void StartGame(){
    Session.SinglePlayerArena();
  }

  public void ReturnToMainMenu(){
    Session.ChangeMenu(Menu.Menus.Main);
  }
}