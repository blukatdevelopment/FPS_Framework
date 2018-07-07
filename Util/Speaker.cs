/*
  A speaker plays audio at a 3D point.
  They are attached to any noise-making class. 
*/
using Godot;
using System;
using System.Collections.Generic;

public class Speaker : AudioStreamPlayer3D{
  
  public void LoadEffect(Sound.Effects effect){
    string soundFile = Sound.EffectFile(effect);
    this.Stream = (AudioStreamSample)GD.Load(soundFile);
  }
  
  public void PlayEffect(Sound.Effects effect){
    this.Stop();
    LoadEffect(effect);
    this.Play();
  }
  
  /* Oh look, it's a pseudo-constructor. */
  public static Speaker Instance(){
    Speaker ret = (Speaker)Session.Instance("res://Scenes/Speaker.tscn");
    
    return ret;
  }
  
}
