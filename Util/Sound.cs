using Godot;

public class Sound {
	public enum Effects{
    None,
		RifleShot,
		RifleReload,
		FistSwing,
		FistImpact,
		ActorDamage,
    ActorDeath
	};

	public enum Songs{
		None,
    FloatingHorizons
	};

	public static void EffectFactory(Effects effect){
    
	}

	public static string SongFile(Songs song){
    string ret = "";
    switch(song){
      case Songs.FloatingHorizons: 
        ret = "res://Audio/Songs/floating_horizons.ogg"; 
        break;
    }
    return ret;
	}
  
  public static void PlaySong(Songs song){
    if(song == Songs.None){
      return;
    }
    
    string fileName = SongFile(song);
    GD.Print("Playing " + fileName);
    AudioStreamOGGVorbis stream = (AudioStreamOGGVorbis)GD.Load(fileName);
    if(Session.session.jukeBox == null){
      Session.session.InitJukeBox();
    }
    Session.session.jukeBox.Stream = stream;
    Session.session.jukeBox.Playing = true;
  }
  
  public static void PauseSong(){
    if(Session.session.jukeBox == null){
      return;
    }
    Session.session.jukeBox.Playing = false;
  }
}