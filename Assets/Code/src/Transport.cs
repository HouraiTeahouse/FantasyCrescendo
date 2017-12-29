using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Transport : MonoBehaviour {

  public int maxConnections = 10;
  public int port = 8888;

  // Network Server
  int socketId;
  int reliableChannel;

  // Network Client
  int connectionId;

  void Start() {
    // Setup server
    NetworkTransport.Init();
    var networkConfig = new ConnectionConfig();

    reliableChannel = networkConfig.AddChannel(QosType.Reliable);

    var topology = new HostTopology(networkConfig, maxConnections);

    socketId = NetworkTransport.AddHost(topology, port);
    Debug.LogFormat("Socket Open: ID: {0}", socketId);

    // Setup Client
    byte error;
    connectionId = NetworkTransport.Connect(socketId, "localhost", port, 0, out error);
    Debug.LogFormat("Connected to Server. Connection ID: {0}", connectionId);
  }

}
