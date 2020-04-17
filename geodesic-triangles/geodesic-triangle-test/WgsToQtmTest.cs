using geodesic_triangles;
using Xunit;

namespace geodesic_triangle_test
{
    public class WgsToQtmTest
    {
        [Fact]
        public void DetermineOctantTest()
        {
            Assert.Equal(1, new Coordinate(4.0,51.2).DetermineOctant());
            Assert.Equal(2, new Coordinate(94.0,51.2).DetermineOctant());


        }
    }
}