using Godot;
using System;

<<<<<<< HEAD
public class MainMenu : Container
{
=======
public class MainMenu : Container, IMenu {
>>>>>>> develop
    
    public Godot.Button startButton;
    public Godot.Button lobbyButton;
    public Godot.Button quitButton;
<<<<<<< HEAD
    
    
    public void Init(){
      SetSinglePlayerButton((Godot.Button)Menu.Button(text : "Single Player", onClick: SinglePlayerGame));
      SetLobbyButton((Godot.Button)Menu.Button(text : "Multiplayer", onClick: Multiplayer));
      SetQuitButton((Godot.Button)Menu.Button(text : "Quit", onClick: Quit));
=======

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
>>>>>>> develop
      Sound.PlaySong(Sound.Songs.FloatingHorizons);
    }
    
    public void SetSinglePlayerButton(Godot.Button button){
      if(startButton != null){ lobbyButton.QueueFree(); }
      startButton = button;
      AddChild(button);
    }
    
    public void SetLobbyButton(Godot.Button button){
      if(lobbyButton != null){ lobbyButton.QueueFree(); }
      lobbyButton = button;
      AddChild(button);
    }
    
    public void SetQuitButton(Godot.Button button){
      if(quitButton != null){ quitButton.QueueFree(); }
      quitButton = button;
      AddChild(button);
    }
    
    public void SinglePlayerGame(){
<<<<<<< HEAD
      Session.session.SinglePlayerGame();
=======
      Session.ChangeMenu(Menu.Menus.Singleplayer);
>>>>>>> develop
    }
    
    public void Multiplayer(){
      Session.session.ChangeMenu(Menu.Menus.Multiplayer);
    }
    
    public void Quit(){
        Session.session.Quit();
    }
    
    public override void _Process(float delta) {
      Reposition();
    }
    
    /* Scales and positions all controls. */
    public void Reposition(){
      Rect2 screen = this.GetViewportRect();
      float width = screen.Size.x;
      float height = screen.Size.y;
      Vector2 size;
      if(startButton != null){
        size = new Vector2(width/10, height/10);
        startButton.SetSize(size);
        startButton.SetPosition(new Vector2(0f, 0f));
      }
      if(lobbyButton != null){
        size = new Vector2(width/10, height/10);
        lobbyButton.SetSize(size);
        lobbyButton.SetPosition(new Vector2(0f, size.y * 1));
      }
      if(quitButton != null){
        size = new Vector2(width/10, height/10);
        quitButton.SetSize(size);
        quitButton.SetPosition(new Vector2(0f, size.y*2));
      }
    }
}
