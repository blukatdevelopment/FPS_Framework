/*
This test class exists to verify the non-graphical features of the adventure
game mode, and in particular, the logic behind managing treadmills and the 
dormant world. This should hopefully also encourage independence between 
graphical and non-graphical modules.


While I aim to keep in-line comments minimal in actual classes, I will aim to
include as many comments inside tests as possible to explain what's going on and
why things are being tested a certain way. This will become useful when tests
break in the future. Similarly, test method names are to be long and descriptive
because they will be called exactly once, reducing the need for brevity.


Lastly, this test class will test against singleplayer functionality.
Multiplayer testing will be a bit more difficult to arrange.
*/
using Godot;
using System;
using System.Collections.Generic;
public class AdventureTest {
  Overworld world;


  public static void RunTests(){
    GD.Print("Running adventure tests"); 
    AdventureTest test = new AdventureTest();
  }
  

  public AdventureTest(){
    world = new Overworld();

    // Turning off HUD menu, which is enabled by default as player 1 is created.
    Session.ChangeMenu(Menu.Menus.None);
    
    // Pause world to allow updates to be run arbitrarily, and not in real time.
    world.Pause();

    WorldWasMadeCorrectSize();
    TreadmillWasInitializedCorrectly();
    PlayerPosIsTracked();
    TestGetCellMap();
    TreadmillShiftsProperly();

    Test.PrintFails();
  }

  public void WorldWasMadeCorrectSize(){
    GD.Print("WorldWasMadeCorrectSize");

    // Subtracting because coords start at [0,0], not [1,1].
    int width = world.GetWorldWidth() -1; 
    int height = world.GetWorldHeight() -1;

    // Min corner cell
    TerrainCell min = world.RequestCell(new Vector2(0, 0));
    Test.Assert(min != null, "1: world has minimum corner cell.");

    // Max corner cell
    TerrainCell max = world.RequestCell(new Vector2(width, height));
    Test.Assert(max != null, "2: world has maximum corner cell.");
  }

  public void TreadmillWasInitializedCorrectly(){
    GD.Print("TreadmillWasInitializedCorrectly");

    // Singleplayer treadmill has id of -1 because there's no peerIds.
    Treadmill treadmill = world.GetTreadmillById(-1);
    Test.Assert(treadmill != null, "1: singleplayer treadmill exists.");

    // Check that the treadmill was provided ActorData
    ActorData actorData = treadmill.actorData;
    Test.Assert(actorData != null, "2: treadmill was provided actorData.");

    // Check that an actor was made with this data
    Actor actor = treadmill.actor;
    Test.Assert(actor != null, "3: treadmill's actor exists.");

    // check that a center cell exists.
    TerrainCell center = treadmill.center;
    Test.Assert(center != null, "4: treadmill contains center cell.");

    // Check that the player is inside the center cell.
    bool isInsideCenter = center.InBounds(actor.Translation);
    Test.Assert(isInsideCenter, "5: actor is inside center cell.");

    // Check that all used coords all exist.
    int gridSize = world.GetWorldWidth();
    List<Vector2> usedCoords = treadmill.UsedCoords();
    foreach(Vector2 coords in usedCoords){
      string message = "";
      bool coordsAreValid = Util.CoordsToCellIndex(coords, gridSize) != -1;
      bool coordsExist = world.RequestCell(coords) != null;
      if(coordsAreValid){
        message = "6: valid coords " + coords + " exist.";
        Test.Assert(coordsExist, message);
      }
      else{
        message = "6: invalid coords " + coords + " do not exist.";
        Test.Assert(!coordsExist, message);
      }
    }

    
  }

