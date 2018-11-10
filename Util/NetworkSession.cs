/*
    The NetworSession manages connections between server and client.
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
  public System.Collections.Generic.Dictionary<int, PlayerData> playerData;
  public int randomSeed;
  public System.Random random;
  public int playersReady;
  public bool isServer;
  public string initAddress;
  public string initPort;
  public string initName;
  
  public override void _Ready(){
    playerData = new System.Collections.Generic.Dictionary<int, PlayerData>();
  }

  public bool Initialized(){
    return peer != null;
  }

  public void UpdateServer(Godot.Object obj, string playerJoin, string playerLeave){
    this.GetTree().Connect("network_peer_connected", obj, playerJoin);
    this.GetTree().Connect("network_peer_disconnected", obj, playerLeave);
  }

  public void InitServer(Godot.Object obj, string playerJoin, string playerLeave, string port = ""){
    isServer = true;
    peer = new Godot.NetworkedMultiplayerENet();
    this.GetTree().Connect("network_peer_connected", obj, playerJoin);
    this.GetTree().Connect("network_peer_disconnected", obj, playerLeave);
    
    int usedPort = DefaultPort;

    int customPort = 0;
    
    if(port != "" && Int32.TryParse(port, out customPort)){
      usedPort = customPort;
    }

    this.randomSeed = (int)DateTime.Now.Ticks;
    this.random = new Random(randomSeed);

    peer.CreateServer(DefaultPort, MaxPlayers);
    this.GetTree().SetNetworkPeer(peer);
    selfPeerId = this.GetTree().GetNetworkUniqueId();
  }

  // Update handler methods without disconnecting.
  public void UpdateClient(Godot.Object obj, string success, string peerJoin, string fail){
    GetTree().Connect("connected_to_server", obj, success);
    GetTree().Connect("connection_failed", obj, fail);
    GetTree().Connect("network_peer_connected", obj, peerJoin);
  }
  
  public void InitClient(string address, Godot.Object obj, string success, string peerJoin, string fail, string port = ""){
    isServer = false;
    GetTree().Connect("connected_to_server", obj, success);
    GetTree().Connect("connection_failed", obj, fail);
    GetTree().Connect("network_peer_connected", obj, peerJoin);

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
  
  public void InitRandom(int seed){
    this.randomSeed = seed;
    this.random = new System.Random(randomSeed);
  }
  
}