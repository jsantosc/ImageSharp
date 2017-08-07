﻿// <copyright file="Lms.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.ColorSpaces
{
    using System;
    using System.ComponentModel;
    using System.Numerics;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// LMS is a color space represented by the response of the three types of cones of the human eye,
    /// named after their responsivity (sensitivity) at long, medium and short wavelengths.
    /// <see href="https://en.wikipedia.org/wiki/LMS_color_space"/>
    /// </summary>
    internal struct Lms : IColorVector, IEquatable<Lms>, IAlmostEquatable<Lms, float>
    {
        /// <summary>
        /// Represents a <see cref="Lms"/> that has L, M, and S values set to zero.
        /// </summary>
        public static readonly Lms Empty = default(Lms);

        /// <summary>
        /// The backing vector for SIMD support.
        /// </summary>
        private readonly Vector3 backingVector;

        /// <summary>
        /// Initializes a new instance of the <see cref="Lms"/> struct.
        /// </summary>
        /// <param name="l">L represents the responsivity at long wavelengths.</param>
        /// <param name="m">M represents the responsivity at medium wavelengths.</param>
        /// <param name="s">S represents the responsivity at short wavelengths.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Lms(float l, float m, float s)
            : this(new Vector3(l, m, s))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Lms"/> struct.
        /// </summary>
        /// <param name="vector">The vector representing the l, m, s components.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Lms(Vector3 vector)
            : this()
        {
            // Not clamping as documentation about this space seems to indicate "usual" ranges
            this.backingVector = vector;
        }

        /// <summary>
        /// Gets the L long component.
        /// <remarks>A value usually ranging between -1 and 1.</remarks>
        /// </summary>
        public float L
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.backingVector.X;
        }

        /// <summary>
        /// Gets the M medium component.
        /// <remarks>A value usually ranging between -1 and 1.</remarks>
        /// </summary>
        public float M
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.backingVector.Y;
        }

        /// <summary>
        /// Gets the S short component.
        /// <remarks>A value usually ranging between -1 and 1.</remarks>
        /// </summary>
        public float S
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.backingVector.Z;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Lms"/> is empty.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsEmpty => this.Equals(Empty);

        /// <inheritdoc />
        public Vector3 Vector
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.backingVector;
        }

        /// <summary>
        /// Compares two <see cref="Lms"/> objects for equality.
        /// </summary>
        /// <param name="left">
        /// The <see cref="Lms"/> on the left side of the operand.
        /// </param>
        /// <param name="right">
        /// The <see cref="Lms"/> on the right side of the operand.
        /// </param>
        /// <returns>
        /// True if the current left is equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Lms left, Lms right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two <see cref="Lms"/> objects for inequality.
        /// </summary>
        /// <param name="left">
        /// The <see cref="Lms"/> on the left side of the operand.
        /// </param>
        /// <param name="right">
        /// The <see cref="Lms"/> on the right side of the operand.
        /// </param>
        /// <returns>
        /// True if the current left is unequal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Lms left, Lms right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.backingVector.GetHashCode();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (this.IsEmpty)
            {
                return "Lms [ Empty ]";
            }

            return $"Lms [ L={this.L:#0.##}, M={this.M:#0.##}, S={this.S:#0.##} ]";
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (obj is Lms)
            {
                return this.Equals((Lms)obj);
            }

            return false;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Lms other)
        {
            return this.backingVector.Equals(other.backingVector);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AlmostEquals(Lms other, float precision)
        {
            var result = Vector3.Abs(this.backingVector - other.backingVector);

            return result.X <= precision
                && result.Y <= precision
                && result.Z <= precision;
        }
    }
}