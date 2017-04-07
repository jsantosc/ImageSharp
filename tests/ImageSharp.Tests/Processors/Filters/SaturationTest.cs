// <copyright file="SaturationTest.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Tests
{
    using System.IO;

    using Xunit;

    public class SaturationTest : FileTestBase
    {
        public static readonly TheoryData<int> SaturationValues
        = new TheoryData<int>
        {
            50 ,
           -50 ,
        };

        [Theory]
        [WithTestPatternImages(nameof(SaturationValues), 324, 240, PixelTypes.Color)]
        public void ImageShouldApplySaturationFilter<TColor>(TestImageProvider<TColor> provider, int value)
            where TColor : struct, IPixel<TColor>
        {
            using (Image<TColor> image = provider.GetImage())
            {
                image
                    .Saturation(value)
                    .DebugSave(provider, new { satuation = value });
            }
        }
    }
}