using Godot;
using System;

public class PauseMenu : Container, IMenu {
  
  public Godot.Button quitButton;
  public Godot.Button mainMenuButton;
  public Godot.Button resumeButton;

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
    SetQuitButton((Godot.Button)Menu.Button(text : "Quit", onClick : Quit));
    SetMainMenuButton((Godot.Button)Menu.Button(text : "Main Menu", onClick : QuitToMainMenu));
    SetResumeButton((Godot.Button)Menu.Button(text : "Resume", onClick : Resume));
  }
  
  void ScaleControls(){
      Rect2 screen = this.GetViewportRect();
      float width = screen.Size.x;
      float height = screen.Size.y;
      float wu = width / 10; // relative height and width units
      float hu = height / 10;
      
      Menu.ScaleControl(resumeButton, 4 * wu, 2 * hu, 3 * wu, 0);
      Menu.ScaleControl(mainMenuButton, 4 * wu, 2 * hu, 3 * wu, 2 * hu);
      Menu.ScaleControl(quitButton, 4 * wu, 2 * hu, 3 * wu,  4 * hu);
  }
  
  public void SetQuitButton(Godot.Button button){
    if(quitButton != null){ quitButton.QueueFree(); }
    quitButton = button;
    AddChild(button);
  }
  
  public void SetMainMenuButton(Godot.Button button){
    if(mainMenuButton != null){ quitButton.QueueFree(); }
    mainMenuButton = button;
    AddChild(button);
  }

  public void SetResumeButton(Godot.Button button){
    if(resumeButton != null){ resumeButton.QueueFree(); }
    resumeButton = button;
    AddChild(button);
  }
  
  public void Quit(){
      Session.session.Quit();
  }
  
  public void QuitToMainMenu(){
    Session.QuitToMainMenu();
  }

  public void Resume(){
    Session.Event(SessionEvent.PauseEvent());
  }
  
}
