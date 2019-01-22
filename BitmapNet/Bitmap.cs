// <copyright file="Bitmap.cs">
// Copyright (c) Matthew Razza All Rights Reserved
// </copyright>

namespace BitmapNet
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A collection of bits (exposed as <c>bool</c> values) that can either be 1 (<c>True</c>) or
    /// 0 (<c>False</c>).
    /// </summary>
    /// <remarks>
    /// This is a replacement for <see cref="BitArray"/> with more advanced functionality. Bitmaps
    /// are useful when performing unions/intersections on large datasets.
    /// 
    /// Data is encoded in <see cref="ulong"/> blocks where the right-most bit in each block
    /// represents the 0th-place.
    /// </remarks>
    public class Bitmap : IEnumerable<bool>
    {
        /// <summary>
        /// The number of bits in a <see cref="ulong"/>.
        /// </summary>
        private const int BITS_IN_ULONG = sizeof(ulong) * 8;

        /// <summary>
        /// The collection of <see cref="ulong"/>s representing the bits in the bitmap.
        /// </summary>
        private ulong[] bits;

        /// <summary>
        /// The size of the bitmap collection (in bits).
        /// </summary>
        /// <remarks>
        /// This is likely to be smaller than the number of bits that exist within
        /// <see cref="bits"/> as we must round up when constructing the <see cref="ulong"/> array
        /// in order to fully store the requested bit size.
        /// </remarks>
        public int Length
        {
            get;
        }

        /// <summary>
        /// Get or set the value of a bit at the specified index.
        /// </summary>
        /// <param name="index">The index of the bit to get or set.</param>
        /// <returns>True if the bit is set (1); otherwise, false.</returns>
        public bool this[int index]
        {
            get
            {
                try
                {
                    return this.GetBit(index);
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw new IndexOutOfRangeException();
                }
            }

            set
            {
                try
                {
                    this.SetBit(index, value);
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw new IndexOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bitmap"/> class with the specified length
        /// setting all bits to the (optionally) specified initial value.
        /// </summary>
        /// <param name="length">The length, in bits, of the bitmap.</param>
        /// <param name="initialValue">The initial value to set each bit to.</param>
        public Bitmap(int length, bool initialValue = false)
        {
            this.Length = length;
            this.bits = new ulong[(length / BITS_IN_ULONG) + 1];

            if (initialValue)
            {
                this.SetAllBits(initialValue);
            }
        }

        /// <summary>
        /// Sets all the bits in the bitmap to the specified value.
        /// </summary>
        /// <param name="value">The value to set each bit to.</param>
        /// <returns>The instance of the modified bitmap.</returns>
        public Bitmap SetAllBits(bool value)
        {
            for (int index = 0; index < this.bits.Length; index++)
            {
                this.bits[index] = value ? ulong.MaxValue : ulong.MinValue;
            }

            return this;
        }

        /// <summary>
        /// Gets the value of the bit at the specified index.
        /// </summary>
        /// <param name="index">The index of the bit.</param>
        /// <returns>The value of the bit at the specified index.</returns>
        public bool GetBit(int index)
        {
            if (!this.IsValidIndex(index))
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return (this.bits[index / BITS_IN_ULONG] & (0x1ul << (index % BITS_IN_ULONG))) > 0;
        }

        /// <summary>
        /// Sets the value of the bit at the specified index to the specified value.
        /// </summary>
        /// <param name="index">The index of the bit to set.</param>
        /// <param name="value">The value to set the bit to.</param>
        /// <returns>The instance of the modified bitmap.</returns>
        public Bitmap SetBit(int index, bool value)
        {
            if (!this.IsValidIndex(index))
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (value)
            {
                this.bits[index / BITS_IN_ULONG] |= 0x1ul << (index % BITS_IN_ULONG);
            }
            else
            {
                this.bits[index / BITS_IN_ULONG] &= ~(0x1ul << (index % BITS_IN_ULONG));
            }

            return this;
        }

        /// <summary>
        /// Takes the union of this bitmap and the specified bitmap and stores the result in this
        /// instance.
        /// </summary>
        /// <param name="bitmap">The bitmap to union with this instance.</param>
        /// <returns>A reference to this instance.</returns>
        public Bitmap Union(Bitmap bitmap)
        {
            if (this.Length != bitmap.Length)
            {
                throw new ArgumentException("Bitmaps must be of equal length to union them.",
                    nameof(bitmap));
            }

            for (int index = 0; index < this.bits.Length; index++)
            {
                this.bits[index] |= bitmap.bits[index];
            }

            return this;
        }

        /// <summary>
        /// Takes the intersection of this bitmap and the specified bitmap and stores the result in
        /// this instance.
        /// </summary>
        /// <param name="bitmap">The bitmap to interesct with this instance.</param>
        /// <returns>A reference to this instance.</returns>
        public Bitmap Intersect(Bitmap bitmap)
        {
            if (this.Length != bitmap.Length)
            {
                throw new ArgumentException("Bitmaps must be of equal length to intersect them.",
                    nameof(bitmap));
            }

            for (int index = 0; index < this.bits.Length; index++)
            {
                this.bits[index] &= bitmap.bits[index];
            }

            return this;
        }

        /// <summary>
        /// Inverts all the bits in this bitmap.
        /// </summary>
        /// <returns>A reference to this instance.</returns>
        public Bitmap Invert()
        {
            for (int index = 0; index < this.bits.Length; index++)
            {
                this.bits[index] = ~this.bits[index];
            }

            return this;
        }

        /// <summary>
        /// Sets a range of bits to the specified value.
        /// </summary>
        /// <param name="start">The index of the bit at the start of the range (inclusive).</param>
        /// <param name="end">The index of the bit at the end of the range (inclusive).</param>
        /// <param name="value">The value to set the bits to.</param>
        /// <returns>A reference to this instance.</returns>
        public Bitmap SetRange(int start, int end, bool value)
        {
            if (!this.IsValidIndex(start))
            {
                throw new ArgumentOutOfRangeException(nameof(start));
            }
            if (!this.IsValidIndex(end))
            {
                throw new ArgumentOutOfRangeException(nameof(end));
            }
            if (start > end)
            {
                throw new ArgumentException("Range is inverted.", nameof(end));
            }
            if (start == end)
            {
                return this.SetBit(start, value);
            }

            int startBucket = start / BITS_IN_ULONG;
            int startOffset = start % BITS_IN_ULONG;
            int endBucket = end / BITS_IN_ULONG;
            int endOffset = end % BITS_IN_ULONG;

            this.bits[startBucket] |= ulong.MaxValue << startOffset;
            for (int bucketIndex = startBucket + 1; bucketIndex < endBucket; bucketIndex++)
            {
                this.bits[bucketIndex] = ulong.MaxValue;
            }
            this.bits[endBucket] |= ulong.MaxValue >> (BITS_IN_ULONG - endOffset - 1);

            return this;
        }

        /// <summary>
        /// Gets the individual set of bits that are enabled in this bitmap.
        /// </summary>
        /// <returns>
        /// An <see cref="ISet"/> object containing the index of all enabled bits.
        /// </returns>
        public ISet<int> GetTrueBits()
        {
            HashSet<int> trueBits = new HashSet<int>();

            for (int bucketIndex = 0; bucketIndex < this.bits.Length; bucketIndex++)
            {
                var bucket = this.bits[bucketIndex];
                if (bucket == 0)
                {
                    continue;
                }

                ulong bucketScan = bucket;
                int bitIndex = 0;
                do
                {
                    if ((bucketScan & 0x1) > 0)
                    {
                        trueBits.Add((bucketIndex * BITS_IN_ULONG) + bitIndex);
                    }

                    bucketScan = bucketScan >> 1;
                    bitIndex++;
                } while (bucketScan > 0);
            }

            return trueBits;
        }

        /// <inheritdoc/>
        public IEnumerator<bool> GetEnumerator()
        {
            for (int index = 0; index < this.Length; index++)
            {
                yield return this.GetBit(index);
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is Bitmap bitmap && Enumerable.SequenceEqual(this, bitmap);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hashCode = 671604886;
            hashCode = this.bits.Take(this.Length)
                .Aggregate(hashCode, (agg, next) => (agg * -1521134295) + next.GetHashCode());
            hashCode = (hashCode * -1521134295) + this.Length.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Checks whether or not the specified index is valid (within bounds) for this
        /// <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="index">The index to check.</param>
        /// <returns>True if valid; otherwise, false.</returns>
        private bool IsValidIndex(int index)
        {
            return index >= 0 && index < this.Length;
        }
    }
}
