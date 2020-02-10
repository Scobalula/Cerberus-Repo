

#include "stdafx.h"
#pragma warning(disable : 4561) // __fastcall' incompatible with the '/clr' option: converting to '__stdcall
#include "DirectXTex.h"
#include "InteropUtility.h"
#include "ScratchImage.h"
#include "DirectXException.h"

PhilLibX::Imaging::ScratchImage::ScratchImage(Metadata^ metaData)
{
	InitializeImage(metaData);
}

PhilLibX::Imaging::ScratchImage::ScratchImage(Metadata^ metaData, array<Byte>^ buffer)
{
	InitializeImage(metaData, buffer);
}

PhilLibX::Imaging::ScratchImage::ScratchImage(array<Byte>^ buffer, ImageFormat format)
{
	Load(buffer, format);
}

PhilLibX::Imaging::ScratchImage::ScratchImage(String^ filePath)
{
	Load(filePath);
}

PhilLibX::Imaging::ScratchImage::ScratchImage(String^ filePath, ImageFormat format)
{
	Load(filePath, format);
}


PhilLibX::Imaging::ScratchImage::~ScratchImage()
{
	ClearLoadedImage();
}

void PhilLibX::Imaging::ScratchImage::InitializeImage(Metadata^ metaData)
{
	// Delete current
	ClearLoadedImage();

	// Results
	HRESULT result;
	// Create Scratch Image
	std::unique_ptr<DirectX::ScratchImage> scratchImage(new (std::nothrow) DirectX::ScratchImage);
	// Validate it
	if (!scratchImage)
		throw gcnew Exception("Failed to create scratch image");

	// Create new Meta Data
	DirectX::TexMetadata nativeMetadata =
	{
		(size_t)metaData->Width,
		(size_t)metaData->Height,
		(size_t)metaData->Depth,
		(size_t)metaData->ArraySize,
		(size_t)metaData->MipLevels,
		(uint32_t)metaData->MiscFlags,
		(uint32_t)metaData->MiscFlags2,
		(DXGI_FORMAT)metaData->Format,
		(DirectX::TEX_DIMENSION)metaData->Dimension
	};

	// Initialize it
	result = scratchImage.get()->Initialize(nativeMetadata);
	// Check result
	if (FAILED(result))
		throw gcnew DirectXException(String::Format("Failed to initialize DirectX::ScratchImage with the metadata, return code: 0x{0:X}", result));

	// Set it
	ScratchImagePointer = scratchImage.release();
}

void PhilLibX::Imaging::ScratchImage::InitializeImage(Metadata^ metaData, array<Byte>^ buffer)
{
	// Call base init
	InitializeImage(metaData);

	// Validate it
	if(!ScratchImagePointer)
		throw gcnew DirectXException(String::Format("Failed to aquire DirectX::ScratchImage, result returned nullptr"));

	// Validate the size
	if(buffer->Length < (int)ScratchImagePointer->GetPixelsSize())
		throw gcnew DirectXException(String::Format("Input buffer size is less than DirectX::ScratchImage->GetPixelsSize()"));

	// Copy Raw Data
	Marshal::Copy(buffer, 0, IntPtr(ScratchImagePointer->GetPixels()), (int)ScratchImagePointer->GetPixelsSize());
}

void PhilLibX::Imaging::ScratchImage::ClearLoadedImage()
{
	// Validate it and delete it if need be
	if (ScratchImagePointer)
		ScratchImagePointer->Release();
}

