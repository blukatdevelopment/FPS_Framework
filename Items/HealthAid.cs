using Godot;
using Newtonsoft.Json;

public class HealthAid : Item, IConsume {
	const int Health = 25;

	public void Consume(object obj){
    IReceiveDamage receiver = obj as IReceiveDamage;
    
    if(receiver == null){
      return;
    }
    
    Damage damage = new Damage(-Health);

    receiver.ReceiveDamage(damage);

    if(Session.NetActive()){
      Actor actor = obj as Actor;
      
      if(actor != null){
        actor.Rpc(nameof(Actor.RemoteReceiveDamage), JsonConvert.SerializeObject(damage, Formatting.Indented));
      }
    }
    
    this.QueueFree();
	}
}