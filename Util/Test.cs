/*
  This class aims to add some barebones automated testing tools within godot
  and is by no means supposed to completely replicate the functionality of
  official testing frameworks.
*/
using System.Collections.Generic;
using Godot;

public class Test{
  public static List<string> fails; // failed test info
  public static bool debugs;

  public static void Init(bool showDebugs = true){
    fails = new List<string>();
    debugs = showDebugs;
  }

  public static void Assert(bool result, string message){
    if(!result){
      Fail(message, StackTrace());
    }
    else if(debugs){
      GD.Print("Passed: " + message);
    }
  }

  public static void Fail(string message, string trace){
    string failedMessage = "Failed: " + message;
    
    if(debugs){
      GD.Print(failedMessage + trace);
    }
    fails.Add(failedMessage);
  }

  public static void PrintFails(){
    GD.Print("##############################################################");
    GD.Print("#                       Error report                         #");
    GD.Print("##############################################################");
    if(fails.Count == 0){
      GD.Print("All tests ran successfully!");
    }
    else{
      GD.Print(fails.Count + " tests failed.");
    }
    foreach(string fail in fails){
      GD.Print(fail);
    }
  }

  public static string StackTrace(){
    return System.Environment.StackTrace;
  }
}