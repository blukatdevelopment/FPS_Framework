/*
  The Device Manager returns a List of InputEvents received from
  a specific device. This abstracts the logic of collecting input
  from different devices from the actual handling of those inputs.
*/
using Godot;
using System;
using System.Collections.Generic;

public class DeviceManager {
  public enum Devices{MouseAndKeyboard};
  private Devices device;
  private bool mouseActive;
  private Spatial eyes;
  private float sensitivityX = 0.2f;
  private float sensitivityY = 0.2f;
  
  List<bool> buttonsDown; // Store button states to check for changes.
  Vector2 mouseCur, mouseLast;
  
  public DeviceManager(Devices device, Spatial eyes = null){
    this.device = device;
    buttonsDown = new List<bool>();
    mouseActive = false;
    switch(device){
      case Devices.MouseAndKeyboard:
        Input.SetMouseMode(Input.MouseMode.Captured);
        mouseActive = true;
        int buttonCount = 70;
        for(int i = 0; i < buttonCount; i++){ buttonsDown.Add(false); }
      break;  
    }
    this.eyes = eyes;
  }
  
  public List<InputEvent> GetInputEvents(){
    switch(device){
      case Devices.MouseAndKeyboard: return KeyboardEvents(); break;
    }
    return new List<InputEvent>();  
  }
  
  private InputEvent Down(InputEvent.Buttons button){
      return new InputEvent(button, InputEvent.Actions.Down);
  }
  
  private InputEvent Up(InputEvent.Buttons button){
      return new InputEvent(button, InputEvent.Actions.Up);
  }
  
  private List<InputEvent> KeyEvents(int key, int buttonIndex, InputEvent.Buttons button){
    List<InputEvent> ret = new List<InputEvent>();
    
    bool press = Input.IsKeyPressed(key);
    
    if(key < 4){
      press = Input.IsMouseButtonPressed(key);
    }
    
    if(press && !buttonsDown[buttonIndex]){ 
      ret.Add(Down(button));
      buttonsDown[buttonIndex] = true;
    }
    else if(!press && buttonsDown[buttonIndex]){ 
      ret.Add(Up(button));
      buttonsDown[buttonIndex] = false;
    }
    
    return ret;
  }
  
  private List<InputEvent> KeyboardEvents(){
    List<InputEvent> ret = new List<InputEvent>();

    ret.AddRange(KeyEvents(87, 0, InputEvent.Buttons.W));
    ret.AddRange(KeyEvents(65, 1, InputEvent.Buttons.A));
    ret.AddRange(KeyEvents(83, 2, InputEvent.Buttons.S));
    ret.AddRange(KeyEvents(68, 3, InputEvent.Buttons.D));
    ret.AddRange(KeyEvents(16777221, 4, InputEvent.Buttons.Enter)); // GD.KEY_RETURN is broken
    ret.AddRange(KeyEvents(16777232, 5, InputEvent.Buttons.Up));
    ret.AddRange(KeyEvents(16777234, 6, InputEvent.Buttons.Down));
    ret.AddRange(KeyEvents(16777231, 7, InputEvent.Buttons.Left));
    ret.AddRange(KeyEvents(16777233, 8, InputEvent.Buttons.Right));
    ret.AddRange(KeyEvents(1, 9, InputEvent.Buttons.M1));
    ret.AddRange(KeyEvents(2, 10, InputEvent.Buttons.M2));
    ret.AddRange(KeyEvents(3, 11, InputEvent.Buttons.M3));
    ret.AddRange(KeyEvents(16777217, 12, InputEvent.Buttons.Esc));
    ret.AddRange(KeyEvents(16777218, 13, InputEvent.Buttons.Tab));
    ret.AddRange(KeyEvents(32, 14, InputEvent.Buttons.Space));
    ret.AddRange(KeyEvents(16777237, 15, InputEvent.Buttons.Shift));
    ret.AddRange(KeyEvents(82, 16, InputEvent.Buttons.R));
    ret.AddRange(KeyEvents(69, 17, InputEvent.Buttons.E));
    ret.AddRange(MouseEvents());

    return ret;
  }

  private List<InputEvent> MouseEvents(){
    List<InputEvent> ret = new List<InputEvent>();
    
    if(eyes != null){
      mouseCur = Util.GetMousePosition(eyes);
    }
    else{ GD.Print("DeviceManager:Eyes null"); }
    
    if(mouseLast == null){ mouseLast = mouseCur; }
    else if((mouseLast.x != mouseCur.x) || (mouseLast.y != mouseCur.y)){
      float dx = mouseLast.x - mouseCur.x;
      float dy = mouseLast.y - mouseCur.y;
      dx *= sensitivityX;
      dy *= sensitivityY;
      
      mouseLast = mouseCur;
      ret.Add(new InputEvent(InputEvent.Axes.Mouse, dx, dy));
    }
    
    return ret;
  }
  
}
