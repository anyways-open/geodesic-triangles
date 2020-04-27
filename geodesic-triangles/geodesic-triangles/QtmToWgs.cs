using System;
using System.Collections.Generic;
using System.Linq;
using static geodesic_triangles.CoordinateExtensions;

// ReSharper disable InconsistentNaming

namespace geodesic_triangles
{
    public static class QtmToWgs
    {
        private static (Coordinate a, Coordinate b, Coordinate c) ToDegrees(this
            (ZotCoordinate a, ZotCoordinate b, ZotCoordinate c) zots)
        {
            return (zots.a.ToRadian().ToDegrees().AsWrappedCoordinate(),
                    zots.b.ToRadian().ToDegrees().AsWrappedCoordinate(),
                    zots.c.ToRadian().ToDegrees().AsWrappedCoordinate()
                );
        }

        public static (Coordinate a, Coordinate b, Coordinate c) GenerateTriangle(int[] id)
        {
            return GenerateTriangleZot(id).ToDegrees();
        }

        public static IEnumerable<(Coordinate a, Coordinate b, Coordinate c)> GenerateAllTriangles(int[] id)
        {
            return GenerateAllTrianglesZot(id).Select(ToDegrees);
        }

        private static IEnumerable<(ZotCoordinate pole, ZotCoordinate east, ZotCoordinate west)>
            GenerateAllTrianglesZot(int[] id)
        {
            var oct = id[0];
            var p0 = new ZotCoordinate(0, 0);
            var east = WgsToQtm.EastMostPointFor(oct);
            var west = WgsToQtm.WestMostPointFor(oct);

            if (oct == 1)
            {
                var p = west;
                west = east;
                east = p;
            }
            
            var triangles = new List<(ZotCoordinate, ZotCoordinate, ZotCoordinate)>
                {(p0, east, west)};

            for (int i = 1; i < id.Length; i++)
            {
                GenerateTriangleQuadrant(id[i], p0, west, east, out p0, out west, out east);
                triangles.Add((p0, east, west));
            }

            return triangles;
        }

        private static (ZotCoordinate pole, ZotCoordinate east, ZotCoordinate west)
            GenerateTriangleZot(int[] id)
        {
            var p0 = new ZotCoordinate(0, 0);
            var east = WgsToQtm.EastMostPointFor(id[0]);
            var west = WgsToQtm.WestMostPointFor(id[0]);
            
            for (int i = 1; i < id.Length; i++)
            {
                GenerateTriangleQuadrant(id[i], p0, west, east, out p0, out west, out east);
            }

            return (p0, east, west);
        }

        private static void GenerateTriangleQuadrant(int quadrant,
            ZotCoordinate perpendicularPoint, ZotCoordinate westMost, ZotCoordinate eastMost,
            out ZotCoordinate subPerpendicular, out ZotCoordinate subWestMost, out ZotCoordinate subEastMost
        )
        {
            switch (quadrant)
            {
                case 0:
                    // Pole inversion
                    subPerpendicular = InBetween(eastMost, westMost);
                    subEastMost = InBetween(westMost, perpendicularPoint);
                    subWestMost = InBetween(eastMost, perpendicularPoint);
                    break;
                case 1:
                    subPerpendicular = InBetween(eastMost, perpendicularPoint);
                    subEastMost = eastMost;
                    subWestMost = InBetween(eastMost, westMost);
                    break;
                case 2:
                    subPerpendicular = InBetween(westMost, perpendicularPoint);
                    subEastMost = InBetween(westMost, eastMost);
                    subWestMost = westMost;
                    break;
                case 3:
                    subPerpendicular = perpendicularPoint;
                    subWestMost = InBetween(perpendicularPoint, westMost);
                    subEastMost = InBetween(eastMost, perpendicularPoint);
                    break;
                default:
                    throw new ArgumentException("Invalid quadrant: " + quadrant);
            }
        }
    }
}