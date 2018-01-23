using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class MessageHandlers {

  readonly Action<NetworkDataMessage>[] handlers;

  public MessageHandlers() {
    handlers = new Action<NetworkDataMessage>[byte.MaxValue];
  }

  public void RegisterHandler(byte code, Action<NetworkDataMessage> handler) {
    if (handler == null) return;
    handlers[code] += handler;
  }

  public void RegisterHandler<T>(byte code, Action<T> handler) where T : MessageBase, new() {
    if (handler == null) return;
    RegisterHandler(code, dataMsg => handler(dataMsg.ReadAs<T>()));
  }

  public void UnregisterHandler(byte code) => handlers[code] = null;

  internal void Execute(INetworkConnection connection, byte[] data, int size) {
    var reader = new NetworkReader(data);
    byte header = reader.ReadByte();
    var handler = handlers[header];
    if (handler == null) return;
    var message = new NetworkDataMessage(connection, reader);
    handler(message);
  }

}


}