using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace geodesic_triangles
{
    public struct Triangle
    {
        /// <summary>
        /// P0 is the leftmost (lowest lon)
        /// The line between P0 and P2 will always be parallel with the equator
        /// </summary>
        public readonly Coordinate P0;

        /// <summary>
        /// P1 is the 'point' of the triangle, which points towards a pole
        /// </summary>
        public readonly Coordinate P1;

        /// <summary>
        /// P2 is the rightmost (highest lat) 
        /// The line between P0 and P2 will always be parallel with the equator
        /// </summary>
        public readonly Coordinate P2;

        public Triangle(Coordinate p0, Coordinate p1, Coordinate p2)
        {
            P0 = p0;
            P1 = p1;
            P2 = p2;
        }

        public double LatMin => Math.Min(P0.Lat, Math.Min(P1.Lat, P2.Lat));
        public double LatMax => Math.Max(P0.Lat, Math.Max(P1.Lat, P2.Lat));
        public double LonMin => Math.Min(P0.Lon, Math.Min(P1.Lon, P2.Lon));
        public double LonMax => Math.Max(P0.Lon, Math.Max(P1.Lon, P2.Lon));

        public IEnumerable<Coordinate> AsLineString()
        {
            if (!P1.IsPole())
            {
                return new List<Coordinate>
                {
                    P0, P1, P2, P0
                };
            }


            return new List<Coordinate>
            {
                new Coordinate(LonMin, LatMin),
                new Coordinate(LonMin, LatMax),
                new Coordinate(LonMax, LatMax),
                new Coordinate(LonMax, LatMin),
                new Coordinate(LonMin, LatMin)
            };
        }

        [Pure]
        public override string ToString()
        {
            return "[" + P0 + ", " + P1 + ", " + P2 + "]";
        }

        [Pure]
        public string ToGeoJson()
        {
            return AsLineString().ToGeoJson();
        }
    }
}