void PhilLibX::Imaging::ScratchImage::ConvertImage(DXGIFormat format)
{
	// Validate it
	if (!ScratchImagePointer)
		throw gcnew DirectXException(String::Format("Failed to aquire DirectX::ScratchImage, result returned nullptr"));

	// Results
	HRESULT result;

	// Get Metadata
	auto metaData = ScratchImagePointer->GetMetadata();
	// Check is the image compressed, if not, decompress it
	if (DirectX::IsCompressed(metaData.format) && !DirectX::IsCompressed((DXGI_FORMAT)format))
	{
		// Create new Image
		std::unique_ptr<DirectX::ScratchImage> newImage(new (std::nothrow) DirectX::ScratchImage);
		// Validate it
		if (!newImage)
			throw gcnew Exception("Failed to create new scratch image for image decompression");
		// Decompress the image
		result = DirectX::Decompress(ScratchImagePointer->GetImages(), ScratchImagePointer->GetImageCount(), metaData, (DXGI_FORMAT)format, *newImage);
		// Check result
		if (FAILED(result))
			throw gcnew Exception(String::Format("Failed to decompress image, return code: 0x{0:X}", result));
		// Set it
		ScratchImagePointer = newImage.release();

	}
	// Check is the image decompress, if not, compress it if necessary
	else if (!DirectX::IsCompressed(metaData.format) && DirectX::IsCompressed((DXGI_FORMAT)format))
	{
		// Create new Image
		std::unique_ptr<DirectX::ScratchImage> newImage(new (std::nothrow) DirectX::ScratchImage);
		// Validate it
		if (!newImage)
			throw gcnew Exception("Failed to create new scratch image for image compression");
		// Decompress the image
		result = DirectX::Compress(ScratchImagePointer->GetImages(), ScratchImagePointer->GetImageCount(), metaData, (DXGI_FORMAT)format, DirectX::TEX_FILTER_FLAGS::TEX_FILTER_DEFAULT, DirectX::TEX_THRESHOLD_DEFAULT, *newImage);
		// Check result
		if (FAILED(result))
			throw gcnew Exception(String::Format("Failed to compress image, return code: 0x{0:X}", result));
		// Set it
		ScratchImagePointer = newImage.release();
	}
	// Convert it
	else if (metaData.format != (DXGI_FORMAT)format)
	{
		// Create new Image
		std::unique_ptr<DirectX::ScratchImage> newImage(new (std::nothrow) DirectX::ScratchImage);
		// Validate it
		if (!newImage)
			throw gcnew Exception("Failed to create new scratch image for image conversion");
		// Convert it
		result = DirectX::Convert(ScratchImagePointer->GetImages(), ScratchImagePointer->GetImageCount(), metaData, (DXGI_FORMAT)format, DirectX::TEX_FILTER_FLAGS::TEX_FILTER_DEFAULT, DirectX::TEX_THRESHOLD_DEFAULT, *newImage);
		// Check result
		if (FAILED(result))
			throw gcnew Exception(String::Format("Failed to convert image, return code: 0x{0:X}", result));
		// Set it
		ScratchImagePointer = newImage.release();
	}
}

void PhilLibX::Imaging::ScratchImage::Resize(int width, int height)
{
	// Validate it
	if (!ScratchImagePointer)
		throw gcnew DirectXException(String::Format("Failed to aquire DirectX::ScratchImage, result returned nullptr"));

	// Create new Image
	std::unique_ptr<DirectX::ScratchImage> newImage(new (std::nothrow) DirectX::ScratchImage);
	// Validate it
	if (!newImage)
		throw gcnew Exception("Failed to create new scratch image for the resulting resized image");

	// Attempt to resize it, we want to resize the alpha separately to avoid issues
	auto result = DirectX::Resize(ScratchImagePointer->GetImages(), ScratchImagePointer->GetImageCount(), ScratchImagePointer->GetMetadata(), (size_t)width, (size_t)height, DirectX::TEX_FILTER_FLAGS::TEX_FILTER_SEPARATE_ALPHA, *newImage);
	// Check result
	if (FAILED(result))
		throw gcnew Exception(String::Format("Failed to resize image, return code: 0x{0:X}", result));

	// Set it
	ScratchImagePointer = newImage.release();
}

void PhilLibX::Imaging::ScratchImage::GenerateMipMaps(int mipMapCount)
{
	// Validate it
	if (!ScratchImagePointer)
		throw gcnew DirectXException(String::Format("Failed to aquire DirectX::ScratchImage, result returned nullptr"));

	// Create new Image
	std::unique_ptr<DirectX::ScratchImage> newImage(new (std::nothrow) DirectX::ScratchImage);
	// Validate it
	if (!newImage)
		throw gcnew Exception("Failed to create new scratch image for mip maps");

	// Resize it
	auto result = DirectX::GenerateMipMaps(ScratchImagePointer->GetImages(), ScratchImagePointer->GetImageCount(), ScratchImagePointer->GetMetadata(), DirectX::TEX_FILTER_FLAGS::TEX_FILTER_SEPARATE_ALPHA, (size_t)mipMapCount, *newImage);
	// Check result
	if (FAILED(result))
		throw gcnew Exception(String::Format("Failed to generated mip maps, return code: 0x{0:X}", result));

	// Set it
	ScratchImagePointer = newImage.release();
}

