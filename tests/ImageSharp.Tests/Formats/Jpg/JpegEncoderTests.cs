// <copyright file="JpegEncoderTests.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImageSharp.Formats;
using Xunit;
using Xunit.Abstractions;
// ReSharper disable InconsistentNaming

namespace ImageSharp.Tests
{
    using ImageSharp.Formats.Jpg;
    using ImageSharp.Processing;

    public class JpegEncoderTests : MeasureFixture
    {
        public static IEnumerable<string> AllBmpFiles => TestImages.Bmp.All;

        public JpegEncoderTests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Theory]
        [WithBlankImages(1, 1, PixelTypes.All)]
        public void WritesFileMarker<TColor>(TestImageProvider<TColor> provider)
           where TColor : struct, IPixel<TColor>
        {
            using (Image<TColor> image = provider.GetImage())
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, new JpegEncoder());

                byte[] data = ms.ToArray().Take(11).ToArray();
                byte[] expected = new byte[]
                {
                    0xff,
                    0xd8,
                    
                    // Write the JFIF headers
                    0xff,
                    0xe0, // Application Marker
                    0x00,
                    0x10,
                    0x4a, // J
                    0x46, // F
                    0x49, // I
                    0x46, // F
                    0x00, // = "JFIF",'\0'
                };
                Assert.Equal(expected, data);
            }
        }

        // what exactly is this proving ???
        //[Theory]
        //[WithFile(TestImages.Jpeg.Baseline.Snake, PixelTypes.StandardImageClass, 75, JpegSubsample.Ratio420)]
        //[WithFile(TestImages.Jpeg.Baseline.Lake, PixelTypes.StandardImageClass, 75, JpegSubsample.Ratio420)]
        //[WithFile(TestImages.Jpeg.Baseline.Snake, PixelTypes.StandardImageClass, 75, JpegSubsample.Ratio444)]
        //[WithFile(TestImages.Jpeg.Baseline.Lake, PixelTypes.StandardImageClass, 75, JpegSubsample.Ratio444)]
        //public void LoadResizeSave<TColor>(TestImageProvider<TColor> provider, int quality, JpegSubsample subsample)
        //    where TColor : struct, IPixel<TColor>
        //{
        //    using (Image<TColor> image = provider.GetImage().Resize(new ResizeOptions { Size = new Size(150, 100), Mode = ResizeMode.Max }))
        //    {
        //        image.MetaData.Quality = quality;
        //        image.MetaData.ExifProfile = null; // Reduce the size of the file
        //        JpegEncoder encoder = new JpegEncoder();
        //        JpegEncoderOptions options = new JpegEncoderOptions { Subsample = subsample, Quality = quality };

        //        provider.Utility.TestName += $"{subsample}_Q{quality}";
        //        provider.Utility.SaveTestOutputFile(image, "png");
        //        provider.Utility.SaveTestOutputFile(image, "jpg", encoder, options);
        //    }
        //}

        [Theory]
        [WithTestPatternImages(320, 240, PixelTypes.Color | PixelTypes.StandardImageClass | PixelTypes.Argb, JpegSubsample.Ratio420, 75)]
        [WithTestPatternImages(320, 240, PixelTypes.Color | PixelTypes.StandardImageClass | PixelTypes.Argb, JpegSubsample.Ratio444, 75)]
        public void SaveTestPatternAsJpeg<TColor>(TestImageProvider<TColor> provider, JpegSubsample subSample, int quality)
           where TColor : struct, IPixel<TColor>
        {
            using (Image<TColor> image = provider.GetImage())
            {
                ImagingTestCaseUtility utility = provider.Utility;
                utility.TestName += "_" + subSample + "_Q" + quality;

                using (MemoryStream outputStream = new MemoryStream())
                {
                    JpegEncoder encoder = new JpegEncoder();

                    image.Save(outputStream, encoder, new JpegEncoderOptions()
                    {
                        Subsample = subSample,
                        Quality = quality
                    });

                    outputStream.DebugSave(provider, extension: "jpg");
                }
            }
        }

        [Fact]
        public void Encode_IgnoreMetadataIsFalse_ExifProfileIsWritten()
        {
            EncoderOptions options = new EncoderOptions()
            {
                IgnoreMetadata = false
            };

            TestFile testFile = TestFile.Create(TestImages.Jpeg.Baseline.Floorplan);

            using (Image input = testFile.CreateImage())
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    input.Save(memStream, new JpegFormat(), options);

                    memStream.Position = 0;
                    using (Image output = Image.Load(memStream))
                    {
                        Assert.NotNull(output.MetaData.ExifProfile);
                    }
                }
            }
        }

        [Fact]
        public void Encode_IgnoreMetadataIsTrue_ExifProfileIgnored()
        {
            JpegEncoderOptions options = new JpegEncoderOptions()
            {
                IgnoreMetadata = true
            };

            TestFile testFile = TestFile.Create(TestImages.Jpeg.Baseline.Floorplan);

            using (Image input = testFile.CreateImage())
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    input.SaveAsJpeg(memStream, options);

                    memStream.Position = 0;
                    using (Image output = Image.Load(memStream))
                    {
                        Assert.Null(output.MetaData.ExifProfile);
                    }
                }
            }
        }
    }
}