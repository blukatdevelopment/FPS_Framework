using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

public class LobbyMenu : Container
{
    Godot.TextEdit messageBox;
    Godot.TextEdit composeBox;
    Godot.Button sendButton;
    Godot.Button mainMenuButton;
    Godot.TextEdit playersBox;
    Godot.Button readyButton;
    

    private bool countDownActive = false;
    private float timer = 0f;
    private int countDown = 10;
    private bool isReady = false;


    private string myName;
    private List<string> messages;

    public override void _Ready() {
      messages = new List<string>();
    }

    public override void _Process(float delta){
      if(countDownActive){
        CountDown(delta);
      }
      
    }

    void CountDown(float delta){
      timer += delta;
      if(timer > 1f){
        countDown--;
        timer = 0;
        BuildPlayers();
      }
      if(countDown < 1){
        countDownActive = false;
        StartGame();
      }
    }
    
    public void Init(){
      messageBox = (Godot.TextEdit)Menu.TextBox();
      messageBox.Readonly = true;
      AddChild(messageBox);
      
      composeBox = (Godot.TextEdit)Menu.TextBox();
      AddChild(composeBox);
      
      sendButton = (Godot.Button)Menu.Button("Send", Send);
      AddChild(sendButton);
      
      mainMenuButton = (Godot.Button)Menu.Button("Main Menu", ReturnToMainMenu);
      AddChild(mainMenuButton);

      readyButton = (Godot.Button)Menu.Button("Ready", ToggleReady);
      AddChild(readyButton);
      
      playersBox = (Godot.TextEdit)Menu.TextBox("");
      playersBox.Readonly = true;
      AddChild(playersBox);

      
      InitNetwork();

      ScaleControls();
    }

    void InitNetwork(){
      if(Session.session.netSes == null){
        GD.Print("No network session found in lobby menu");
        return;
      }

      NetworkSession netSes = Session.session.netSes;
      
      myName = "Server";

      if(netSes.isServer){
        netSes.InitServer(obj: this, playerJoin: "PlayerJoined", playerLeave: "PlayerQuit", port : netSes.initPort);
        ReceiveMessage("Server initialized with random seed: " + netSes.randomSeed);
      }
      else{
        netSes.InitClient(address: netSes.initAddress, obj: this, success: "ConnectionSucceeded", fail: "ConnectionFailed", port: netSes.initPort);
      }
      BuildPlayers();
    }
    
    void ScaleControls(){
      Rect2 screen = this.GetViewportRect();
      float width = screen.Size.x;
      float height = screen.Size.y;
      float wu = width/10; // relative height and width units
      float hu = height/10;
      
      Menu.ScaleControl(mainMenuButton, 2 * wu, hu, 0, height - hu);
      Menu.ScaleControl(composeBox, 6 * wu, 2 * hu, 3 * wu, 8 * hu);
      Menu.ScaleControl(messageBox, 6 * wu, 8 * hu, 3 * wu, 0);
      Menu.ScaleControl(sendButton, wu, 2 * hu, 9 * wu, 8 * hu);
      Menu.ScaleControl(playersBox, 2 * wu, 8 * hu, 0, 0);
      Menu.ScaleControl(readyButton, wu, hu, 2 * wu, 0);
    }
    
    public void ReturnToMainMenu(){
      Session.session.ChangeMenu(Menu.Menus.Main);
    }
    
    public void Send(){
      if(composeBox != null && composeBox.GetText() != ""){
        
        string message = composeBox.GetText(); 
        ReceiveNamedMessage(message, myName);
        Rpc(nameof(ReceiveNamedMessage), message, myName);
        composeBox.SetText("");
      }
    }
    
    public void PlayerJoined(int id){
      //ReceiveMessage("Player " + id + " joined.");
      RpcId(id, nameof(InitRandomSeed), Session.session.netSes.randomSeed);
    }

    [Remote]
    public void InitRandomSeed(int randomSeed){
      Session.session.netSes.InitRandom(randomSeed);
      GD.Print("Random seed initialized as " + randomSeed);
    }

