# Bitmap.NET [![Build Status](https://travis-ci.org/mrazza/BitmapNet.svg?branch=master)](https://travis-ci.org/mrazza/BitmapNet) [![Code Coverage](https://img.shields.io/codecov/c/github/mrazza/BitmapNet.svg)](https://codecov.io/gh/mrazza/BitmapNet/) [![nuget](https://img.shields.io/nuget/v/Bitmap.Net.svg)](https://www.nuget.org/packages/Bitmap.Net/) ![](https://img.shields.io/nuget/dt/Bitmap.Net.svg)


A [bitmap](https://en.wikipedia.org/wiki/Bit_array) library (think [BitArray](https://docs.microsoft.com/en-us/dotnet/api/system.collections.bitarray), [BitVector32](https://docs.microsoft.com/en-us/dotnet/api/system.collections.specialized.bitvector32), or Bit Index -- not bmp) written in C# with more functionality than the .NET library options.

Bitmap.NET runs on .NET Standard 2.0, supports arbitrary bitmap lengths, and supports the following operations:
* Per-bit Get
* Per-bit Set
* Union with another bitmap
* Intersection with another bitmap
* Invert the bitmap
* Set a range of bits to a value
* Set all bits to a value
* Effeciently get the set of all `True` bits

Look at the test for some simple examples.
