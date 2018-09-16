// Utility methods.

using Godot;
using System;
using System.Collections.Generic;

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

  // Flatten a Vector2 into an int index
  public static int CoordsToCellIndex(Vector2 coords, int gridSize){
    int x = (int)coords.x;
    int y = (int)coords.y;

    if(y < 0 || y >= gridSize || x < 0 || x >= gridSize){
      return -1;
    }
    return y * gridSize + x;
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
    Map cell coordinates to 3D positions based off of 
    the center's position and scale of each cell.
  */
  public static System.Collections.Generic.Dictionary<Vector2, Vector3> GetCellMap(
      List<Vector2> cells, // Coordinates of each cell getting assigned a position
      Vector2 centerCoords,
      Vector3 centerPos, // Make sure this matches either center of cell or top left corner
      float scale // Width and length of each cell
  ){
    System.Collections.Generic.Dictionary<Vector2, Vector3> ret;
    ret = new System.Collections.Generic.Dictionary<Vector2, Vector3>();

    foreach(Vector2 cell in cells){
      Vector2 offset = cell - centerCoords;
      //GD.Print(cell + " VS " + centerCoords + ":" + offset);
      float cellX = centerPos.x  + (scale * offset.x);
      float cellZ = centerPos.z  + (scale * offset.y);
      float cellY = centerPos.y;
      Vector3 cellPos = new Vector3(cellX, cellY, cellZ);
      ret.Add(cell, cellPos);
    }

    return ret;

  }

  /*
    Recenter map in new position
  */
  public static System.Collections.Generic.Dictionary<Vector2, Vector3> RecenterMap(
    Vector2 center,
    System.Collections.Generic.Dictionary<Vector2, Vector3> map,
    Vector3 destination
  ){
    GD.Print("Recenter start");
    if(!map.ContainsKey(center)){
      GD.Print("RecenterMap: Key " + center + " not found");
      return map;
    }

    Vector3 centerPos = map[center];
    Vector3 translation = destination - centerPos;
    return TranslateMap(map, translation);
  }


  /*
    Apply translation to every value in dictionary.
  */
  public static System.Collections.Generic.Dictionary<Vector2, Vector3> TranslateMap(
    System.Collections.Generic.Dictionary<Vector2, Vector3> map,
    Vector3 translation
  ){
    string debug = "Util.TranslateMap:";
    System.Collections.Generic.Dictionary<Vector2, Vector3> translatedMap;
    translatedMap = new System.Collections.Generic.Dictionary<Vector2, Vector3>();

    foreach(Vector2 key in map.Keys){
      GD.Print("Translating  " + map[key] + " to " + map[key] + translation);
      translatedMap.Add(key, map[key] + translation);
    }

    return translatedMap;
  }

  /* Given coords, position, and scale of center cell, determine coordinates of
     an arbitrary position.
  */
  public static Vector2 PosToCoords(
    Vector3 centerPos,    // Position dead center of the center cell
    Vector2 centerCoords, // Coordinates of center cell
    float centerScale,    // Size of center position in x and z dimensions
    Vector3 pos           // Arbitrary position
  ){
    string debug = "[Util.PosToCoords]\n";
    debug += "centerPos: " + centerPos + "\n";
    debug += "centerCoords: " + centerCoords + "\n";
    debug += "centerScale: " + centerScale + "\n";
    debug += "pos: " + pos + "\n";
    

    float halfScale = centerScale/2f;


    // Check if position is inside center cell. If so, return center coords.
    Vector3 cMinBounds = centerPos - new Vector3(halfScale, 0, halfScale);
    Vector3 cMaxBounds = centerPos + new Vector3(halfScale, centerScale, halfScale);
    if(InBounds(cMinBounds, cMaxBounds, pos)){
      debug += "returning " + centerCoords;
      //GD.Print(debug);
      return centerCoords;
    }
    else{
      debug += "Not within  " + cMinBounds + " and " + cMaxBounds + "\n";
    }

    // Get offset in full cell widths
    Vector3 posOffset = pos - centerPos;
    int xCoordsOffset = (int)pos.x / (int)centerScale; 
    int yCoordsOffset = (int)pos.z / (int)centerScale;

    // Check if remainder offset is large enough to leave center cell
    int xOffsetRemainder = (int)posOffset.x % (int)centerScale;
    int yOffsetRemainder = (int)posOffset.z % (int)centerScale;
    
    // If that remainder is large enough, add it to to offset
    xCoordsOffset += xOffsetRemainder/(int)halfScale;
    yCoordsOffset += yOffsetRemainder/(int)halfScale;

    Vector2 coordsOffset = new Vector2(xCoordsOffset, yCoordsOffset);

    debug += "returning " + (centerCoords + coordsOffset);
    //GD.Print(debug); 
    return centerCoords + coordsOffset;
  }

  /*   Returns true if pos is between min and max */
  public static bool InBounds(Vector3 min, Vector3 max, Vector3 pos){
    string debug = "Util.InBounds\n";
    debug += "min: " + min + "\n";
    debug += "max: " + max + "\n";
    debug += "pos: " + pos + "\n";
    

    if(min.x > pos.x || min.y > pos.y || min.z > pos.z){
      debug += " was false";
      //GD.Print(debug);
      return false;
    }

    if(max.x < pos.x || max.y < pos.y || max.z < pos.z){
      debug += " was false";
      //GD.Print(debug);
      return false;
    }

    debug += " was true";
    //GD.Print(debug);
    return true;
  }

}