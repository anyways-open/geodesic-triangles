using System;
using System.Linq;
using Anyways.GeodesicTriangles.Internal;
using Xunit;

namespace Anyways.GeodesicTriangles.Test
{
    public class WgsToQtmTest
    {
        // Oct 1
        private static readonly Coordinate _brugge = new Coordinate(3.22001, 51.21575);
        private static readonly Coordinate _stockHolm = new Coordinate(18.07092, 59.32513);
        private static readonly Coordinate _edgeCase = new Coordinate(10, 63);

        // Oct 2
        private static readonly Coordinate _kualaLumpurPark = new Coordinate(101.70733, 3.17777);

        // Oct 3
        private static readonly Coordinate _anchorage = new Coordinate(-149.78555917739868, 61.210123179468766);

        // Oct 4
        private static readonly Coordinate _newYork = new Coordinate(-73.96647870540619, 40.78137414240531);

        // OCt 5
        // Not the Heidelberg we'd expect
        private static readonly Coordinate _heidelbergSouthAfrica =
            new Coordinate(20.96343219280243, -34.09378814846834);


        // Oct 6
        private static readonly Coordinate _sydney = new Coordinate(151.21541, -33.85708);

        // Oct 7
        private static readonly Coordinate _tahiti = new Coordinate(-149.57198292016983, -17.540692953732645);

        // Oct 8
        private static readonly Coordinate _rio = new Coordinate(-43.14636, -22.93461);


        [Fact]
        public void GenerateQTMId_Brugge_CorrectId()
        {
            var id = _brugge.GetId(20);
            var expectedId = new[]
            {
                1, 3, 1, 1, 3, 1, 2, 0, 2, 2, 3, 0, 1, 3, 3, 3, 1, 1, 0, 2
            };
            Assert.Equal(expectedId.Length, id.Length);
            for (var i = 0; i < expectedId.Length; i++)
            {
                Assert.True(expectedId[i] == id[i],
                    $"Index {i} is different. Expected {expectedId[i]} but got {id[i]}");
            }

           
        }


        [Fact]
        public void GenerateQTMId_SaintLawrence_CorrectId()
        {
            var id = _edgeCase.GetId(20);

            var expectedId = new[]
            {
                1, 3, 1, 3, 3, 2, 0, 2, 0, 2, 1, 3, 3, 1, 0, 2, 0, 1, 0, 1
            };
            Assert.Equal(expectedId.Length, id.Length);
            for (var i = 0; i < expectedId.Length; i++)
            {
                Assert.True(expectedId[i] == id[i],
                    $"Index {i} is different. Expected {expectedId[i]} but got {id[i]}");
            }
        }

        [Fact]
        public void GenerateQTMId_StockHolm_CorrectId()
        {
            var id = _stockHolm.GetId(20);
            var expectedId = new[]
            {
                1, 3, 1, 3, 2, 3, 2, 0, 3, 1, 3, 2, 2, 1, 2, 2, 0, 1, 0, 2
            };
            Assert.Equal(expectedId.Length, id.Length);
            for (var i = 0; i < expectedId.Length; i++)
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
            for (var i = 0; i < expectedId.Length; i++)
            {
                Assert.True(expectedId[i] == id[i],
                    $"Index {i} is different. Expected {expectedId[i]} but got {id[i]}");
            }
        }


        [Fact]
        public void GenerateQTMId_KualaLumpur_CorrectId()
        {
            var id = _kualaLumpurPark.GetId(20);

            var expectedId = new[]
            {
                2, 1, 1, 2, 0, 0, 1, 2, 3, 1, 2, 2, 1, 3, 2, 0, 1, 1, 1, 2
            };
            for (var i = 0; i < expectedId.Length; i++)
            {
                Assert.True(expectedId[i] == id[i],
                    $"Index {i} is different. Expected {expectedId[i]} but got {id[i]}");
            }

            Assert.Equal(expectedId.Length, id.Length);
        }


