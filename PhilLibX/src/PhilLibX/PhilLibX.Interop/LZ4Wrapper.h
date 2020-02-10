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
// File: LZ4Wrapper.h
// Author: Philip/Scobalula
// Description: A basic wrapper around LZ4
#pragma once

using namespace System;
using namespace System::IO;
#pragma warning(disable : 4635) // XML document comment applied to....

namespace PhilLibX
{
	namespace Compression
	{
		/// <summary>
		/// LZ4 Interop Logic
		/// </summary>
		public ref class LZ4 abstract sealed
		{
		public:
			/// <summary>
			/// Decompresses an array of bytes of decompressed data with a known decompressed size
			/// </summary>
			/// <param name="compressedData">Byte array of compressed data</param>
			/// <param name="decompressedSize">Decompressed Size</param>
			static array<Byte>^ Decompress(array<System::Byte>^ compressedData, int decompressedSize);

			/// <summary>
			/// Compresses an array of bytes of data
			/// </summary>
			/// <param name="inputData">Byte array of data</param>
			/// <param name="compressionLevel">Compression Level</param>
			static array<Byte>^ Compress(array<Byte>^ inputData, Byte compressionLevel);
		};
	}
}