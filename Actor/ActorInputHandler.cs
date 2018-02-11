using Godot;
using System;
using System.Collections.Generic;

public class ActorInputHandler : Brain {
  private DeviceManager device;
  private Dictionary<InputEvent.Buttons, bool> held; 
  
  public ActorInputHandler(Actor actor, Eyes eyes) : base (actor, eyes){
    InitHeld();
    InitEyes(eyes);
    device = new DeviceManager(DeviceManager.Devices.MouseAndKeyboard, this.eyes);
  }
  
  private void InitEyes(Node eyes){
    if(eyes == null){ GD.Print("ActorInputHandler:Eyes were null"); }
    else{
      this.eyes = eyes as Eyes;
      if(eyes == null){
        GD.Print("ActorInputHandler:Eyes failed to cast.");
      }
    }
  }
  
  private void InitHeld(){
    held = new Dictionary<InputEvent.Buttons, bool>();
    foreach(InputEvent.Buttons button in Enum.GetValues(typeof(InputEvent.Buttons))){
      held.Add(button, false);
    }
  }
  
  public override void Update(float delta){
    List<InputEvent> events = device.GetInputEvents();
    for(int i = 0; i < events.Count; i++){
        if(events[i].IsButton()){ HandleButton(events[i]); }
        else{ HandleAxis(events[i]); }
    }
    HandleMovement();
  }
  
  private void HandleMovement(){
    int dx = 0;
    int dy = 0;
    if(held[InputEvent.Buttons.W]){ dy++; }
    if(held[InputEvent.Buttons.A]){ dx--; }
    if(held[InputEvent.Buttons.S]){ dy--; }
    if(held[InputEvent.Buttons.D]){ dx++; }
    if(dx != 0 || dy != 0){ actor.Move(dx, dy); }
  }
  
  private void HandleButton(InputEvent evt){
    if(evt.action == InputEvent.Actions.Down){
      held[evt.button] = true;
      Press(evt);
    }
    else if(evt.action == InputEvent.Actions.Up){
      held[evt.button] = false;
    }
  }
  
  private void Press(InputEvent evt){
    switch(evt.button){
      case InputEvent.Buttons.Esc: Session.session.Quit(); break;
    }
  }
  
  private void HandleAxis(InputEvent evt){
    if(evt.axis == InputEvent.Axes.Mouse){
      actor.Turn(evt.x, evt.y);
    }
    actor.Turn(evt.x, evt.y);
  }
}
