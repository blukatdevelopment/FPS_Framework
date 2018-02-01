using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class LobbyMenu : Container
{
    private Godot.Button mainMenuButton;
    private Godot.Button sendButton;
    private Godot.TextEdit composeBox;
    private Godot.TextEdit messageBox;
    
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
    
    public void ReturnToMainMenu(){
      Session.session.ChangeMenu(Menu.Menus.Main);
    }
    
    public void Send(){
      if(composeBox != null && composeBox.GetText() != ""){
        ReceiveMessage("Player: " + composeBox.GetText());
        composeBox.SetText("");
      }
    }
    
    public void ReceiveMessage(string message){
      if(messages.Count > 50){ messages.Remove(messages.First()); }
      messages.Add(message);
      string str = "";
      for(int i = 0; i < messages.Count; i++){
        str += messages[i] + "\n";
      }
      messageBox.SetText(str);
    }
    
}
