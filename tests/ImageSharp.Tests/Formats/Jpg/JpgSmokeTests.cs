// <copyright file="PngSmokeTests.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Tests.Formats.Png
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    using Xunit;
    using ImageSharp.Formats;
    using System.Linq;
    using ImageSharp.IO;

    public class JpgSmokeTests
    {
        public const PixelTypes PixelTypesToTest = PixelTypes.StandardImageClass | PixelTypes.Argb;

        [Theory]
        [WithTestPatternImages(320, 240, PixelTypesToTest, JpegSubsample.Ratio420, 75)]
        [WithTestPatternImages(320, 240, PixelTypesToTest, JpegSubsample.Ratio444, 75)]
        public void GeneralTest<TColor>(TestImageProvider<TColor> provider, JpegSubsample subSample, int quality)
            where TColor : struct, IPixel<TColor>
        {
            using (Image<TColor> image = provider.GetImage())
            using (MemoryStream ms = new MemoryStream())
            {
                JpegEncoderOptions options = new JpegEncoderOptions { Subsample = subSample, Quality = quality };
                image.Save(ms, new JpegEncoder(), options);
                ms.Position = 0;
                image.DebugSave(provider, new
                {
                    subSample,
                    quality
                }, extension: "bmp");
                using (Image<TColor> img2 = Image.Load<TColor>(ms, new JpegDecoder()))
                {
                    img2.DebugSave(provider, new
                    {
                        subSample,
                        quality,
                        decoded = true
                    }, extension: "bmp");
                    ImageComparer.CheckSimilarity(image, img2);
                }
            }
        }
    }
}