void PhilLibX::Imaging::ScratchImage::Load(array<Byte>^ buffer, ImageFormat format)
{
	// Results
	HRESULT result;

	// Unmanaged Buffer
	std::unique_ptr<uint8_t[]> bufferPtr(new uint8_t[buffer->Length]);
	// Copy it
	Marshal::Copy(buffer, 0, IntPtr(bufferPtr.get()), buffer->Length);

	// Create Scratch Image
	std::unique_ptr<DirectX::ScratchImage> scratchImage(new (std::nothrow) DirectX::ScratchImage);
	// Validate it
	if (!scratchImage)
		throw gcnew Exception("Failed to create scratch image");

	// Switch by Type
	switch (format)
	{
	case ImageFormat::DDS:
	{
		// Load it
		result = DirectX::LoadFromDDSMemory(bufferPtr.get(), (size_t)buffer->Length, DirectX::DDS_FLAGS::DDS_FLAGS_NONE, nullptr, *scratchImage);
		// Done
		break;
	}
	case ImageFormat::TIF:
	case ImageFormat::JPG:
	case ImageFormat::PNG:
	case ImageFormat::BMP:
	{
		// Load it
		result = DirectX::LoadFromWICMemory(bufferPtr.get(), (size_t)buffer->Length, DirectX::WIC_FLAGS_NONE, nullptr, *scratchImage);
		// Done
		break;
	}
	case ImageFormat::TGA:
	{
		// Load it
		result = DirectX::LoadFromTGAMemory(bufferPtr.get(), (size_t)buffer->Length, nullptr, *scratchImage);
		// Done
		break;
	}
	}
	// Check result
	if (FAILED(result))
		throw gcnew Exception(String::Format("Failed to save image, return code: 0x{0:X}", result));

	// Set it
	ScratchImagePointer = scratchImage.release();
}

void PhilLibX::Imaging::ScratchImage::Load(String^ filePath)
{
	Load(filePath, GetImageFormatForExtension(System::IO::Path::GetExtension(filePath)));
}

void PhilLibX::Imaging::ScratchImage::Load(String^ filePath, ImageFormat format)
{
	// Results
	HRESULT result;
	// Output Path
	std::wstring filePathStd;
	// Convert String
	InteropUtility::ToStdWString(filePath, filePathStd);

	// Create Scratch Image
	std::unique_ptr<DirectX::ScratchImage> scratchImage(new (std::nothrow) DirectX::ScratchImage);
	// Validate it
	if (!scratchImage)
		throw gcnew Exception("Failed to create scratch image");

	// Switch by Type
	switch (format)
	{
	case ImageFormat::DDS:
	{
		// Load it
		result = DirectX::LoadFromDDSFile(filePathStd.data(), DirectX::DDS_FLAGS::DDS_FLAGS_NONE, nullptr, *scratchImage);
		// Done
		break;
	}
	case ImageFormat::TIF:
	case ImageFormat::JPG:
	case ImageFormat::PNG:
	case ImageFormat::BMP:
	{
		// Save it
		result = DirectX::LoadFromWICFile(filePathStd.data(), DirectX::WIC_FLAGS_NONE, nullptr, *scratchImage);
		// Done
		break;
	}
	case ImageFormat::TGA:
	{
		// Save it
		result = DirectX::LoadFromTGAFile(filePathStd.data(), nullptr, *scratchImage);
		// Done
		break;
	}
	}

	// Check result
	if (FAILED(result))
		throw gcnew Exception(String::Format("Failed to save image, return code: 0x{0:X}", result));

	// Set it
	ScratchImagePointer = scratchImage.release();
}

void PhilLibX::Imaging::ScratchImage::Save(String^ filePath)
{
	Save(filePath, GetImageFormatForExtension(System::IO::Path::GetExtension(filePath)));
}

