using Godot;
using System;

public class HUDMenu : Container, IMenu{

  public float delay = 0.0f;
  public Godot.Label healthBox;
  public Godot.Label itemBox;
  public Godot.Label objectiveBox;
  public Godot.Label interactionBox;
  
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
  
  public void Resize(float minX, float minY, float maxX, float maxY){}

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
    
    string objectiveText = Session.GetObjectiveText();
    objectiveBox.Text = objectiveText;

    IInteract interactor = player.VisibleObject() as IInteract;
    
    if(interactor == null){
      interactionBox.Hide();
    }
    else{
      Item.Uses interaction = player.GetActiveInteraction();
      string interactionText = interactor.GetInteractionText(interaction);
      interactionBox.Show();
      interactionBox.SetText(interactionText);
    }
  }

  void InitControls(){
    healthBox = Menu.Label("health");
    AddChild(healthBox);

    itemBox = Menu.Label("item");
    AddChild(itemBox);
    
    objectiveBox = Menu.Label("Objective Info");
    AddChild(objectiveBox);

    interactionBox = Menu.Label("Interaction text");
    AddChild(interactionBox);
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
    Menu.ScaleControl(interactionBox, 4 * wu, hu, 3 * wu, 7 * hu);
  }
}
