using Godot;
using System;

public class MainMenu : Container, IMenu {
    
    public Godot.Button soloButton;
    public Godot.Button multiplayerButton;
    public Godot.Button settingsButton;
    public Godot.Button quitButton;

    public void Init(float minX, float minY, float maxX, float maxY){
      InitControls();
      ScaleControls();
    }
    
    public void Resize(float minX, float minY, float maxX, float maxY){

    }

    public bool IsSubMenu(){
      return false;
    }

    public void Clear(){
      this.QueueFree();
    }
    

    void InitControls(){
      soloButton = Menu.Button(text : "Solo", onClick : SinglePlayerGame);
      AddChild(soloButton);
      
      multiplayerButton = Menu.Button(text : "Multiplayer", onClick : Multiplayer);
      AddChild(multiplayerButton);
      
      quitButton = Menu.Button(text : "Quit", onClick : Quit);
      AddChild(quitButton);
      
      settingsButton = (Godot.Button)Menu.Button(text : "Settings", onClick : Settings);
      AddChild(settingsButton);
      
      Sound.PlaySong(Sound.Songs.FloatingHorizons);
    }

    void ScaleControls(){
      Rect2 screen = this.GetViewportRect();
      float width = screen.Size.x;
      float height = screen.Size.y;
      float wu = width/10; // relative height and width units
      float hu = height/10;
      
      Menu.ScaleControl(soloButton, 2 * wu, 2 * hu, 0, 0);
      Menu.ScaleControl(multiplayerButton, 2 * wu, 2 * hu, 0, 2 * hu);
      Menu.ScaleControl(settingsButton, 2 * wu, 2 * hu, 0, 4 * hu);
      Menu.ScaleControl(quitButton, 2 * wu, 2 * hu, 0, 8 * hu);
    }
    
    public void SinglePlayerGame(){
      Session.ChangeMenu(Menu.Menus.Singleplayer);
    }
    
    public void Multiplayer(){
      Session.ChangeMenu(Menu.Menus.Multiplayer);
    }

    public void Settings(){
      GD.Print("Settings menu");
      Session.ChangeMenu(Menu.Menus.Settings);
    }
    
    public void Quit(){
        Session.session.Quit();
    }
    
    
}
