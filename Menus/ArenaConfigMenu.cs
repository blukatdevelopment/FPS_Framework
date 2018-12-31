using Godot;

public class ArenaConfigMenu : Container, IMenu {
	
	public Godot.TextEdit gamemodeLabel;
	public Godot.HSlider botsSlider;
	public Godot.TextEdit botsLabel;
	public Godot.HSlider durationSlider;
	public Godot.TextEdit durationLabel;
	public Godot.Button powerupsButton;
	public Godot.Button kitsButton;
	public float minX, minY, maxX, maxY;
	public ArenaSettings config;


  public void Init(float minX, float minY, float maxX, float maxY){
  	this.minX = minX;
  	this.minY = minY;
  	this.maxX = maxX;
  	this.maxY = maxY;

    InitArenaSettings();
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


  void InitArenaSettings(){
  	config = new ArenaSettings();
  	Session.session.arenaSettings = config;
  }

  void InitControls(){
  	gamemodeLabel = (Godot.TextEdit)Menu.TextBox("Arena Config");
    AddChild(gamemodeLabel);

    botsSlider = Menu.HSlider(0f, 16f, config.bots, 1f);
    AddChild(botsSlider);
    botsSlider.Connect("value_changed", this, nameof(UpdateBots));
    botsLabel = Menu.TextBox("Bots: " + config.bots);
    AddChild(botsLabel);

    durationSlider = Menu.HSlider(0f, 60f, config.duration, 1f);
    AddChild(durationSlider);
    durationSlider.Connect("value_changed", this, nameof(UpdateDuration));
    durationLabel = Menu.TextBox("Duration: " + config.duration + " minutes");
    AddChild(durationLabel);

    powerupsButton = Menu.Button("Spawn powerups: " + config.usePowerups, TogglePowerups);
    AddChild(powerupsButton);

    kitsButton = Menu.Button("Spawn with kits: " + config.useKits, ToggleKits);
    AddChild(kitsButton);

  }

  void ScaleControls(){
    float width = maxX - minX;
    float height = maxY - minY;
    float wu = width/10; // relative height and width units
    float hu = height/10;
     
    Menu.ScaleControl(gamemodeLabel, width, hu, minX, minY);
    Menu.ScaleControl(botsLabel, 4 * wu, hu / 2f, minX, minY + hu);
    Menu.ScaleControl(botsSlider, 4 * wu, hu / 2f, minX, minY + 1.5f * hu);
    Menu.ScaleControl(durationLabel, 4 * wu, hu / 2f, minX, minY + 2 * hu);
    Menu.ScaleControl(durationSlider, 4 * wu, hu / 2f, minX, minY + 2.5f * hu);
    Menu.ScaleControl(powerupsButton, 2 * wu, hu, minX, minY + 8 * hu);
    Menu.ScaleControl(kitsButton, 2 * wu, hu, minX, minY + 9 * hu);

  }


  public void UpdateDuration(float val){
    int duration = (int)val;
    config.duration = duration;
    string str = "Duration: " + config.duration + " minutes";
    durationLabel.SetText(str);
  }

  public void UpdateBots(float val){
  	int bots = (int)val;
  	config.bots = bots;
  	string str = "Bots: " + config.bots;
  	botsLabel.SetText(str);
  }

  public void TogglePowerups(){
  	config.usePowerups = !config.usePowerups;
  	string str = "Spawn powerups: " + config.usePowerups;
  	powerupsButton.SetText(str);
  }

  public void ToggleKits(){
  	config.useKits = !config.useKits;
  	string str = "Spawn with kits: " + config.useKits;
  	kitsButton.SetText(str);
  }
}