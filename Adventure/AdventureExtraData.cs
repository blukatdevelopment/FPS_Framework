/*
    Used to store an adventure game's settings.
*/

using Godot;
using Newtonsoft.Json;


public class AdventureExtraData {
  public Vector2 startingCell;
  public int actorId;


  public AdventureExtraData(){}

  public static AdventureExtraData FromJson(string json){
    return JsonConvert.DeserializeObject<AdventureExtraData>(json);
  }

  public string ToJson(){
    return JsonConvert.SerializeObject(this, Formatting.Indented);
  }
}