using System;
using System.Diagnostics.CodeAnalysis;

namespace geodesic_triangles
{
    public static class WgsToQtm
    {
        /// <summary>
        /// If a great-arc is drawn between p0 and p1, the shortest distance (in radians) between c and this great-arc is calculated.
        /// The sign of the cross-track-distance indicates if the point is on the left or on the right.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <returns></returns>
        public static double CrossTrackDistance(this Coordinate c, Coordinate p0, Coordinate p1)
        {
            // source: https://www.movable-type.co.uk/scripts/latlong.html
            var d13 = AngularDistanceBetween(p0, c);
            var bearing12 = InitialBearingRadians(p0, c); 
            var bearing13 = InitialBearingRadians(p0, p1); 
            return Math.Asin(Math.Sin(d13) * Math.Sin(bearing13 - bearing12));
        }

        public static double AngularDistanceBetween(Coordinate p0, Coordinate p1)
        {
            var lat0 = p0.Lat.ToRadians();
            var lat1 = p1.Lat.ToRadians();
            var deltaLat = (p1.Lat - p0.Lat).ToRadians();
            var deltaLon = (p1.Lon - p0.Lon).ToRadians();

            var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                    Math.Cos(lat0) * Math.Cos(lat1) *
                    Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return c;
        }

        public static double InitialBearingRadians(Coordinate p0, Coordinate p1)
        {
            var lon1 = p1.Lon.ToRadians();
            var lon0 = p0.Lon.ToRadians();

            var lat1 = p1.Lat.ToRadians();
            var lat0 = p0.Lat.ToRadians();

            var y = Math.Sin(lon1 - lon0) * Math.Cos(lat1);
            var x = Math.Cos(lat0) * Math.Sin(lat1) -
                    Math.Sin(lat0) * Math.Cos(lat1) * Math.Cos(lon1 - lon0);
            return Math.Atan2(y, x);
        }


        private static double ToRadians(this double degrees)
        {
            return Math.PI * 2 * degrees / 360;
        }

        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public static int DetermineQuadrant(Coordinate c, Coordinate p0, Coordinate p1, Coordinate p2)
        {
            }

        public static int[] GenerateId(this Coordinate c, int precision)
        {
            var id = new int[precision];
            c = c.AsPositiveCoordinate();
            var octant = DetermineOctant(c);
            id[0] = octant;
            QtmToWgs.GenerateTopTriangle(octant, out var p0, out var p1, out var p2);


            for (int i = 1; i < precision; i++)
            {
                var quadrant = DetermineQuadrant(c, p0, p1, p2);
                id[i] = quadrant;
                QtmToWgs.GetTriangle(quadrant, p0, p1, p2, out p0, out p1, out p2);
            }

            return id;
        }

      
        
      
    }
}