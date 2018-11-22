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


Lastly, this test class will test against local functionality.
Online testing will be a bit more difficult to arrange.
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

    //WorldWasMadeCorrectSize();
    TestCoordsFromPosition();
    TestCellPositionFromCoords();

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

  public void TestCoordsFromPosition(){
    GD.Print("TestCoordsFromPosition");
    
    float scale = 100f;
    Vector3[] positions = new Vector3[] {
      new Vector3(0, 0, 0),
      new Vector3(100, 0, 0),
      new Vector3(0, 0, 100),
      new Vector3(0, 100, 0),
      new Vector3(-100, 0, 0),
      new Vector3(0, 0, -100),
      new Vector3(50, 0, 50),
      new Vector3(-50, 0, -50),
      new Vector3(150, 0, 150)
    };

    Vector2[] expecteds = new Vector2[] {
      new Vector2(0, 0),
      new Vector2(1, 0),
      new Vector2(0, 1),
      new Vector2(0, 0),
      new Vector2(0, 0),
      new Vector2(0, 0),
      new Vector2(1, 1),
      new Vector2(0, 0),
      new Vector2(2, 2)
    };

    for(int i = 0; i < positions.Length; i++){
      Vector3 pos = positions[i];
      Vector2 actual = Util.CoordsFromPosition(pos, scale);
      Vector2 expected = expecteds[i];
      string output = (i + 1) + ". Expected " + expected + " and got " + actual; 
      Test.Assert(actual == expected, output);      
    }
  }


  public void TestCellPositionFromCoords(){
    GD.Print("TestCellPositionFromCoords");
    float scale = 100f;

    Vector2[] coords = new Vector2[] {
      new Vector2(0, 0),
      new Vector2(1, 0),
      new Vector2(0, 1),
      new Vector2(-1, 0),
      new Vector2(0, -1)
    };

    Vector3[] expecteds = new Vector3[] {
      new Vector3(0, 0, 0),
      new Vector3(100, 0, 0),
      new Vector3(0, 0, 100),
      new Vector3(-100, 0, 0),
      new Vector3(0, 0, -100)
    };

    for(int i = 0; i < coords.Length; i++){
      Vector3 expected = expecteds[i];
      Vector3 actual = Util.CellPositionFromCoords(coords[i], scale);
      string output = (i + 1) + ". Expected " + expected + " and got " + actual;
      Test.Assert(actual == expected, output);
    }

  }

}