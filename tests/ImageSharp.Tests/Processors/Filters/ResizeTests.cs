// <copyright file="ResizeTests.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>
namespace ImageSharp.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Processing;
    using Xunit;

    public class ResizeTests
    {
        public static Dictionary<string, Func<IResampler>> ResamplerFactories = new Dictionary<string, Func<IResampler>>
        {
              { "Bicubic", ()=> new BicubicResampler() },
                    { "Triangle", ()=> new TriangleResampler() },
                    { "NearestNeighbor", ()=> new NearestNeighborResampler() },

                    { "Box",()=>  new BoxResampler() },
                    { "Lanczos3", ()=> new Lanczos3Resampler() },
                    { "Lanczos5", ()=> new Lanczos5Resampler() },
                    { "MitchellNetravali", ()=> new MitchellNetravaliResampler() },

                    { "Lanczos8", ()=> new Lanczos8Resampler() },
                    { "Hermite", ()=> new HermiteResampler() },
                    { "Spline", ()=> new SplineResampler() },
                    { "Robidoux",()=>  new RobidouxResampler() },
                    { "RobidouxSharp",()=>  new RobidouxSharpResampler() },
                    { "Welch", ()=> new WelchResampler() }
        };

        public static readonly TheoryData<string> ReSamplers =
            new TheoryData<string>
                {
                    { "Bicubic" },
                    { "Triangle" },
                    { "NearestNeighbor"},

                    // Perf: Enable for local testing only
                    // { "Box" },
                    // { "Lanczos3"},
                    // { "Lanczos5" },
                    { "MitchellNetravali" },

                    // { "Lanczos8" },
                    // { "Hermite" },
                    // { "Spline" },
                    // { "Robidoux" },
                    // { "RobidouxSharp" },
                    // { "Welch" }
                };

        [Theory]
        [WithTestPatternImages(nameof(ReSamplers), 480, 360, PixelTypes.Color)]
        public void ImageShouldResize<TColor>(TestImageProvider<TColor> provider, string name)
            where TColor : struct, IPixel<TColor>
        {
            IResampler sampler = ResamplerFactories[name]();

            using (Image<TColor> image = provider.GetImage())
            {
                image.Resize(image.Width / 2, image.Height / 2, sampler, true)
                    .DebugSave(provider, new
                    {
                        name = sampler.GetType().Name
                    });

                Assert.Equal(240, image.Width);
                Assert.Equal(180, image.Height);
            }
        }

        [Theory]
        [WithTestPatternImages(nameof(ReSamplers), 480, 360, PixelTypes.Color)]
        public void ImageShouldResizeFromSourceRectangle<TColor>(TestImageProvider<TColor> provider, string name)
            where TColor : struct, IPixel<TColor>
        {
            IResampler sampler = ResamplerFactories[name]();

            using (Image<TColor> image = provider.GetImage())
            {
                Rectangle sourceRectangle = new Rectangle(image.Width / 8, image.Height / 8, image.Width / 4, image.Height / 4);
                Rectangle destRectangle = new Rectangle(image.Width / 4, image.Height / 4, image.Width / 2, image.Height / 2);
                image.Resize(image.Width, image.Height, sampler, sourceRectangle, destRectangle, false)
                .DebugSave(provider, new
                {
                    name = sampler.GetType().Name
                });

                Assert.Equal(480, image.Width);
                Assert.Equal(360, image.Height);
            }
        }

        [Theory]
        //[WithTestPatternImages(480, 360, PixelTypes.Color, "Bicubic", "NearestNeighbor")]
        [WithTestPatternImages(480, 360, PixelTypes.Color, "Bicubic", "Triangle")]
        public void ImageShouldResizeFromSourceRectangleCompared<TColor>(TestImageProvider<TColor> provider, string sampler1, string sampler2)
            where TColor : struct, IPixel<TColor>
        {
            List<Image<TColor>> images = new List<Image<TColor>>();

            foreach (string name in ReSamplers.Select(x => x.First().ToString()))
            {
                using (Image<TColor> image = provider.GetImage())
                using (Image<TColor> sampler1Image = new Image<TColor>(image))
                using (Image<TColor> sampler2Image = new Image<TColor>(image))
                {

                    Rectangle sourceRectangle = new Rectangle(image.Width / 8, image.Height / 8, image.Width / 4, image.Height / 4);
                    Rectangle destRectangle = new Rectangle(image.Width / 4, image.Height / 4, image.Width / 2, image.Height / 2);


                    IResampler sampler = ResamplerFactories[sampler1]();
                    sampler1Image.Resize(image.Width, image.Height, sampler, sourceRectangle, destRectangle, false)
                        .DebugSave(provider, new
                        {
                            name = sampler.GetType().Name
                        });
                    Assert.Equal(480, sampler1Image.Width);
                    Assert.Equal(360, sampler1Image.Height);


                    sampler = ResamplerFactories[sampler2]();
                    sampler2Image.Resize(image.Width, image.Height, sampler, sourceRectangle, destRectangle, false)
                        .DebugSave(provider, new
                        {
                            name = sampler.GetType().Name
                        });
                    Assert.Equal(480, sampler2Image.Width);
                    Assert.Equal(360, sampler2Image.Height);

                    ImageComparer.CheckSimilarity(sampler1Image, sampler2Image);
                }
            }
        }

        [Theory]
        [WithTestPatternImages(nameof(ReSamplers), 480, 360, PixelTypes.Color)]
        public void ImageShouldResizeWidthAndKeepAspect<TColor>(TestImageProvider<TColor> provider, string name)
            where TColor : struct, IPixel<TColor>
        {
            IResampler sampler = ResamplerFactories[name]();
            using (Image<TColor> image = provider.GetImage())
            {
                image.Resize(image.Width / 3, 0, sampler, false)
                .DebugSave(provider, new
                {
                    name = sampler.GetType().Name
                });
            }
        }

        [Theory]
        [WithTestPatternImages(nameof(ReSamplers), 480, 360, PixelTypes.Color)]
        public void ImageShouldResizeHeightAndKeepAspect<TColor>(TestImageProvider<TColor> provider, string name)
            where TColor : struct, IPixel<TColor>
        {
            IResampler sampler = ResamplerFactories[name]();
            using (Image<TColor> image = provider.GetImage())
            {
                image.Resize(0, image.Height / 3, sampler, false)
                .DebugSave(provider, new
                {
                    name = sampler.GetType().Name
                });
            }
        }

        [Theory]
        [WithTestPatternImages(nameof(ReSamplers), 480, 360, PixelTypes.Color)]
        public void ImageShouldResizeWithCropWidthMode<TColor>(TestImageProvider<TColor> provider, string name)
            where TColor : struct, IPixel<TColor>
        {
            IResampler sampler = ResamplerFactories[name]();
            using (Image<TColor> image = provider.GetImage())
            {
                ResizeOptions options = new ResizeOptions
                {
                    Sampler = sampler,
                    Size = new Size(image.Width / 2, image.Height)
                };

                image.Resize(options)
                    .DebugSave(provider, new
                    {
                        name = sampler.GetType().Name
                    });
            }
        }

        [Theory]
        [WithTestPatternImages(nameof(ReSamplers), 480, 360, PixelTypes.Color)]
        public void ImageShouldResizeWithCropHeightMode<TColor>(TestImageProvider<TColor> provider, string name)
            where TColor : struct, IPixel<TColor>
        {
            IResampler sampler = ResamplerFactories[name]();
            using (Image<TColor> image = provider.GetImage())
            {
                ResizeOptions options = new ResizeOptions
                {
                    Sampler = sampler,
                    Size = new Size(image.Width, image.Height / 2)
                };

                image.Resize(options)
                    .DebugSave(provider, new
                    {
                        name = sampler.GetType().Name
                    });
            }
        }

        [Theory]
        [WithTestPatternImages(nameof(ReSamplers), 480, 360, PixelTypes.Color)]
        public void ImageShouldResizeWithPadMode<TColor>(TestImageProvider<TColor> provider, string name)
            where TColor : struct, IPixel<TColor>
        {
            IResampler sampler = ResamplerFactories[name]();
            using (Image<TColor> image = provider.GetImage())
            {
                ResizeOptions options = new ResizeOptions
                {
                    Size = new Size(image.Width + 200, image.Height),
                    Mode = ResizeMode.Pad
                };

                image.Resize(options)
                    .DebugSave(provider, new
                    {
                        name = sampler.GetType().Name
                    });
            }
        }

        [Theory]
        [WithTestPatternImages(nameof(ReSamplers), 480, 360, PixelTypes.Color)]
        public void ImageShouldResizeWithBoxPadMode<TColor>(TestImageProvider<TColor> provider, string name)
            where TColor : struct, IPixel<TColor>
        {
            IResampler sampler = ResamplerFactories[name]();
            using (Image<TColor> image = provider.GetImage())
            {
                ResizeOptions options = new ResizeOptions
                {
                    Sampler = sampler,
                    Size = new Size(image.Width + 200, image.Height + 200),
                    Mode = ResizeMode.BoxPad
                };


                image.Resize(options)
                    .DebugSave(provider, new
                    {
                        name = sampler.GetType().Name
                    });
            }
        }

        [Theory]
        [WithTestPatternImages(nameof(ReSamplers), 480, 360, PixelTypes.Color)]
        public void ImageShouldResizeWithMaxMode<TColor>(TestImageProvider<TColor> provider, string name)
            where TColor : struct, IPixel<TColor>
        {
            IResampler sampler = ResamplerFactories[name]();
            using (Image<TColor> image = provider.GetImage())
            {
                ResizeOptions options = new ResizeOptions
                {
                    Sampler = sampler,
                    Size = new Size(300, 300),
                    Mode = ResizeMode.Max
                };

                image.Resize(options)
                    .DebugSave(provider, new
                    {
                        name = sampler.GetType().Name
                    });
            }
        }

        [Theory]
        [WithTestPatternImages(nameof(ReSamplers), 480, 360, PixelTypes.Color)]
        public void ImageShouldResizeWithMinMode<TColor>(TestImageProvider<TColor> provider, string name)
            where TColor : struct, IPixel<TColor>
        {
            IResampler sampler = ResamplerFactories[name]();
            using (Image<TColor> image = provider.GetImage())
            {
                ResizeOptions options = new ResizeOptions
                {
                    Sampler = sampler,
                    Size = new Size((int)Math.Round(image.Width * .75F), (int)Math.Round(image.Height * .95F)),
                    Mode = ResizeMode.Min
                };

                image.Resize(options)
                    .DebugSave(provider, new
                    {
                        name = sampler.GetType().Name
                    });
            }
        }

        [Theory]
        [WithTestPatternImages(nameof(ReSamplers), 480, 360, PixelTypes.Color)]
        public void ImageShouldResizeWithStretchMode<TColor>(TestImageProvider<TColor> provider, string name)
            where TColor : struct, IPixel<TColor>
        {
            IResampler sampler = ResamplerFactories[name]();
            using (Image<TColor> image = provider.GetImage())
            {
                ResizeOptions options = new ResizeOptions
                {
                    Sampler = sampler,
                    Size = new Size(image.Width / 2, image.Height),
                    Mode = ResizeMode.Stretch
                };
                image.Resize(options)
                    .DebugSave(provider, new
                    {
                        name = sampler.GetType().Name
                    });
            }
        }

        [Theory]
        [InlineData(-2, 0)]
        [InlineData(-1, 0)]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        [InlineData(2, 0)]
        public static void BicubicWindowOscillatesCorrectly(float x, float expected)
        {
            BicubicResampler sampler = new BicubicResampler();
            float result = sampler.GetValue(x);

            Assert.Equal(result, expected);
        }

        [Theory]
        [InlineData(-2, 0)]
        [InlineData(-1, 0)]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        [InlineData(2, 0)]
        public static void TriangleWindowOscillatesCorrectly(float x, float expected)
        {
            TriangleResampler sampler = new TriangleResampler();
            float result = sampler.GetValue(x);

            Assert.Equal(result, expected);
        }

        [Theory]
        [InlineData(-2, 0)]
        [InlineData(-1, 0)]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        [InlineData(2, 0)]
        public static void Lanczos3WindowOscillatesCorrectly(float x, float expected)
        {
            Lanczos3Resampler sampler = new Lanczos3Resampler();
            float result = sampler.GetValue(x);

            Assert.Equal(result, expected);
        }

        [Theory]
        [InlineData(-4, 0)]
        [InlineData(-2, 0)]
        [InlineData(0, 1)]
        [InlineData(2, 0)]
        [InlineData(4, 0)]
        public static void Lanczos5WindowOscillatesCorrectly(float x, float expected)
        {
            Lanczos5Resampler sampler = new Lanczos5Resampler();
            float result = sampler.GetValue(x);

            Assert.Equal(result, expected);
        }
    }
}