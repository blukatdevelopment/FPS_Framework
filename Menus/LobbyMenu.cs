using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class LobbyMenu : Container
{
    // TODO: Get RpcId working. Perhaps you need an object reference?
    private Godot.Button mainMenuButton;
    private Godot.Button sendButton;
    private Godot.Button joinButton;
    private Godot.Button hostButton;
    
    private Godot.TextEdit composeBox;
    private Godot.TextEdit messageBox;
    private Godot.TextEdit addressBox;
    private Godot.TextEdit nameBox;
    
    private List<string> messages;

    public override void _Ready() {
        messages = new List<string>();
    }
    
    public override void _Process(float delta) {
      Reposition();
    }
    
    /* Scales and positions all controls. */
    public void Reposition(){
      Rect2 screen = this.GetViewportRect();
      float width = screen.Size.x;
      float height = screen.Size.y;
      float w = width/10;
      float h = height/10;
      Vector2 size;
      if(mainMenuButton != null){
        size = new Vector2(w, h);
        mainMenuButton.SetSize(size);
        mainMenuButton.SetPosition(new Vector2(0f, size.y * 9));
      }
      if(sendButton != null){
        size = new Vector2(w, h);
        sendButton.SetSize(size);
        sendButton.SetPosition(new Vector2(size.x * 9, size.y * 9));
      }
      if(hostButton != null){
        size = new Vector2(w, h);
        hostButton.SetSize(size);
        hostButton.SetPosition(new Vector2(size.x * 4, 0));
      }
      if(joinButton != null){
        size = new Vector2(w, h);
        joinButton.SetSize(size);
        joinButton.SetPosition(new Vector2(size.x * 5, 0));
      }
      if(composeBox != null){
        size = new Vector2(4* w, 1* h);
        composeBox.SetSize(size);
        composeBox.SetPosition(new Vector2(w * 5, h * 9));
      }
      if(messageBox != null){
        size = new Vector2(8 * w, 2 *h);
        messageBox.SetSize(size);
        messageBox.SetPosition(new Vector2(w, 7 * h));
      }
      if(nameBox != null){
        size = new Vector2(3 * w, h);
        nameBox.SetSize(size);
        nameBox.SetPosition(new Vector2(2 * w, 9 * h));
      }
      
      if(addressBox != null){
        size = new Vector2(4 * w, h);
        addressBox.SetSize(size);
        addressBox.SetPosition(new Vector2(0, 0));
      }
      
    }
    
    public void SetMainMenuButton(Godot.Button button){
      if(mainMenuButton != null){ mainMenuButton.QueueFree(); }
      mainMenuButton = button;
      AddChild(button);
    }
    
    public void SetSendButton(Godot.Button button){
      if(sendButton != null){ sendButton.QueueFree(); }
      sendButton = button;
      AddChild(button);
    }
    
    public void SetHostButton(Godot.Button button){
      if(hostButton != null){ hostButton.QueueFree(); }
      hostButton = button;
      AddChild(button);
    }
    
    public void SetJoinButton(Godot.Button button){
      if(joinButton != null){ joinButton.QueueFree(); }
      joinButton = button;
      AddChild(button);
    }
    
    public void SetComposeBox(Godot.TextEdit box){
      if(composeBox != null){ composeBox.QueueFree(); }
      composeBox = box;
      AddChild(box);  
    }
    
    public void SetMessageBox(Godot.TextEdit box){
      if(messageBox != null){ messageBox.QueueFree(); }
      messageBox = box;
      box.SetReadonly(true);
      AddChild(box);
    }
    
    public void SetNameBox(Godot.TextEdit box){
      if(nameBox != null){ nameBox.QueueFree(); }
      nameBox = box;
      AddChild(box);
    }
    
    public void SetAddressBox(Godot.TextEdit box){
      if(addressBox != null){ addressBox.QueueFree(); }
      addressBox = box;
      AddChild(box);
    }
    
    public void ReturnToMainMenu(){
      Session.session.ChangeMenu(Menu.Menus.Main);
    }
    
    public void Send(){
      if(composeBox != null && composeBox.GetText() != "" && nameBox != null){
        string name = nameBox.GetText();
        if(name == ""){
          name = "Player ";
        }
        string message = name + ": " + composeBox.GetText(); 
        ReceiveMessage(message);
        Rpc(nameof(ReceiveMessage), message);
        RpcId(1, nameof(ReceiveMessage), message);
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
      GD.Print(message);
      messageBox.SetText(str);
    }
    
    public void Join(){
      string address = "";
      if(addressBox != null){
        address = addressBox.GetText();
      }
      ReceiveMessage("Connecting to server: " + address + " on port " + Session.DefaultPort + "...");
      Session.session.InitClient(address, (Godot.Object)this, nameof(JoinSucceed), nameof(JoinFail));
    }
    
    
    public void JoinSucceed(){
      ReceiveMessage("Connected.");
      string name = "Player";
      if(nameBox != null){
        name = nameBox.GetText();  
      }
      int id = this.GetTree().GetNetworkUniqueId();
      List<string> info = new List<string>();
      info.Add(name);
      System.Object[] args = new System.Object[]{id}; 
      this.Rpc(nameof(RegisterPlayer), args);
    }
    
    public void JoinFail(){
      ReceiveMessage("Connection Failed.");
    }
    
    public void Host(){
      ReceiveMessage("Hosting Server on port " + Session.DefaultPort + ".");
      Session.session.InitServer((Godot.Object)this, nameof(PlayerJoined), nameof(PlayerLeft));
    }
    
    public void PlayerJoined(int id){
      //ReceiveMessage("Player " + id + " joined.");
    }
    
    [Remote]
    public void RegisterPlayer(int id){
      ReceiveMessage("Welcome to my server, player " + id + "!");
      System.Object[] args = new System.Object[]{ "Howdy!"};
      this.RpcId(id, nameof(ReceiveMessage), "Howdy");
    }
    
    public void PlayerLeft(int id){
      ReceiveMessage("Player " + id + " left.");
    }
    
}
