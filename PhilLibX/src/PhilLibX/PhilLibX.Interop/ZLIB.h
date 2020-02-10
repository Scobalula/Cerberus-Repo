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
// File: ZLIB.h
// Author: Philip/Scobalula
// Description: A basic wrapper around MiniZ Zlib
#pragma once

using namespace System::IO;
#pragma warning(disable : 4635) // XML document comment applied to....

namespace PhilLibX
{
	namespace Compression
	{
		/// <summary>
		/// ZLIB Interop Logic
		/// </summary>
		public ref class ZLIB abstract sealed
		{
		public:
			/// <summary>
			/// Decompresses an array of bytes of compressed data
			/// </summary>
			/// <param name="inputData">Input Data</param>
			static array<System::Byte>^ Decompress(array<System::Byte>^ inputData);

			/// <summary>
			/// Decompresses a zlib stream to the output stream
			/// </summary>
			/// <param name="inputStream">Input Stream</param>
			/// <param name="outputStream">Output Stream</param>
			static void Decompress(Stream^ inputStream, Stream^ outputStream);

			/// <summary>
			/// Compresses an array of bytes of data
			/// </summary>
			/// <param name="inputData">Byte array of data</param>
			/// <param name="compressionLevel">Compression Level</param>
			static array<System::Byte>^ Compress(array<System::Byte>^ inputData, int compressionLevel);

			/// <summary>
			/// Compresses a stream to the output stream
			/// </summary>
			/// <param name="inputStream">Input Stream</param>
			/// <param name="outputStream">Output Stream</param>
			/// <param name="compressionLevel">Compression Level</param>
			static void Compress(Stream^ inputStream, Stream^ outputStream, int compressionLevel);
		};
	}
}