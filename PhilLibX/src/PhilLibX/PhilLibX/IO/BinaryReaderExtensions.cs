// ------------------------------------------------------------------------
// PhilLibX - My Utility Library
// Copyright(c) 2018 Philip/Scobalula
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// ------------------------------------------------------------------------
// File: IO/BinaryReaderExtensions.cs
// Author: Philip/Scobalula
// Description: BinaryReader extensions for reading null terminated strings, scanning files, etc.
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace PhilLibX.IO
{
    /// <summary>
    /// IO Utilities/Extensions
    /// </summary>
    public static class BinaryReaderExtensions
    {
        /// <summary>
        /// Reads a string terminated by a null byte
        /// </summary>
        /// <returns>Read String</returns>
        public static string ReadNullTerminatedString(this BinaryReader br, int maxSize = -1)
        {
            // Create String Builder
            StringBuilder str = new StringBuilder();
            // Current Byte Read
            int byteRead;
            // Size of String
            int size = 0;
            // Loop Until we hit terminating null character
            while ((byteRead = br.BaseStream.ReadByte()) != 0x0 && size++ != maxSize)
                str.Append(Convert.ToChar(byteRead));
            // Ship back Result
            return str.ToString();
        }

        /// <summary>
        /// Reads a string terminated by a null byte and returns the reader to the original position
        /// </summary>
        /// <returns>Read String</returns>
        public static string PeekNullTerminatedString(this BinaryReader br, long offset, int maxSize = -1)
        {
            // Create String Builder
            StringBuilder str = new StringBuilder();
            // Seek to position
            var temp = br.BaseStream.Position;
            br.BaseStream.Position = offset;
            // Current Byte Read
            int byteRead;
            // Size of String
            int size = 0;
            // Loop Until we hit terminating null character
            while ((byteRead = br.BaseStream.ReadByte()) != 0x0 && size++ != maxSize)
                str.Append(Convert.ToChar(byteRead));
            // Go back
            br.BaseStream.Position = temp;
            // Ship back Result
            return str.ToString();
        }

        /// <summary>
        /// Reads a UTF16 string terminated by a null byte
        /// </summary>
        /// <returns>Read String</returns>
        public static string ReadUTF16NullTerminatedString(this BinaryReader br, int maxSize = -1)
        {
            // Create String Builder
            StringBuilder str = new StringBuilder();
            // Current Byte Read
            ushort byteRead;
            // Size of String
            int size = 0;
            // Loop Until we hit terminating null character
            while ((byteRead = br.ReadUInt16()) != 0x0 && size++ != maxSize)
                str.Append(Convert.ToChar(byteRead));
            // Ship back Result
            return str.ToString();
        }

        /// <summary>
        /// Reads a string of fixed size
        /// </summary>
        /// <param name="br">Reader</param>
        /// <param name="numBytes">Size of string in bytes</param>
        /// <returns>Read String</returns>
        public static string ReadFixedString(this BinaryReader br, int numBytes)
        {
            // Purge Null Bytes and Return 
            return Encoding.ASCII.GetString(br.ReadBytes(numBytes)).TrimEnd('\0');
        }

        /// <summary>
        /// Reads an array of the given type
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="br">Reader</param>
        /// <param name="count">Number of items</param>
        /// <returns>Resulting array</returns>
        public static T[] ReadArray<T>(this BinaryReader br, int count)
        {
            // Get Byte Count
            var size = count * Marshal.SizeOf<T>();
            // Allocate Array
            var result = new T[count];
            // Check for primitives, we can use BlockCopy for them
            if (typeof(T).IsPrimitive)
            {
                // Copy
                Buffer.BlockCopy(br.ReadBytes(size), 0, result, 0, size);
            }
            // Slightly more complex structures, we can use the struct functs
            else
            {
                // Loop through
                for (int i = 0; i < count; i++)
                {
                    // Read it into result
                    result[i] = br.ReadStruct<T>();
                }
            }
            // Done
            return result;
        }

        /// <summary>
        /// Reads the given structure from the reader
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="br"></param>
        /// <returns></returns>
        public static T ReadStruct<T>(this BinaryReader br)
        {
            byte[] data = br.ReadBytes(Marshal.SizeOf<T>());
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            T theStructure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return theStructure;
        }

        /// <summary>
        /// Sets the position of the Base Stream
        /// </summary>
        /// <param name="br"></param>
        /// <param name="offset">Offset to seek to.</param>
        /// <param name="seekOrigin">Seek Origin</param>
        public static void Seek(this BinaryReader br, long offset, SeekOrigin seekOrigin)
        {
            // Set stream position
            br.BaseStream.Seek(offset, seekOrigin);
        }

        /// <summary>
        /// Finds occurences of a string in the stream
        /// </summary>
        /// <param name="br">Reader to use for scanning</param>
        /// <param name="needle">String Needle to search for</param>
        /// <param name="firstOccurence">Stops at first result</param>
        /// <returns>Resulting offsets</returns>
        public static long[] FindString(this BinaryReader br, string needle, bool firstOccurence = false)
        {
            // Convert to bytes and scan
            return br.FindBytes(Encoding.ASCII.GetBytes(needle), firstOccurence);
        }

        /// <summary>
        /// Reads a 4-byte big endian signed integer from the current stream and advances the current position of the stream by four bytes.
        /// </summary>
        /// <param name="br">Reader</param>
        /// <returns>Resulting 32Bit Integer</returns>
        public static int ReadBEInt32(this BinaryReader br)
        {
            // Read bytes
            byte[] buffer = br.ReadBytes(4);
            // Return resulting 4 byte int
            return (buffer[0] << 24) | (buffer[1] << 16) | (buffer[2] << 8) | buffer[3];
        }

        /// <summary>
        /// Reads a 3-byte big endian signed integer from the current stream and advances the current position of the stream by three bytes.
        /// </summary>
        /// <param name="br">Reader</param>
        /// <returns>Resulting 32Bit Integer</returns>
        public static int ReadBEInt24(this BinaryReader br)
        {
            // Read bytes
            byte[] buffer = br.ReadBytes(3);
            // Return resulting 3 byte int
            return (buffer[0] << 16) | (buffer[1] << 8) | (buffer[2]);
        }

        /// <summary>
        /// Reads a variable length integer from the current stream 
        /// </summary>
        /// <param name="br">Reader</param>
        /// <returns>Resulting 32Bit Integer</returns>
        public static int Read7BitEncodedInt(this BinaryReader br)
        {
            int result = 0;
            int shift = 0;

            // Loop until the high bit is 0
            while (true)
            {
                byte value = br.ReadByte();
                result |= (value & 0x7F) << shift;

                if ((value & 0x80) == 0) return result;

                shift += 7;
            }
        }

        /// <summary>
        /// Finds occurences of bytes
        /// </summary>
        /// <param name="br">Reader</param>
        /// <param name="needle">Byte Array Needle to search for</param>
        /// <param name="firstOccurence">Stops at first result</param>
        /// <returns>Resulting offsets</returns>
        public static long[] FindBytes(this BinaryReader br, byte[] needle, bool firstOccurence = false)
        {
            // List of offsets in file.
            List<long> offsets = new List<long>();

            // Buffer
            byte[] buffer = new byte[1048576];

            // Bytes Read
            int bytesRead = 0;

            // Starting Offset
            long readBegin = br.BaseStream.Position;

            // Needle Index
            int needleIndex = 0;

            // Byte Array Index
            int bufferIndex = 0;

            // Read chunk of file
            while ((bytesRead = br.BaseStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                // Loop through byte array
                for (bufferIndex = 0; bufferIndex < bytesRead; bufferIndex++)
                {
                    // Check if current bytes match
                    if (needle[needleIndex] == buffer[bufferIndex])
                    {
                        // Increment
                        needleIndex++;

                        // Check if we have a match
                        if (needleIndex == needle.Length)
                        {
                            // Add Offset
                            offsets.Add(readBegin + bufferIndex + 1 - needle.Length);

                            // Reset Index
                            needleIndex = 0;

                            // If only first occurence, end search
                            if (firstOccurence)
                                return offsets.ToArray();
                        }
                    }
                    else
                    {
                        // Reset Index
                        needleIndex = 0;
                    }
                }
                // Set next offset
                readBegin += bytesRead;
            }
            // Return offsets as an array
            return offsets.ToArray();
        }
    }
}
