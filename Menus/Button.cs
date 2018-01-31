using Godot;
using System;

public class Button : Godot.Button
{
    Action onClick;
    
    public override void _Ready() {
      
    }
    
    public override void _Draw() {
      /*
      Rect2 r = new Rect2(new Vector2(), GetSize());
      if(tapped){
        DrawRect(r, new Color(1, 0, 0));
      }
      else{
        DrawRect(r, new Color(0, 0, 1));
       }
      */
    }
    
    public void SetOnClick(Action onClick){
      this.onClick = onClick;
    }
    
    public override void _Input(Godot.InputEvent ev) {
      
      if(ev is InputEventMouse && ev.IsPressed()){
        if(onClick != null){
          onClick();
        }
        //Update(); //Probably not necessary, but keeping it here for kicks.
       }
      
    }
}
