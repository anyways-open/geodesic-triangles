using System.Collections.Generic;
using geodesic_triangles;
using Xunit;

namespace geodesic_triangle_test
{
    public class QtmToWgsTest
    {
        [Fact]
        public void GetTriangleFor_Brugge_ExpectsBrugge()
        {
            var id = new[]
            {
                1, 3, 1, 1, 3, 1, 2, 0, 1, 0, 0, 1, 3, 1, 0, 3, 1, 0, 3, 1
            };

            var triangle = id.GetTriangle();
            Assert.Equal(
                "[[3.2196807861328125, 51.215858459472656], [3.2200241088867188, 51.2156867980957], [3.2200241088867188, 51.215858459472656]]",
                triangle.ToString());
        }


        [Fact]
        public void GetTriangleFor_Sydney_ExpectsSydney()
        {
             var id = new[]
            {
                6, 0, 2, 0, 1, 1, 2, 2, 0, 3, 0, 2, 0, 0, 3, 0, 1, 2, 2, 1
            };

            var triangle = id.GetTriangle();
            Assert.Equal(
                "[[151.2151336669922, -33.85711669921875], [151.21530532836914, -33.8569450378418], [151.2154769897461, -33.85711669921875]]",
                triangle.ToString());
        }


        [Fact]
        public void GetTriangleFor_Rio_ExpectsRio()
        {
            var id = new[]
            {
                8, 0, 0, 2, 1, 1, 1, 2, 3, 1, 0, 1, 1, 1, 0, 1, 0, 3, 1, 2
            };

            var triangle = id.GetTriangle(); Assert.Equal(
                "[[-43.146400451660156, -22.934646606445312], [-43.1462287902832, -22.93447494506836], [-43.14605712890625, -22.934646606445312]]",
                triangle.ToString());
        }

        [Fact]
        public void GenerateLevel1Triangle_Europe_CorrectTriangles()
        {
            var europe = QtmToWgs.GenerateTopTriangle(1);

            Assert.Equal(
                new Triangle(new Coordinate(0, 0),
                    new Coordinate(0, 90),
                    new Coordinate(90, 0)),
                europe);
        }

        [Fact]
        public void GenerateTopTriangle_SomeOctants_CorrectTriangle()
        {
            var europe = QtmToWgs.GenerateTopTriangle(1);
            Assert.Equal(new List<Coordinate>
            {
                new Coordinate(0, 0),
                new Coordinate(0, 90),
                new Coordinate(90, 90),
                new Coordinate(90, 0),
                new Coordinate(0, 0)
            }, europe.AsLineString());

            var africa = QtmToWgs.GenerateTopTriangle(5);
            Assert.Equal(new List<Coordinate>
            {
                new Coordinate(0, -90),
                new Coordinate(0, 0),
                new Coordinate(90, 0),
                new Coordinate(90, -90),
                new Coordinate(0, -90)
            }, africa.AsLineString());

            var northAmerica = QtmToWgs.GenerateTopTriangle(4);
            Assert.Equal(new List<Coordinate>
            {
                new Coordinate(270, 0),
                new Coordinate(270, 90),
                new Coordinate(360, 90),
                new Coordinate(360, 0),
                new Coordinate(270, 0)
            }, northAmerica.AsLineString());
        }
    }
}