using System;
using Xunit;

namespace Anyways.GeodesicTriangles.Test
{
    public class ApiTest
    {
        private static readonly (double lon, double lat) _brugge = (3.22001, 51.21575);

        private const double _tolerance = 0.0001;

        [Fact]
        public void Encode_Brugge_DecodedIsNear()
        {
            var c = _brugge;
            var id = c.TriangleId(30);
            Assert.Equal(2548345555578004455ul, id);
            var c0 = id.TriangleCenterPoint();

            Assert.True(Math.Abs(c.lat - c0.lat) < _tolerance);
            Assert.True(Math.Abs(c.lon - c0.lon) < _tolerance);
        }

        [Fact]
        public void Encode_CloseBy_SameId()
        {
            var c0 = (3.22001, 51.21575);
            var c1 = (3.22011, 51.21565);
            var id0 = c0.TriangleId(16);
            var id1 = c1.TriangleId(16);

            Assert.Equal(id0, id1);
        }

        [Fact]
        public void EncodeAndGeoJson_Brugge_Brugge()
        {
            var c = _brugge;
            var geojson = c.TriangleId(16).PolygonAround().ToGeoJson();
            var expected =
                "{ \"type\": \"FeatureCollection\", \"features\": [ { \"type\": \"Feature\", \"properties\": {}, \"geometry\": { \"type\": \"Polygon\", \"coordinates\": [[[3.2197085228698183, 51.21826171874999], [3.2239810243833245, 51.2155151367187], [3.2194681955917686, 51.21551513671877], [3.2197085228698183, 51.21826171874999]]]} }]}";
            Assert.Equal(expected, geojson);
        }
    }
}