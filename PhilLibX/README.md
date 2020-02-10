# PhilLibX - My .NET Utility Library
[![Releases](https://img.shields.io/github/downloads/Scobalula/PhilLibX/total.svg)](https://github.com/Scobalula/PhilLibX/releases)

This is a set of classes, structures, methods, wrappers for DirectXTex, Compression, etc. that I use throughout my own .NET code that is available here for anyone to use. This library is built to suit my needs and is provided as is.

## Using the Library

The latest Debug and Release x86/x64 builds can be found from the [Releases](https://github.com/Scobalula/PhilLibX/releases) page, from there you can add them as a reference to your project. They are built using .NET Framework 4.7.2 on Visual Studio 2019, with MSVC V142 Windows 10 SDK used to compile the external native libraries and PhilLibX.Interop.dll, you may need to grab latest MSVC Runtimes from Microsoft to use them. PhilLibX.Interop.dll and PhilLibX.dll are not required to use each other, and so you can use either or both in your code.

To build the library simply download/clone the repo and ensure you have the MSVC 142 compile tools and .NET 4.7.2, if you are having issues feel free to file an issue and I'll see if I can help, but I generally have limited time and so ensure you have tried everything before asking!

Documentation is provided in form of XML files for now, since the library is mostly written on the fly for my own needs as I see fit/when I see myself using the same feature/s across code there isn't really any examples or tests, if you're unsure about a portion of the library you want to use feel free to file an issue. I will be adding limited documentation detailing each part of the library to the wiki over time.

## License/Disclaimer

The library is available under the MIT License and can be used for any purpose, I have chosen the MIT License specifically to ensure the license does not hinder your usage of the library.

The library is provided as is, it is built to suit my needs and the code, the extent of the wrappers, etc. reflects this. I do not accept PRs/requests unless I see myself using the feature.

The external libraries used are compatible with the MIT License, most are either BSD or MIT with some exceptions. The licenses used by external libraries can be seen in the [License](https://github.com/Scobalula/PhilLibX/blob/master/LICENSE) file.
