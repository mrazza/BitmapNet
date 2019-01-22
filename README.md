# Bitmap.NET
[![Build Status](https://travis-ci.org/mrazza/BitmapNet.svg?branch=master)](https://travis-ci.org/mrazza/BitmapNet)

A Bitmap library (think BitArray or Bit Index, not bmp) written in C# with more functionality than the .NET default.

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
