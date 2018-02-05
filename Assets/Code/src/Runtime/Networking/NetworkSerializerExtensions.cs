using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Networking{

public static class NetworkSerializerExtensions {

  public static short ReadPackedInt16(this Deserializer reader) {
    return (short)DecodeZigZag(reader.ReadPackedUInt32());
  }

  public static void WritePackedInt16(this Serializer writer, short value) {
    writer.WritePackedUInt32((uint)EncodeZigZag(value, 16));
  }

  public static int ReadPackedInt32(this Deserializer reader) {
    return (int)DecodeZigZag(reader.ReadPackedUInt32());
  }

  public static void WritePackedInt32(this Serializer writer, int value) {
    writer.WritePackedUInt32((uint)EncodeZigZag(value, 32));
  }

  public static long ReadPackedInt64(this Deserializer reader) {
    return DecodeZigZag(reader.ReadPackedUInt64());
  }

  public static void WritePackedInt64(this Serializer writer, long value) {
    writer.WritePackedUInt64(EncodeZigZag(value, 64));
  }

  static ulong EncodeZigZag(long value, int bitLength) {
    unchecked {
      return (ulong)((value << 1) ^ (value >> (bitLength - 1)));
    }
  }

  static long DecodeZigZag(ulong value) {
    unchecked {
      if ((value & 0x1) == 0x1) {
        return -1 * ((long)(value >> 1) + 1);
      }
      return (long)(value >> 1);
    }
  }


}

}