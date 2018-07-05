using Godot;
using System;

public class SettingsMenu : Container {
    Godot.Button mainMenuButton;

    public void Init(){
      InitControls();
    }

    public void InitControls(){
      mainMenuButton = (Godot.Button)Menu.Button("Main Menu", MainMenu);
      AddChild(mainMenuButton);
    }

    void ScaleControls(){
      Rect2 screen = this.GetViewportRect();
      float width = screen.Size.x;
      float height = screen.Size.y;
      float wu = width/10; // relative height and width units
      float hu = height/10;
      
      Menu.ScaleControl(mainMenuButton, 2 * wu, hu, 0, height - hu);
    }

    public void MainMenu(){
      Session.ChangeMenu(Menu.Menus.Main);
    }

}
