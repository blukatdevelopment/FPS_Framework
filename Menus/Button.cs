using Godot;
using System;

public class Button : Control
{
    bool tapped = false;
    public override void _Ready() {
      
    }
    
    public override void _Draw() {
      Rect2 r = new Rect2(new Vector2(), GetSize());
      if(tapped){
        DrawRect(r, new Color(1, 0, 0));
      }
      else{
        DrawRect(r, new Color(0, 0, 1));
       }
    }
    
    public override void _Input(Godot.InputEvent ev) {
      if(ev is InputEventMouse && ev.IsPressed()){
        tapped = true;
        Update();
       }
    }
}
