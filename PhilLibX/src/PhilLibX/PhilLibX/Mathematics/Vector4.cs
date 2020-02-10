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
// File: Mathematics/Vectors/Vector4.cs
// Author: Philip/Scobalula
// Description: A class to hold a 4-Dimensional Vector
using System;

namespace PhilLibX.Mathematics
{
    /// <summary>
    /// A class to hold a 4-Dimensional Vector
    /// </summary>
    public struct Vector4
    {
        /// <summary>
        /// X Value
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y Value
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Z Value
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        /// W Value
        /// </summary>
        public double W { get; set; }

        /// <summary>
        /// Initializes a 4-Dimensional Vector with the given values
        /// </summary>
        /// <param name="x">X Value</param>
        /// <param name="y">Y Value</param>
        /// <param name="z">Z Value</param>
        /// <param name="w">W Value</param>
        public Vector4(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Returns the dot product of the 2 vectors
        /// </summary>
        /// <param name="input">Input vector</param>
        /// <returns>Single result</returns>
        public double DotProduct(Vector4 input)
        {
            return (X * input.X) + (Y * input.Y) + (Z * input.Z) + (W * input.W);
        }

        /// <summary>
        /// Subtracts two given vectors
        /// </summary>
        public static Vector4 operator -(Vector4 a, Vector4 b)
        {
            return new Vector4(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
        }

        /// <summary>
        /// Subtracts the given value from the vector
        /// </summary>
        public static Vector4 operator -(Vector4 a, double value)
        {
            return new Vector4(a.X - value, a.Y - value, a.Z - value, a.W - value);
        }

        /// <summary>
        /// Adds two given vectors
        /// </summary>
        public static Vector4 operator +(Vector4 a, Vector4 b)
        {
            return new Vector4(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        }

        /// <summary>
        /// Adds the given value to the vector
        /// </summary>
        public static Vector4 operator +(Vector4 a, double value)
        {
            return new Vector4(a.X + value, a.Y + value, a.Z + value, a.W + value);
        }

        /// <summary>
        /// Multiplies two given vectors
        /// </summary>
        public static Vector4 operator *(Vector4 a, Vector4 b)
        {
            return new Vector4(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W);
        }

        /// <summary>
        /// Multiplies the vector by the given value
        /// </summary>
        public static Vector4 operator *(Vector4 a, double value)
        {
            return new Vector4(a.X * value, a.Y * value, a.Z * value, a.W * value);
        }

        /// <summary>
        /// Divides two given vectors
        /// </summary>
        public static Vector4 operator /(Vector4 a, Vector4 b)
        {
            return new Vector4(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W);
        }

        /// <summary>
        /// Divides the vector by the given value
        /// </summary>
        public static Vector4 operator /(Vector4 a, double value)
        {
            return new Vector4(a.X / value, a.Y / value, a.Z / value, a.W / value);
        }

        /// <summary>
        /// Normalizes the Vector
        /// </summary>
        /// <returns>Normalized Vector</returns>
        public Vector4 Normalize()
        {
            double length = Math.Sqrt(DotProduct(this));
            return new Vector4(X / length, Y / length, Z / length, W / length);
        }

        /// <summary>
        /// Gets a string representation of the vector
        /// </summary>
        /// <returns>String representation of the vector</returns>
        public override string ToString()
        {
            return string.Format(" {0}, {1}, {2}, {3}", X, Y, Z, W);
        }
    }
}
