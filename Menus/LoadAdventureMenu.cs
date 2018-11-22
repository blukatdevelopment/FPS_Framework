using Godot;
using System.Collections.Generic;

public class LoadAdventureMenu : Container, IMenu {
  public Godot.ItemList savesList;
  public Godot.Button backButton;
  public Godot.Button loadButton;

  public void Init(float minX, float minY, float maxX, float maxY){
    Session.session.adventureSettings = new AdventureSettings();
    InitControls();
    ScaleControls();
  }

  public void InitControls(){
    savesList = new ItemList();
    AddChild(savesList);
    savesList.AllowRmbSelect = true;
    savesList.Connect("item_selected", this, nameof(SelectSave));
    savesList.Connect("item_activated", this, nameof(LoadSave));

    foreach(string save in SettingsDb.GetAllSaves("adventure")){
      savesList.AddItem(save);
    }

    backButton = Menu.Button("back", Back);
    AddChild(backButton);

    loadButton = Menu.Button("load", Load);
    AddChild(loadButton);
  }

  public void SelectSave(int index){
    string save = savesList.GetItemText(index) + ".adventure";
    Session.session.adventureSettings.fileName = save;
  }

  public void LoadSave(int index){
    Load();
  }

  public void Resize(float minX, float minY, float maxX, float maxY){}

  public void Load(){
    string save = Session.session.adventureSettings.fileName;
    
    if(save == null || save == ""){
      GD.Print("Can't load a blank file");
      return;
    }

    Session.session.adventureSettings.load = true;
    Session.LocalAdventure();
  }

  public void Back(){
    string destination = "";

    if(Session.session.adventure == null){
      Session.ChangeMenu(Menu.Menus.Local);
    }
    else{
      Session.ChangeMenu(Menu.Menus.Pause);
    }
  }

  public bool IsSubMenu(){
    return false;
  }

  public void ScaleControls(){
    Rect2 screen = this.GetViewportRect();
    float width = screen.Size.x;
    float height = screen.Size.y;
    float wu = width / 10; // relative height and width units
    float hu = height / 10;
    
    Menu.ScaleControl(savesList, 5 * wu, 4 * hu, 0, 5 * hu);
    Menu.ScaleControl(backButton, 2 * wu, hu, 0, height - hu);
    Menu.ScaleControl(loadButton, 2 * wu, hu, width - 2 * wu, height - hu);
  }

  public void Clear(){
    this.QueueFree();
  }
}