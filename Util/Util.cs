// Methods I wish Godot had built in.

using Godot;
using System;

public class Util{
  
  
  /* Transform methods. IF you pass a global or local transform, it will match.*/
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

  public static object RayCast (Vector3 start, Vector3 end, World world) {
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
}