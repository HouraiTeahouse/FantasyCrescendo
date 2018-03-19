using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

internal class LocalInterface : NetworkInterface  {

  public struct Message {
    public byte[] Data;
    public int Size;
  }

  protected readonly Queue<Message> UnreadMessages;
  public readonly LocalInterface Mirror;
  public readonly NetworkConnection Connection;

  LocalInterface(LocalInterface mirror) : base(1500) {
    UnreadMessages = new Queue<Message>();
    Mirror = mirror;
    Connection = OnNewConnection(0);
    Connection.Status = ConnectionStatus.Connected;
  }

  public LocalInterface() : base(1500) {
    Mirror = new LocalInterface(this);
    UnreadMessages = new Queue<Message>();
    Connection = OnNewConnection(0);
    Connection.Status = ConnectionStatus.Connected;
  }

  public override void Update() {
    var pool = ArrayPool<byte>.Shared;
    while (UnreadMessages.Count > 0) {
      var message = UnreadMessages.Dequeue();
      OnRecieveData(Connection.Id, message.Data, message.Size);
      pool.Return(message.Data);
    }
  }

  protected override void SendImpl(int connectionId, byte[] data, int count, 
                                   NetworkReliablity reliablity = NetworkReliablity.Reliable) {
    var array = ArrayPool<byte>.Shared.Rent(count);
    Buffer.BlockCopy(data, 0, array, 0, count);
    Mirror.UnreadMessages.Enqueue(new Message {
      Data = array,
      Size = count
    });
  }

  public override Task<NetworkConnection> Connect(NetworkConnectionConfig config) {
    return Task.FromResult(Connection);
  }

  public override void Disconnect(int connectionId) {}
  public override void Dispose() {}

}

}