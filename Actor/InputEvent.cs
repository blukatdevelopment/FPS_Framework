using Godot;
using System;
using System.Collections.Generic;

/*
  An InputEvent stores information about a specific button or axis input.
*/

public class InputEvent {
  public enum  Buttons{
    None,
    // Keyboard keys
    Esc,
    Q, W, E, R, T, Y, U, I, O, P, 
    A, S, D, F, G, H, J, K, L, Z, 
    X, C, V, B, N, M, Comma, Period,
    Tab, Shift, Ctrl, Space,
    Enter, Up, Down, Left, Right,
    One, Two, Three, Four, Five, Six, Seven, Eight, Nine, Zero,
    // Mouse buttons
    M1, M2, M3,
    MUp, MDown // Mouse Scroll wheel
  };
  public enum Actions {None, Up, Down};
  public enum Axes {None, Mouse };
  public Buttons button;
  public Actions action;
  public Axes axis;
  public float x, y; // Movement of axes.
  
  /* Constructor for button input. */
  public InputEvent(Buttons button, Actions action){
    this.button = button;
    this.action = action;
    this.axis = Axes.None;
    x = y = 0f;
  }
    
  /* Constructor for  Axis input. */
  public InputEvent(Axes axis, float x, float y){
    this.button = Buttons.None;
    this.action = Actions.None;
    this.axis = axis;
    this.x = x;
    this.y = y;
  }
  
  public bool IsButton(){
    if(this.button == Buttons.None){
      return false;  
    }

    return true;
  }
  
  public override string ToString(){
    string ret = "";
    if(button != Buttons.None){
      ret += button.ToString() + " : ";
      ret += action.ToString();
    }
    else{
      ret += axis.ToString() + " : ";
      ret += "[" + x + "," + y + "]";
    }
    return ret;
  }
}
