/*
  The purpose of the MultiplayerMenu is to set up a NetworkSession
  and to establish or connect to a server. After a connection has
  been established, the MultiplayerMenu should flow to the Lobbymenu.
*/
using Godot;
using System;

public class MultiplayerMenu : Container
{
    Godot.Button mainMenuButton;
    
    Godot.Button selectionButton;
    Godot.Button serverButton;
    Godot.Button clientButton;
    
    Godot.TextEdit portBox;
    Godot.TextEdit addressBox;
    Godot.TextEdit nameBox;
    Godot.Button startServerButton;
    Godot.Button startClientButton;
    
    public override void _Ready(){
        
        
    }
    
    public void Init(){
      InitControls();
      ShowSelection();
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
      
      portBox = (Godot.TextEdit)Menu.TextBox("" + NetworkSession.DefaultPort);
      AddChild(portBox);
      
      startServerButton = (Godot.Button)Menu.Button("Host Game", StartServer);
      AddChild(startServerButton);
      
      addressBox = (Godot.TextEdit)Menu.TextBox(NetworkSession.DefaultServerAddress);
      AddChild(addressBox);
      
      nameBox = (Godot.TextEdit)Menu.TextBox("Name");
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
      
      ScaleControl(mainMenuButton, 2 * wu, hu, 0, height - hu);
      ScaleControl(serverButton, 4 * wu, 4 * hu, 0, hu);
      ScaleControl(clientButton, 4 * wu, 4 * hu, width - 4 * wu, hu);
      ScaleControl(selectionButton, 2 * wu, hu, 4 * wu, 9 * hu);
      
      ScaleControl(startServerButton, 2 * wu, hu, 8 * wu, 3 * hu);
      ScaleControl(startClientButton, 2 * wu, hu, 8 * wu, 3 * hu);
      ScaleControl(portBox, 1 * wu, hu, 7 * wu, 3 * hu);
      ScaleControl(addressBox, 3 * wu, hu, 4 * wu, 3 * hu);
      ScaleControl(nameBox, 2 * wu, hu, 2 * wu, 3 * hu);
      
      
    }
    
    /* Despite checking for nulls, this still ends up with nulls. I'm going to attribute this to race conditions. */
    public void ScaleControl(Control control, float width, float height, float x, float y){
      if(control == null){ return; }
      control.SetSize(new Vector2(width, height)); 
      control.SetPosition(new Vector2(x, y)); 
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
    
    /* Show server configuration settings. */
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
    }
    
    public void StartServer(){
      GD.Print("Start Server");
    }
    
    /* Show client configuration settings */
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
    }
    
    public void StartClient(){
      GD.Print("Start client");
    }
    
    
    public void MainMenu(){
      Session.session.ChangeMenu(Menu.Menus.Main);
    }
    
    
    public void SetMainMenuButton(Godot.Button button){
      if(mainMenuButton != null){ mainMenuButton.QueueFree(); }
      mainMenuButton = button;
      AddChild(button);
    }
}