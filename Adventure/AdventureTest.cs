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

    Test.PrintFails();
  }

  public void WorldWasMadeCorrectSize(){
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

      if(coords != treadmill.center.coords){
        playerInBounds = treadmill.center.InBounds(actor.Translation);
        Test.Assert(!playerInBounds, "3: player outside center cell's bounds");
      }
    }

    actor.Translation = treadmill.center.GetPos();
    playerInBounds = treadmill.center.InBounds(actor.Translation);
    Test.Assert(playerInBounds, "4: player in bounds of centerCell when moved there ");
  }
}