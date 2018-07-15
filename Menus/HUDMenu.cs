using Godot;
using System;

public class HUDMenu : Container, IMenu{

  public float delay = 0.0f;

<<<<<<< HEAD
  Godot.TextEdit healthBox;
  Godot.TextEdit itemBox;
  Godot.TextEdit objectiveBox;

  public override void _Ready(){
      
  }
=======
  Godot.Label healthBox;
  Godot.Label itemBox;
  Godot.Label objectiveBox;
  Godot.Label interactionBox;
>>>>>>> develop
  
  public override void _Process(float delta){
    delay += delta;
    if(delay > 0.033f){
      delay -= 0.033f;
      Update();
    }
  }

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

  public void Update(){
    Actor player = Session.session.player;
    
    if(player == null){
      GD.Print("Player 1 doesn't exist.");
      return;
    }
    
    string healthText = "Health: " + player.GetHealth();
    healthBox.Text = healthText;

    string itemText = player.ItemInfo();
    itemBox.Text = itemText;
    
<<<<<<< HEAD
    string objectiveText = Session.session.GetObjectiveText();
    objectiveBox.SetText(objectiveText);
=======
    string objectiveText = Session.GetObjectiveText();
    objectiveBox.Text = objectiveText;
>>>>>>> develop

  }

  void InitControls(){

    healthBox = Menu.Label("health");
    AddChild(healthBox);

    itemBox = Menu.Label("item");
    AddChild(itemBox);
    
<<<<<<< HEAD
    objectiveBox = (Godot.TextEdit)Menu.TextBox("Object Info");
    objectiveBox.Readonly = true;
    AddChild(objectiveBox);
    
    ScaleControls();
=======
    objectiveBox = Menu.Label("Objective Info");
    AddChild(objectiveBox);

    interactionBox = Menu.Label("Interaction text");
    AddChild(interactionBox);
>>>>>>> develop
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
