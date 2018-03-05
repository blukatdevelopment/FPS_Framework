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
      
      ScaleControls();
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
        string name = "Name!";
        string message = composeBox.GetText(); 
        ReceiveNamedMessage(message, name);
        Rpc(nameof(ReceiveNamedMessage), message, name);
        composeBox.SetText("");
      }
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
    public void UpdatePlayers(){
      NetworkSession netSes = Session.session.netSes;
      if(netSes == null){ return; }
      
    }
    
}
