using System;

public class SessionEvent {
  public Actor[] actors;
  public Types type;
  public enum Types { 
  	None,		// Default 
  	ActorDied,  // Death of an actor
  	Pause 		// Toggling pause
  };
  
  public SessionEvent() {
    this.type = Types.None;
    this.actors = new Actor[0];
  }

  public static SessionEvent PauseEvent(){
  	SessionEvent se  = new SessionEvent();
  	se.type = Types.Pause;
  	return se;
  }
}