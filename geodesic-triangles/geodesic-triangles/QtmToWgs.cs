using System.Diagnostics.Contracts;

namespace geodesic_triangles
{
    public static class QtmToWgs
    {
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
            var southernHemisphere = (octant - 1) / 4;
            var latMin = 0;
            var latMax = 90 - 180 * southernHemisphere;

            var quadrant = (octant - 1) % 4;

            var lonMin = 0 + quadrant * 90;
            var lonMax = 90 + quadrant * 90;


            return new Triangle(
                new Coordinate(lonMin, latMin),
                new Coordinate(lonMin, latMax),
                new Coordinate(lonMax, latMin)
            );
        }
    }
}