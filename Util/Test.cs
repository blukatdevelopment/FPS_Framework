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

  public static void Assert(bool result){
    string output = StackTrace();

    if(!result){
      Fail(output);
    }
  }

  public static void Assert(bool result, string message){
    string output = message + "\n";
    output += StackTrace();

    if(!result){
      Fail(output);
    }
    else if(debugs){
      GD.Print("Passed: " + message);
    }
  }

  public static void Fail(string message){
    if(debugs){
      GD.Print(message);
    }
    fails.Add(message);
  }

  public static void PrintFails(){
    foreach(string fail in fails){
      GD.Print(fail);
    }
  }

  public static string StackTrace(){
    return System.Environment.StackTrace;
  }
}