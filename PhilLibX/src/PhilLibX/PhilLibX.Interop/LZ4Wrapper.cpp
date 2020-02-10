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
// File: LZ4Wrapper.cpp
// Author: Philip/Scobalula
// Description: A basic wrapper around LZ4
#include "stdafx.h"
#include "LZ4Wrapper.h"
#include "lz4.h"
#include "CompressionException.h"
#include "zstd.h"

using namespace System;
using namespace System::IO;
using namespace System::Runtime::InteropServices;
using namespace PhilLibX::Compression;

array<Byte>^ LZ4::Decompress(array<Byte>^ compressedData, int decompressedSize)
{
	// Unmanaged Buffer
	std::unique_ptr<char[]> buffer(new char[compressedData->Length]);
	// Copy it
	Marshal::Copy(compressedData, 0, IntPtr(buffer.get()), compressedData->Length);

	// Allocate result
	std::unique_ptr<char[]> bufferResult(new char[(size_t)decompressedSize]);
	// Decompress it
	auto const result = LZ4_decompress_safe(buffer.get(), bufferResult.get(), compressedData->Length, decompressedSize);

	// Check for errors
	if (result < 0)
		throw gcnew CompressionException(String::Format("Failed to decompress data: {0}", result));

	// Result 
	auto resultingArray = gcnew array<Byte>((int)result);
	Marshal::Copy(IntPtr(bufferResult.get()), resultingArray, 0, (int)result);

	// Done
	return resultingArray;
}

array<Byte>^ LZ4::Compress(array<Byte>^ inputData, Byte compressionLevel)
{
	// Unmanaged Buffer
	std::unique_ptr<char[]> buffer(new char[inputData->Length]);
	Marshal::Copy(inputData, 0, IntPtr(buffer.get()), inputData->Length);

	// Calculate output size
	auto const result = LZ4_compressBound((size_t)inputData->Length);
	// Allocate result and compress
	std::unique_ptr<char[]> bufferResult(new char[(size_t)result]);
	size_t const sizeResult = LZ4_compress_fast(buffer.get(), bufferResult.get(), (size_t)inputData->Length, result, compressionLevel);

	// Check for errors
	if (sizeResult < 0)
		throw gcnew CompressionException(String::Format("Failed to compress data: {0}", sizeResult));

	// Result 
	auto resultingArray = gcnew array<Byte>((int)sizeResult);
	Marshal::Copy(IntPtr(bufferResult.get()), resultingArray, 0, (int)sizeResult);

	// Done
	return resultingArray;
}