using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class Serializer {

  const int k_MaxStringLength = 1024 * 32;
  NetBuffer writeBuffer;
  static Encoding s_Encoding;
  static byte[] s_StringWriteBuffer;

  public Serializer() {
    writeBuffer = new NetBuffer();
    if (s_Encoding == null) {
      s_Encoding = new UTF8Encoding();
      s_StringWriteBuffer = new byte[k_MaxStringLength];
    }
  }

  public Serializer(byte[] buffer) {
    this.writeBuffer = new NetBuffer(buffer);
    if (s_Encoding == null) {
      s_Encoding = new UTF8Encoding();
      s_StringWriteBuffer = new byte[k_MaxStringLength];
    }
  }

  public short Position { get { return (short)writeBuffer.Position; } }

  public byte[] ToArray() {
    var newArray = new byte[writeBuffer.AsArraySegment().Count];
    Array.Copy(writeBuffer.AsArraySegment().Array, newArray, writeBuffer.AsArraySegment().Count);
    return newArray;
  }

  public byte[] AsArray() => AsArraySegment().Array;

  internal ArraySegment<byte> AsArraySegment() => writeBuffer.AsArraySegment();

  // http://sqlite.org/src4/doc/trunk/www/varint.wiki

  public void WritePackedUInt32(UInt32 value) {
    if (value <= 240) {
      Write((byte)value);
      return;
    }
    if (value <= 2287) {
      Write((byte)((value - 240) / 256 + 241));
      Write((byte)((value - 240) % 256));
      return;
    }
    if (value <= 67823) {
      Write((byte)249);
      Write((byte)((value - 2288) / 256));
      Write((byte)((value - 2288) % 256));
      return;
    }
    if (value <= 16777215) {
      Write((byte)250);
      Write((byte)(value & 0xFF));
      Write((byte)((value >> 8) & 0xFF));
      Write((byte)((value >> 16) & 0xFF));
      return;
    }

    // all other values of uint
    Write((byte)251);
    Write((byte)(value & 0xFF));
    Write((byte)((value >> 8) & 0xFF));
    Write((byte)((value >> 16) & 0xFF));
    Write((byte)((value >> 24) & 0xFF));
  }

  public void WritePackedUInt64(UInt64 value) {
    if (value <= 240) {
      Write((byte)value);
    } else if (value <= 2287) {
      Write((byte)((value - 240) / 256 + 241));
      Write((byte)((value - 240) % 256));
    } else if (value <= 67823) {
      Write((byte)249);
      Write((byte)((value - 2288) / 256));
      Write((byte)((value - 2288) % 256));
    } else if (value <= 16777215) {
      Write((byte)250);
      Write((byte)(value & 0xFF));
      Write((byte)((value >> 8) & 0xFF));
      Write((byte)((value >> 16) & 0xFF));
    } else if (value <= 4294967295) {
      Write((byte)251);
      Write((byte)(value & 0xFF));
      Write((byte)((value >> 8) & 0xFF));
      Write((byte)((value >> 16) & 0xFF));
      Write((byte)((value >> 24) & 0xFF));
    } else if (value <= 1099511627775) {
      Write((byte)252);
      Write((byte)(value & 0xFF));
      Write((byte)((value >> 8) & 0xFF));
      Write((byte)((value >> 16) & 0xFF));
      Write((byte)((value >> 24) & 0xFF));
      Write((byte)((value >> 32) & 0xFF));
    } else if (value <= 281474976710655) {
      Write((byte)253);
      Write((byte)(value & 0xFF));
      Write((byte)((value >> 8) & 0xFF));
      Write((byte)((value >> 16) & 0xFF));
      Write((byte)((value >> 24) & 0xFF));
      Write((byte)((value >> 32) & 0xFF));
      Write((byte)((value >> 40) & 0xFF));
    } else if (value <= 72057594037927935) {
      Write((byte)254);
      Write((byte)(value & 0xFF));
      Write((byte)((value >> 8) & 0xFF));
      Write((byte)((value >> 16) & 0xFF));
      Write((byte)((value >> 24) & 0xFF));
      Write((byte)((value >> 32) & 0xFF));
      Write((byte)((value >> 40) & 0xFF));
      Write((byte)((value >> 48) & 0xFF));
    } else {
      Write((byte)255);
      Write((byte)(value & 0xFF));
      Write((byte)((value >> 8) & 0xFF));
      Write((byte)((value >> 16) & 0xFF));
      Write((byte)((value >> 24) & 0xFF));
      Write((byte)((value >> 32) & 0xFF));
      Write((byte)((value >> 40) & 0xFF));
      Write((byte)((value >> 48) & 0xFF));
      Write((byte)((value >> 56) & 0xFF));
    }
  }

  public void Write(char value) => writeBuffer.WriteByte((byte)value);
  public void Write(byte value) => writeBuffer.WriteByte(value);
  public void Write(sbyte value) => writeBuffer.WriteByte((byte)value);
  public void Write(short value) {
    writeBuffer.WriteByte2((byte)(value & 0xff), (byte)((value >> 8) & 0xff));
  }

  public void Write(ushort value) {
    writeBuffer.WriteByte2((byte)(value & 0xff), (byte)((value >> 8) & 0xff));
  }

  public void Write(int value) {
    // little endian...
    writeBuffer.WriteByte4(
        (byte)(value & 0xff),
        (byte)((value >> 8) & 0xff),
        (byte)((value >> 16) & 0xff),
        (byte)((value >> 24) & 0xff));
  }

  public void Write(uint value) {
    writeBuffer.WriteByte4(
        (byte)(value & 0xff),
        (byte)((value >> 8) & 0xff),
        (byte)((value >> 16) & 0xff),
        (byte)((value >> 24) & 0xff));
  }

  public void Write(long value) {
    writeBuffer.WriteByte8(
        (byte)(value & 0xff),
        (byte)((value >> 8) & 0xff),
        (byte)((value >> 16) & 0xff),
        (byte)((value >> 24) & 0xff),
        (byte)((value >> 32) & 0xff),
        (byte)((value >> 40) & 0xff),
        (byte)((value >> 48) & 0xff),
        (byte)((value >> 56) & 0xff));
  }

  public void Write(ulong value) {
    writeBuffer.WriteByte8(
        (byte)(value & 0xff),
        (byte)((value >> 8) & 0xff),
        (byte)((value >> 16) & 0xff),
        (byte)((value >> 24) & 0xff),
        (byte)((value >> 32) & 0xff),
        (byte)((value >> 40) & 0xff),
        (byte)((value >> 48) & 0xff),
        (byte)((value >> 56) & 0xff));
  }

#if !INCLUDE_IL2CPP
  static UIntFloat s_FloatConverter;
#endif

  public void Write(float value) {
#if INCLUDE_IL2CPP
    Write(BitConverter.ToUInt32(BitConverter.GetBytes(value), 0));
#else
    s_FloatConverter.floatValue = value;
    Write(s_FloatConverter.intValue);
#endif
  }

  public void Write(double value) {
#if INCLUDE_IL2CPP
    Write(BitConverter.ToUInt64(BitConverter.GetBytes(value), 0));
#else
    s_FloatConverter.doubleValue = value;
    Write(s_FloatConverter.longValue);
#endif
  }

  public void Write(string value) {
    if (value == null) {
      writeBuffer.WriteByte2(0, 0);
      return;
    }

    int len = s_Encoding.GetByteCount(value);

    if (len >= k_MaxStringLength) {
      throw new IndexOutOfRangeException("Serialize(string) too long: " + value.Length);
    }

    Write((ushort)(len));
    int numBytes = s_Encoding.GetBytes(value, 0, value.Length, s_StringWriteBuffer, 0);
    writeBuffer.WriteBytes(s_StringWriteBuffer, (ushort)numBytes);
  }

  public void Write(bool value) => writeBuffer.WriteByte((byte)(value ? 1 : 0));

  public void Write(byte[] buffer, int count) {
    if (count > UInt16.MaxValue) {
      Debug.LogError("NetworkWriter Write: buffer is too large (" + count + ") bytes. The maximum buffer size is 64K bytes.");
      return;
    }
    writeBuffer.WriteBytes(buffer, (UInt16)count);
  }

  public void Write(byte[] buffer, int offset, int count) {
    if (count > UInt16.MaxValue) {
      Debug.LogError("NetworkWriter Write: buffer is too large (" + count + ") bytes. The maximum buffer size is 64K bytes.");
      return;
    }
    this.writeBuffer.WriteBytesAtOffset(buffer, (ushort)offset, (ushort)count);
  }

  public void WriteBytesAndSize(byte[] buffer, int count) {
    if (buffer == null || count == 0) {
      Write((UInt16)0);
      return;
    }

    if (count > UInt16.MaxValue) {
      Debug.LogError("NetworkWriter WriteBytesAndSize: buffer is too large (" + count + ") bytes. The maximum buffer size is 64K bytes.");
      return;
    }

    Write((UInt16)count);
    this.writeBuffer.WriteBytes(buffer, (UInt16)count);
  }

  //NOTE: this will write the entire buffer.. including trailing empty space!
  public void WriteBytesFull(byte[] buffer) {
    if (buffer == null) {
      Write((UInt16)0);
      return;
    }
    if (buffer.Length > UInt16.MaxValue) {
      Debug.LogError("NetworkWriter WriteBytes: buffer is too large (" + buffer.Length + ") bytes. The maximum buffer size is 64K bytes.");
      return;
    }
    Write((UInt16)buffer.Length);
    this.writeBuffer.WriteBytes(buffer, (UInt16)buffer.Length);
  }

  public void Write(Vector2 value) {
    Write(value.x);
    Write(value.y);
  }

  public void Write(Vector3 value) {
    Write(value.x);
    Write(value.y);
    Write(value.z);
  }

  public void Write(Vector4 value) {
    Write(value.x);
    Write(value.y);
    Write(value.z);
    Write(value.w);
  }

  public void Write(Color value) {
    Write(value.r);
    Write(value.g);
    Write(value.b);
    Write(value.a);
  }

  public void Write(Color32 value) {
    Write(value.r);
    Write(value.g);
    Write(value.b);
    Write(value.a);
  }

  public void Write(Quaternion value) {
    Write(value.x);
    Write(value.y);
    Write(value.z);
    Write(value.w);
  }

  public void Write(Rect value) {
    Write(value.xMin);
    Write(value.yMin);
    Write(value.width);
    Write(value.height);
  }

  public void Write(Plane value) {
    Write(value.normal);
    Write(value.distance);
  }

  public void Write(Ray value) {
    Write(value.direction);
    Write(value.origin);
  }

  public void Write(Matrix4x4 value) {
    Write(value.m00);
    Write(value.m01);
    Write(value.m02);
    Write(value.m03);
    Write(value.m10);
    Write(value.m11);
    Write(value.m12);
    Write(value.m13);
    Write(value.m20);
    Write(value.m21);
    Write(value.m22);
    Write(value.m23);
    Write(value.m30);
    Write(value.m31);
    Write(value.m32);
    Write(value.m33);
  }

  public void Write(INetworkSerializable msg) => msg.Serialize(this);

  public void SeekZero() => writeBuffer.SeekZero();

  public void StartMessage(short msgType) {
    SeekZero();

    // two bytes for size, will be filled out in FinishMessage
    writeBuffer.WriteByte2(0, 0);

    // two bytes for message type
    Write(msgType);
  }

  public void FinishMessage()
  {
      // writes correct size into space at start of buffer
      writeBuffer.FinishMessage();
  }
};

}