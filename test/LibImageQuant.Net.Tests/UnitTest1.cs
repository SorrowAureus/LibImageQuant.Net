using LibImageQuant.Net.Codec;
using LibImageQuant.Net.Core;
using System;
using System.IO;
using Xunit;

namespace LibImageQuant.Net.Tests
{
    public class UnitTest1
    {

        private static byte[] ManagedCoder(byte[] imageBytes)
        {
            var Decoder = new Decoder();
            Decoder.ReadPng(imageBytes);
            IProvideImages ipi;
            if (Decoder.ColorType == ColorType.RGBA)
            {
                ipi = new ManagedProvider<ARGBFiller>(Decoder, new ARGBFiller());
            }
            else if (Decoder.ColorType == ColorType.RGB)
            {
                ipi = new ManagedProvider<RGBFiller>(Decoder, new RGBFiller());
            }
            else
                throw new ArgumentException("Colortype does not support quantization");

            using var quantizer = new Quantizer { DitheringLevel = 0.6f, Quality = (0, 100) };
            using var result = quantizer.Quantize(ipi, Decoder.width, Decoder.height);
            return new Coder().CreateBytes(result, Decoder.width, Decoder.height);
        }

        [Fact]
        public void Test1()
        {
            var fileName = @"frau-mode-vintage-illustration-1622417428ANN.png";
            var bytes = File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), fileName));
            var result = ManagedCoder(bytes);
            Assert.True(bytes.Length > result.Length);
            File.WriteAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "out.png"), result);
        }
    }
}
