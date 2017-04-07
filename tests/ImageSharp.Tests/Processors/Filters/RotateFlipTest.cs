// <copyright file="RotateFlipTest.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Tests
{
    using System.IO;
    using Processing;
    using Xunit;

    public class RotateFlipTest
    {
        public static readonly TheoryData<RotateType, FlipType> RotateFlipValues
            = new TheoryData<RotateType, FlipType>
        {
            { RotateType.None, FlipType.Vertical },
            { RotateType.None, FlipType.Horizontal },
            { RotateType.Rotate90, FlipType.None },
            { RotateType.Rotate180, FlipType.None },
            { RotateType.Rotate270, FlipType.None },
        };

        [Theory]
        [WithTestPatternImages(nameof(RotateFlipValues), 320, 240, PixelTypes.StandardImageClass)]
        public void ImageShouldRotateFlip<TColor>(TestImageProvider<TColor> provider, RotateType rotateType, FlipType flipType)
            where TColor : struct, IPixel<TColor>
        {
            using (Image<TColor> image = provider.GetImage())
            {
                image.RotateFlip(rotateType, flipType)
                .DebugSave(provider, new
                {
                    rotateType,
                    flipType
                });
            }
        }
    }
}