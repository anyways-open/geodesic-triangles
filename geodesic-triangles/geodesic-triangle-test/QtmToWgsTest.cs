using System.Collections.Generic;
using geodesic_triangles;
using Xunit;

namespace geodesic_triangle_test
{
    public class QtmToWgsTest
    {
        [Fact]
        public void GenerateLevel1Triangle_Europe_CorrectTriangles()
        {
            var europe = QtmToWgs.GenerateTopTriangle(1);
            var africa = QtmToWgs.GenerateTopTriangle(5);
            var asia = QtmToWgs.GenerateTopTriangle(2);
            var oceania = QtmToWgs.GenerateTopTriangle(6);

            var json = new List<Triangle>{europe, africa, asia, oceania}.SplitAndGeojson(4);
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