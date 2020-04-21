using geodesic_triangles;
using Xunit;

namespace geodesic_triangle_test
{
    public class WgsToQtmTest
    {
        private static readonly Coordinate brugge = new Coordinate(3.22001, 51.21575);
        private static readonly Coordinate sydney = new Coordinate(151.21541, -33.85708);
        private static readonly Coordinate rio = new Coordinate(-43.14636, -22.93461);
       
        private static readonly Coordinate EdgeCase = new Coordinate(10, 63); 

        [Fact]
        public void GenerateQTMId_SaintLawrence_CorrectId()
        {
            var json = Utils.DebugGenerateId(EdgeCase, 20);
            var id = EdgeCase.GenerateId(20);

            
            var expectedId = new[]
            {
                6, 0, 2, 0, 1, 1, 2, 2, 0, 3, 0, 2, 0, 0, 3, 0, 1, 2, 2, 1
            };
            Assert.Equal(expectedId.Length, id.Length);
            for (int i = 0; i < expectedId.Length; i++)
            {
                Assert.True(expectedId[i] == id[i],
                    $"Index {i} is different. Expected {expectedId[i]} but got {id[i]}");
            }
        }
        
        
        [Fact]
        public void DetermineOctant_SomeCoordinates_CorrectOctant()
        {
            Assert.Equal(1, new Coordinate(4.0, 51.2).DetermineOctant());
            Assert.Equal(2, new Coordinate(94.0, 51.2).DetermineOctant());
        }

        [Fact]
        public void GenerateQTMId_Sydney_CorrectId()
        {
            var id = sydney.GenerateId(20);
            var expectedId = new[]
            {
                6, 0, 2, 0, 1, 1, 2, 2, 0, 3, 0, 2, 0, 0, 3, 0, 1, 2, 2, 1
            };
            Assert.Equal(expectedId.Length, id.Length);
            for (int i = 0; i < expectedId.Length; i++)
            {
                Assert.True(expectedId[i] == id[i],
                    $"Index {i} is different. Expected {expectedId[i]} but got {id[i]}");
            }
        }
        
        [Fact]
        public void GenerateQTMId_Rio_CorrectId()
        {
            
            var id = rio.GenerateId(20);
            var expectedId = new[]
            {
                8, 0, 0, 2, 1, 1, 1, 2, 3, 1, 0, 1, 1, 1, 0, 1, 0, 3, 1, 2
            };
            Assert.Equal(expectedId.Length, id.Length);
            for (int i = 0; i < expectedId.Length; i++)
            {
                Assert.True(expectedId[i] == id[i],
                    $"Index {i} is different. Expected {expectedId[i]} but got {id[i]}");
            }
        }

        [Fact]
        public void GenerateQTMId_Brugge_CorrectId()
        {
            var id = brugge.GenerateId(20);
            var expectedId = new[]
            {
                1, 3, 1, 1, 3, 1, 2, 0, 1, 0, 0, 1, 3, 1, 0, 3, 1, 0, 3, 1
            };
            Assert.Equal(expectedId.Length, id.Length);
            for (int i = 0; i < expectedId.Length; i++)
            {
                Assert.True(expectedId[i] == id[i],
                    $"Index {i} is different. Expected {expectedId[i]} but got {id[i]}");
            }
        }
    }
}