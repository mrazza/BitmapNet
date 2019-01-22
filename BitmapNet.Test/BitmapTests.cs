// <copyright file="BitmapTests.cs">
// Copyright (c) Matthew Razza All Rights Reserved
// </copyright>

namespace BitmapNet.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class BitmapTests
    {
        private const int BITMAP_LENGTH = 200;
        private Bitmap bitmap;

        [TestInitialize]
        public void Setup()
        {
            this.bitmap = new Bitmap(BITMAP_LENGTH);
        }

        [TestMethod]
        public void CorrectLength()
        {
            Assert.AreEqual(BITMAP_LENGTH, this.bitmap.Length);
        }

        [TestMethod]
        public void AllBitsFalse()
        {
            foreach (var bit in this.bitmap)
            {
                Assert.IsFalse(bit);
            }
        }

        [TestMethod]
        public void AllBitsTrue()
        {
            this.bitmap = new Bitmap(BITMAP_LENGTH, true);

            foreach (var bit in this.bitmap)
            {
                Assert.IsTrue(bit);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetBitIndexTooSmall()
        {
            this.bitmap.GetBit(-1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetBitIndexTooBig()
        {
            this.bitmap.GetBit(this.bitmap.Length);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void GetBitArrayIndexTooSmall()
        {
            var value = this.bitmap[-1];
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void GetBitArrayIndexTooBig()
        {
            var value = this.bitmap[this.bitmap.Length];
        }

        [TestMethod]
        public void SetAllBits()
        {
            this.bitmap.SetAllBits(true);
            foreach (var bit in this.bitmap)
            {
                Assert.IsTrue(bit);
            }

            this.bitmap.SetAllBits(false);
            foreach (var bit in this.bitmap)
            {
                Assert.IsFalse(bit);
            }
        }

        [TestMethod]
        public void SetValue()
        {
            this.bitmap.SetBit(10, true);
            Assert.IsTrue(this.IsSingleBitSet(10));
        }

        [TestMethod]
        public void SetValueArray()
        {
            this.bitmap[10] = true;
            Assert.IsTrue(this.IsSingleBitSet(10));
            Assert.IsTrue(this.bitmap[10]);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void SetBitArrayIndexTooSmall()
        {
            this.bitmap[-1] = true;
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void SetBitArrayIndexTooBig()
        {
            this.bitmap[this.bitmap.Length] = true;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SetBitIndexTooSmall()
        {
            this.bitmap.SetBit(-1, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SetBitIndexTooBig()
        {
            this.bitmap.SetBit(this.bitmap.Length, true);
        }

        [TestMethod]
        public void Intersect()
        {
            this.bitmap.SetBit(10, true);
            this.bitmap.SetBit(11, true);

            Bitmap intersectingBitmap = new Bitmap(this.bitmap.Length);
            intersectingBitmap.SetBit(10, true);
            this.bitmap.Intersect(intersectingBitmap);

            Assert.IsTrue(this.IsSingleBitSet(10));
        }

        [TestMethod]
        public void Union()
        {
            Bitmap unionBitmap = new Bitmap(this.bitmap.Length);
            unionBitmap.SetBit(10, true);
            this.bitmap.Union(unionBitmap);

            Assert.IsTrue(this.IsSingleBitSet(10));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void IntersectDifferenceLengths()
        {
            Bitmap intersectingBitmap = new Bitmap(this.bitmap.Length / 2);
            this.bitmap.Intersect(intersectingBitmap);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UnionDifferentLengths()
        {
            Bitmap unionBitmap = new Bitmap(this.bitmap.Length / 2);
            this.bitmap.Union(unionBitmap);
        }

        [TestMethod]
        public void Invert()
        {
            this.bitmap.SetAllBits(true);
            this.bitmap.SetBit(10, false);
            this.bitmap.Invert();

            Assert.IsTrue(this.IsSingleBitSet(10));
        }

        [TestMethod]
        public void GetTrueBits()
        {
            int[] trueBits = { 0, 10, 32, 63, 64, 65, 128 };

            foreach (var trueBit in trueBits)
            {
                this.bitmap.SetBit(trueBit, true);
            }

            Assert.IsTrue(Enumerable.SequenceEqual(trueBits, this.bitmap.GetTrueBits()));
        }

        [TestMethod]
        public void SetRange()
        {
            const int RANGE_START = 30;
            const int RANGE_END = 68;
            this.bitmap.SetRange(RANGE_START, RANGE_END, true);
            Assert.IsTrue(Enumerable.SequenceEqual(
                Enumerable.Range(RANGE_START, RANGE_END - RANGE_START + 1),
                this.bitmap.GetTrueBits()));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SetRangeOutOfRangeTooSmall()
        {
            const int RANGE_START = -1;
            const int RANGE_END = 68;
            this.bitmap.SetRange(RANGE_START, RANGE_END, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SetRangeOutOfRangeTooBig()
        {
            const int RANGE_START = 30;
            const int RANGE_END = BITMAP_LENGTH + 1;
            this.bitmap.SetRange(RANGE_START, RANGE_END, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetRangeBadValue()
        {
            const int RANGE_START = 30;
            const int RANGE_END = 15;
            this.bitmap.SetRange(RANGE_START, RANGE_END, true);
        }

        [TestMethod]
        public void SingleBitBoundary()
        {
            const int RANGE_START = 64;
            const int RANGE_END = 64;
            this.bitmap.SetRange(RANGE_START, RANGE_END, true);
            Assert.IsTrue(Enumerable.SequenceEqual(
                Enumerable.Range(RANGE_START, RANGE_END - RANGE_START + 1),
                this.bitmap.GetTrueBits()));
        }

        [TestMethod]
        public void EntireBlock()
        {
            const int RANGE_START = 64;
            const int RANGE_END = 127;
            this.bitmap.SetRange(RANGE_START, RANGE_END, true);
            Assert.IsTrue(Enumerable.SequenceEqual(
                Enumerable.Range(RANGE_START, RANGE_END - RANGE_START + 1),
                this.bitmap.GetTrueBits()));
        }

        [TestMethod]
        public void GetAllTrueBitsRange()
        {
            const int RANGE_START = 64;
            const int RANGE_END = 127;
            this.bitmap.SetRange(RANGE_START, RANGE_END, true);

            ISet<int> trueBits = this.bitmap.GetTrueBits();
            for (int index = 0; index < this.bitmap.Length; index++)
            {
                Assert.AreEqual(trueBits.Contains(index), this.bitmap.GetBit(index));
            }
        }

        private bool IsSingleBitSet(int bitIndex)
        {
            for (int index = 0; index < this.bitmap.Length; index++)
            {
                if (index == bitIndex && !this.bitmap[index])
                {
                    return false;
                }
                else if (index != bitIndex && this.bitmap[index])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
