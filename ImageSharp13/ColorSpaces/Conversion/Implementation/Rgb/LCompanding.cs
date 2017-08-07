﻿// <copyright file="LCompanding.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.ColorSpaces.Conversion.Implementation.Rgb
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Implements L* companding
    /// </summary>
    /// <remarks>
    /// For more info see:
    /// <see href="http://www.brucelindbloom.com/index.html?Eqn_RGB_to_XYZ.html"/>
    /// <see href="http://www.brucelindbloom.com/index.html?Eqn_XYZ_to_RGB.html"/>
    /// </remarks>
    public class LCompanding : ICompanding
    {
        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Expand(float channel)
        {
            return channel <= 0.08 ? 100 * channel / CieConstants.Kappa : MathF.Pow((channel + 0.16F) / 1.16F, 3);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Compress(float channel)
        {
            return channel <= CieConstants.Epsilon
                ? channel * CieConstants.Kappa / 100F
                : MathF.Pow(1.16F * channel, 0.3333333F) - 0.16F;
        }
    }
}
