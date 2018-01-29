using MessageHeader = System.Byte;

namespace HouraiTeahouse.FantasyCrescendo {

public static class MessageCodes {

  public const MessageHeader Connect = 0;
  public const MessageHeader Disconnect = 1;
  public const MessageHeader Error = 2;

  public const MessageHeader ClientReady = 3;
  public const MessageHeader ServerReady = 4;
  public const MessageHeader UpdateConfig = 5;
  public const MessageHeader MatchStart = 6;
  public const MessageHeader MatchFinish = 7;

  public const MessageHeader UpdateInput = 8;
  public const MessageHeader UpdateState = 9;

}

}