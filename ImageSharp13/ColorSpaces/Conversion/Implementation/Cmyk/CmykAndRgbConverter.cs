﻿// <copyright file="CmykAndRgbConverter.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.ColorSpaces.Conversion.Implementation.Cmyk
{
    using System;
    using System.Runtime.CompilerServices;

    using ImageSharp.ColorSpaces;

    /// <summary>
    /// Color converter between CMYK and Rgb
    /// </summary>
    internal class CmykAndRgbConverter : IColorConversion<Cmyk, Rgb>, IColorConversion<Rgb, Cmyk>
    {
        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rgb Convert(Cmyk input)
        {
            float r = (1F - input.C) * (1F - input.K);
            float g = (1F - input.M) * (1F - input.K);
            float b = (1F - input.Y) * (1F - input.K);

            return new Rgb(r, g, b);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Cmyk Convert(Rgb input)
        {
            // To CMYK
            float c = 1F - input.R;
            float m = 1F - input.G;
            float y = 1F - input.B;

            // To CMYK
            float k = MathF.Min(c, MathF.Min(m, y));

            if (MathF.Abs(k - 1F) < Constants.Epsilon)
            {
                return new Cmyk(0, 0, 0, 1F);
            }

            c = (c - k) / (1F - k);
            m = (m - k) / (1F - k);
            y = (y - k) / (1F - k);

            return new Cmyk(c, m, y, k);
        }
    }
}