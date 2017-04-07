// <copyright file="PolaroidTest.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Tests
{
    using System.IO;

    using Xunit;

    public class PolaroidTest : FileTestBase
    {
        [Theory]
        [WithTestPatternImages(324, 240, PixelTypes.Color)]
        public void ImageShouldApplyPolaroidFilter<TColor>(TestImageProvider<TColor> provider)
            where TColor : struct, IPixel<TColor>
        {
            using (Image<TColor> image = provider.GetImage())
            {
                image.Polaroid()
                    .DebugSave(provider);
            }
        }
    }
}