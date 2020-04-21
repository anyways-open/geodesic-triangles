using System;
using System.Diagnostics.Contracts;

// ReSharper disable InconsistentNaming

namespace geodesic_triangles
{
    public static class QtmToWgs
    {
        public static Triangle GetTriangle(this int[] id)
        {
            GetTriangle(out var P0, out var P1, out var P2, id);
            P0 = P0.AsWrappedCoordinate();
            P1 = P1.AsWrappedCoordinate();
            P2 = P2.AsWrappedCoordinate();
            return new Triangle(P0, P1, P2);
        }
        
        public static Coordinate GetCenterPoint(this int[] id)
        {
            GetTriangle(out var P0, out var P1, out var P2, id);
            return new Coordinate((P0.Lon + P1.Lon + P2.Lon) / 3,
                (P0.Lat + P1.Lat + P2.Lat) / 3).AsWrappedCoordinate();
        }

        public static void GetTriangle(out Coordinate P0, out Coordinate P1, out Coordinate P2,
            params int[] id)
        {
            GenerateTopTriangle(id[0], out P0, out P1, out P2);

            for (int i = 1; i < id.Length; i++)
            {
                var quadrant = id[i];
                GetTriangle(quadrant, P0, P1, P2, out P0, out P1, out P2);
            }

          

        }


        public static Triangle GetQuadrant(this Triangle t, int quadrant)
        {
            GetTriangle(quadrant, t.P0, t.P1, t.P2, out var p0, out var p1, out var p2);
            return new Triangle(p0, p1, p2);
        }

        /// <summary>
        /// Given the points of a triangle (P0, P1, P2) and the number of the the needed quadrant,
        /// saves the new triangle coordinates in the respective 'out'-variables.
        ///
        /// The quadrants are numbered as following:
        ///
        /// 0: the center triangle
        /// 1: the lowest longitude triangle
        /// 2: the highest longitude triangle
        /// 3: the pole-pointing triangle
        ///
        /// Note that the order of quadrants is slightly different then in the paper (which assigns 1 to the triangle closest to the meridian).
        /// 
        /// </summary>
        /// <param name="quadrant"></param>
        /// <param name="P0"></param>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <param name="subP0"></param>
        /// <param name="subP1"></param>
        /// <param name="subP2"></param>
        public static void GetTriangle(int quadrant, Coordinate P0, Coordinate P1, Coordinate P2,
            out Coordinate subP0, out Coordinate subP1, out Coordinate subP2)
        {
            var equatorLineSplit =
                new Coordinate((P0.Lon + P2.Lon) / 2,
                    P0.Lat); // P0.Lat == P2.Lat ==> avg(P0.Lat, P2.lat) == P0.Lat == P2.Lat

            var minLonSplit =
                new Coordinate((P0.Lon + P1.Lon) / 2, (P0.Lat + P1.Lat) / 2);

            var p1 = P1;
            if (p1.IsPole())
            {
                // If the polepoint IS the pole, then we have to rewrite the longitude
                // We can do this, as on the pole, the longitude is irrelevant anyway
                // We have to do it to make sure the longitude is the same in the end
                p1 = new Coordinate(P2.Lon, p1.Lat);
            }


            var maxLonSplit =
                new Coordinate((p1.Lon + P2.Lon) / 2, (p1.Lat + P2.Lat) / 2);

            switch (quadrant)
            {
                case 0:
                    subP0 = minLonSplit;
                    subP1 = equatorLineSplit;
                    subP2 = maxLonSplit;
                    return;
                case 1:
                    subP0 = P0;
                    subP1 = minLonSplit;
                    subP2 = equatorLineSplit;
                    return;
                case 2:
                    subP0 = equatorLineSplit;
                    subP1 = maxLonSplit;
                    subP2 = P2;
                    return;
                case 3:
                    subP0 = minLonSplit;
                    subP1 = P1;
                    subP2 = maxLonSplit;
                    return;
                default:
                    throw new ArgumentException("Invalid quadrant id: only values between 0 and 3 are allowed");
            }
        }


