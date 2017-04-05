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

    public class PngSmokeTests
    {
        public const PixelTypes PixelTypesToTest = PixelTypes.StandardImageClass;

        [Theory]
        [WithTestPatternImages(300, 300, PixelTypesToTest)]
        public void GeneralTest<TColor>(TestImageProvider<TColor> provider)
            where TColor : struct, IPixel<TColor>
        {
            // does saving a file then repoening mean both files are identical???
            using (Image<TColor> image = provider.GetImage())
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, new PngEncoder());
                ms.Position = 0;
                image.DebugSave(provider, extension: "bmp");
                using (Image<TColor> img2 = Image.Load<TColor>(ms, new PngDecoder()))
                {
                    img2.DebugSave(provider, new { decoded = true }, extension: "bmp");
                    ImageComparer.CheckSimilarity(image, img2);
                }
            }
        }

        [Theory(Skip = "failing see https://github.com/JimBobSquarePants/ImageSharp/issues/165")]
        [WithSolidFilledImages(10, 10, 255,0,0, PixelTypes.StandardImageClass)]
        public void CanSaveIndexedPng<TColor>(TestImageProvider<TColor> provider)
            where TColor : struct, IPixel<TColor>
        {
            // does saving a file then repoening mean both files are identical???
            using (Image<TColor> image = provider.GetImage())
            using (MemoryStream ms = new MemoryStream())
            {
                image.MetaData.Quality = 256;
                image.Save(ms, new PngEncoder());
                ms.Position = 0;
                image.DebugSave(provider, extension: "bmp");

                using (Image<TColor> img2 = Image.Load<TColor>(ms, new PngDecoder()))
                {
                    
                    img2.DebugSave(provider, new { decoded = true }, extension: "bmp");
                    Assert.Equal(img2.Pixels[0], image.Pixels[0]);
                    //ImageComparer.CheckSimilarity(image, img2);
                }
            }
        }
    }
}
