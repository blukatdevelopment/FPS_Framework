// Methods I wish Godot had built in.

using Godot;
using System;
using System.Collections.Generic;

public class Util{
  
  
  /* Transform methods */
  public static Vector3 TUp(Transform t){
    return t.basis.y;
  }
  
  public static Vector3 TForward(Transform t){
    return -t.basis.z;
  }
  
  public static Vector3 TRight(Transform t){
    return t.basis.x;
  }
  
  public static float ToDegrees(float radians){
    return radians * 180f / (float)Math.PI;
  }
  
  public static float ToRadians(float degrees){
    return degrees * (float)Math.PI / 180f;
  }
  
  public static Vector3 ToDegrees(Vector3 radians){
    float x = ToDegrees(radians.x);
    float y = ToDegrees(radians.y);
    float z = ToDegrees(radians.z);
    return new Vector3(x, y, z);
  }
  
  public static Vector3 ToRadians(Vector3 degrees){
    float x = ToRadians(degrees.x);
    float y = ToRadians(degrees.y);
    float z = ToRadians(degrees.z);
    return new Vector3(x, y, z);
  }
<<<<<<< HEAD
=======

  public static float ToFloat(string val){
    if(val == ""){
      GD.Print("ToFloat: Blank String");
      return 0.0f;
    }
    
    double ret = 0;
    if(Double.TryParse(val, out ret)){
      return (float)ret;
    }
    return 0.0f;
  }

  public static int ToInt(string val){
    if(val == ""){
      GD.Print("ToInt: Blank String");
      return 0;
    }
    int ret = 0;
    if(Int32.TryParse(val, out ret)){
      return ret;
    }
    GD.Print("ToInt failed");
    return 0;
  }

  public static bool ToBool(string val){
    if(val == ""){
      GD.Print("ToBool: Blank String");
      return false;
    }
    bool ret = false;
    if(Boolean.TryParse(val, out ret)){
      return ret;
    }
    GD.Print("ToBool failed");
    return false;
  }

  public static object RayCast(Vector3 start, Vector3 end, World world) {
    if(world == null){
      return null;
    }
    PhysicsDirectSpaceState spaceState = world.DirectSpaceState as PhysicsDirectSpaceState;
    var result = spaceState.IntersectRay(start, end);
    
    if(!result.ContainsKey("collider")){
      return null;
    }

    object collider = result["collider"];

    return collider;
  }

  // Alternative to boxcasting that respects cover. Returns a list of unique
  // objects sighted.
  public static List<object> GridCast(
    Vector3 start,
    Vector3 end,
    World world,
    int scale = 3, // Used for height and width of grid
    float spacing = 0.5f // How far apart will each raycast be from one another
  ){
    List<object> ret = new List<object>();
    
    // Negative scale is converted to positive.
    if(scale < 0){
      scale *= -1;
    }

    float offX, offY; // Offsets for each raycast.

    for(int i = -scale; i < scale; i++){
      for(int j = -scale; j < scale; j++){
        Vector3 otherStart = start;
        Vector3 otherEnd = end;

        offX = spacing * j;
        offY = spacing * i;
        
        otherStart += new Vector3(offX, offY, 0f);
        otherEnd += new Vector3(offX, offY, 0f);

        object candidate = RayCast(otherStart, otherEnd, world);
        if(candidate != null && !ret.Contains(candidate)){
          ret.Add(candidate);
        }
      }
    }
    return ret;
  }
>>>>>>> develop
}