        /// <summary>
        /// Splits a triangle into for sub-triangles (quadrants)
        ///
        /// Quadrants are numbered in the following way:
        /// THe center triangle gets 0
        /// The triangle closest to the meridian (lon closest to 0 OR to 180) is triangle with id '1'
        /// The triangle furthest to the meridian is 2
        /// The triangle closest to the pole is 3. (Note the a triangle touching the poles will look like a rectangle on a mercator-map; it still is a triangle!)
        /// 
        /// </summary>
        /// <param name="triangle"></param>
        /// <returns></returns>
        [Pure]
        public static (Triangle center0, Triangle minLonSplit, Triangle maxLonSplit, Triangle poleClosest3)
            Split(this Triangle triangle)
        {
            var equatorLineSplit =
                new Coordinate((triangle.P0.Lon + triangle.P2.Lon) / 2,
                    triangle.P0.Lat); // P0.Lat == P2.Lat ==> avg(P0.Lat, P2.lat) == P0.Lat == P2.Lat

            var minLonSplit =
                new Coordinate((triangle.P0.Lon + triangle.P1.Lon) / 2, (triangle.P0.Lat + triangle.P1.Lat) / 2);

            var p1 = triangle.P1;
            if (p1.IsPole())
            {
                // If the polepoint IS the pole, then we have to rewrite the longitude
                // We can do this, as on the pole, the longitude is irrelevant anyway
                // We have to do it to make sure the longitude is the same in the end
                p1 = new Coordinate(triangle.P2.Lon, p1.Lat);
            }


            var maxLonSplit =
                new Coordinate((p1.Lon + triangle.P2.Lon) / 2, (p1.Lat + triangle.P2.Lat) / 2);


            var centerTriangle =
                new Triangle(minLonSplit, equatorLineSplit, maxLonSplit);

            var poleTriangle =
                new Triangle(
                    minLonSplit,
                    triangle.P1,
                    maxLonSplit
                );

            var minLonTriangle =
                new Triangle(
                    triangle.P0,
                    minLonSplit,
                    equatorLineSplit
                );

            var maxLonTriangle =
                new Triangle(
                    equatorLineSplit,
                    maxLonSplit,
                    triangle.P2
                );

            return (centerTriangle, minLonTriangle, maxLonTriangle, poleTriangle);
        }

        /// <summary>
        /// Generates one of the eight top triangles, where 1 -> 4 are the northern hemisphere and 5 -> 8 are the southern hemisphere
        /// 1,5 span latitudes 0 -> 90,
        /// 2,6 span latitudes 90 -> 180,
        /// 3,7 span latitudes 180 -> 270 (-180 -> -90)
        /// 4,8 span latitudes 270 -> 360 (-90 -> 0)
        ///
        ///  And even though this is a triangle, the polygon looks like a rectangle on a map as the pole is stretched.
        /// 
        /// </summary>
        /// <param name="octant"></param>
        /// <returns></returns>
        [Pure]
        public static Triangle GenerateTopTriangle(int octant)
        {
            GenerateTopTriangle(octant, out var P0, out var P1, out var P2);
            return new Triangle(P0, P1, P2);
        }

        public static void GenerateTopTriangle(int octant, out Coordinate P0, out Coordinate P1, out Coordinate P2)
        {
            if (octant < 1 || octant > 8)
            {
                throw new ArgumentException(
                    "Octant out of range, expected a value between 1 (included) and 8 (included) but got " + octant);
            }
            var southernHemisphere = (octant - 1) / 4;
            var latMin = 0;
            var latMax = 90 - 180 * southernHemisphere;

            var quadrant = (octant - 1) % 4;

            var lonMin = 0 + quadrant * 90;
            var lonMax = 90 + quadrant * 90;


            P0 = new Coordinate(lonMin, latMin);
            P1 = new Coordinate(lonMin, latMax);
            P2 = new Coordinate(lonMax, latMin);
        }
    }
}