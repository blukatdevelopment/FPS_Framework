/*
This test class exists to verify the non-graphical features of the adventure
game mode, and in particular, the logic behind managing treadmills and the 
dormant world. This should hopefully also encourage independence between 
graphical and non-graphical modules.
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
    Session.ChangeMenu(Menu.Menus.None);
    WorldWasMadeCorrectSize();
  }

  public void WorldWasMadeCorrectSize(){
    int width = world.GetWorldWidth();
    int height = world.GetWorldHeight();

    // Min corner cell
    TerrainCell min = world.RequestCell(new Vector2(0, 0));
    Test.Assert(min != null, "1: world has minimum corner cell.");

  }



}