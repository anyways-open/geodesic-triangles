using System;
using Anyways.GeodesicTriangles;
using Anyways.GeodesicTriangles.Internal;
using Xunit;

namespace geodesic_triangle_test
{
    public static class IdEncoderTest
    {
        [Fact]
        public static void Encode_1111_CorrectId()
        {
            var id = new[] {1, 1, 1, 1};
            var l = id.EncodeLong();
            var binary = Convert.ToString((long) l, 2);
            Assert.Equal("1000010101", binary);
        }

        [Fact]
        public static void Encode_3000_CorrectId()
        {
            var id = new[] {3, 0, 0, 0};
            var l = id.EncodeLong();
            var binary = Convert.ToString((long) l, 2);
            Assert.Equal("1010000000", binary);
        }

        [Fact]
        public static void Encode_7000_CorrectId()
        {
            var id = new[] {7, 0, 0, 0};
            var l = id.EncodeLong();
            var binary = Convert.ToString((long) l, 2);
            Assert.Equal("1110000000", binary);
        }

        [Fact]
        public static void Encode_7123_CorrectId()
        {
            var id = new[] {7, 1, 2, 3};
            var l = id.EncodeLong();
            var binary = Convert.ToString((long) l, 2);
            Assert.Equal("1110011011", binary);
        }

        [Fact]
        public static void Decode_1111_CorrectId()
        {
            var l = Convert.ToUInt64("1000010101", 2);
            var id = l.Decode();
            Assert.Equal(4, id.Length);


            Assert.Equal(new[] {1, 1, 1, 1}, id);
        }

        [Fact]
        public static void Decode_3000_CorrectId()
        {
            var l = Convert.ToUInt64("1010000000", 2);
            var id = l.Decode();
            Assert.Equal(4, id.Length);

            Assert.Equal(new[] {3, 0, 0, 0}, id);
        }

        [Fact]
        public static void Decode_7000_CorrectId()
        {
            var l = Convert.ToUInt64("1110000000", 2);
            var id = l.Decode();
            Assert.Equal(4, id.Length);

            Assert.Equal(new[] {7, 0, 0, 0}, id);
        }

        [Fact]
        public static void Decode_7123_CorrectId()
        {
            var l = Convert.ToUInt64("1110011011", 2);
            var id = l.Decode();
            Assert.Equal(4, id.Length);

            Assert.Equal(new[] {7, 1, 2, 3}, id);

        }
    }
}