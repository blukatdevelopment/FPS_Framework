using Godot;
using System;

public class MainMenu : Container
{
    
    public Godot.Button startButton;
    public Godot.Button lobbyButton;
    public Godot.Button quitButton;
    
    public override void _Ready() {
        
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
      Session.session.SinglePlayerGame();
    }
    
    public void Lobby(){
      Session.session.ChangeMenu(Menu.Menus.Lobby);
    }
    
    public void Quit(){
        GetTree().Quit();
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
