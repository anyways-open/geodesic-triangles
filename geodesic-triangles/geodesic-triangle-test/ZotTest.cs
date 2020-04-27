using System;
using geodesic_triangles;
using Xunit;

namespace geodesic_triangle_test
{
    public class ZotTest
    {
        private static double TOLERANCE = 0.0000000000001;

        private static readonly Coordinate brugge = new Coordinate(3.22001, 51.21575);

        [Fact]
        public void ToZotAndBack_Brugge_Same()
        {
            var zot = brugge.ToZot();
            var bruggeBack = zot.ToRadian().ToDegrees();
            Assert.True(Math.Abs(brugge.Lat - bruggeBack.Lat) < TOLERANCE);
            Assert.True(Math.Abs(brugge.Lon - bruggeBack.Lon) < TOLERANCE);
        }

        [Fact]
        public void ToZotAndBack_NorthPole_Same()
        {
            var zot = new Coordinate(0, 90).ToZot();
            var c = zot.ToRadian().ToDegrees();
            Assert.True(Math.Abs(90 - c.Lat) < TOLERANCE);
        }

        [Fact]
        public void ToZotAndBack_NullIsland_Same()
        {
            var zot = new Coordinate(0, 0).ToZot();
            var c = zot.ToRadian().ToDegrees();
            Assert.True(Math.Abs(c.Lat) < TOLERANCE);
            Assert.True(Math.Abs(c.Lon) < TOLERANCE);
        }

        [Fact]
        public void ToZotAndBack_45Lon_Same()
        {
            var zot = new Coordinate(45, 0).ToZot();
            Assert.True(Math.Abs(zot.Px - Math.PI / 4) < TOLERANCE, "Px is " + zot.Px);
            Assert.True(Math.Abs(zot.Py + Math.PI / 4) < TOLERANCE, "Py is " + zot.Py);
            var c = zot.ToRadian().ToDegrees();

            Assert.True(Math.Abs(c.Lat) < TOLERANCE);
            Assert.True(Math.Abs(45 - c.Lon) < TOLERANCE);
        }


        [Fact]
        public void ToZotAndBack_4545_Same()
        {
            var zot = new Coordinate(45, 45).ToZot();
            var c = zot.ToRadian().ToDegrees();
            Assert.True(Math.Abs(45 - c.Lat) < TOLERANCE);
            Assert.True(Math.Abs(45 - c.Lon) < TOLERANCE);
        }
        
        [Fact]
        public void ToZotAndBack_9505_Same()
        {
            var orig = new Coordinate(95, 5);
            var zot = orig.ToZot();
            var c = zot.ToRadian().ToDegrees();
            Assert.True(Math.Abs(orig.Lat - c.Lat) < TOLERANCE);
            Assert.True(Math.Abs(orig.Lon - c.Lon) < TOLERANCE);
        }
        
        
            
        [Fact]
        public void ToZotAndBack_18505_Same()
        {
            var orig = new Coordinate(185, 5);
            var zot = orig.ToZot();
            var c = zot.ToRadian().ToDegrees().AsPositiveCoordinate();
            Assert.True(Math.Abs(orig.Lat - c.Lat) < TOLERANCE);
            Assert.True(Math.Abs(orig.Lon - c.Lon) < TOLERANCE);
        }
        
        
              
        [Fact]
        public void ToZotAndBack_27505_Same()
        {
            var orig = new Coordinate(275, 5);
            var zot = orig.ToZot();
            var c = zot.ToRadian().ToDegrees().AsPositiveCoordinate();
            Assert.True(Math.Abs(orig.Lat - c.Lat) < TOLERANCE);
            Assert.True(Math.Abs(orig.Lon - c.Lon) < TOLERANCE);
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
            Assert.True(Math.Abs(zot.Px) < TOLERANCE);
            Assert.True(Math.Abs(zot.Py) < TOLERANCE);
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
            Assert.True(Math.Abs(Math.PI / 2 - zot.Px) < TOLERANCE);
            Assert.True(Math.Abs(zot.Py) < TOLERANCE);
        }
        
    }
}