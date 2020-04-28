using System;
using Anyways.GeodesicTriangles;
using Anyways.GeodesicTriangles.Internal;
using Xunit;

namespace geodesic_triangle_test
{
    public class ZotTest
    {
        private const double _tolerance = 0.0000000000001;

        private static readonly Coordinate _brugge = new Coordinate(3.22001, 51.21575);
        private static readonly Coordinate _heidelbergSouthAfrica = 
            new Coordinate(20.96343219280243,-34.09378814846834);
        [Fact]
        public void ToZotAndBack_Brugge_Same()
        {
            var zot = _brugge.ToZot();
            var bruggeBack = zot.ToRadian().ToDegrees();
            Assert.True(Math.Abs(_brugge.Lat - bruggeBack.Lat) < _tolerance);
            Assert.True(Math.Abs(_brugge.Lon - bruggeBack.Lon) < _tolerance);
        }

        [Fact]
        public void ToZotAndBack_NorthPole_Same()
        {
            var zot = new Coordinate(0, 90).ToZot();
            var c = zot.ToRadian().ToDegrees();
            Assert.True(Math.Abs(90 - c.Lat) < _tolerance);
        }

        [Fact]
        public void ToZotAndBack_NullIsland_Same()
        {
            var zot = new Coordinate(0, 0).ToZot();
            var c = zot.ToRadian().ToDegrees();
            Assert.True(Math.Abs(c.Lat) < _tolerance);
            Assert.True(Math.Abs(c.Lon) < _tolerance);
        }

        [Fact]
        public void ToZotAndBack_SouthernHemisphere_Same()
        {
            var zot = new Coordinate(0, -45).ToZot();
            var c = zot.ToRadian().ToDegrees();
            Assert.True(Math.Abs(c.Lat + 45) < _tolerance);
            Assert.True(Math.Abs(c.Lon) < _tolerance);
            Assert.True(zot.SouthernHemisphere);
        }

        [Fact]
        public void ToZotAndBack_45Lon_Same()
        {
            var zot = new Coordinate(45, 0).ToZot();
            Assert.True(Math.Abs(zot.Px - Math.PI / 4) < _tolerance, "Px is " + zot.Px);
            Assert.True(Math.Abs(zot.Py + Math.PI / 4) < _tolerance, "Py is " + zot.Py);
            Assert.False(zot.SouthernHemisphere);
            var c = zot.ToRadian().ToDegrees();

            Assert.True(Math.Abs(c.Lat) < _tolerance);
            Assert.True(Math.Abs(45 - c.Lon) < _tolerance);
        }


        [Fact]
        public void ToZotAndBack_4545_Same()
        {
            var zot = new Coordinate(45, 45).ToZot();
            var c = zot.ToRadian().ToDegrees();
            Assert.True(Math.Abs(45 - c.Lat) < _tolerance);
            Assert.True(Math.Abs(45 - c.Lon) < _tolerance);
        }

        [Fact]
        public void ToZotAndBack_9505_Same()
        {
            var orig = new Coordinate(95, 5);
            var zot = orig.ToZot();
            var c = zot.ToRadian().ToDegrees();
            Assert.True(Math.Abs(orig.Lat - c.Lat) < _tolerance);
            Assert.True(Math.Abs(orig.Lon - c.Lon) < _tolerance);
        }


        [Fact]
        public void ToZotAndBack_13500_Same()
        {
            var orig = new Coordinate(135, 0);
            var zot = orig.ToZot();
            var c = zot.ToRadian().ToDegrees();
            Assert.True(Math.Abs(orig.Lat - c.Lat) < _tolerance);
            Assert.True(Math.Abs(orig.Lon - c.Lon) < _tolerance);

            Assert.True(Math.Abs(zot.Px - Math.PI / 4) < _tolerance);
            Assert.True(Math.Abs(zot.Py - Math.PI / 4) < _tolerance);
        }


        [Fact]
        public void ToZotAndBack_13505_Same()
        {
            var orig = new Coordinate(135, 5);
            var zot = orig.ToZot();
            var c = zot.ToRadian().ToDegrees();
            Assert.True(Math.Abs(orig.Lat - c.Lat) < _tolerance);
            Assert.True(Math.Abs(orig.Lon - c.Lon) < _tolerance);
        }


        [Fact]
        public void ToZotAndBack_18505_Same()
        {
            var orig = new Coordinate(185, 5);
            var zot = orig.ToZot();
            var c = zot.ToRadian().ToDegrees().AsPositiveCoordinate();
            Assert.True(Math.Abs(orig.Lat - c.Lat) < _tolerance);
            Assert.True(Math.Abs(orig.Lon - c.Lon) < _tolerance);
        }


        [Fact]
        public void ToZotAndBack_27505_Same()
        {
            var orig = new Coordinate(275, 5);
            var zot = orig.ToZot();
            var c = zot.ToRadian().ToDegrees().AsPositiveCoordinate();
            Assert.True(Math.Abs(orig.Lat - c.Lat) < _tolerance);
            Assert.True(Math.Abs(orig.Lon - c.Lon) < _tolerance);
        }
        
        
        [Fact]
        public void ToZotAndBack_HeidelbergSA_Same()
        {
            var orig = _heidelbergSouthAfrica;
            var zot = orig.ToZot();
            var c = zot.ToRadian().ToDegrees().AsPositiveCoordinate();
            Assert.True(Math.Abs(orig.Lat - c.Lat) < _tolerance);
            Assert.True(Math.Abs(orig.Lon - c.Lon) < _tolerance);
            Assert.True(zot.SouthernHemisphere);

        }

        [Fact]
        public void ToZot_NorthPole_00()
        {
            var c = new Coordinate(0, 90);
            var zot = c.ToZot();
            Assert.Equal(0, zot.Px);
            Assert.Equal(0, zot.Py);
        }


        [Fact]
        public void ToZot_NorthPoleDiffLon_00()
        {
            var c = new Coordinate(45, 90);
            var zot = c.ToZot();
            Assert.True(Math.Abs(zot.Px) < _tolerance);
            Assert.True(Math.Abs(zot.Py) < _tolerance);
        }

        [Fact]
        public void ToZot_NullIsland_0MinusOne()
        {
            var c = new Coordinate(0, 0);
            var zot = c.ToZot();
            Assert.Equal(0, zot.Px);
            Assert.Equal(-Math.PI / 2, zot.Py);
        }

        [Fact]
        public void ToZot_90West_0MinusOne()
        {
            var c = new Coordinate(90, 0);
            var zot = c.ToZot();
            Assert.True(Math.Abs(Math.PI / 2 - zot.Px) < _tolerance);
            Assert.True(Math.Abs(zot.Py) < _tolerance);
        }
    }
}