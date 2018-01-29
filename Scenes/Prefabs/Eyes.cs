using Godot;
using System;

public class Eyes : Camera {
  private Vector2 mousePosition;
  
  public override void _Process(float delta){
    mousePosition = this.GetViewport().GetMousePosition();
  }
  
  public Vector2 GetMousePosition(){
    return mousePosition;
  }
}
