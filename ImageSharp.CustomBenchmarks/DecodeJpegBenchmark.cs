namespace ImageSharp.CustomBenchmarks
{
    using System;
    using System.IO;

    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Attributes.Jobs;

    [MemoryDiagnoser]
    [RyuJitX64Job, LegacyJitX86Job]
    public class DecodeJpegBenchmark
    {
        private readonly byte[] _jpegBytes;
        private const string ImageRoot =
                @"C:\Users\Javier\Source\Repos\ImageSharp\tests\ImageSharp.Tests\TestImages\Formats\Jpg\baseline\Calliphora.jpg";


        public DecodeJpegBenchmark()
        {
            _jpegBytes = File.ReadAllBytes(ImageRoot);
        }

        [Params(30, 40, 50)]
        public int Loops { get; set; }

        //[Benchmark]
        //public int StandardDecode()
        //{
        //    int pixels = 0;
        //    using (MemoryStream memoryStream = new MemoryStream(_jpegBytes))
        //    {
        //        for (int i = 0; i < Loops; ++i)
        //        {
        //            memoryStream.Position = 0;
        //            using (var image = Image.Load<Rgba32>(memoryStream))
        //            {
        //                pixels += image.Width * image.Height;
        //            }
        //        }
        //    }
        //    return pixels;
        //}
        [Benchmark]
        public void StandardDecodeFormat()
        {
            int pixels = 0;
            using (var memoryStream = new MemoryStream(_jpegBytes))
            {
                for (int i = 0; i < Loops; ++i)
                {
                    memoryStream.Position = 0;
                    var format = Image.DetectFormat(memoryStream);
                    if (format == null)
                    {
                        throw new Exception();
                    }
                }
            }
        }
        [Benchmark]
        public void SpanDecodeFormat()
        {
            int pixels = 0;
            var spanMemory = SpanUnmanagedArray<byte>.FromByteArray(this._jpegBytes);
            for (int i = 0; i < Loops; ++i)
            {
                var format = Image.DetectFormat(spanMemory);
                if (format == null)
                {
                    throw new Exception();
                }
            }
        }
    }
}