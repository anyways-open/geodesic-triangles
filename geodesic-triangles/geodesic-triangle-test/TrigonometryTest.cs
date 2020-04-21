using geodesic_triangles;
using Xunit;

namespace geodesic_triangle_test
{
    public class TrigonometryTest
    {
        [Fact]
        public void TestCrossTrackDistance()
        {
            var c = new Coordinate(10, 63);
            var p0 = new Coordinate(45, 45);
            var p1 = new Coordinate(0, 67.5);
            var ctd = c.CrossTrackDistance(p0, p1);

            var cLeft = new Coordinate(8.980636596679688,
                62.998743727686374);
            var ctdLeft = cLeft.CrossTrackDistance(p0, p1);
            // If connected with a straight line on a mercator map, the point falls on the left of p0 --> p1
            // If connected with an arc-circle, it should fall on the right

        }
    }
}