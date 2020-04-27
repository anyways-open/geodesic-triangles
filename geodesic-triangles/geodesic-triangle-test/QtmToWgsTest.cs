using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using geodesic_triangles;
using Xunit;

namespace geodesic_triangle_test
{
    public class QtmToWgsTest
    {
        private const double Tolerance = 0.0000000001;

        [Fact]
        public void GenerateTriangle_CenterTriangle_CorrectTriangle()
        {
            var triangle = QtmToWgs.GenerateTriangle(new[] {1, 0});

            var pole = triangle.a.ToRadians().ToDegrees();
            var west = triangle.b.ToRadians().ToDegrees();
            var east = triangle.c.ToRadians().ToDegrees();

   

            Assert.True(Math.Abs(0 - east.Lon) < Tolerance);
            Assert.True(Math.Abs(45 - east.Lat) < Tolerance, "east is "+east);

            Assert.True(Math.Abs(90 - west.Lon) < Tolerance);
            Assert.True(Math.Abs(45 - west.Lat) < Tolerance);
            
            Assert.True(Math.Abs(45 - pole.Lon) < Tolerance);
            Assert.True(Math.Abs(0 - pole.Lat) < Tolerance);
        }


        [Fact]
        public void GenerateField_AsJson()
        {
            var allIDs = Utils.GenerateField(5);
            var triangles = allIDs.Select(id => (QtmToWgs.GenerateTriangle(id.ToArray()), "#009900"));
            var geojson = Utils.ToGeoJson(triangles);
            File.WriteAllText("Driehoeken.geojson", geojson);
        }
    }
}