    public void PlayerQuit(int id){
      //ReceiveMessage("Player " + id + " quit.");
      RemovePlayer(id);
      Rpc(nameof(RemovePlayer), id);
    }

    public void ConnectionSucceeded(){
      GD.Print("Connection succeeded!");
      NetworkSession netSes = Session.session.netSes;
      myName = netSes.initName;
      int myId = netSes.selfPeerId;

      if(myName == "Name"){
        myName = "Player #" + myId.ToString();
      }

      PlayerData dat = new PlayerData(myName, myId);

      Rpc(nameof(PrintPlayer), JsonConvert.SerializeObject(dat, Formatting.Indented));

      AddPlayer(myId, myName);
      Rpc(nameof(AddPlayer), myId, myName);

      string message = myName + " joined!"; 
      ReceiveMessage(message);
      Rpc(nameof(ReceiveMessage), message);
    }

    public void ConnectionFailed(){
      ReceiveMessage("Connection failed. :(");
    }

    [Remote]
    public void ReceiveMessage(string message){
      if(messages.Count > 50){ messages.Remove(messages.First()); }
      messages.Add(message);
      string str = "";
      for(int i = 0; i < messages.Count; i++){
        str += messages[i] + "\n";
      }
      messageBox.SetText(str);
    }
    
    [Remote]
    public void ReceiveNamedMessage(string message, string name){
      string fullMessage = name + ": " + message;
      ReceiveMessage(fullMessage);  
    }
    
    [Remote]
    public void AddPlayer(int id, string name){
      NetworkSession netSes = Session.session.netSes;
      if(netSes == null){ 
        GD.Print("No network session detected");
        return; 
      }
      PlayerData dat = new PlayerData(name, id);

      netSes.playerData.Add(id, dat);
      GD.Print("Added " + id + ", " + name);
      BuildPlayers();
    }

    [Remote]
    public void RemovePlayer(int id){
      Session.session.netSes.playerData.Remove(id);
      BuildPlayers();
    }

    void ToggleReady(){
      isReady = !isReady;
      int myId = Session.session.netSes.selfPeerId;
      
      TogglePlayerReady(myId);
      Rpc(nameof(TogglePlayerReady), myId);
      
      if(isReady){  
        readyButton.SetText("Waiting");
      }
      else{
        readyButton.SetText("Ready");
      }

    }

    [Remote]
    public void TogglePlayerReady(int playerId){
      GD.Print("Toggling player " + playerId);
      PlayerData dat = Session.session.netSes.playerData[playerId];
      
      if(dat == null){
        GD.Print("Player " + playerId + " doesn't exist.");
        return;
      }

      dat.ready = !dat.ready;
      string message = dat.name;
      message += dat.ready ? "is ready." : "is not ready.";

      if(AllPlayersReady()){
        StartCountDown();

      }
      else{
        StopCountDown();
      }

      //ReceiveMessage(message);
      Rpc(nameof(ReceiveMessage), message);
    }

    public bool AllPlayersReady(){
      NetworkSession netSes = Session.session.netSes;
      foreach(KeyValuePair<int, PlayerData> entry in netSes.playerData){
        if(entry.Value.ready == false){
          return false;
        }
      }
      return true;
    }

    public void StartGame(){
      Session.session.MultiPlayerGame();
    }

    void StopCountDown(){
      countDownActive = false;
      BuildPlayers();
      GD.Print("Stopping countdown");
    }

    [Remote]
    void StartCountDown(){
      GD.Print("Starting countdown");
      timer = 0f;
      countDown = 10;
      countDownActive = true;
      BuildPlayers();
    }

    [Remote]
    void PrintPlayer(string dat){
      GD.Print("Printing player " + dat);
    }


    void BuildPlayers(){
      NetworkSession netSes = Session.session.netSes;
      string names = "Players(" + netSes.playerData.Count + ")";
      
      if(countDownActive){
        names += " Starting in " + countDown;
      }

      names += "\n";
      
      foreach(KeyValuePair<int, PlayerData> entry in netSes.playerData){
        names += entry.Value.name + "\n";
      }
      playersBox.SetText(names);
    }
    
}