void PhilLibX::Imaging::ScratchImage::Save(String^ filePath, ImageFormat format)
{
	// Validate it
	if (!ScratchImagePointer)
		throw gcnew DirectXException(String::Format("Failed to aquire DirectX::ScratchImage, result returned nullptr"));

	// Results
	HRESULT result;
	// Output Path
	std::wstring filePathStd;
	// Convert String
	InteropUtility::ToStdWString(filePath, filePathStd);

	// Get images
	const DirectX::Image* images = ScratchImagePointer->GetImages();
	auto imageCount = ScratchImagePointer->GetImageCount();
	// Validate it
	if(!images || imageCount <= 0)
		throw gcnew DirectXException(String::Format("Failed to aquire DirectX::ScratchImage, result returned nullptr or image count was 0"));

	// Switch by Type
	switch (format)
	{
	case ImageFormat::DDS:
	{
		// Load it
		result = DirectX::SaveToDDSFile(images, imageCount, ScratchImagePointer->GetMetadata(), DirectX::DDS_FLAGS::DDS_FLAGS_NONE, filePathStd.data());
		// Done
		break;
	}
	case ImageFormat::TIF:
	{
		// Save it
		result = DirectX::SaveToWICFile(*images, DirectX::WIC_FLAGS::WIC_FLAGS_NONE, DirectX::GetWICCodec(DirectX::WIC_CODEC_PNG), filePathStd.data(), nullptr, SetCustomPropertiesTIFF);
		// Done
		break;
	}
	case ImageFormat::JPG:
	{
		// Save it
		result = DirectX::SaveToWICFile(*images, DirectX::WIC_FLAGS::WIC_FLAGS_NONE, DirectX::GetWICCodec(DirectX::WIC_CODEC_JPEG), filePathStd.data(), nullptr, SetCustomPropertiesJPG);
		// Done
		break;
	}
	case ImageFormat::PNG:
	{
		// Save it
		result = DirectX::SaveToWICFile(*images, DirectX::WIC_FLAGS::WIC_FLAGS_NONE, DirectX::GetWICCodec(DirectX::WIC_CODEC_PNG), filePathStd.data());
		// Done
		break;
	}
	case ImageFormat::BMP:
	{
		// Save it
		result = DirectX::SaveToWICFile(*images, DirectX::WIC_FLAGS::WIC_FLAGS_NONE, DirectX::GetWICCodec(DirectX::WIC_CODEC_BMP), filePathStd.data());
		// Done
		break;
	}
	case ImageFormat::TGA:
	{
		// Save it
		result = DirectX::SaveToTGAFile(*images, filePathStd.data());
		// Done
		break;
	}
	}

	// Check result
	if (FAILED(result))
		throw gcnew Exception(String::Format("Failed to save image, return code: 0x{0:X}", result));
}

Bitmap^ PhilLibX::Imaging::ScratchImage::ToBitmap()
{
	return ToBitmap(0, 0, 0);
}

Bitmap^ PhilLibX::Imaging::ScratchImage::ToBitmap(int mip, int item, int slice)
{
	// Validate it
	if (!ScratchImagePointer)
		throw gcnew DirectXException(String::Format("Failed to aquire DirectX::ScratchImage, result returned nullptr"));

	// Results
	HRESULT result;

	// Create blob
	DirectX::Blob blob;
	// Get the first image
	const DirectX::Image* img = ScratchImagePointer->GetImage((size_t)mip, (size_t)item, (size_t)slice);

	// Validate it
	if (!img)
		throw gcnew DirectXException(String::Format("Failed to aquire DirectX::Image, result returned nullptr"));

	// Save to PNG in-memory, then pass to .NET (let the DX Lib handle it)
	result = DirectX::SaveToWICMemory(*img, DirectX::WIC_FLAGS_NONE, DirectX::GetWICCodec(DirectX::WIC_CODEC_PNG), blob);
	// Check result
	if (FAILED(result))
		throw gcnew DirectXException(String::Format("Failed to save the WIC Image to memory via DirectX::SaveToWICMemory(...), return code: 0x{0:X}", result));

	// Allocate an array and copy into it
	auto buffer = gcnew array<Byte>((int)blob.GetBufferSize());
	Marshal::Copy(IntPtr(blob.GetBufferPointer()), buffer, 0, (int)buffer->Length);

	// Return result
	return gcnew Bitmap(gcnew System::IO::MemoryStream(buffer));
}


