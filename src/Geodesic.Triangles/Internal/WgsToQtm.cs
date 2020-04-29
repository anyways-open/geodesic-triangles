using System;

namespace Geodesic.Triangles.Internal
{
    internal static class WgsToQtm
    {
        public static int[] GetId(this Coordinate c, int depth)
        {
            return c.ToRadians().ToZot().GetId(depth);
        }

        private static double pi2 = Math.PI / 2;


        private static readonly ZotCoordinate[] _eastMostPointsForOctant
            =
            {
                new ZotCoordinate(0, -pi2, false), // Octant zero == octant 4
                new ZotCoordinate(pi2, 0, false),

                new ZotCoordinate(0, pi2, false),
                new ZotCoordinate(-pi2, 0, false),
                new ZotCoordinate(0, -pi2, false),
            };


        public static ZotCoordinate EastMostPointFor(int octant)
        {
            return _eastMostPointsForOctant[octant % 5];
        }

        public static ZotCoordinate WestMostPointFor(int octant)
        {
            return _eastMostPointsForOctant[octant % 5 - 1];
        }

        public static int[] GetId(this (double lon, double lat) coordinate, int precision)
        {
            return new Coordinate(coordinate.lon, coordinate.lat).ToRadians().ToZot().GetId(precision);
        }

        public static int[] GetId(this ZotCoordinate c, int precision)
        {
            int oct;
            {
                var xSign = Math.Sign(c.Px);
                var ySign = Math.Sign(c.Py);
                if (xSign == 1)
                {
                    oct = ySign == 1 ? 2 : 1;
                }
                else
                {
                    oct = ySign == 1 ? 3 : 4;
                }
            }

            var id = new int[precision];
            id[0] = oct;
            if (c.SouthernHemisphere)
            {
                id[0] += 4;
            }


            if (oct == 2)
            {
                c = new ZotCoordinate(c.Py, -c.Px, false);
                oct = 1;
            }

            if (oct == 4)
            {
                c = new ZotCoordinate(-c.Py, c.Px, false);
                oct = 1;
            }

            var eastMost = _eastMostPointsForOctant[oct];
            var westMost = _eastMostPointsForOctant[oct - 1]; // one octant less = 90° longitude westward


            // Pole node
            var polePointing = new ZotCoordinate(0, 0, false);
            var inverted = false;

            for (var i = 1; i < precision; i++)
            {
                var q = DetermineQuadrant(c, polePointing, eastMost, westMost,
                    out polePointing, out eastMost, out westMost);
                if (inverted)
                {
                    if (q == 1)
                    {
                        q = 2;
                    }
                    else if (q == 2)
                    {
                        q = 1;
                    }
                }

                if (q == 0)
                {
                    var p = eastMost;
                    eastMost = westMost;
                    westMost = p;
                    inverted = !inverted;
                }


                id[i] = q;
            }

            return id;
        }


        /// <summary>
        /// Determines in which quadrant of the triangle the point lies, where p0, p1 and p2 are corners of the triangle
        /// </summary>
        /// <param name="c"></param>
        /// <param name="perpendicularPoint">The node which is the origin of the coordinate system describing c</param>
        /// <param name="westMost">The clockwise/western most node (differs from paper)</param>
        /// <param name="eastMost">The counterclockwise/eastern most node (differs from paper)</param>
        /// <param name="subPerpendicular">Then new point having the perpendicular angle, of the newly selected quadrant</param>
        /// <param name="subWestMost">Then new clockwise point of the newly selected quadrant</param>
        /// <param name="subEastMost">Then new counterclockwise point of the newly selected quadrant</param>
        /// <returns></returns>
        private static int DetermineQuadrant(ZotCoordinate c,
            ZotCoordinate perpendicularPoint, ZotCoordinate westMost, ZotCoordinate eastMost,
            out ZotCoordinate subPerpendicular, out ZotCoordinate subWestMost, out ZotCoordinate subEastMost
        )
        {
            // Length of the side of the current triangle. west and east should always behave neatly, as they are aligned with the grid


            var s = Math.Abs(westMost.Px - eastMost.Px);
            var s2 = s / 2;


            var dx = Math.Abs(perpendicularPoint.Px - c.Px);
            var dy = Math.Abs(perpendicularPoint.Py - c.Py);


            // Manhattan distance is < s/2 from the pole point
            // We return 1 (pole closest)
            if (dx + dy < s2)
            {
                // p1n - the new pole-pointing-point lies between p2 and p3
                subPerpendicular = perpendicularPoint;


                // p2n - the new westmost point lies between the polepoint p1 and the current westmost point p2
                subWestMost = CoordinateExtensions.InBetween(perpendicularPoint, westMost);
                // analog for p3
                subEastMost = CoordinateExtensions.InBetween(eastMost, perpendicularPoint);

                return 3;
            }

            if (dy > s2)
            {
                // The centerpoint moves a bit, halfway between eastmost and polePointing
                // However, the orientation doesn't change, so we don't have to rewrite the coordinates
                subPerpendicular = CoordinateExtensions.InBetween(eastMost, perpendicularPoint);

                // Eastmost triangle
                // The eastmost point stays the same
                subEastMost = eastMost;
                // The westmost point will lie halfway westmost and eastmost
                subWestMost = CoordinateExtensions.InBetween(eastMost, westMost);

                return 1;
            }

            if (dx > s2)
            {
                // The westmost triangle
                subPerpendicular = CoordinateExtensions.InBetween(westMost, perpendicularPoint);
                subEastMost = CoordinateExtensions.InBetween(westMost, eastMost);
                subWestMost = westMost;

                return 2;
            }

            subPerpendicular = CoordinateExtensions.InBetween(eastMost, westMost);
            // Swapping is handled explicitly at the start of the method
            subEastMost = CoordinateExtensions.InBetween(eastMost, perpendicularPoint);
            subWestMost = CoordinateExtensions.InBetween(westMost, perpendicularPoint);
            return 0;
        }
    }
}