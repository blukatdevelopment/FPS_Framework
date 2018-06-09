/*
  A bundle of information for a networked player.
*/
using System;

[Serializable]
public class PlayerData {
  public string name;
  public int id;
  public bool ready;

  public PlayerData(string name, int id){
    this.name = name;
    this.id = id;
    this.ready = false;
  }
}