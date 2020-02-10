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
// File: Cryptograhpy/Hash/MurMur3.cs
// Author: Philip/Scobalula
// Description: Class to handle calculating MurMur3 Hash
using System.IO;
using System.Text;

namespace PhilLibX.Cryptography.Hash
{
    /// <summary>
    /// Class to handle calculating SDBM Hash
    /// </summary>
    public static class MurMur3
    {
        #region HelperFunctions
        private static uint Hash(Stream stream, uint seed)
        {
            const uint c1 = 0xcc9e2d51;
            const uint c2 = 0x1b873593;

            uint h1 = seed;
            uint k1 = 0;
            uint streamLength = 0;

            using (BinaryReader reader = new BinaryReader(stream))
            {
                byte[] chunk = reader.ReadBytes(4);
                while (chunk.Length > 0)
                {
                    streamLength += (uint)chunk.Length;
                    switch (chunk.Length)
                    {
                        case 4:
                            /* Get four bytes from the input into an uint */
                            k1 = (uint)
                               (chunk[0]
                              | chunk[1] << 8
                              | chunk[2] << 16
                              | chunk[3] << 24);

                            /* bitmagic hash */
                            k1 *= c1;
                            k1 = rotl32(k1, 15);
                            k1 *= c2;

                            h1 ^= k1;
                            h1 = rotl32(h1, 13);
                            h1 = h1 * 5 + 0xe6546b64;
                            break;
                        case 3:
                            k1 = (uint)
                               (chunk[0]
                              | chunk[1] << 8
                              | chunk[2] << 16);
                            k1 *= c1;
                            k1 = rotl32(k1, 15);
                            k1 *= c2;
                            h1 ^= k1;
                            break;
                        case 2:
                            k1 = (uint)
                               (chunk[0]
                              | chunk[1] << 8);
                            k1 *= c1;
                            k1 = rotl32(k1, 15);
                            k1 *= c2;
                            h1 ^= k1;
                            break;
                        case 1:
                            k1 = (uint)(chunk[0]);
                            k1 *= c1;
                            k1 = rotl32(k1, 15);
                            k1 *= c2;
                            h1 ^= k1;
                            break;

                    }
                    chunk = reader.ReadBytes(4);
                }
            }

            // finalization, magic chants to wrap it all up
            h1 ^= streamLength;
            h1 = fmix(h1);

            return h1;
        }

        private static uint rotl32(uint x, byte r)
        {
            return (x << r) | (x >> (32 - r));
        }

        private static uint fmix(uint h)
        {
            h ^= h >> 16;
            h *= 0x85ebca6b;
            h ^= h >> 13;
            h *= 0xc2b2ae35;
            h ^= h >> 16;
            return h;
        }
        #endregion

        /// <summary>
        /// Calculates MurMur3 Hash for the given string.
        /// </summary>
        /// <param name="value">String to generate a hash from</param>
        /// <param name="seed">Initial Hash Value (0)</param>
        /// <returns>Resulting Unsigned Hash Value</returns>
        public static uint Calculate(string value, uint seed = 0xFFFFFFFF)
        {
            return Calculate(Encoding.ASCII.GetBytes(value), seed);
        }

        /// <summary>
        /// Calculates MurMur3 Hash for a sequence of bytes.
        /// </summary>
        /// <param name="value">Bytes to generate a hash from</param>
        /// <param name="seed">Initial Hash Value (Defaults to 0))</param>
        /// <returns>Resulting Unsigned Hash Value</returns>
        public static uint Calculate(byte[] value, uint seed = 0xFFFFFFFF)
        {
            // Return
            return Hash(new MemoryStream(value), seed);
        }
    }
}
