/*
	A record containing settings used for Arena gamemode.
*/
using System;
using System.Collections.Generic;

public class ArenaSettings {
	public bool useKits, usePowerups;
	public int duration; // Minutes
	public int bots;
	public List<int> botIds; 


	public ArenaSettings(){
		useKits = true;
		usePowerups = true;
		duration = 5;
		bots = 0;
		botIds = new List<int>();
	}
}