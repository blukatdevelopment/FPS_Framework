/*
	A record containing settings used for Arena gamemode.
*/
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

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

	public static ArenaSettings FromJson(string json){
		return JsonConvert.DeserializeObject<ArenaSettings>(json);
	}

	public static string ToJson(ArenaSettings dat){
		return JsonConvert.SerializeObject(dat, Formatting.Indented);
	}
}