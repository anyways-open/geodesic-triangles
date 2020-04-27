using System;
using System.Diagnostics.CodeAnalysis;
using static geodesic_triangles.CoordinateExtensions;

namespace geodesic_triangles
{
    public static class WgsToQtm
    {
        public static int[] GetId(this Coordinate c, int depth)
        {
            return c.ToRadians().ToZot().GetId(depth);
        }

        private static double pi2 = Math.PI / 2;


        private static ZotCoordinate[] EastMostPointsForOctant
            =
            {
                new ZotCoordinate(0, -pi2), // Octant zero == octant 4
                new ZotCoordinate(pi2, 0),
              
                new ZotCoordinate(0, pi2),
                new ZotCoordinate(-pi2, 0),
                new ZotCoordinate(0, -pi2),
            };

        public static ZotCoordinate EastMostPointFor(int octant)
        {
            return EastMostPointsForOctant[octant];
        }

        public static ZotCoordinate WestMostPointFor(int octant)
        {
            return EastMostPointsForOctant[octant - 1];
        }

        public static int[] GetId(this ZotCoordinate c, int precision)
        {
            var oct = 0; // TODO

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

            var id = new int[precision];
            id[0] = oct;


            // With the octant known,
            // We turn the coordinate into the well-known first octant
            if (oct == 2)
            {
                c = new ZotCoordinate(
                    c.Py,
                    -c.Px
                    );
            }
            
            
            var eastMost = EastMostPointsForOctant[1];
            var westMost = EastMostPointsForOctant[1 - 1]; // one octant less = 90° longitude westward

            // Pole node
            var polePointing = new ZotCoordinate(0, 0);
            if (oct > 5)
            {
                // Southern hemisphere: the extremities of both other points
                polePointing = new ZotCoordinate(eastMost.Px + westMost.Px, eastMost.Py + westMost.Py);
            }


            for (int i = 1; i < precision; i++)
            {
                id[i] = DetermineQuadrant(c, polePointing, eastMost, westMost, out polePointing, out eastMost,
                    out westMost, out c);
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
        /// <returns></returns>
        private static int DetermineQuadrant(this ZotCoordinate c,
            ZotCoordinate perpendicularPoint, ZotCoordinate westMost, ZotCoordinate eastMost,
            out ZotCoordinate subPerpendicular, out ZotCoordinate subWestMost, out ZotCoordinate subEastMost,
            out ZotCoordinate newCoordinate
        )
        {
            // Length of the side of the current triangle
            var s = Math.Abs(westMost.Px - perpendicularPoint.Px);
            var s2 = s / 2;

            var dx = Math.Abs(perpendicularPoint.Px - c.Px);
            var dy = Math.Abs(perpendicularPoint.Py - c.Py);
            newCoordinate = c;

            // Manhatten distance is < s/2 from the pole point
            // We return 1 (pole closest)
            if (dx + dy < s2)
            {
                // p1n - the new pole-pointing-point lies between p2 and p3
                subPerpendicular = perpendicularPoint;


                // p2n - the new westmost point lies between the polepoint p1 and the current westmost point p2
                subWestMost = InBetween(perpendicularPoint, westMost);
                // analog for p3
                subEastMost = InBetween(eastMost, perpendicularPoint);

                return 3;
            }

            if (dy > s2)
            {
                // The centerpoint moves a bit, halfway between eastmost and polePointing
                // However, the orientation doesn't change, so we don't have to rewrite the coordinates
                subPerpendicular = InBetween(eastMost, perpendicularPoint);

                // Eastmost triangle
                // The eastmost point stays the same
                subEastMost = eastMost;
                // The westmost point will lie halfway westmost and eastmost
                subWestMost = InBetween(eastMost, westMost);


                return 1;
            }

            if (dx > s2)
            {
                // The westmost triangle
                subPerpendicular = InBetween(westMost, perpendicularPoint);
                subEastMost = InBetween(westMost, eastMost);
                subWestMost = westMost;

                return 2;
            }


            // The center triangle
            // The polepointing orientation changes
            // This implies that the reference of the ZOT-coordinate changes too!
            // 

            subPerpendicular = InBetween(eastMost, westMost);
            // S is calculated using westmost.px, so there should be a difference between them
            subEastMost = InBetween(westMost, perpendicularPoint);
            subWestMost = InBetween(eastMost, perpendicularPoint);
            return 0;
        }
    }
}