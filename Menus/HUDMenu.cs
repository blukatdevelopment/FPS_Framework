using Godot;
using System;

public class HUDMenu : Container{

  public float delay = 0.0f;

  Godot.TextEdit healthBox;
  Godot.TextEdit itemBox;
  Godot.TextEdit objectiveBox;
  Godot.TextEdit interactionText;

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
    Actor player = Session.session.player;
    
    if(player == null){
      GD.Print("Player 1 doesn't exist.");
      return;
    }
    
    string healthText = "Health: " + player.GetHealth();
    healthBox.SetText(healthText);

    string itemText = player.ItemInfo();
    itemBox.SetText(itemText);
    
    string objectiveText = Session.session.GetObjectiveText();
    IHasInfo infoObj = player.VisibleObject() as IHasInfo;
    if(infoObj != null){
      objectiveText = infoObj.GetInfo();
      GD.Print(objectiveText);
    }
    objectiveBox.SetText(objectiveText);

  }

  public void Init(){
    healthBox = (Godot.TextEdit)Menu.TextBox("health");
    healthBox.Readonly = true;
    AddChild(healthBox);

    itemBox = (Godot.TextEdit)Menu.TextBox("item");
    itemBox.Readonly = true;
    AddChild(itemBox);
    
    objectiveBox = (Godot.TextEdit)Menu.TextBox("Object Info");
    objectiveBox.Readonly = true;
    AddChild(objectiveBox);
    
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
    Menu.ScaleControl(objectiveBox, 4 * wu, hu, 3 * wu, 0);
  }

}
