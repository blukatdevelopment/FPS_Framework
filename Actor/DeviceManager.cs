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
  List<bool> buttonsDown; // Store button states to check for changes.
  Vector2 mouseCur, mouseLast;
  
  public DeviceManager(Devices device){
    this.device = device;
    buttonsDown = new List<bool>();
    mouseActive = false;
    switch(device){
      case Devices.MouseAndKeyboard:
        mouseActive = true;
        int buttonCount = 70; // TODO: Make this number exact.
        for(int i = 0; i < buttonCount; i++){ buttonsDown.Add(false); }
      break;  
    }
  }
  
  /* Returns inputs using device-specific method. */
  public List<InputEvent> GetInputEvents(){
    switch(device){
      case Devices.MouseAndKeyboard: return KeyboardEvents(); break;
    }
    return new List<InputEvent>();  
  }
  
  /* Convenience method for button down events. */
  private InputEvent Down(InputEvent.Buttons button){
      return new InputEvent(button, InputEvent.Actions.Down);
  }
  
  /* Convenience method for button up events. */
  private InputEvent Up(InputEvent.Buttons button){
      return new InputEvent(button, InputEvent.Actions.Up);
  }
  
  /* Returns a list of InputEvents for a given key press */
  private List<InputEvent> KeyEvents(int key, int buttonIndex, InputEvent.Buttons button){
    List<InputEvent> ret = new List<InputEvent>();
    bool press = Input.IsKeyPressed(key);
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
  
  /* There's no way to get mouse position with a static method.
   public override void _Input(InputEvent ev){
    if(mouseActive && ev is InputEventMouse){
      InputEventMouse mouseEvent = (InputEventMouse)ev;
      mouseCur = mouseEvent.Position;
    }
  }
  */
  
  /* Returns input events from MouseAndKeyboard inputs. */
  private List<InputEvent> KeyboardEvents(){
    List<InputEvent> ret = new List<InputEvent>();
    // TODO: Replace all literals with appropriate constants when their location is known.
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
    //ret.AddRange(MouseEvents);
    return ret;
  }
  
  private List<InputEvent> MouseEvents(){
    List<InputEvent> ret = new List<InputEvent>();
    if(mouseLast == null){ mouseLast = mouseCur; }
    else if(mouseLast.x != mouseCur.x || mouseLast.y != mouseCur){
      float dx = mouseLast.x - mouseCur.x;
      float dy = mouseLast.y - mouseCur.y;
      mouseLast = mouseCur;
      ret.Add(new InputEvent(InputEvent.Axes.Mouse, dx, dy));
    }
    return ret;
  }
  
  // Use this to find methods for classes.
  static void ShowMethods(Type type){
    foreach (var method in type.GetMethods()){
      GD.Print("" + method.ReturnType +"," + method.Name);
    }
  }
  
}