  public void PlayerPosIsTracked(){
    GD.Print("PlayerPosIsTracked");

    // Singleplayer treadmill has id of -1 because there's no peerIds.
    Treadmill treadmill = world.GetTreadmillById(-1);
    Test.Assert(treadmill != null, "1: singleplayer treadmill exists.");
    List<Vector2> usedCoords = treadmill.UsedCoords();

    string output = "";
    bool playerInBounds = false;

    // Treadmill should have an actor it's following.
    Actor actor = treadmill.actor;
    Test.Assert(actor != null, "2: treadmill's actor exists");

    foreach(Vector2 coords in usedCoords){
      TerrainCell cell = world.RequestCell(coords);
      bool cellExists = cell != null;
      bool validCoords = world.CoordsToCellId(coords) != -1;
      Test.Assert(!validCoords || cellExists, "3: cell is invalid or exists " + coords);

      if(!cellExists){
        Test.Assert(!validCoords, "skipping remaining tests for invalid coords" + coords);
        continue;
      }      

      Vector3 centeredPos = cell.GetPos();
      Test.Assert(cell.InBounds(centeredPos), "3: centered position was inside cell");

      actor.Translation = centeredPos;
      Vector2 calculatedCoords = treadmill.PosToCoords(actor.Translation);
      output = "3: coords " + calculatedCoords;
      output += " calculated correctly from position " + actor.Translation;
      Test.Assert(calculatedCoords == coords, output);

      if(coords != treadmill.CenterCoords()){
        playerInBounds = treadmill.center.InBounds(actor.Translation);
        Test.Assert(!playerInBounds, "3: player outside center cell's bounds");
      }
    }

    actor.Translation = treadmill.center.GetPos();
    playerInBounds = treadmill.center.InBounds(actor.Translation);
    Test.Assert(playerInBounds, "4: player in bounds of centerCell when moved there ");
  }


  public void TestGetCellMap(){
    GD.Print("TestGetCellMap");

    float scale = Overworld.CellWidth();

    // Test expected starting conditions
    TestGetCellMapForLocation(new Vector2(0, 0), new Vector3(0, 0, 0), 1);

    // Testing larger radius
    TestGetCellMapForLocation(new Vector2(0, 0), new Vector3(0, 0, 0), 10);    

    // Test shifted center position
    TestGetCellMapForLocation(new Vector2(0, 0), new Vector3(5f, 0, -5f), 1);

    // Test center shifted diagnally
    TestGetCellMapForLocation(new Vector2(1, 1), new Vector3(scale, 0, scale), 1);

  }

  public void TestGetCellMapForLocation(Vector2 centerCoords, Vector3 centerPos, int radius){
    List<Vector2> candidateCoords = Util.CoordsInRadius(centerCoords, radius);
    
    float scale = Overworld.CellWidth();
    System.Collections.Generic.Dictionary<Vector2, Vector3> cellMap;
    cellMap = Util.GetCellMap(candidateCoords, centerCoords, centerPos, scale);
    
    GD.Print("Testing " + centerCoords + ":" + centerPos);
    foreach(Vector2 key in cellMap.Keys){
      TestCoordsPosMap(centerCoords, centerPos, key, cellMap[key], scale);
    }
  }

  public void TestCoordsPosMap(
    Vector2 centerCoords, 
    Vector3 centerPos, 
    Vector2 coords,
    Vector3 pos,
    float scale)
  {    
    GD.Print(coords + ":" + pos);

    // y should be the same for all coordinate centers
    Test.Assert(pos.y == centerPos.y, "1: y doesn't match center ");

    Vector3 off = pos - centerPos;

    // Check that x and y are multiples of scale
    Test.Assert((int)off.x % (int)scale == 0, "2: x should be multiple of scale");
    Test.Assert((int)off.z % (int)scale == 0, "3: z should be multiple of scale");

  }


  public void TreadmillShiftsProperly(){
    GD.Print("TreadmillShiftsProperly");

    // Singleplayer treadmill has id of -1 because there's no peerIds.
    Treadmill treadmill = world.GetTreadmillById(-1);
    Test.Assert(treadmill != null, "1: singleplayer treadmill exists.");
    List<Vector2> usedCoords = treadmill.UsedCoords();

    Actor actor = treadmill.actor;
    Test.Assert(actor != null, "2: actor is not null");

    Vector2 originalCenterCoords = treadmill.center.coords;
    Vector3 originalCenterPos = treadmill.center.GetPos();

    // Move in each direction and check that the moved state is
    foreach(Vector2 coords in usedCoords) {
      // Can't shift into centerCoords
      if(coords == originalCenterCoords){
        continue;
      }

      // Branching test to handle valid or invalid cells to shift into
      bool validCoords = world.CoordsToCellId(coords) != -1;
      bool shiftedBack = false;
      if(validCoords){
        GD.Print("3: testing shift to valid coords" + coords);
        PerformValidSingleShiftTest(treadmill, coords);
        shiftedBack = treadmill.CenterCoords() == originalCenterCoords;
        Test.Assert(shiftedBack, "3: shifted back to center from valid coords " + coords);
      }
      else{
        GD.Print("3: testing shift to invalid coords " + coords);
        PerformInvalidSingleShiftTest(treadmill, coords);
        shiftedBack = treadmill.CenterCoords() == originalCenterCoords;
        Test.Assert(shiftedBack, "3: shifted back to center from invalid coords " + coords);
      }
    }
  }

