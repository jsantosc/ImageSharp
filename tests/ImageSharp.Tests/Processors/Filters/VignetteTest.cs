// <copyright file="VignetteTest.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Tests
{
    using System.IO;

    using Xunit;

    public class VignetteTest
    {
        [Theory]
        [WithTestPatternImages(640, 480, PixelTypes.StandardImageClass)]
        public void ImageShouldApplyVignetteFilter<TColor>(TestImageProvider<TColor> provider)
            where TColor : struct, IPixel<TColor>
        {
            using (Image<TColor> image = provider.GetImage())
            {
                image.Vignette()
                     .DebugSave(provider);
            }
        }

        [Theory]
        [WithTestPatternImages(640, 480, PixelTypes.StandardImageClass)]
        public void ImageShouldApplyVignetteFilterColor<TColor>(TestImageProvider<TColor> provider)
            where TColor : struct, IPixel<TColor>
        {
            using (Image<TColor> image = provider.GetImage())
            {
                image.Vignette(NamedColors<TColor>.HotPink)
                     .DebugSave(provider);
            }
        }

        [Theory]
        [WithTestPatternImages(640, 480, PixelTypes.StandardImageClass)]
        public void ImageShouldApplyVignetteFilterRadius<TColor>(TestImageProvider<TColor> provider)
            where TColor : struct, IPixel<TColor>
        {
            using (Image<TColor> image = provider.GetImage())
            {
                image.Vignette(image.Width / 4F, image.Height / 4F)
                     .DebugSave(provider);
            }
        }

        [Theory]
        [WithTestPatternImages(640, 480, PixelTypes.StandardImageClass)]
        public void ImageShouldApplyVignetteFilterInBox<TColor>(TestImageProvider<TColor> provider)
            where TColor : struct, IPixel<TColor>
        {
            using (Image<TColor> image = provider.GetImage())
            {
                image.Vignette(new Rectangle(image.Width / 4, image.Height / 4, image.Width / 2, image.Height / 2))
                     .DebugSave(provider);
            }
        }
    }
}