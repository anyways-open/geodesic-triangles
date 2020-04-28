using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Anyways.GeodesicTriangles;
using Anyways.GeodesicTriangles.Internal;
using Xunit;

namespace geodesic_triangle_test
{
    public class QtmToWgsTest
    {
        private const double Tolerance = 0.0000000001;

        [Fact]
        public void GenerateTriangle_CenterTriangle_CorrectTriangle()
        {
            var triangle = QtmToWgs.GenerateTriangleZot(new[] {1, 0}).ToDegrees().ToList();

            var pole = triangle[0];
            var west = triangle[1];
            var east = triangle[2];

   

            Assert.True(Math.Abs(0 - west.Lon) < Tolerance);
            Assert.True(Math.Abs(45 - west.Lat) < Tolerance, "east is "+east);

            Assert.True(Math.Abs(90 - east.Lon) < Tolerance);
            Assert.True(Math.Abs(45 - east.Lat) < Tolerance);
            
            Assert.True(Math.Abs(45 - pole.Lon) < Tolerance);
            Assert.True(Math.Abs(0 - pole.Lat) < Tolerance);
        }
        
    }
}