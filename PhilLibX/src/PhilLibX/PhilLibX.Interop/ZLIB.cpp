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
// File: ZLIB.cpp
// Author: Philip/Scobalula
// Description: A basic wrapper around MiniZ Zlib
#include "stdafx.h"
#include "ZLIB.h"
#include "miniz.h"
#include "CompressionException.h"
#include "zstd.h"

using namespace System::IO;
using namespace System::Runtime::InteropServices;
using namespace PhilLibX::Compression;

array<System::Byte>^ ZLIB::Decompress(array<System::Byte>^ inputData)
{
	// Create streams
	auto inpStream = gcnew MemoryStream(inputData);
	auto outStream = gcnew MemoryStream();
	// Decompress
	Decompress(inpStream, outStream);
	// Return result
	return outStream->ToArray();
}

void ZLIB::Decompress(Stream^ inputStream, Stream^ outputStream)
{
	// Buffers
	std::unique_ptr<unsigned char[]> inpBuffer(new unsigned char[0x100000]);
	std::unique_ptr<unsigned char[]> outBuffer(new unsigned char[0x100000]);
	auto inpStreamBuffer = gcnew array<System::Byte>(0x100000);
	auto outStreamBuffer = gcnew array<System::Byte>(0x100000);

	// Init stream
	z_stream stream;
	memset(&stream, 0, sizeof(stream));
	// Init Values
	stream.next_in = inpBuffer.get();
	stream.avail_in = 0;
	stream.next_out = outBuffer.get();
	stream.avail_out = 0x100000;
	int status;

	// Attempt to init deflate
	if (inflateInit(&stream) != Z_OK)
		throw gcnew CompressionException(String::Format("Failed to init inflate stream"));

	// Wrap in a try/catch as we need to clean up even if exceptions are thrown
	try
	{
		// Loop while we can
		while (true)
		{
			// Check do we need to read from the buffer
			if (stream.avail_in <= 0)
			{
				// Read from stream
				int result = inputStream->Read(inpStreamBuffer, 0, 0x100000);
				// Check result
				if (result <= 0)
					throw gcnew CompressionException(String::Format("Unexpected end of stream"));
				// Copy it
				Marshal::Copy(inpStreamBuffer, 0, IntPtr(inpBuffer.get()), inpStreamBuffer->Length);
				// Set
				stream.next_in = inpBuffer.get();
				stream.avail_in = result;
			}

			// Deflate it
			status = inflate(&stream, Z_SYNC_FLUSH);

			// Check result
			if (status == Z_STREAM_END || stream.avail_out <= 0)
			{
				// Compute size
				auto outputSize = 0x100000 - stream.avail_out;
				// Copy it
				Marshal::Copy(IntPtr(outBuffer.get()), outStreamBuffer, 0, (int)outputSize);
				// Write it
				outputStream->Write(outStreamBuffer, 0, outputSize);
				// Reset values
				stream.next_out = outBuffer.get();
				stream.avail_out = 0x100000;
			}

			// Check result
			if (status == Z_STREAM_END)
				break;
			else if (status != Z_OK)
				throw gcnew CompressionException(String::Format("Failed to deflate: {0}", status));
		}
	}
	catch (System::Exception ^ exception)
	{
		// Throw it
		throw exception;
	}
	finally
	{
		// Attempt to close 
		if (inflateEnd(&stream) != Z_OK)
			throw gcnew CompressionException(String::Format("Failed to close deflate stream"));
	}
}

array<System::Byte>^ ZLIB::Compress(array<System::Byte>^ inputData, int compressionLevel)
{
	// Create streams
	auto inpStream = gcnew MemoryStream(inputData);
	auto outStream = gcnew MemoryStream();
	// Decompress
	Compress(inpStream, outStream, compressionLevel);
	// Return result
	return outStream->ToArray();
}

void ZLIB::Compress(Stream^ inputStream, Stream^ outputStream, int compressionLevel)
{
	// Buffers
	std::unique_ptr<unsigned char[]> inpBuffer(new unsigned char[0x100000]);
	std::unique_ptr<unsigned char[]> outBuffer(new unsigned char[0x100000]);
	auto inpStreamBuffer = gcnew array<System::Byte>(0x100000);
	auto outStreamBuffer = gcnew array<System::Byte>(0x100000);

	// Init stream
	z_stream stream;
	memset(&stream, 0, sizeof(stream));
	// Init Values
	stream.next_in = inpBuffer.get();
	stream.avail_in = 0;
	stream.next_out = outBuffer.get();
	stream.avail_out = 0x100000;
	int status;

	// Attempt to init deflate
	if (deflateInit(&stream, compressionLevel) != Z_OK)
		throw gcnew CompressionException(String::Format("Failed to init deflate stream"));
	
	// Wrap in a try/catch as we need to clean up even if exceptions are thrown
	try
	{
		// Loop while we can
		while (true)
		{
			// Check do we need to read from the buffer
			if (stream.avail_in <= 0)
			{
				// Read from stream
				int result = inputStream->Read(inpStreamBuffer, 0, 0x100000);
				// Check result
				if (result <= 0)
					throw gcnew CompressionException(String::Format("Unexpected end of stream"));
				// Copy it
				Marshal::Copy(inpStreamBuffer, 0, IntPtr(inpBuffer.get()), inpStreamBuffer->Length);
				// Set
				stream.next_in = inpBuffer.get();
				stream.avail_in = result;
			}

			// Deflate it
			status = deflate(&stream, inputStream->Position < inputStream->Length ? Z_NO_FLUSH : Z_FINISH);

			// Check result
			if (status == Z_STREAM_END || stream.avail_out <= 0)
			{
				// Compute size
				auto outputSize = 0x100000 - stream.avail_out;
				// Copy it
				Marshal::Copy(IntPtr(outBuffer.get()), outStreamBuffer, 0, (int)outputSize);
				// Write it
				outputStream->Write(outStreamBuffer, 0, outputSize);
				// Reset values
				stream.next_out = outBuffer.get();
				stream.avail_out = 0x100000;
			}

			// Check result
			if (status == Z_STREAM_END)
				break;
			else if (status != Z_OK)
				throw gcnew CompressionException(String::Format("Failed to deflate: {0}", status));
		}
	}
	catch (System::Exception^ exception)
	{
		// Throw it
		throw exception;
	}
	finally
	{
		// Attempt to close 
		if(deflateEnd(&stream) != Z_OK)
			throw gcnew CompressionException(String::Format("Failed to close deflate stream"));
	}
}
