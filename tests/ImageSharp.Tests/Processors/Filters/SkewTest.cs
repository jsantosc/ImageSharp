// <copyright file="SkewTest.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Tests
{
    using System.IO;

    using Xunit;

    public class SkewTest
    {
        public static readonly TheoryData<float, float> SkewValues
        = new TheoryData<float, float>
        {
            { 20, 10 },
            { -20, -10 }
        };

        [Theory]
        [WithTestPatternImages(nameof(SkewValues), 640, 480, PixelTypes.StandardImageClass)]
        public void ImageShouldApplySkewSampler<TColor>(TestImageProvider<TColor> provider, float x, float y)
            where TColor : struct, IPixel<TColor>
        {
            // Matches live example
            // http://www.w3schools.com/css/tryit.asp?filename=trycss3_transform_skew
            using (Image<TColor> image = provider.GetImage())
            {
                image.Skew(x, y)
                    .DebugSave(provider, new
                    {
                        x,
                        y
                    });
            }
        }
    }
}