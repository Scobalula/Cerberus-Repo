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
// File: ZStandard.cpp
// Author: Philip/Scobalula
// Description: A basic wrapper around ZStandard
#pragma once

using namespace System;
using namespace System::IO;
#pragma warning(disable : 4635) // XML document comment applied to....

namespace PhilLibX
{
	namespace Compression
	{
		/// <summary>
		/// ZStandard Interop Logic
		/// </summary>
		public ref class ZStandard abstract sealed
		{
		public:
			/// <summary>
			/// Decompresses an array of bytes of decompressed data
			/// </summary>
			/// <param name="compressedData">Byte array of compressed data</param>
			static array<Byte>^ Decompress(array<Byte>^ compressedData);

			/// <summary>
			/// Compresses an array of bytes of data
			/// </summary>
			/// <param name="inputData">Byte array of data</param>
			/// <param name="compressionLevel">Compression Level between 1 and 22</param>
			static array<Byte>^ Compress(array<Byte>^ inputData, int compressionLevel);
		};
	}
}