// Provides a powerup effect on contact.
using Godot;
public class PowerUp : Item {
  
  [Remote]
  public override void DoOnCollide(object body){
    ApplyPowerUp(body);
  }
  
  public virtual void ApplyPowerUp(object obj){}

  public override void Interact(object interactor, Item.Uses interaction = Item.Uses.A){
		ApplyPowerUp(interactor);
		Node interactorNode = interactor as Node;
		string interactorPath = "";
		
    if(interactorNode != null){
			interactorPath = interactorNode.GetPath().ToString();
		}
		
    Rpc(nameof(DeferredApplyPowerUp), interactorPath);
  }

  [Remote]
  public void DeferredApplyPowerUp(string interactorPath){
  	Node interactorNode = null;
  	if(interactorPath != ""){
  		NodePath path = new NodePath(interactorPath);
  		interactorNode = Session.session.GetNode(path);
  	}	
  	ApplyPowerUp(interactorNode as object);
  }
  
}