        [Fact]
        public void GenerateQTMId_Anchorage_CorrectId()
        {
            var id = _anchorage.GetId(20);

            var expectedId = new[]
            {
                3, 3, 0, 1, 3, 2, 2, 0, 1, 1, 1, 2, 3, 3, 0, 2, 2, 1, 2, 3
            };
            Assert.Equal(expectedId.Length, id.Length);
            for (var i = 0; i < expectedId.Length; i++)
            {
                Assert.True(expectedId[i] == id[i],
                    $"Index {i} is different. Expected {expectedId[i]} but got {id[i]}");
            }
        }


        [Fact]
        public void GenerateQTMId_NewYork_CorrectId()
        {
            var id = _newYork.GetId(20);
            var expectedId = new[]
            {
                4, 0, 1, 1, 2, 3, 0, 2, 2, 1, 2, 1, 1, 1, 1, 1, 1, 0, 3, 3
            };
            Assert.Equal(expectedId.Length, id.Length);
            for (var i = 0; i < expectedId.Length; i++)
            {
                Assert.True(expectedId[i] == id[i],
                    $"Index {i} is different. Expected {expectedId[i]} but got {id[i]}");
            }
        }

        [Fact]
        public void GenerateQTMId_Heidelberg_CorrectId()
        {
            var id = _heidelbergSouthAfrica.GetId(20);

            var expectedId = new[]
            {
                5, 0, 1, 0, 1, 2, 2, 1, 0, 1, 1, 1, 1, 0, 2, 3, 0, 3, 1, 3
            };
            Assert.Equal(expectedId.Length, id.Length);
            for (var i = 0; i < expectedId.Length; i++)
            {
                Assert.True(expectedId[i] == id[i],
                    $"Index {i} is different. Expected {expectedId[i]} but got {id[i]}");
            }
        }


        [Fact]
        public void GenerateQTMId_Sydney_CorrectId()
        {
            var id = _sydney.GetId(20);
            var expectedId = new[]
            {
                6, 0, 2, 0, 1, 1, 2, 2, 2, 1, 3, 1, 2, 3, 3, 0, 2, 2, 1, 0
            };
            Assert.Equal(expectedId.Length, id.Length);
            for (var i = 0; i < expectedId.Length; i++)
            {
                Assert.True(expectedId[i] == id[i],
                    $"Index {i} is different. Expected {expectedId[i]} but got {id[i]}");
            }
        }


        [Fact]
        public void GenerateQTMId_Tahiti_CorrectId()
        {
            var id = _tahiti.GetId(20);
            var expectedId = new[]
            {
                7, 1, 2, 3, 3, 2, 2, 0, 1, 1, 0, 3, 1, 0, 1, 3, 3, 0, 1, 3
            };
            Assert.Equal(expectedId.Length, id.Length);
            for (var i = 0; i < expectedId.Length; i++)
            {
                Assert.True(expectedId[i] == id[i],
                    $"Index {i} is different. Expected {expectedId[i]} but got {id[i]}");
            }
        }

        [Fact]
        public void GenerateQTMId_Rio_CorrectId()
        {
            var id = _rio.GetId(20);
            var expectedId = new[]
            {
                8, 0, 0, 2, 1, 1, 1, 2, 3, 0, 3, 2, 2, 0, 3, 1, 1, 1, 3, 3
            };
            Assert.Equal(expectedId.Length, id.Length);
            for (var i = 0; i < expectedId.Length; i++)
            {
                Assert.True(expectedId[i] == id[i],
                    $"Index {i} is different. Expected {expectedId[i]} but got {id[i]}");
            }
        }


        [Fact]
        public void GenerateTriangles_11111111_CorrectId()
        {
            var id = new[] {1, 1, 1, 1, 1, 1, 1};
            var triangles = QtmToWgs.GenerateAllTrianglesZot(id).Select(CoordinateExtensions.ToDegrees);
            Assert.Equal(new Coordinate(0, 0), triangles.Last().Last());
        }

        private static double Tolerance = 0.00000001;

        [Fact]
        public void GenerateTriangles_21_CorrectId()
        {
            var id = new[] {2, 1};
            var triangles = QtmToWgs.GenerateAllTrianglesZot(id).Select(CoordinateExtensions.ToDegrees);
            var c = triangles.Last().ToList()[1];
            Assert.True(Math.Abs(c.Lon - 135) < Tolerance);
            Assert.True(Math.Abs(c.Lat - 0) < Tolerance);
        }
    }
}