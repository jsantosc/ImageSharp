using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSharp.CustomBenchmarks
{
    using BenchmarkDotNet.Running;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Inicio");
            Console.ReadLine();
            var a = new DecodeJpegBenchmark { Loops = 100 };
            Console.WriteLine("Standard");
            Console.ReadLine();
            a.StandardDecodeFormat();
            Console.WriteLine("Span");
            Console.ReadLine();
            a.SpanDecodeFormat();
            Console.ReadLine();
            //var summary = BenchmarkRunner.Run<DecodeJpegBenchmark>();
            //Console.ReadLine();
        }
    }
}
