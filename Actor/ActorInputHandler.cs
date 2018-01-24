using Godot;
using System;
using System.Collections.Generic;

public class ActorInputHandler : Brain {
  private DeviceManager device;
  
  public ActorInputHandler(Actor actor) : base (actor){
    device = new DeviceManager(DeviceManager.Devices.MouseAndKeyboard);
  }
  
  public override void Update(float delta){
    List<InputEvent> events = device.GetInputEvents();
    for(int i = 0; i < events.Count; i++){
        GD.Print(events[i].ToString());
    }
  }
}
