using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

/// <summary>
/// A CRC32 implementation optimized for speed and GC.
/// </summary>
public unsafe static class Crc32 {

    static uint* table;

    static Crc32() {
        const uint poly = 0xedb88320;
        uint temp = 0;
        table = (uint*)UnsafeUtility.Malloc(byte.MaxValue * sizeof(uint),
                                            UnsafeUtility.AlignOf<uint>(),
                                            Allocator.Persistent);
        for(uint i = 0; i < byte.MaxValue; ++i) {
            temp = i;
            for(int j = 8; j > 0; --j) {
                if((temp & 1) == 1) {
                    temp = (uint)((temp >> 1) ^ poly);
                } else {
                    temp >>= 1;
                }
            }
            table[i] = temp;
        }
    }

    /// <summary>
    /// Computes the CRC32 checksum from a buffer.
    /// </summary>
    /// <param name="buffer">the start of the buffer.</param>
    /// <param name="len">the length of the buffer</param>
    /// <returns>the CRC32 checksum.</returns>
    /// <exception cref="System.ArgumentNullException">thrown if buffer is null.</exception>
    public static uint ComputeChecksum(byte* buffer, int len) {
        if (buffer == null) {
            throw new ArgumentNullException(nameof(buffer));
        }
        uint crc = 0xffffffff;
        for(int i = 0; i < len; ++i) {
            byte index = (byte)((crc & 0xff) ^ buffer[i]);
            crc = (uint)((crc >> 8) ^ table[index]);
        }
        return ~crc;
    }

    /// <summary>
    /// Computes the CRC32 checksum from a buffer.
    /// </summary>
    /// <param name="buffer">the start of the buffer.</param>
    /// <param name="len">the length of the buffer, uses the length of the array if negative.</param>
    /// <returns>the CRC32 checksum</returns>
    /// <exception cref="System.ArgumentNullException">thrown if buffer is null.</exception>
    public static uint ComputeChecksum(byte[] buffer, int len = -1) {
        if (buffer == null) {
            throw new ArgumentNullException(nameof(buffer));
        }
        if (len < 0 )  len = buffer.Length;
        fixed (byte* bufPtr = buffer) {
            return ComputeChecksum(bufPtr, len);
        }
    }

}

}