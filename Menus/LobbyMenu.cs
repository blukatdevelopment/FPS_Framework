using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class LobbyMenu : Container
{
    Godot.TextEdit messageBox;
    Godot.TextEdit composeBox;
    Godot.Button sendButton;
    Godot.Button mainMenuButton;
    Godot.TextEdit playersBox;
    

    private string myName;
    private List<string> messages;

    public override void _Ready() {
        messages = new List<string>();
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
      
      playersBox = (Godot.TextEdit)Menu.TextBox("Player1 \n Player2 \n Player3 \n etc");
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

      if(netSes.isServer){
        netSes.InitServer(obj: this, playerJoin: "PlayerJoined", playerLeave: "PlayerQuit", port : netSes.initPort);
      }
      else{
        netSes.InitClient(address: netSes.initAddress, obj: this, success: "ConnectionSucceeded", fail: "ConnectionFailed", port: netSes.initPort);
      }

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
      int myId = netSes.peer.GetUniqueId();

      if(myName == "Name"){
        myName = "Player #" + myId.ToString();
      }

      PlayerData data = new PlayerData(myName, myId);
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
      string fullMessage = name + ":" + message;
      ReceiveMessage(fullMessage);  
    }
    
    [Remote]
    public void AddPlayer(int id, string name){
      NetworkSession netSes = Session.session.netSes;
      if(netSes == null){ 
        GD.Print("No network session detected");
        return; 
      }
      netSes.playerData.Add(id, name);
      GD.Print("Added " + id + ", " + name);
      BuildPlayers();
    }

    [Remote]
    public void RemovePlayer(int id){
      Session.session.netSes.playerData.Remove(id);
      BuildPlayers();
    }

    void BuildPlayers(){
      NetworkSession netSes = Session.session.netSes;
      string names = "Players(" + netSes.playerData.Count + ")\n";
      foreach(KeyValuePair<int, string> entry in netSes.playerData){
        names += entry.Value + "\n";
      }
      playersBox.SetText(names);
    }
    
}
