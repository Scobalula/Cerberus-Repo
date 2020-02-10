using System;
using System.IO;
using System.Text;

namespace Cerberus.Logic
{
    /// <summary>
    /// Utility Functions
    /// </summary>
    internal static class Utility
    {
        /// <summary>
        /// Computes the number of bytes require to pad this value
        /// </summary>
        public static int ComputePadding(int value, int alignment) => (((value) + ((alignment) - 1)) & ~((alignment) - 1)) - value;

        /// <summary>
        /// Aligns the value to the given alignment
        /// </summary>
        public static int AlignValue(int value, int alignment) => (((value) + ((alignment) - 1)) & ~((alignment) - 1));

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
        /// Counts the number of lines in the given string
        /// </summary>
        public static int GetLineCount(string str)
        {
            int index = -1;
            int count = 0;
            while ((index = str.IndexOf(Environment.NewLine, index + 1)) != -1)
                count++;
            return count + 1;
        }

        public static string SanitiseString(string value) => value.Replace("/", "\\").Replace("\b", "\\b");
    }
}
