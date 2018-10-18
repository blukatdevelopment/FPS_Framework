using Godot;
using System.Collections.Generic;

public class AdventureConfigMenu : Container, IMenu {
  
  public Label adventureLabel;
  public Label nameLabel;
  public TextEdit nameBox;
  public static int MaxNameLength = 16;

  const string NameLabelText = "Adventure Name";
  const string NameLabelWarning = "Warning: file already exists!";


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
    adventureLabel = Menu.Label("Adventure Mode");
    AddChild(adventureLabel);

    nameLabel = Menu.Label(NameLabelText);
    AddChild(nameLabel);

    nameBox = Menu.TextBox("new_adventure", false);
    nameBox.Connect("cursor_changed", this, nameof(UpdateName));
    AddChild(nameBox);
  }

  // Coerce the name to prevent validation errors
  public void UpdateName(){
    int column = nameBox.CursorGetColumn();
    string name = nameBox.GetText();
    string[] forebidden = new string[] {
      " ", "\\", "/", "?", "@", "#", "$", "%", "^", "&", "*", "(", ")",
      "-", "=", "+", "{", "}", "\'", "\"", "~", "`", "!", "|", "[", "]"
    };
    for(int i = 0; i < forebidden.Length; i++){
      name = name.Replace(forebidden[i], "_");
    }

    if(name.Length > MaxNameLength){
      name = name.Substring(0, MaxNameLength);
    }
    
    nameBox.SetText(name);
    nameBox.CursorSetColumn(column);
    config.fileName = name + ".adventure";
    if(SettingsDb.SaveExists(config.fileName)){
      nameLabel.SetText(NameLabelWarning);
    }
    else{
      nameLabel.SetText(NameLabelText);
    }
  }

  void ScaleControls(){
    float width = maxX - minX;
    float height = maxY - minY;
    float wu = width/10; // relative height and width units
    float hu = height/10;
     
    Menu.ScaleControl(adventureLabel, width, hu, minX, minY);
    Menu.ScaleControl(nameLabel, 2 * wu, hu, minX, minY + hu);
    Menu.ScaleControl(nameBox, 2 * wu, hu, minX + 2.5f * wu, minY + hu);
  }
  
}