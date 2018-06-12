using System;

public class SessionEvent {
  public string[] args;
  public Types type;
  public enum Types { 
  	None,		// Default 
  	ActorDied,  // Death of an actor
  	Pause 		// Toggling pause
  };
  
  public SessionEvent() {
    this.type = Types.None;
    this.args = new string[0];
  }

  public static SessionEvent PauseEvent(){
  	SessionEvent se  = new SessionEvent();
  	se.type = Types.Pause;
  	return se;
  }

  public static SessionEvent ActorDiedEvent(string dead, string killer){
  	SessionEvent se = new SessionEvent();
  	se.type = Types.ActorDied;
  	se.args = new string[2];
  	se.args[0] = dead;
  	se.args[1] = killer;
  	return se;
  }
}