  public void PerformValidSingleShiftTest(Treadmill treadmill, Vector2 coords){
    TerrainCell cell = world.RequestCell(coords);
    Test.Assert(cell != null, "3: cell at valid coords " + coords + " exists");

    Vector2 originalCenterCoords = treadmill.center.coords;
    Vector3 originalCenterPos = treadmill.center.GetPos();

    string output = "";
    Actor actor = treadmill.actor;

    actor.Translation = cell.GetPos();
    bool actorInNewCell = cell.InBounds(actor.Translation);
    GD.Print("cell pos " + cell.GetPos() + " vs player pos " + actor.Translation);
    Test.Assert(actorInNewCell, "3: actor shifted successfully to valid coordinates" + coords);

    bool actorInCenter = treadmill.center.InBounds(actor.Translation);
    output = "3: actor has left center to valid coordinates ";
    output += "\n\t coords " + coords + " pos:" + cell.GetPos();
    output += "\n\t actor pos: " + actor.Translation;
    output += "\n\t center " + treadmill.CenterCoords() + " pos: " + treadmill.center.GetPos(); 
    Test.Assert(!actorInCenter,  output);

    treadmill.CheckPlayerPos();
    Test.Assert(coords == treadmill.CenterCoords(), "3: treadmill has shifted to " + treadmill.CenterCoords());

    // Determine what the centercoords new position is.
    Vector3 originalCenterNewPos = treadmill.GridToPos(originalCenterCoords);
    output = "3: original center position should be different now";
    output += "\n\t center old pos " + originalCenterPos;
    output += "\n\t center new pos " + originalCenterNewPos;
    Test.Assert(originalCenterPos != originalCenterNewPos, output);
    bool centerMovedVertically = originalCenterPos != originalCenterNewPos;
    Test.Assert(!centerMovedVertically, "3: Center position did not move on y axis.");

    actor.Translation = originalCenterNewPos;
    treadmill.CheckPlayerPos();

    bool shiftedBack = treadmill.CenterCoords() == originalCenterCoords;
    output = "3: valid coords returned to center cell";
    output += "\n\t coords: " + coords;
    output += "\n\t original center " + originalCenterCoords + ": " + originalCenterPos;
    output += "\n\t new center " + treadmill.CenterCoords() + ": " + treadmill.center.GetPos();
    Test.Assert(shiftedBack, output);
  }

  public void PerformInvalidSingleShiftTest(Treadmill treadmill, Vector2 coords){
    TerrainCell cell = world.RequestCell(coords);
    Test.Assert(cell == null, "3: cell at invalid coords " + coords + " does not exist.");

    Vector2 originalCenterCoords = treadmill.center.coords;
    Vector3 originalCenterPos = treadmill.center.GetPos();

    Actor actor = treadmill.actor;

    string output = "";

    Vector3 coordsPos = treadmill.GridToPos(coords);

    actor.Translation = coordsPos;
    bool actorInCenter = treadmill.center.InBounds(actor.Translation);
    Test.Assert(!actorInCenter, "3: actor has left center to invalid coordinates " + coords);

    treadmill.CheckPlayerPos();
    Test.Assert(coords != treadmill.CenterCoords(), "3: treadmill did not shift to invalid coords "+ coords);

    // originalCenterPos should remain the same because treadmill should not
    // have moved from its original position.

    actor.Translation = originalCenterPos;
    treadmill.CheckPlayerPos();

    bool shiftedBack = treadmill.CenterCoords() == originalCenterCoords;
    output = "3: invalid coords returned to center cell";
    output += "\n\t coords: " + coords;
    output += "\n\t original center: " + originalCenterCoords;
    output += "\n\t original center pos: " + originalCenterPos;
    output += "\n\t new center coords: " + treadmill.CenterCoords();
    Test.Assert(shiftedBack, output);
  }
}