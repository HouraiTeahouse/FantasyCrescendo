using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {
    
public static class BitUtil {

  public static bool GetBit(byte value, int bit) => (value & (1 << bit)) != 0;
  public static void SetBit(ref byte value, int bit, bool bitValue) {
    var mask = 1 << bit;
    value = (byte)(bitValue ? (value | mask) : (value & ~mask));
  }

  public static byte AllBits(int max) => (byte)((1 << max) - 1);

  public static int GetBitCount(byte val) {
    int x = val;
    x = (x & 0x55) + (x >> 1 & 0x55);
    x = (x & 0x33) + (x >> 2 & 0x33);
    x = (x & 0x0f) + (x >> 4 & 0x0f);
    return x;
  }

}

}