using Godot;

public class AdventureConfigMenu : Container, IMenu {
  
  public Label adventureLabel;

  float minX, minY, maxX, maxY; // For scaling
  AdventureSettings config;


  public void Init(float minX, float minY, float maxX, float maxY){
    this.minX = minX;
    this.minY = minY;
    this.maxX = maxX;
    this.maxY = maxY;
    GD.Print("Init adventure config");
    InitAdventureSettings();
    InitControls();
    ScaleControls();
  }
  
  public void Resize(float minX, float minY, float maxX, float maxY){
    this.minX = minX;
    this.minY = minY;
    this.maxX = maxX;
    this.maxY = maxY;
    ScaleControls();
  }

  public bool IsSubMenu(){
    return true;
  }

  public void Clear(){
    this.QueueFree();
  }


  void InitAdventureSettings(){
    config = new AdventureSettings();
    Session.session.adventureSettings = config;
  }

  void InitControls(){
    

  }

  void ScaleControls(){
    float width = maxX - minX;
    float height = maxY - minY;
    float wu = width/10; // relative height and width units
    float hu = height/10;
     
    Menu.ScaleControl(adventureLabel, width, hu, minX, minY);
  }
  
}