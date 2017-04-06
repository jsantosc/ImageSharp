// <copyright file="DitherTest.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using ImageSharp.Dithering;
    using ImageSharp.Dithering.Ordered;

    using Xunit;

    public class DitherTest : FileTestBase
    {
        public static Dictionary<string, Func<IOrderedDither>> DitherersFunc = new Dictionary<string, Func<IOrderedDither>>(StringComparer.OrdinalIgnoreCase) {
             { "Ordered", ()=>new Ordered() },
            { "Bayer", ()=>new Bayer() }
        };

        public static readonly TheoryData<string> Ditherers = new TheoryData<string>
        {
            { "Ordered"},
            { "Bayer" }
        };

        [Theory]
        [WithTestPatternImages(nameof(Ditherers), 640, 480, PixelTypes.Color)]
        public void ImageShouldApplyDitherFilter<TColor>(TestImageProvider<TColor> provider, string name)
            where TColor : struct, IPixel<TColor>
        {
            IOrderedDither ditherer = DitherersFunc[name]();

            using (Image<TColor> image = provider.GetImage())
            {
                image.Dither(ditherer)
                    .DebugSave(provider, new
                    {
                        ditherer = name
                    });
            }
        }

        [Theory]
        [WithTestPatternImages(nameof(Ditherers), 640, 480, PixelTypes.Color)]
        public void ImageShouldApplyDitherFilterInBox<TColor>(TestImageProvider<TColor> provider, string name)
            where TColor : struct, IPixel<TColor>
        {
            IOrderedDither ditherer = DitherersFunc[name]();
            using (Image<TColor> image = provider.GetImage())
            {
                image.Dither(ditherer, new Rectangle(100, 100, image.Width / 2, image.Height / 2))
                    .DebugSave(provider, new
                    {
                        ditherer = name
                    });
            }
        }
    }
}