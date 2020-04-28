using System;
using Xunit;

namespace Anyways.GeodesicTriangles.Test
{
    public class DifferenceTest
    {
        [Fact]
        public void ToQTM_FromQTM_DifferenceIsSmall()
        {
            for (int lat = -850; lat < 850; lat += 5)
            {
                for (int lon = -1790; lon < 1800; lon += 1)
                {
                    var c = (lon / 10.0, lat / 10.0);
                    var id = c.TriangleId();
                    var c0 = id.TriangleCenterPoint();
                    var d = DistanceEstimateInMeter(c, c0);

                    Assert.True(d < 1, $"Distance is {d} for {c}, becomes {c0}");
                }
            }
        }
        
        private const double _radiusOfEarth = 6371000;

        /// <summary>
        /// Returns an estimate of the distance between the two given coordinates.
        /// Stolen from https://github.com/itinero/routing/blob/1764afc75db43a1459789592de175283f642123f/src/Itinero/LocalGeo/Coordinate.cs
        /// </summary>
        /// <remarks>Accuracy decreases with distance.</remarks>
        public static float DistanceEstimateInMeter((double Lon, double Lat) c1, (double Lon, double Lat)  c2)
        {
            var lat1Rad = c1.Lat / 180d * Math.PI;
            var lon1Rad = c1.Lon / 180d * Math.PI;
            var lat2Rad = c2.Lat / 180d * Math.PI;
            var lon2Rad = c2.Lon / 180d * Math.PI;

            var x = (lon2Rad - lon1Rad) * Math.Cos((lat1Rad + lat2Rad) / 2.0);
            var y = lat2Rad - lat1Rad;

            var m = Math.Sqrt(x * x + y * y) * _radiusOfEarth;

            return (float) m;
        }
    }
}