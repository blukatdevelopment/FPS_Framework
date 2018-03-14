using Godot;
using System;

public class PauseMenu : Container{
  
  public Godot.Button quitButton;
  
  public override void _Ready(){
    
  }

  public void Init(){
    SetQuitButton((Godot.Button)Menu.Button(text : "Quit", onClick: Quit));
  }
  
  public void SetQuitButton(Godot.Button button){
    if(quitButton != null){ quitButton.QueueFree(); }
    quitButton = button;
    AddChild(button);
  }
  
  public void Quit(){
      Session.session.Quit();
  }
  
}
