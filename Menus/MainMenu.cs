using Godot;
using System;

public class MainMenu : Container
{
    public Godot.Button startButton;
    
    public override void _Ready()
    {
        
    }
    
    public void SetStartButton(Godot.Button button){
      if(startButton != null){ startButton.QueueFree(); }
      startButton = button;
      AddChild(button);
    }
    
    public void Start(){
      Session.session.ChangeMenu(Menu.Menus.None);
    }
}
