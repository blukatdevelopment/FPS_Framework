using System;

public class SessionEvent {
  public string[] args;
  public Types type;
  public enum Types { 
  	None,		// Default 
  	ActorDied,  // Death of an actor
  	Pause, 		// Toggling pause
    ItemDiscarded // Actor dropped an item from inventory.
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

  // Used for scoring kills in the arena
  public static SessionEvent ActorDiedEvent(string dead, string killer){
  	SessionEvent se = new SessionEvent();
  	se.type = Types.ActorDied;
  	se.args = new string[2];
  	se.args[0] = dead;
  	se.args[1] = killer;
  	return se;
  }

  // Used for updating UI when online and using DeferredDiscardItem
  public static SessionEvent ItemDiscardedEvent(string actor){
    SessionEvent se = new SessionEvent();
    se.type = Types.ItemDiscarded;
    se.args = new string[1];
    se.args[0] = actor;
    return se;
  }
}