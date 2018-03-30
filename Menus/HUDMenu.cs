using Godot;
using System;

public class HUDMenu : Container{

  public Actor player;

  public float delay = 0.0f;

  Godot.TextEdit healthBox;
  Godot.TextEdit itemBox;

  public override void _Ready(){
      
  }
  
  public override void _Process(float delta){
    delay += delta;
    if(delay > 0.033f){
      delay -= 0.033f;
      Update();
    }
  }

  public void Update(){
    if(player == null && Session.session.player != null){
      player = Session.session.player;
    }
    
    if(player == null){
      return;
    }
    
    
    string healthText = "Health: " + player.GetHealth();
    healthBox.SetText(healthText);

    string itemText = "Item: None";
    itemBox.SetText(itemText);

  }

  public void Init(){
    healthBox = (Godot.TextEdit)Menu.TextBox("health");
    healthBox.Readonly = true;
    AddChild(healthBox);

    itemBox = (Godot.TextEdit)Menu.TextBox("item");
    itemBox.Readonly = true;
    AddChild(itemBox);
    
    ScaleControls();
  }

  public void ScaleControls(){
    Rect2 screen = this.GetViewportRect();
    float width = screen.Size.x;
    float height = screen.Size.y;
    float wu = width/10; // relative height and width units
    float hu = height/10;

    Menu.ScaleControl(healthBox, 2 * wu, hu, 0, height - hu);
    Menu.ScaleControl(itemBox, 2 * wu, hu, 8 * wu, 9 * hu);

  }

}
