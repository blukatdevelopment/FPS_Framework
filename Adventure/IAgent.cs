/*
	An IAgent is an AI for the dormant portion of the overworld.
	While an Ai controls an Actor, the Agent manipulates an ActorData.
*/
interface IAgent{

	void Init(Overworld world, ActorData host);

	// Perform updates. Delta is in overworld time units.
	void Update(int delta);

}