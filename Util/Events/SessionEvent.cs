using System;

public class SessionEvent {
  public Actor[] actors;
  public Types type;
  public enum Types { None, ActorDied };
  
  public SessionEvent() {
    this.type = Types.None;
    this.actors = new Actor[0];
  }
}