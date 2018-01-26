using MessageHeader = System.Byte;

namespace HouraiTeahouse.FantasyCrescendo {

public static class MessageCodes {

  public const MessageHeader Connect = 0;
  public const MessageHeader Disconnect = 1;
  public const MessageHeader Error = 2;

  public const MessageHeader PeerReady = 3;
  public const MessageHeader UpdateConfig = 4;
  public const MessageHeader MatchStart = 5;
  public const MessageHeader MatchFinish = 6;

  public const MessageHeader UpdateInput = 7;
  public const MessageHeader UpdateState = 8;

}

}