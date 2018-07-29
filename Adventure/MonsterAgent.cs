/*
	Wanders the world aimlessly.
*/
using Godot;

public class MonsterAgent : IAgent {

	public Overworld world;

	public void Init(Overworld world, ActorData host){
		GD.Print("MonsterAgent.Init not implemented");
		this.world = world;
	}

	public void Update(int delta){
		GD.Print("MonsterAgent.Update not implemented");
	}
	
}