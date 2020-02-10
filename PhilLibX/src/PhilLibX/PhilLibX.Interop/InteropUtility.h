#pragma once

#include "stdafx.h"

using namespace System;
using namespace System::Runtime::InteropServices;

namespace PhilLibX
{
	private ref class InteropUtility abstract sealed
	{
	public:
		/// <summary>
		/// Converts a managed string to a standard C++ string
		/// </summary>
		/// <param name="stringInput">Input Managed String</param>
		/// <param name="stringOutput">Output C++ String</param>
		static void ToStdString(String^ stringInput, std::string& stringOutput)
		{
			// Convert the Managed String
			const char* result = (const char*)(Marshal::StringToHGlobalAnsi(stringInput)).ToPointer();
			// Set String
			stringOutput = result;
			// Free the characters
			Marshal::FreeHGlobal(IntPtr((void*)result));
		}

		/// <summary>
		/// Converts a managed string to a standard wide C++ string
		/// </summary>
		/// <param name="stringInput">Input Managed String</param>
		/// <param name="stringOutput">Output wide C++ String</param>
		static void ToStdWString(String^ stringInput, std::wstring& stringOutput)
		{
			// Convert the Managed String
			const wchar_t* result = (const wchar_t*)(Marshal::StringToHGlobalUni(stringInput)).ToPointer();
			// Set String
			stringOutput = result;
			// Free the characters
			Marshal::FreeHGlobal(IntPtr((void*)result));
		}
	};
}
