using Godot;
using System.Collections.Generic;

public class AdventureConfigMenu : Container, IMenu {
  
  public Label adventureLabel;
  public Label nameLabel;
  public TextEdit nameBox;
  public const int MaxNameLength = 16;
  public Button loadAdventureButton;
  public const string NameLabelText = "Adventure Name";
  public const string NameLabelWarning = "Warning: file already exists!";
  public float minX, minY, maxX, maxY;
  public AdventureSettings config;

  public void Init(float minX, float minY, float maxX, float maxY){
    this.minX = minX;
    this.minY = minY;
    this.maxX = maxX;
    this.maxY = maxY;

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

    loadAdventureButton = Menu.Button("Load Adventure", GoToLoadAdventure);
    AddChild(loadAdventureButton);
  }

  public void GoToLoadAdventure(){
    Session.ChangeMenu(Menu.Menus.LoadAdventure);
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
    Menu.ScaleControl(loadAdventureButton, wu, hu, minX, minY + 2 * hu);
  }
  
}