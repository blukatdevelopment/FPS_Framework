/*
    The NetworSession is like the Session's little brother that tags along for multiplayer games.
    The prime purpose of this class is to abstract away networked info and functionality that
    the Session may not need during singlePlayer games.
*/

using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;


public class NetworkSession : Node {
  public NetworkedMultiplayerENet peer;
  public const int DefaultPort = 27182;
  private const int MaxPlayers = 10;
  public const string DefaultServerAddress = "127.0.0.1";
  public int selfPeerId;
  public Dictionary<int, PlayerData> playerData;
  
  // Init variables
  public bool isServer;
  public string initAddress;
  public string initPort;
  public string initName;

  
  public override void _Ready(){
    playerData = new Dictionary<int, PlayerData>();
  }

  public void InitServer(Godot.Object obj, string playerJoin, string playerLeave, string port = ""){
    GD.Print("Initializing Client");

    peer = new Godot.NetworkedMultiplayerENet();
    this.GetTree().Connect("network_peer_connected", obj, playerJoin);
    this.GetTree().Connect("network_peer_disconnected", obj, playerLeave);
    
    int usedPort = DefaultPort;

    int customPort = 0;
    if(port != "" && Int32.TryParse(port, out customPort)){
      usedPort = customPort;
    }

    this.initName = "Server";

    peer.CreateServer(DefaultPort, MaxPlayers);
    this.GetTree().SetNetworkPeer(peer);
    selfPeerId = this.GetTree().GetNetworkUniqueId();
  }
  
  public void InitClient(string address, Godot.Object obj, string success, string fail, string port = ""){
    GD.Print("Initializing Client");

    GetTree().Connect("connected_to_server", obj, success);
    GetTree().Connect("connection_failed", obj, fail);
    
    int usedPort = DefaultPort;

    int customPort = 0;
    if(port != "" && Int32.TryParse(port, out customPort)){
      usedPort = customPort;
    }

    peer = new Godot.NetworkedMultiplayerENet();
    peer.CreateClient(address, usedPort);

    this.GetTree().SetNetworkPeer(peer);
    selfPeerId = this.GetTree().GetNetworkUniqueId();
  }
  
  
}