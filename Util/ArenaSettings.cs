/*
	A record containing settings used for Arena gamemode.
*/
using System;

public class ArenaSettings {
	public bool useKits, usePowerups;
	public int duration; // Minutes
	public int bots;		


	public ArenaSettings(){
		useKits = true;
		usePowerups = true;
		duration = 5;
		bots = 0;
	}
}