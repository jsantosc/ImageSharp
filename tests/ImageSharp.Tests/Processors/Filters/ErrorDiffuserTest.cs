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

    public class ErrorDiffuserTest : FileTestBase
    {
        public static Dictionary<string, Func<IErrorDiffuser>> ErrorDiffusersFunc = new Dictionary<string, Func<IErrorDiffuser>>(StringComparer.OrdinalIgnoreCase) {
            { "Atkinson", () => new Atkinson() },
            { "Burks", () => new Burks() },
            { "FloydSteinberg", () => new FloydSteinberg() },
            { "JarvisJudiceNinke", () => new JarvisJudiceNinke() },
            { "Sierra2", () => new Sierra2() },
            { "Sierra3", () => new Sierra3() },
            { "SierraLite", () => new SierraLite() },
            { "Stucki", () => new Stucki() },
        };
        
        public static readonly TheoryData<string> ErrorDiffusers = new TheoryData<string>
        {
            { "Atkinson" },
            { "Burks" },
            { "FloydSteinberg" },
            { "JarvisJudiceNinke" },
            { "Sierra2" },
            { "Sierra3" },
            { "SierraLite" },
            { "Stucki" },
        };

        [Theory]
        [WithTestPatternImages(nameof(ErrorDiffusers), 640, 480, PixelTypes.Color)]
        public void ImageShouldApplyDiffusionFilter<TColor>(TestImageProvider<TColor> provider, string name)
            where TColor : struct, IPixel<TColor>
        {
            IErrorDiffuser diffuser = ErrorDiffusersFunc[name]();
            using (Image<TColor> image = provider.GetImage())
            {
                image.Dither(diffuser, .5F)
                    .DebugSave(provider, new
                    {
                        diffuser = name
                    });
            }
        }

        [Theory]
        [WithTestPatternImages(nameof(ErrorDiffusers), 640, 480, PixelTypes.Color)]
        public void ImageShouldApplyDiffusionFilterInBox<TColor>(TestImageProvider<TColor> provider, string name)
            where TColor : struct, IPixel<TColor>
        {
            IErrorDiffuser diffuser = ErrorDiffusersFunc[name]();
            using (Image<TColor> image = provider.GetImage())
            {
                image.Dither(diffuser, .5F, new Rectangle(100, 100, image.Width / 2, image.Height / 2))
                    .DebugSave(provider, new
                    {
                        diffuser = name
                    });
            }
        }
    }
}