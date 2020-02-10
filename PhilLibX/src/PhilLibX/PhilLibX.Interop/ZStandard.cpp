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
#include "stdafx.h"
#include "ZStandard.h"
#include "CompressionException.h"
#include "zstd.h"

using namespace System;
using namespace System::IO;
using namespace System::Runtime::InteropServices;
using namespace PhilLibX::Compression;

array<Byte>^ PhilLibX::Compression::ZStandard::Decompress(array<Byte>^ compressedData)
{
	// Unmanaged Buffer
	std::unique_ptr<int8_t[]> buffer(new int8_t[compressedData->Length]);
	// Copy it
	Marshal::Copy(compressedData, 0, IntPtr(buffer.get()), compressedData->Length);

	// Resolve Size
	auto result = ZSTD_findDecompressedSize(buffer.get(), compressedData->Length);

	// Check for error
	if (result == ZSTD_CONTENTSIZE_ERROR)
		throw gcnew CompressionException("Failed to compute content size, this data was not compressed by ZStandard.");
	// Check for unknown content (we don't support streaming at the moment)
	if (result == ZSTD_CONTENTSIZE_UNKNOWN)
		throw gcnew NotImplementedException("Streaming Decompression is not supported at this time.");

	// Allocate result
	std::unique_ptr<int8_t[]> bufferResult(new int8_t[(size_t)result]);
	// Decompress it
	size_t const decompressedSize = ZSTD_decompress(bufferResult.get(), (size_t)result, buffer.get(), compressedData->Length);

	// Check size
	if (decompressedSize != result)
		throw gcnew CompressionException(String::Format("Failed to decompress data: {0}", gcnew String(ZSTD_getErrorName(decompressedSize))));

	// Result
	auto resultingArray = gcnew array<Byte>((int)result);
	// Copy it to our managed array
	Marshal::Copy(IntPtr(bufferResult.get()), resultingArray, 0, (int)result);

	// Done
	return resultingArray;
}

array<Byte>^ ZStandard::Compress(array<Byte>^ inputData, int compressionLevel)
{
	// Unmanaged Buffer
	std::unique_ptr<int8_t[]> buffer(new int8_t[inputData->Length]);
	// Copy it
	Marshal::Copy(inputData, 0, IntPtr(buffer.get()), inputData->Length);

	// Calculate output size
	auto const result = ZSTD_compressBound((size_t)inputData->Length);

	// Allocate result and compress
	std::unique_ptr<int8_t[]> bufferResult(new int8_t[(size_t)result]);
	size_t const sizeResult = ZSTD_compress(bufferResult.get(), result, buffer.get(), (size_t)inputData->Length, (size_t)compressionLevel);

	// Check for errors
	if (ZSTD_isError(sizeResult))
		throw gcnew CompressionException(String::Format("Failed to compress data: {0}", gcnew String(ZSTD_getErrorName(sizeResult))));

	// Result 
	auto resultingArray = gcnew array<Byte>((int)sizeResult);
	Marshal::Copy(IntPtr(bufferResult.get()), resultingArray, 0, (int)sizeResult);

	// Done
	return resultingArray;
}