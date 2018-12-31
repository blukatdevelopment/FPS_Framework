// Utility methods.

using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

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

  // Returns true if index is positive and within max bounds.
  public static bool ValidCellIndex(int index, int gridSize){
    if(index < 0){
      return false;
    }
    if(index >= gridSize * gridSize){
      return false;
    }

    return true;
  }

  // Flatten a Vector2 into an int index
  public static int CoordsToCellIndex(Vector2 coords, int gridSize){
    int x = (int)coords.x;
    int y = (int)coords.y;

    if(y < 0 || y >= gridSize || x < 0 || x >= gridSize){
      return -1;
    }

    return y * gridSize + x;
  }

  public static List<Vector2> NeighborCoords(Vector2 center, int gridSize){
    List<Vector2> coordsInRadius = CoordsInRadius(center, 1);

    List<Vector2> neighbors = new List<Vector2>();

    foreach(Vector2 coords in coordsInRadius){
      if(coords != center){
        neighbors.Add(coords);
      }
    }

    return neighbors;
  }

  public static List<int> NeighborIndices(int index, int gridSize){
    Vector2 centerCoords = CellIndexToCoords(index, gridSize);
    List<Vector2> neighborCoords = CoordsInRadius(centerCoords, 1);
    List<int> neighborIndices = new List<int>();

    foreach(Vector2 coords in neighborCoords){
      int neighborIndex = CoordsToCellIndex(coords, gridSize);
      
      if(neighborIndex != index){
        neighborIndices.Add(neighborIndex);
      }
    }

    return neighborIndices;
  }

  // Turn an int index for position into a Vector2
  public static Vector2 CellIndexToCoords(int index, int gridSize){
    int y = index % gridSize;
    int x = index / gridSize;

    return new Vector2(x, y);
  }

  public static List<Vector2> CoordsInRadius(Vector2 center, int radius){
    List<Vector2> ret = new List<Vector2>();
    
    for(int i = -radius; i <= radius; i++){
      for(int j = -radius; j <= radius; j++){
        Vector2 offset = new Vector2(i, j);
        Vector2 coord = center + offset;
        ret.Add(coord);
      }
    }

    return ret;
  }

  /*
    Assuming a world starting at coords [0,0] and going to [size-1, size-1],
    determine what coordinates a specific position falls inside. Doesn't care
    about world bounds. [0, 0, 0] should be at the center of [0, 0]
  */
  public static Vector2 CoordsFromPosition(
    Vector3 position, 
    float scale // Width/height of cells in world units
  ){
    int x = (int)position.x;
    int z = (int)position.z;

    x += (int)(scale/2f);
    z += (int)(scale/2f);

    x /= (int)scale;
    z /= (int)scale;

    return new Vector2(x, z);
  }

  // Returns the center position of a cell, given position.
  public static Vector3 CellPositionFromCoords( 
    Vector2 coords, 
    float scale // Width/height of cells in world units
  ){
    float x = coords.x * scale;
    float y = 0f; // 0 All positions aught to start at 0
    float z = coords.y * scale;

    Vector3 ret = new Vector3(x, y, z);

    return ret;
  }


  /*   Returns true if pos is between min and max */
  public static bool InBounds(Vector3 min, Vector3 max, Vector3 pos){
    if(min.x > pos.x || min.y > pos.y || min.z > pos.z){
      return false;
    }

    if(max.x < pos.x || max.y < pos.y || max.z < pos.z){
      return false;
    }
    
    return true;
  }

  // Use this to find methods for classes.
  public static void ShowMethods(Type type){
    foreach (var method in type.GetMethods()){
      string ret = "" + method.ReturnType +"," + method.Name;
      foreach( var parameter in method.GetParameters()){
        ret += ", " + parameter.ParameterType + " " + parameter.Name; 
      }
      GD.Print(ret);
      System.Threading.Thread.Sleep(100);
    }
  }
  
  // Use this to find variables for classes
  public static void ShowProperties(Type type){
    foreach(PropertyInfo prop in type.GetProperties()){
      GD.Print(prop.Name + " : " + prop.PropertyType );
    }
  }
  
  // Use this to print out class variables 
  public static void ShowVariables(Type type){
    BindingFlags bindingFlags = BindingFlags.Public |
                            BindingFlags.NonPublic |
                            BindingFlags.Instance |
                            BindingFlags.Static;

    foreach (FieldInfo field in type.GetFields(bindingFlags)) {
        GD.Print(field.Name);
        System.Threading.Thread.Sleep(100);
    }
  }

  public static Vector2 GetMousePosition(Spatial spat){
    return spat.GetViewport().GetMousePosition();
  }
}