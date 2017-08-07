namespace ImageSharp
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Formats;

    using ImageSharp.PixelFormats;

    public static partial class Image
    {
        /// <summary>
        /// By reading the header on the provided stream this calculates the images mime type.
        /// </summary>
        /// <param name="stream">The image stream to read the header from.</param>
        /// <returns>The mime type or null if none found.</returns>
        public static IImageFormat DetectFormat(SpanUnmanagedArray<byte> stream)
        {
            return DetectFormat(null, stream);
        }

        /// <summary>
        /// By reading the header on the provided stream this calculates the images mime type.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="stream">The image stream to read the header from.</param>
        /// <returns>The mime type or null if none found.</returns>
        public static IImageFormat DetectFormat(Configuration config, SpanUnmanagedArray<byte> stream)
        {
            return InternalDetectFormat(stream, config ?? Configuration.Default);
        }

        /// <summary>
        /// Create a new instance of the <see cref="Image{Rgba32}"/> class from the given stream.
        /// </summary>
        /// <param name="config">The config for the decoder.</param>
        /// <param name="stream">The stream containing image information.</param>
        /// <exception cref="NotSupportedException">
        /// Thrown if the stream is not readable nor seekable.
        /// </exception>
        /// <returns>A new <see cref="Image{Rgba32}"/>.</returns>>
        public static Image<Rgba32> Load(Configuration config, SpanUnmanagedArray<byte> stream) => Load<Rgba32>(config, stream);
        
        /// <summary>
        /// Create a new instance of the <see cref="Image{TPixel}"/> class from the given stream.
        /// </summary>
        /// <param name="stream">The stream containing image information.</param>
        /// <exception cref="NotSupportedException">
        /// Thrown if the stream is not readable nor seekable.
        /// </exception>
        /// <typeparam name="TPixel">The pixel format.</typeparam>
        /// <returns>A new <see cref="Image{TPixel}"/>.</returns>>
        public static Image<TPixel> Load<TPixel>(SpanUnmanagedArray<byte> stream)
            where TPixel : struct, IPixel<TPixel>
        {
            return Load<TPixel>(null, stream);
        }

        /// <summary>
        /// Create a new instance of the <see cref="Image{TPixel}"/> class from the given stream.
        /// </summary>
        /// <param name="config">The configuration options.</param>
        /// <param name="stream">The stream containing image information.</param>
        /// <exception cref="NotSupportedException">
        /// Thrown if the stream is not readable nor seekable.
        /// </exception>
        /// <typeparam name="TPixel">The pixel format.</typeparam>
        /// <returns>A new <see cref="Image{TPixel}"/>.</returns>>
        public static Image<TPixel> Load<TPixel>(Configuration config, SpanUnmanagedArray<byte> stream)
            where TPixel : struct, IPixel<TPixel>
        {
            return Load<TPixel>(config, stream, out var _);
        }

        /// <summary>
        /// Create a new instance of the <see cref="Image{TPixel}"/> class from the given stream.
        /// </summary>
        /// <param name="config">The configuration options.</param>
        /// <param name="stream">The stream containing image information.</param>
        /// <param name="format">the mime type of the decoded image.</param>
        /// <exception cref="NotSupportedException">
        /// Thrown if the stream is not readable nor seekable.
        /// </exception>
        /// <typeparam name="TPixel">The pixel format.</typeparam>
        /// <returns>A new <see cref="Image{TPixel}"/>.</returns>>
        public static Image<TPixel> Load<TPixel>(Configuration config, SpanUnmanagedArray<byte> stream, out IImageFormat format)
            where TPixel : struct, IPixel<TPixel>
        {
            config = config ?? Configuration.Default;
            (Image<TPixel> img, IImageFormat format) data = Decode<TPixel>(stream, config);

            format = data.format;

            if (data.img != null)
            {
                return data.img;
            }

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Image cannot be loaded. Available decoders:");

            foreach (KeyValuePair<IImageFormat, IImageDecoder> val in config.ImageDecoders)
            {
                stringBuilder.AppendLine($" - {val.Key.Name} : {val.Value.GetType().Name}");
            }

            throw new NotSupportedException(stringBuilder.ToString());
        }


    }
}