/*
  ActorInputHandler collects input from a device manager and causes its actor to behave accordingly. 
*/

using Godot;
using System;
using System.Collections.Generic;

public class ActorInputHandler : Brain {
  private DeviceManager device;
  private System.Collections.Generic.Dictionary<InputEvent.Buttons, bool> held; 
  private float delta = 1f;
  private float syncTimer = 0f;
  public const float syncRate = 0.05f;
  
  public ActorInputHandler(Actor actor, Spatial eyes) : base (actor, eyes){
    InitHeld();
    InitEyes(eyes);
    device = new DeviceManager(DeviceManager.Devices.MouseAndKeyboard, eyes);
  }
  
  private void InitEyes(Node eyes){
    if(eyes == null){ GD.Print("ActorInputHandler:Eyes were null"); }
    else{
      this.eyes = eyes as Spatial;
      
      if(eyes == null){
        GD.Print("ActorInputHandler:Eyes failed to cast.");
      }
    }
  }
  
  private void InitHeld(){
    held = new System.Collections.Generic.Dictionary<InputEvent.Buttons, bool>();
    foreach(InputEvent.Buttons button in Enum.GetValues(typeof(InputEvent.Buttons))){
      held.Add(button, false);
    }
  }
  
  public override void Update(float delta){
    this.delta = delta;
    
    List<InputEvent> events = device.GetInputEvents();
    
    if(actor.menuActive == false){
      for(int i = 0; i < events.Count; i++){
          if(events[i].IsButton()){ HandleButton(events[i]); }
          else{ HandleAxis(events[i]); }
      }
      
      HandleMovement();
    }
    else{
      HandleMenuInput(events);
    }
    
    if(Session.NetActive()){
      NetUpdate();
    }
  }

  public void NetUpdate(){
    syncTimer += delta;
    
    if(syncTimer >= syncRate){
      syncTimer = 0f;
      actor.SyncPosition();
      actor.SyncAim();
    }
  }
  
  private void HandleMenuInput(List<InputEvent> events){
    for(int i = 0; i < events.Count; i++){
      if(events[i].button == InputEvent.Buttons.Esc){ HandleButton(events[i]); }
      if(events[i].button == InputEvent.Buttons.Tab){ HandleButton(events[i]); }
    }
  }
  
  private void HandleMovement(){
    int dx = 0;
    int dz = 0;
    
    if(held[InputEvent.Buttons.W]){ dz--; }
    if(held[InputEvent.Buttons.A]){ dx--; }
    if(held[InputEvent.Buttons.S]){ dz++; }
    if(held[InputEvent.Buttons.D]){ dx++; }

    if(dx != 0 || dz != 0){
      Vector3 movement = new Vector3(dx, 0, dz);
      movement *= actor.GetMovementSpeed(); 
      actor.Move(movement, this.delta); 
    }
  }
  
  
  private void HandleButton(InputEvent evt){
    if(evt.action == InputEvent.Actions.Down){
      held[evt.button] = true;
      Press(evt);
    }
    else if(evt.action == InputEvent.Actions.Up){
      held[evt.button] = false;
      
      if(evt.button == InputEvent.Buttons.Shift){
        actor.SetSprint(false);
      }
    }
  }

  private void Press(InputEvent evt){    
    switch(evt.button){
      case InputEvent.Buttons.Esc: 
        Session.Event(SessionEvent.PauseEvent());
        break;
      case InputEvent.Buttons.Tab: actor.ToggleInventory(); break;
      case InputEvent.Buttons.Space: actor.Jump(); break;
      case InputEvent.Buttons.Shift: actor.SetSprint(true); break;
      case InputEvent.Buttons.M1: actor.Use(Item.Uses.A); break;
      case InputEvent.Buttons.M2: actor.Use(Item.Uses.B); break;
      case InputEvent.Buttons.M3: actor.Use(Item.Uses.C); break;
      case InputEvent.Buttons.R: actor.Use(Item.Uses.D); break;
      case InputEvent.Buttons.E: actor.InitiateInteraction(); break;
    }
  }

  private void HandleAxis(InputEvent evt){
    float wx = Session.session.mouseSensitivityX;
    float wy = Session.session.mouseSensitivityY;
    
    if(evt.axis == InputEvent.Axes.Mouse){
      actor.Turn(evt.x * wx, evt.y * wy);
    }
    
    actor.Turn(evt.x * wx, evt.y * wy);
  }
}
