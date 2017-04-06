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

        [Theory]
        [WithSolidFilledImages(100, 100, 126, 0, 0, PixelTypes.StandardImageClass)]
        public void CanSaveIndexedPngAlphaBug<TColor>(TestImageProvider<TColor> provider)
            where TColor : struct, IPixel<TColor>
        {
            // does saving a file then repoening mean both files are identical???
            using (Image<TColor> image = provider.GetImage())
            using (MemoryStream ms = new MemoryStream())
            {
                image.MetaData.Quality = 256;
                image.Save(ms, new PngEncoder());
                ms.Position = 0;
                image.DebugSave(provider, extension: "png");

                using (Image<TColor> img2 = Image.Load<TColor>(ms, new PngDecoder()))
                {
                    img2.DebugSave(provider, new { decoded = true }, extension: "png");

                    using (PixelAccessor<TColor> accessor1 = image.Lock())
                    using (PixelAccessor<TColor> accessor2 = img2.Lock())
                    {
                        for (int y = 0; y < image.Height; y++)
                        {
                            for (int x = 0; x < image.Width; x++)
                            {
                                Assert.Equal(accessor1[x, y], accessor2[x, y]);
                            }
                        }
                    }
                }
            }
        }

        [Theory]
        [WithTestPatternImages(10, 2, PixelTypes.StandardImageClass)]
        public void CanSaveIndexedPng<TColor>(TestImageProvider<TColor> provider)
            where TColor : struct, IPixel<TColor>
        {
            // does saving a file then repoening mean both files are identical???
            using (Image<TColor> image = provider.GetImage())
            using (MemoryStream ms = new MemoryStream())
            {
                image.DebugSave(provider, new { raw = true }, extension: "png");
                image.MetaData.Quality = 256;
                image.Save(ms, new PngEncoder());
                ms.Position = 0;
                image.DebugSave(provider, extension: "png");

                using (Image<TColor> img2 = Image.Load<TColor>(ms, new PngDecoder()))
                {
                    img2.DebugSave(provider, new { decoded = true }, extension: "png");
                    ImageComparer.CheckSimilarity(image, img2);
                }
            }
        }
    }
}
