﻿// <copyright file="BmpDecoderCore.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>
namespace ImageSharp.Formats
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using ImageSharp.Memory;
    using ImageSharp.PixelFormats;

    /// <summary>
    /// Performs the bmp decoding operation.
    /// </summary>
    internal sealed class BmpDecoderCore
    {
        /// <summary>
        /// The mask for the red part of the color for 16 bit rgb bitmaps.
        /// </summary>
        private const int Rgb16RMask = 0x00007C00;

        /// <summary>
        /// The mask for the green part of the color for 16 bit rgb bitmaps.
        /// </summary>
        private const int Rgb16GMask = 0x000003E0;

        /// <summary>
        /// The mask for the blue part of the color for 16 bit rgb bitmaps.
        /// </summary>
        private const int Rgb16BMask = 0x0000001F;

        /// <summary>
        /// The stream to decode from.
        /// </summary>
        private Stream currentStream;

        /// <summary>
        /// The file header containing general information.
        /// TODO: Why is this not used? We advance the stream but do not use the values parsed.
        /// </summary>
        private BmpFileHeader fileHeader;

        /// <summary>
        /// The info header containing detailed information about the bitmap.
        /// </summary>
        private BmpInfoHeader infoHeader;

        private Configuration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="BmpDecoderCore"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="options">The options</param>
        public BmpDecoderCore(Configuration configuration, IBmpDecoderOptions options)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// Decodes the image from the specified this._stream and sets
        /// the data to image.
        /// </summary>
        /// <typeparam name="TPixel">The pixel format.</typeparam>
        /// <param name="stream">The stream, where the image should be
        /// decoded from. Cannot be null (Nothing in Visual Basic).</param>
        /// <exception cref="System.ArgumentNullException">
        ///    <para><paramref name="stream"/> is null.</para>
        /// </exception>
        /// <returns>The decoded image.</returns>
        public Image<TPixel> Decode<TPixel>(Stream stream)
            where TPixel : struct, IPixel<TPixel>
        {
            this.currentStream = stream;

            try
            {
                this.ReadFileHeader();
                this.ReadInfoHeader();

                // see http://www.drdobbs.com/architecture-and-design/the-bmp-file-format-part-1/184409517
                // If the height is negative, then this is a Windows bitmap whose origin
                // is the upper-left corner and not the lower-left.The inverted flag
                // indicates a lower-left origin.Our code will be outputting an
                // upper-left origin pixel array.
                bool inverted = false;
                if (this.infoHeader.Height < 0)
                {
                    inverted = true;
                    this.infoHeader.Height = -this.infoHeader.Height;
                }

                int colorMapSize = -1;

                if (this.infoHeader.ClrUsed == 0)
                {
                    if (this.infoHeader.BitsPerPixel == 1 ||
                        this.infoHeader.BitsPerPixel == 4 ||
                        this.infoHeader.BitsPerPixel == 8)
                    {
                        colorMapSize = (int)Math.Pow(2, this.infoHeader.BitsPerPixel) * 4;
                    }
                }
                else
                {
                    colorMapSize = this.infoHeader.ClrUsed * 4;
                }

                byte[] palette = null;

                if (colorMapSize > 0)
                {
                    // 256 * 4
                    if (colorMapSize > 1024)
                    {
                        throw new ImageFormatException($"Invalid bmp colormap size '{colorMapSize}'");
                    }

                    palette = new byte[colorMapSize];

                    this.currentStream.Read(palette, 0, colorMapSize);
                }

                if (this.infoHeader.Width > Image<TPixel>.MaxWidth || this.infoHeader.Height > Image<TPixel>.MaxHeight)
                {
                    throw new ArgumentOutOfRangeException(
                        $"The input bitmap '{this.infoHeader.Width}x{this.infoHeader.Height}' is "
                        + $"bigger then the max allowed size '{Image<TPixel>.MaxWidth}x{Image<TPixel>.MaxHeight}'");
                }

                Image<TPixel> image = new Image<TPixel>(this.configuration, this.infoHeader.Width, this.infoHeader.Height);
                using (PixelAccessor<TPixel> pixels = image.Lock())
                {
                    switch (this.infoHeader.Compression)
                    {
                        case BmpCompression.RGB:
                            if (this.infoHeader.HeaderSize != 40)
                            {
                                throw new ImageFormatException($"Header Size value '{this.infoHeader.HeaderSize}' is not valid.");
                            }

                            if (this.infoHeader.BitsPerPixel == 32)
                            {
                                this.ReadRgb32(pixels, this.infoHeader.Width, this.infoHeader.Height, inverted);
                            }
                            else if (this.infoHeader.BitsPerPixel == 24)
                            {
                                this.ReadRgb24(pixels, this.infoHeader.Width, this.infoHeader.Height, inverted);
                            }
                            else if (this.infoHeader.BitsPerPixel == 16)
                            {
                                this.ReadRgb16(pixels, this.infoHeader.Width, this.infoHeader.Height, inverted);
                            }
                            else if (this.infoHeader.BitsPerPixel <= 8)
                            {
                                this.ReadRgbPalette(pixels, palette, this.infoHeader.Width, this.infoHeader.Height, this.infoHeader.BitsPerPixel, inverted);
                            }

                            break;
                        default:
                            throw new NotSupportedException("Does not support this kind of bitmap files.");
                    }
                }

                return image;
            }
            catch (IndexOutOfRangeException e)
            {
                throw new ImageFormatException("Bitmap does not have a valid format.", e);
            }
        }

        /// <summary>
        /// Returns the y- value based on the given height.
        /// </summary>
        /// <param name="y">The y- value representing the current row.</param>
        /// <param name="height">The height of the bitmap.</param>
        /// <param name="inverted">Whether the bitmap is inverted.</param>
        /// <returns>The <see cref="int"/> representing the inverted value.</returns>
        private static int Invert(int y, int height, bool inverted)
        {
            int row;

            if (!inverted)
            {
                row = height - y - 1;
            }
            else
            {
                row = y;
            }

            return row;
        }

        /// <summary>
        /// Calculates the amount of bytes to pad a row.
        /// </summary>
        /// <param name="width">The image width.</param>
        /// <param name="componentCount">The pixel component count.</param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int CalculatePadding(int width, int componentCount)
        {
            int padding = (width * componentCount) % 4;

            if (padding != 0)
            {
                padding = 4 - padding;
            }

            return padding;
        }

        /// <summary>
        /// Reads the color palette from the stream.
        /// </summary>
        /// <typeparam name="TPixel">The pixel format.</typeparam>
        /// <param name="pixels">The <see cref="PixelAccessor{TPixel}"/> to assign the palette to.</param>
        /// <param name="colors">The <see cref="T:byte[]"/> containing the colors.</param>
        /// <param name="width">The width of the bitmap.</param>
        /// <param name="height">The height of the bitmap.</param>
        /// <param name="bits">The number of bits per pixel.</param>
        /// <param name="inverted">Whether the bitmap is inverted.</param>
        private void ReadRgbPalette<TPixel>(PixelAccessor<TPixel> pixels, byte[] colors, int width, int height, int bits, bool inverted)
            where TPixel : struct, IPixel<TPixel>
        {
            // Pixels per byte (bits per pixel)
            int ppb = 8 / bits;

            int arrayWidth = (width + ppb - 1) / ppb;

            // Bit mask
            int mask = 0xFF >> (8 - bits);

            // Rows are aligned on 4 byte boundaries
            int padding = arrayWidth % 4;
            if (padding != 0)
            {
                padding = 4 - padding;
            }

            byte[] row = new byte[arrayWidth + padding];
            TPixel color = default(TPixel);

            Rgba32 rgba = default(Rgba32);

            for (int y = 0; y < height; y++)
            {
                int newY = Invert(y, height, inverted);
                this.currentStream.Read(row, 0, row.Length);
                int offset = 0;
                Span<TPixel> pixelRow = pixels.GetRowSpan(y);

                // TODO: Could use PixelOperations here!
                for (int x = 0; x < arrayWidth; x++)
                {
                    int colOffset = x * ppb;

                    for (int shift = 0; shift < ppb && (x + shift) < width; shift++)
                    {
                        int colorIndex = ((row[offset] >> (8 - bits - (shift * bits))) & mask) * 4;
                        int newX = colOffset + shift;

                        // Stored in b-> g-> r order.
                        rgba.Bgr = Unsafe.As<byte, Bgr24>(ref colors[colorIndex]);
                        color.PackFromRgba32(rgba);
                        pixelRow[newX] = color;
                    }

                    offset++;
                }
            }
        }

        /// <summary>
        /// Reads the 16 bit color palette from the stream
        /// </summary>
        /// <typeparam name="TPixel">The pixel format.</typeparam>
        /// <param name="pixels">The <see cref="PixelAccessor{TPixel}"/> to assign the palette to.</param>
        /// <param name="width">The width of the bitmap.</param>
        /// <param name="height">The height of the bitmap.</param>
        /// <param name="inverted">Whether the bitmap is inverted.</param>
        private void ReadRgb16<TPixel>(PixelAccessor<TPixel> pixels, int width, int height, bool inverted)
            where TPixel : struct, IPixel<TPixel>
        {
            // We divide here as we will store the colors in our floating point format.
            const int ScaleR = 8; // 256/32
            const int ScaleG = 4; // 256/64
            const int ComponentCount = 2;

            TPixel color = default(TPixel);
            Rgba32 rgba = new Rgba32(0, 0, 0, 255);

            using (PixelArea<TPixel> row = new PixelArea<TPixel>(width, ComponentOrder.Xyz))
            {
                for (int y = 0; y < height; y++)
                {
                    row.Read(this.currentStream);

                    int newY = Invert(y, height, inverted);

                    Span<TPixel> pixelRow = pixels.GetRowSpan(newY);

                    int offset = 0;
                    for (int x = 0; x < width; x++)
                    {
                        short temp = BitConverter.ToInt16(row.Bytes, offset);

                        rgba.R = (byte)(((temp & Rgb16RMask) >> 11) * ScaleR);
                        rgba.G = (byte)(((temp & Rgb16GMask) >> 5) * ScaleG);
                        rgba.B = (byte)((temp & Rgb16BMask) * ScaleR);

                        color.PackFromRgba32(rgba);
                        pixelRow[x] = color;
                        offset += ComponentCount;
                    }
                }
            }
        }

        /// <summary>
        /// Reads the 24 bit color palette from the stream
        /// </summary>
        /// <typeparam name="TPixel">The pixel format.</typeparam>
        /// <param name="pixels">The <see cref="PixelAccessor{TPixel}"/> to assign the palette to.</param>
        /// <param name="width">The width of the bitmap.</param>
        /// <param name="height">The height of the bitmap.</param>
        /// <param name="inverted">Whether the bitmap is inverted.</param>
        private void ReadRgb24<TPixel>(PixelAccessor<TPixel> pixels, int width, int height, bool inverted)
            where TPixel : struct, IPixel<TPixel>
        {
            int padding = CalculatePadding(width, 3);
            using (PixelArea<TPixel> row = new PixelArea<TPixel>(width, ComponentOrder.Zyx, padding))
            {
                for (int y = 0; y < height; y++)
                {
                    row.Read(this.currentStream);

                    int newY = Invert(y, height, inverted);
                    pixels.CopyFrom(row, newY);
                }
            }
        }

        /// <summary>
        /// Reads the 32 bit color palette from the stream
        /// </summary>
        /// <typeparam name="TPixel">The pixel format.</typeparam>
        /// <param name="pixels">The <see cref="PixelAccessor{TPixel}"/> to assign the palette to.</param>
        /// <param name="width">The width of the bitmap.</param>
        /// <param name="height">The height of the bitmap.</param>
        /// <param name="inverted">Whether the bitmap is inverted.</param>
        private void ReadRgb32<TPixel>(PixelAccessor<TPixel> pixels, int width, int height, bool inverted)
            where TPixel : struct, IPixel<TPixel>
        {
            int padding = CalculatePadding(width, 4);
            using (PixelArea<TPixel> row = new PixelArea<TPixel>(width, ComponentOrder.Zyxw, padding))
            {
                for (int y = 0; y < height; y++)
                {
                    row.Read(this.currentStream);

                    int newY = Invert(y, height, inverted);
                    pixels.CopyFrom(row, newY);
                }
            }
        }

        /// <summary>
        /// Reads the <see cref="BmpInfoHeader"/> from the stream.
        /// </summary>
        private void ReadInfoHeader()
        {
            byte[] data = new byte[BmpInfoHeader.Size];

            this.currentStream.Read(data, 0, BmpInfoHeader.Size);

            this.infoHeader = new BmpInfoHeader
            {
                HeaderSize = BitConverter.ToInt32(data, 0),
                Width = BitConverter.ToInt32(data, 4),
                Height = BitConverter.ToInt32(data, 8),
                Planes = BitConverter.ToInt16(data, 12),
                BitsPerPixel = BitConverter.ToInt16(data, 14),
                ImageSize = BitConverter.ToInt32(data, 20),
                XPelsPerMeter = BitConverter.ToInt32(data, 24),
                YPelsPerMeter = BitConverter.ToInt32(data, 28),
                ClrUsed = BitConverter.ToInt32(data, 32),
                ClrImportant = BitConverter.ToInt32(data, 36),
                Compression = (BmpCompression)BitConverter.ToInt32(data, 16)
            };
        }

        /// <summary>
        /// Reads the <see cref="BmpFileHeader"/> from the stream.
        /// </summary>
        private void ReadFileHeader()
        {
            byte[] data = new byte[BmpFileHeader.Size];

            this.currentStream.Read(data, 0, BmpFileHeader.Size);

            this.fileHeader = new BmpFileHeader
            {
                Type = BitConverter.ToInt16(data, 0),
                FileSize = BitConverter.ToInt32(data, 2),
                Reserved = BitConverter.ToInt32(data, 6),
                Offset = BitConverter.ToInt32(data, 10)
            };
        }
    }
}
