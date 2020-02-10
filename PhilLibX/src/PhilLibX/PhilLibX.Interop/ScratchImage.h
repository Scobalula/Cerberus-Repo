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
// File: ScratchImage.h
// Author: Philip/Scobalula
// Description: A basic wrapper around Microsoft's DirectXTex ScratchImage
#pragma once

using namespace System;
using namespace System::Drawing;
using namespace System::Runtime::InteropServices;

namespace PhilLibX
{
	namespace Imaging
	{
		/// <summary>
		/// A basic wrapper around Microsoft's DirectXTex ScratchImage
		/// </summary>
		public ref class ScratchImage
		{
		private:
			/// <summary>
			/// Native Scratch Image Pointer
			/// </summary>
			DirectX::ScratchImage* ScratchImagePointer;

			/// <summary>
			/// Sets custom properties for JPG Images (for use with LoadFromWICMemory(...))
			/// </summary>
			/// <param name="props">Property Bag to apply the custom properties to</param>
			static void SetCustomPropertiesJPG(IPropertyBag2* props)
			{
				PROPBAG2 options = {};
				VARIANT varValues = {};
				options.pstrName = const_cast<wchar_t*>(L"ImageQuality");
				varValues.vt = VT_R4;
				varValues.fltVal = 1.f;
				(void)props->Write(1, &options, &varValues);
			}

			/// <summary>
			/// Sets custom properties for TIFF Images (for use with LoadFromWICMemory(...))
			/// </summary>
			/// <param name="props">Property Bag to apply the custom properties to</param>
			static void SetCustomPropertiesTIFF(IPropertyBag2* props)
			{
				PROPBAG2 options = {};
				VARIANT varValues = {};
				options.pstrName = const_cast<wchar_t*>(L"TiffCompressionMethod");
				varValues.vt = VT_UI1;
				varValues.bVal = 0x00000001; // WICTiffCompressionNone
				(void)props->Write(1, &options, &varValues);
			}
		public:
			/// <summary>
			/// DDS/DXGI Formats
			/// </summary>
			enum class DXGIFormat : UInt32
			{
				UNKNOWN = 0,
				R32G32B32A32TYPELESS = 1,
				R32G32B32A32FLOAT = 2,
				R32G32B32A32UINT = 3,
				R32G32B32A32SINT = 4,
				R32G32B32TYPELESS = 5,
				R32G32B32FLOAT = 6,
				R32G32B32UINT = 7,
				R32G32B32SINT = 8,
				R16G16B16A16TYPELESS = 9,
				R16G16B16A16FLOAT = 10,
				R16G16B16A16UNORM = 11,
				R16G16B16A16UINT = 12,
				R16G16B16A16SNORM = 13,
				R16G16B16A16SINT = 14,
				R32G32TYPELESS = 15,
				R32G32FLOAT = 16,
				R32G32UINT = 17,
				R32G32SINT = 18,
				R32G8X24TYPELESS = 19,
				D32FLOATS8X24UINT = 20,
				R32FLOATX8X24TYPELESS = 21,
				X32TYPELESSG8X24UINT = 22,
				R10G10B10A2TYPELESS = 23,
				R10G10B10A2UNORM = 24,
				R10G10B10A2UINT = 25,
				R11G11B10FLOAT = 26,
				R8G8B8A8TYPELESS = 27,
				R8G8B8A8UNORM = 28,
				R8G8B8A8UNORMSRGB = 29,
				R8G8B8A8UINT = 30,
				R8G8B8A8SNORM = 31,
				R8G8B8A8SINT = 32,
				R16G16TYPELESS = 33,
				R16G16FLOAT = 34,
				R16G16UNORM = 35,
				R16G16UINT = 36,
				R16G16SNORM = 37,
				R16G16SINT = 38,
				R32TYPELESS = 39,
				D32FLOAT = 40,
				R32FLOAT = 41,
				R32UINT = 42,
				R32SINT = 43,
				R24G8TYPELESS = 44,
				D24UNORMS8UINT = 45,
				R24UNORMX8TYPELESS = 46,
				X24TYPELESSG8UINT = 47,
				R8G8TYPELESS = 48,
				R8G8UNORM = 49,
				R8G8UINT = 50,
				R8G8SNORM = 51,
				R8G8SINT = 52,
				R16TYPELESS = 53,
				R16FLOAT = 54,
				D16UNORM = 55,
				R16UNORM = 56,
				R16UINT = 57,
				R16SNORM = 58,
				R16SINT = 59,
				R8TYPELESS = 60,
				R8UNORM = 61,
				R8UINT = 62,
				R8SNORM = 63,
				R8SINT = 64,
				A8UNORM = 65,
				R1UNORM = 66,
				R9G9B9E5SHAREDEXP = 67,
				R8G8B8G8UNORM = 68,
				G8R8G8B8UNORM = 69,
				BC1TYPELESS = 70,
				BC1UNORM = 71,
				BC1UNORMSRGB = 72,
				BC2TYPELESS = 73,
				BC2UNORM = 74,
				BC2UNORMSRGB = 75,
				BC3TYPELESS = 76,
				BC3UNORM = 77,
				BC3UNORMSRGB = 78,
				BC4TYPELESS = 79,
				BC4UNORM = 80,
				BC4SNORM = 81,
				BC5TYPELESS = 82,
				BC5UNORM = 83,
				BC5SNORM = 84,
				B5G6R5UNORM = 85,
				B5G5R5A1UNORM = 86,
				B8G8R8A8UNORM = 87,
				B8G8R8X8UNORM = 88,
				R10G10B10XRBIASA2UNORM = 89,
				B8G8R8A8TYPELESS = 90,
				B8G8R8A8UNORMSRGB = 91,
				B8G8R8X8TYPELESS = 92,
				B8G8R8X8UNORMSRGB = 93,
				BC6HTYPELESS = 94,
				BC6HUF16 = 95,
				BC6HSF16 = 96,
				BC7TYPELESS = 97,
				BC7UNORM = 98,
				BC7UNORMSRGB = 99,
				AYUV = 100,
				Y410 = 101,
				Y416 = 102,
				NV12 = 103,
				P010 = 104,
				P016 = 105,
				OPAQUE420 = 106,
				YUY2 = 107,
				Y210 = 108,
				Y216 = 109,
				NV11 = 110,
				AI44 = 111,
				IA44 = 112,
				P8 = 113,
				A8P8 = 114,
				B4G4R4A4UNORM = 115,
				FORCEUINT = 0xffffffff
			};

			/// <summary>
			/// Texture Dimension
			/// </summary>
			enum class TexDimension : UInt32
			{
				TEXTURE1D = 2,
				TEXTURE2D = 3,
				TEXTURE3D = 4,
			};

			/// <summary>
			/// Misc. Texture Flags
			/// </summary>
			enum class TexMiscFlags : UInt32
			{
				NONE = 0,
				TEXTURECUBE = 0x4,
			};

			/// <summary>
			/// Misc. Texture Flags
			/// </summary>
			enum class TexMiscFlags2 : UInt32
			{
				NONE = 0,
				TEXMISC2ALPHAMODEMASK = 0x7,
			};

			/// <summary>
			/// Image Format
			/// </summary>
			enum class ImageFormat
			{
				// Automatically resolve output based off extension
				Automatic,
				DDS,
				PNG,
				TGA,
				JPG,
				TIF,
				BMP,
			};

			/// <summary>
			/// Gets the output format for the given extension, if unrecognized, DDS is returned
			/// </summary>
			/// <param name="extension">Extension to check</param>
			static ImageFormat GetImageFormatForExtension(String^ extension)
			{
				// DDS Files
				if (extension == ".dds")
					return ImageFormat::DDS;
				// PNG Files
				if (extension == ".png")
					return ImageFormat::PNG;
				// BMP Files
				if (extension == ".bmp")
					return ImageFormat::BMP;
				// Targa Files
				if (extension == ".tga")
					return ImageFormat::TGA;
				// JPG Files
				if (extension == ".jpg")
					return ImageFormat::JPG;
				// TIFF Files
				if (extension == ".tiff" || extension == ".tif")
					return ImageFormat::TIF;
				// Return DDS by default
				return ImageFormat::DDS;
			}

			/// <summary>
			/// Texture Metadata
			/// </summary>
			ref struct Metadata
			{
				UInt64 Width;
				UInt64 Height;     // Should be 1 for 1D textures
				UInt64 Depth;      // Should be 1 for 1D or 2D textures
				UInt64 ArraySize;  // For cubemap, this is a multiple of 6
				UInt64 MipLevels;
				TexMiscFlags MiscFlags;
				TexMiscFlags2 MiscFlags2;
				DXGIFormat Format;
				TexDimension Dimension;
			};

			/// <summary>
			/// Initializes an instance of the <see cref="ScratchImage"/> class with the given params
			/// </summary>
			/// <param name="metaData">Metadata to initialize the image with</param>
			ScratchImage(Metadata^ metaData);

			/// <summary>
			/// Initializes an instance of the <see cref="ScratchImage"/> class with the params and given raw image buffer
			/// </summary>
			/// <param name="metaData">Metadata to initialize the image with</param>
			/// <param name="buffer">Pixel buffer to apply to the image</param>
			ScratchImage(Metadata^ metaData, array<Byte>^ buffer);

			/// <summary>
			/// Initializes an instance of the <see cref="ScratchImage"/> class with the given image buffer and type
			/// </summary>
			/// <param name="buffer">Image file buffer</param>
			/// <param name="format">The <see cref="ImageFormat"/>/Type of the Image</param>
			ScratchImage(array<Byte>^ buffer, ImageFormat format);

			/// <summary>
			/// Initializes an instance of the <see cref="ScratchImage"/> class with the given image path
			/// </summary>
			/// <param name="filePath">Image file path</param>
			ScratchImage(String^ filePath);

			/// <summary>
			/// Initializes an instance of the <see cref="ScratchImage"/> class with the given image path
			/// </summary>
			/// <param name="filePath">Image file path</param>
			/// <param name="format">The <see cref="ImageFormat"/>/Type of the Image</param>
			ScratchImage(String^ filePath, ImageFormat format);

			/// <summary>
			/// Initializes the native DirectX::ScratchImage with the given params
			/// </summary>
			/// <param name="metaData">Metadata to initialize the image with</param>
			void InitializeImage(Metadata^ metaData);

			/// <summary>
			/// Initializes the native DirectX::ScratchImage with the params and given raw image buffer
			/// </summary>
			/// <param name="metaData">Metadata to initialize the image with</param>
			/// <param name="buffer">Pixel buffer to apply to the image</param>
			void InitializeImage(Metadata^ metaData, array<Byte>^ buffer);

			/// <summary>
			/// Destructs the currently loaded native DirectX::ScratchImage
			/// </summary>
			void ClearLoadedImage();

			/// <summary>
			/// Converts the native DirectX::ScratchImage to the given format
			/// </summary>
			/// <param name="format"><see cref="DXGIFormat"/><see cref="DXGIFormat"/> to convert the image to</param>
			void ConvertImage(DXGIFormat format);

			/// <summary>
			/// Resizes the native DirectX::ScratchImage to the given width and height
			/// </summary>
			/// <param name="width">Image Width</param>
			/// <param name="width">Image Height</param>
			void Resize(int width, int height);

			/// <summary>
			/// Generates Mip Maps for the native DirectX::ScratchImage
			/// </summary>
			/// <param name="mipMapCount">Number of Mip Maps to generate</param>
			void GenerateMipMaps(int mipMapCount);

			/// <summary>
			/// Loads the buffer into the <see cref="ScratchImage"/> from the given image buffer and type
			/// </summary>
			/// <param name="buffer">Image file buffer</param>
			/// <param name="format">The <see cref="ImageFormat"/>/Type of the Image</param>
			void Load(array<Byte>^ buffer, ImageFormat format);

			/// <summary>
			/// Loads the file into the <see cref="ScratchImage"/> from the given image path
			/// </summary>
			/// <param name="filePath">Image file path</param>
			void Load(String^ filePath);

			/// <summary>
			/// Loads the file into the <see cref="ScratchImage"/> from the given image path and type
			/// </summary>
			/// <param name="filePath">Image file path</param>
			/// <param name="format">The <see cref="ImageFormat"/>/Type of the Image</param>
			void Load(String^ filePath, ImageFormat format);

			/// <summary>
			/// Saves the <see cref="ScratchImage"/> to the given image path
			/// </summary>
			/// <param name="filePath">Image file path</param>
			void Save(String^ filePath);

			/// <summary>
			/// Saves the <see cref="ScratchImage"/> to the given image path and type
			/// </summary>
			/// <param name="filePath">Image file path</param>
			/// <param name="format">The <see cref="ImageFormat"/>/Type of the Image</param>
			void Save(String^ filePath, ImageFormat format);

			/// <summary>
			/// Converts the first mip/image and slice of the native DirectX::ScratchImage to a .NET <see cref="Bitmap"/>
			/// </summary>
			Bitmap^ ToBitmap();

			/// <summary>
			/// Converts the given mip/slice of the native DirectX::ScratchImage to a .NET <see cref="Bitmap"/>
			/// </summary>
			/// <param name="mip">Mip Map to convert</param>
			/// <param name="item">Item to convert</param>
			/// <param name="slice">Slice to convert</param>
			Bitmap^ ToBitmap(int mip, int item, int slice);

			/// <summary>
			/// Destructs the ScratchImage and deletes the native DirectX::ScratchImage
			/// </summary>
			~ScratchImage();
		};
	}
}

