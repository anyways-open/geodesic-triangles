using System;
using System.Linq;
using Geodesic.Triangles.Internal;
using Xunit;

namespace Geodesic.Triangles.Test
{
    public class QtmToWgsTest
    {
        private const double _tolerance = 0.0000000001;

        [Fact]
        public void GenerateTriangle_CenterTriangle_CorrectTriangle()
        {
            var triangle = QtmToWgs.GenerateTriangleZot(new[] {1, 0}).ToDegrees().ToList();

            var pole = triangle[0];
            var west = triangle[1];
            var east = triangle[2];

   

            Assert.True(Math.Abs(0 - west.Lon) < _tolerance);
            Assert.True(Math.Abs(45 - west.Lat) < _tolerance, "east is "+east);

            Assert.True(Math.Abs(90 - east.Lon) < _tolerance);
            Assert.True(Math.Abs(45 - east.Lat) < _tolerance);
            
            Assert.True(Math.Abs(45 - pole.Lon) < _tolerance);
            Assert.True(Math.Abs(0 - pole.Lat) < _tolerance);
        }
        
    }
}