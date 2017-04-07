// <copyright file="RotateTest.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Tests
{
    using System.IO;
    using Processing;
    using Xunit;

    public class RotateTest 
    {
        public static readonly TheoryData<float> RotateFloatValues
            = new TheoryData<float>
        {
             170 ,
            -170 ,
        };

        public static readonly TheoryData<RotateType> RotateEnumValues
            = new TheoryData<RotateType>
        {
            RotateType.None,
            RotateType.Rotate90,
            RotateType.Rotate180,
            RotateType.Rotate270
        };

        [Theory]
        [WithTestPatternImages(nameof(RotateFloatValues), 320, 240, PixelTypes.Color)]
        public void ImageShouldApplyRotateSampler<TColor>(TestImageProvider<TColor> provider, float value)
            where TColor : struct, IPixel<TColor>
        {
            using (Image<TColor> image = provider.GetImage())
            {
                image.Rotate(value)
                .DebugSave(provider, new
                {
                    angle = value
                });
            }
        }

        [Theory]
        [WithTestPatternImages(nameof(RotateEnumValues), 320, 240, PixelTypes.Color)]
        public void ImageShouldApplyRotateSampler<TColor>(TestImageProvider<TColor> provider, RotateType value)
            where TColor : struct, IPixel<TColor>
        {
            using (Image<TColor> image = provider.GetImage())
            {
                image.Rotate(value)
                .DebugSave(provider, new
                {
                    type = value
                });
            }
        }
    }
}