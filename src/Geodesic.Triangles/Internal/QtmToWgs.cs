using System;
using System.Collections.Generic;

// ReSharper disable InconsistentNaming

namespace Geodesic.Triangles.Internal
{
    internal static class QtmToWgs
    {

        public static IEnumerable<(ZotCoordinate pole, ZotCoordinate east, ZotCoordinate west)>
            GenerateAllTrianglesZot(int[] id)
        {
            var oct = id[0];
            ZotCoordinate p0;
            ZotCoordinate east;
            ZotCoordinate west;
            if (oct <= 4)
            {
                p0 = new ZotCoordinate(0, 0, false);
                east = WgsToQtm.EastMostPointFor(oct);
                west = WgsToQtm.WestMostPointFor(oct);
            }
            else
            {
                p0 = new ZotCoordinate(0, 0, true);
                east = WgsToQtm.EastMostPointFor(oct - 4).SetHemisphere(true);
                west = WgsToQtm.WestMostPointFor(oct - 4).SetHemisphere(true);
            }

            bool inverted = false;

            var triangles = new List<(ZotCoordinate, ZotCoordinate, ZotCoordinate)>
                {(p0, east, west)};

            for (var i = 1; i < id.Length; i++)
            {
                var q = id[i];

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
                    inverted = !inverted;
                }

                GenerateTriangleQuadrant(q, p0, west, east,
                    out p0, out west, out east);

                triangles.Add((p0, east, west));
            }

            return triangles;
        }

        public static ZotCoordinate GenerateCenterPoint(int[] id)
        {
            var (p, east, west) = GenerateTriangleZot(id);
            return CoordinateExtensions.InBetween(p, CoordinateExtensions.InBetween(east, west));
        }

        public static (ZotCoordinate pole, ZotCoordinate east, ZotCoordinate west)
            GenerateTriangleZot(int[] id)
        {
            var oct = id[0];
            ZotCoordinate p0;
            ZotCoordinate east;
            ZotCoordinate west;
            if (oct <= 4)
            {
                p0 = new ZotCoordinate(0, 0, false);
                east = WgsToQtm.EastMostPointFor(oct);
                west = WgsToQtm.WestMostPointFor(oct);
            }
            else
            {
                p0 = new ZotCoordinate(0, 0, true);
                east = WgsToQtm.EastMostPointFor(oct - 4).SetHemisphere(true);
                west = WgsToQtm.WestMostPointFor(oct - 4).SetHemisphere(true);
            }

            bool inverted = false;


            for (var i = 1; i < id.Length; i++)
            {
                var q = id[i];

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
                    inverted = !inverted;
                }

                GenerateTriangleQuadrant(q, p0, west, east,
                    out p0, out west, out east);
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
                    subPerpendicular = CoordinateExtensions.InBetween(eastMost, westMost);
                    subEastMost = CoordinateExtensions.InBetween(westMost, perpendicularPoint);
                    subWestMost = CoordinateExtensions.InBetween(eastMost, perpendicularPoint);
                    break;
                case 2:
                    subPerpendicular = CoordinateExtensions.InBetween(eastMost, perpendicularPoint);
                    subEastMost = eastMost;
                    subWestMost = CoordinateExtensions.InBetween(eastMost, westMost);
                    break;
                case 1:
                    subPerpendicular = CoordinateExtensions.InBetween(westMost, perpendicularPoint);
                    subEastMost = CoordinateExtensions.InBetween(westMost, eastMost);
                    subWestMost = westMost;
                    break;
                case 3:
                    subPerpendicular = perpendicularPoint;
                    subWestMost = CoordinateExtensions.InBetween(perpendicularPoint, westMost);
                    subEastMost = CoordinateExtensions.InBetween(eastMost, perpendicularPoint);
                    break;
                default:
                    throw new ArgumentException("Invalid quadrant: " + quadrant);
            }
        }
    }
}