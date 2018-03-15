using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

class NetBuffer {

  byte[] Buffer;
  uint position;
  const int kInitialSize = 64;
  const float kGrowthFactor = 1.5f;
  const int kBufferSizeWarning = 1024 * 1024 * 128;

  public uint Position => position; 
  public int Size => Buffer.Length;

  public NetBuffer() {
    Buffer = ArrayPool<byte>.Shared.Rent(kInitialSize);
  }

  // this does NOT copy the buffer
  public NetBuffer(byte[] buffer) {
    Buffer = buffer;
  }

  public byte ReadByte() {
    if (position >= Buffer.Length) {
      throw new IndexOutOfRangeException("NetworkReader:ReadByte out of range:" + ToString());
    }
    return Buffer[position++];
  }

  public void ReadBytes(byte[] buffer, uint count) {
    if (position + count > Buffer.Length) {
      throw new IndexOutOfRangeException("NetworkReader:ReadBytes out of range: (" + count + ") " + ToString());
    }

    for (ushort i = 0; i < count; i++) {
      buffer[i] = Buffer[position + i];
    }
    position += count;
  }

  public void ReadChars(char[] buffer, uint count) {
    if (position + count > Buffer.Length) {
      throw new IndexOutOfRangeException("NetworkReader:ReadChars out of range: (" + count + ") " + ToString());
    }
    for (ushort i = 0; i < count; i++) {
      buffer[i] = (char)Buffer[position + i];
    }
    position += count;
  }

  internal ArraySegment<byte> AsArraySegment() {
    return new ArraySegment<byte>(Buffer, 0, (int)position);
  }

  public void WriteByte(byte value) {
    WriteCheckForSpace(1);
    Buffer[position] = value;
    position += 1;
  }

  public void WriteByte2(byte value0, byte value1) {
    WriteCheckForSpace(2);
    Buffer[position] = value0;
    Buffer[position + 1] = value1;
    position += 2;
  }

  public void WriteByte4(byte value0, byte value1, byte value2, byte value3) {
    WriteCheckForSpace(4);
    Buffer[position] = value0;
    Buffer[position + 1] = value1;
    Buffer[position + 2] = value2;
    Buffer[position + 3] = value3;
    position += 4;
  }

  public void WriteByte8(byte value0, byte value1, byte value2, byte value3, byte value4, byte value5, byte value6, byte value7) {
    WriteCheckForSpace(8);
    Buffer[position] = value0;
    Buffer[position + 1] = value1;
    Buffer[position + 2] = value2;
    Buffer[position + 3] = value3;
    Buffer[position + 4] = value4;
    Buffer[position + 5] = value5;
    Buffer[position + 6] = value6;
    Buffer[position + 7] = value7;
    position += 8;
  }

  // every other Write() function in this class writes implicitly at the end-marker m_Pos.
  // this is the only Write() function that writes to a specific location within the buffer
  public void WriteBytesAtOffset(byte[] buffer, ushort targetOffset, ushort count) {
    uint newEnd = (uint)(count + targetOffset);
    WriteCheckForSpace((ushort)newEnd);
    if (targetOffset == 0 && count == buffer.Length) {
      buffer.CopyTo(Buffer, position);
    } else {
      //CopyTo doesnt take a count :(
      for (int i = 0; i < count; i++) {
        Buffer[targetOffset + i] = buffer[i];
      }
    }

    // although this writes within the buffer, it could move the end-marker
    if (newEnd > position) {
      position = newEnd;
    }
  }

  public void WriteBytes(byte[] buffer, ushort count) {
    WriteCheckForSpace(count);

    if (count == buffer.Length) {
      buffer.CopyTo(Buffer, position);
    } else {
      // CopyTo doesnt take a count :(
      for (int i = 0; i < count; i++) {
        Buffer[position + i] = buffer[i];
      }
    }
    position += count;
  }

  void WriteCheckForSpace(ushort count) {
    if (position + count < Buffer.Length) return;

    int newLen = (int)(Buffer.Length * kGrowthFactor);
    while (position + count >= newLen) {
      newLen = (int)(newLen * kGrowthFactor);
      if (newLen > kBufferSizeWarning) {
        Debug.LogWarning("NetworkBuffer size is " + newLen + " bytes!");
      }
    }

    // only do the copy once, even if newLen is increased multiple times
    var pool = ArrayPool<byte>.Shared;
    byte[] tmp = pool.Rent(newLen);
    Buffer.CopyTo(tmp, 0);
    pool.Return(Buffer);
    Buffer = tmp;
  }

  public void FinishMessage() {
    // two shorts (size and msgType) are in header.
    ushort sz = (ushort)(position - (sizeof(ushort) * 2));
    Buffer[0] = (byte)(sz & 0xff);
    Buffer[1] = (byte)((sz >> 8) & 0xff);
  }

  public void SeekZero() => position = 0;

  public void Replace(byte[] buffer) {
    ArrayPool<byte>.Shared.Return(Buffer);
    Buffer = buffer;
    position = 0;
  }

  public override string ToString() {
    return string.Format("NetBuf sz:{0} pos:{1}", Buffer.Length, position);
  }
} // end NetBuffer

// -- helpers for float conversion --
// This cannot be used with IL2CPP because it cannot convert FieldOffset at the moment
// Until that is supported the IL2CPP codepath will use BitConverter instead of this. Use
// of BitConverter is otherwise not optimal as it allocates a byte array for each conversion.
#if !INCLUDE_IL2CPP
[StructLayout(LayoutKind.Explicit)]
internal struct UIntFloat
{
    [FieldOffset(0)]
    public float floatValue;

    [FieldOffset(0)]
    public uint intValue;

    [FieldOffset(0)]
    public double doubleValue;

    [FieldOffset(0)]
    public ulong longValue;
}

internal class FloatConversion {

  public static float ToSingle(uint value) {
    UIntFloat uf = new UIntFloat();
    uf.intValue = value;
    return uf.floatValue;
  }

  public static double ToDouble(ulong value) {
    UIntFloat uf = new UIntFloat();
    uf.longValue = value;
    return uf.doubleValue;
  }

}
#endif // !INCLUDE_IL2CPP

}