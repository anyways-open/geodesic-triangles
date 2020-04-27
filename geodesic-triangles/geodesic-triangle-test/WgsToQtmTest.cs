using System.Linq;
using geodesic_triangles;
using Xunit;

namespace geodesic_triangle_test
{
    public class WgsToQtmTest
    {
        private static readonly Coordinate brugge = new Coordinate(3.22001, 51.21575);
        private static readonly Coordinate sydney = new Coordinate(151.21541, -33.85708);
        private static readonly Coordinate rio = new Coordinate(-43.14636, -22.93461);
        private static readonly Coordinate KualaLumpurPark = new Coordinate(101.70733, 3.17777);
        private static readonly Coordinate EdgeCase = new Coordinate(10, 63);
        private static readonly Coordinate StockHolm = new Coordinate(18.07092, 59.32513);

        [Fact]
        public void GenerateQTMId_SaintLawrence_CorrectId()
        {
            var id = EdgeCase.GetId(20);

            var triangles = QtmToWgs.GenerateAllTriangles(id);
            var geojson = triangles.Select(triangle => (triangle, "#00ff00")).ToGeoJson(EdgeCase);

            var expectedId = new[]
            {
                1, 3, 1, 3, 3, 2, 0, 1, 0, 2, 1, 3, 3, 1, 0, 1, 0, 1, 0, 2
            };
            Assert.Equal(expectedId.Length, id.Length);
            for (int i = 0; i < expectedId.Length; i++)
            {
                Assert.True(expectedId[i] == id[i],
                    $"Index {i} is different. Expected {expectedId[i]} but got {id[i]}");
            }
        }


        [Fact]
        public void GenerateQTMId_Sydney_CorrectId()
        {
            var id = sydney.GetId(20);
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
            var id = rio.GetId(20);
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
        public void GenerateQTMId_KualaLumpur_CorrectId()
        {
            var id = KualaLumpurPark.GetId(20);
            var triangles = QtmToWgs.GenerateAllTriangles(id);
            var geojson = triangles.Select(triangle => (triangle, "#00ff00")).ToGeoJson(KualaLumpurPark);

            var expectedId = new[]
            {
                2, 1, 
            };
            for (int i = 0; i < expectedId.Length; i++)
            {
                Assert.True(expectedId[i] == id[i],
                    $"Index {i} is different. Expected {expectedId[i]} but got {id[i]}");
            }
            Assert.Equal(expectedId.Length, id.Length);
        }


        [Fact]
        public void GenerateQTMId_Brugge_CorrectId()
        {
            var id = brugge.GetId(20);
            var triangles = QtmToWgs.GenerateAllTriangles(id);
            var geojson = triangles.Select(triangle => (triangle, "#00ff00")).ToGeoJson(brugge);
            var expectedId = new[]
            {
                1, 3, 1, 1, 3, 1, 2, 0, 1, 1, 3, 0, 1, 3, 3, 3, 1, 1, 0, 1
            };
            Assert.Equal(expectedId.Length, id.Length);
            for (int i = 0; i < expectedId.Length; i++)
            {
                Assert.True(expectedId[i] == id[i],
                    $"Index {i} is different. Expected {expectedId[i]} but got {id[i]}");
            }
        }

        [Fact]
        public void GenerateQTMId_StockHolm_CorrectId()
        {
            var id = StockHolm.GetId(20);
            var triangles = QtmToWgs.GenerateAllTriangles(id);
            var geojson = triangles.Select(triangle => (triangle, "#00ff00")).ToGeoJson(StockHolm);
            var expectedId = new[]
            {
                1, 3, 1, 3, 2, 3, 2, 0, 3, 2, 3, 1, 1, 2, 1, 1, 0, 1, 0, 1
            };
            Assert.Equal(expectedId.Length, id.Length);
            for (int i = 0; i < expectedId.Length; i++)
            {
                Assert.True(expectedId[i] == id[i],
                    $"Index {i} is different. Expected {expectedId[i]} but got {id[i]}");
            }
        }

        [Fact]
        public void GenerateQTMId_AboveCenter_CorrectId()
        {
            // We force a center triangle to test with
            var c = new Coordinate(45, 5);
            var id = c.GetId(3);
            var expectedId = new[] {1, 0, 3};
            Assert.Equal(expectedId.Length, id.Length);
            for (int i = 0; i < expectedId.Length; i++)
            {
                Assert.True(expectedId[i] == id[i],
                    $"Index {i} is different. Expected {expectedId[i]} but got {id[i]}");
            }
        }
    }
}