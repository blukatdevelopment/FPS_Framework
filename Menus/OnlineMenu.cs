/*
  The purpose of the OnlineMenu is to set up a NetworkSession
  and to establish or connect to a server. After a connection has
  been established, the OnlineMenu should flow to the Lobbymenu.
*/
using Godot;
using System;

public class OnlineMenu : Container, IMenu {
  public Godot.Button mainMenuButton;
  public Godot.Button selectionButton;
  public Godot.Button serverButton;
  public Godot.Button clientButton;
  public Godot.TextEdit portBox;
  public Godot.TextEdit addressBox;
  public Godot.TextEdit nameBox;
  public Godot.Button startServerButton;
  public Godot.Button startClientButton;

  public void Init(float minX, float minY, float maxX, float maxY){
    InitControls();
    ScaleControls();
    ShowSelection();
  }
  
  public void Resize(float minX, float minY, float maxX, float maxY){}

  public bool IsSubMenu(){
    return false;
  }

  public void Clear(){
    this.QueueFree();
  }
    
  public void InitControls(){
    mainMenuButton = (Godot.Button)Menu.Button("Main Menu", MainMenu);
    AddChild(mainMenuButton);
    
    serverButton = (Godot.Button)Menu.Button("Host", ShowServer);
    AddChild(serverButton);
    
    clientButton = (Godot.Button)Menu.Button("Join", ShowClient);
    AddChild(clientButton);
    
    selectionButton = (Godot.Button)Menu.Button("Back", ShowSelection);
    AddChild(selectionButton);
    
    portBox = (Godot.TextEdit)Menu.TextBox("" + NetworkSession.DefaultPort, false);
    AddChild(portBox);
    
    startServerButton = (Godot.Button)Menu.Button("Host Game", StartServer);
    AddChild(startServerButton);
    
    addressBox = (Godot.TextEdit)Menu.TextBox(NetworkSession.DefaultServerAddress, false);
    AddChild(addressBox);
    
    nameBox = (Godot.TextEdit)Menu.TextBox("Name", false);
    AddChild(nameBox);
    
    startClientButton = (Godot.Button)Menu.Button("Join Game", StartClient);
    AddChild(startClientButton);
  }
  
  void ScaleControls(){
    Rect2 screen = this.GetViewportRect();
    float width = screen.Size.x;
    float height = screen.Size.y;
    float wu = width/10; // relative height and width units
    float hu = height/10;
    
    Menu.ScaleControl(mainMenuButton, 2 * wu, hu, 0, height - hu);
    Menu.ScaleControl(serverButton, 4 * wu, 4 * hu, 0, hu);
    Menu.ScaleControl(clientButton, 4 * wu, 4 * hu, width - 4 * wu, hu);
    Menu.ScaleControl(selectionButton, 2 * wu, hu, 4 * wu, 9 * hu);
    Menu.ScaleControl(startServerButton, 2 * wu, hu, 8 * wu, 3 * hu);
    Menu.ScaleControl(startClientButton, 2 * wu, hu, 8 * wu, 3 * hu);
    Menu.ScaleControl(portBox, 1 * wu, hu, 7 * wu, 3 * hu);
    Menu.ScaleControl(addressBox, 3 * wu, hu, 4 * wu, 3 * hu);
    Menu.ScaleControl(nameBox, 2 * wu, hu, 2 * wu, 3 * hu); 
  }
  
  /* Let user select server or client. */
  public void ShowSelection(){
    HideClient();
    HideServer();

    serverButton.Show();
    clientButton.Show();

    ScaleControls();
  }
  
  public void HideSelection(){
    serverButton.Hide();
    clientButton.Hide();
  }
  
  public void ShowServer(){
    HideSelection();
    selectionButton.Show();
    startServerButton.Show();
    portBox.Show();
    ScaleControls();
  }
  
  public void HideServer(){
    selectionButton.Hide();
    startServerButton.Hide();
    portBox.Hide();
    startServerButton.SetText("Host Game");
  }
  
  public void ShowClient(){
    HideSelection();
    selectionButton.Show();
    startClientButton.Show();
    portBox.Show();
    addressBox.Show();
    nameBox.Show();
    ScaleControls();
  }
  
  public void HideClient(){
    selectionButton.Hide();
    startClientButton.Hide();
    portBox.Hide();
    addressBox.Hide();
    nameBox.Hide();
    startClientButton.SetText("Join Game");
  }
  
  public void StartClient(){
    NetworkSession netSes = new NetworkSession();
    netSes.initAddress = addressBox.GetText();
    netSes.initPort = portBox.GetText();
    Session.session.AddChild(netSes);
    Session.session.netSes = netSes;

    netSes.isServer = false;
    netSes.initName = nameBox.GetText();

    Session.ChangeMenu(Menu.Menus.Lobby);
  }
  
  
  public void StartServer(){    
    NetworkSession netSes = new NetworkSession();
    Session.session.AddChild(netSes);
    Session.session.netSes = netSes;

    netSes.isServer = true;
    netSes.initPort = portBox.GetText();

    Session.ChangeMenu(Menu.Menus.Lobby);
  }
  
  public void MainMenu(){
    Session.ChangeMenu(Menu.Menus.Main);
  }
  
  public void SetMainMenuButton(Godot.Button button){
    if(mainMenuButton != null){ mainMenuButton.QueueFree(); }
    
    mainMenuButton = button;
    AddChild(button);
  }
}