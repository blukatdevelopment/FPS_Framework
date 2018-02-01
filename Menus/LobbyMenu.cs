using Godot;
using System;

public class LobbyMenu : Container
{
    private Godot.Button mainMenuButton;
    private Godot.Button sendButton;

    public override void _Ready() {
        
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
      if(mainMenuButton != null){
        size = new Vector2(width/10, height/10);
        mainMenuButton.SetSize(size);
        mainMenuButton.SetPosition(new Vector2(0f, size.y * 9));
      }
      if(sendButton != null){
        size = new Vector2(width/10, height/10);
        sendButton.SetSize(size);
        sendButton.SetPosition(new Vector2(size.x * 9, size.y * 9));
      }
    }
    
    public void SetMainMenuButton(Godot.Button button){
      if(mainMenuButton != null){ mainMenuButton.QueueFree(); }
      mainMenuButton = button;
      AddChild(button);
    }
    
    public void SetSendButton(Godot.Button button){
      if(sendButton != null){ sendButton.QueueFree(); }
      sendButton = button;
      AddChild(button);
    }
    
    public void ReturnToMainMenu(){
      Session.session.ChangeMenu(Menu.Menus.Main);
    }
    
    public void Send(){
      if(sendButton != null){
        GD.Print(sendButton.GetText());
      }
    }
    
}
