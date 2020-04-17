using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace geodesic_triangles
{
    public struct Coordinate
    {
        public readonly double Lon;
        public readonly double Lat;

        public Coordinate(double lon, double lat)
        {
            Lon = lon;

            Lat = lat;
            if (lat > 90 || lat < -90)
            {
                throw new ArgumentException("Latitude out of range, it is " + lat);
            }
        }

        [Pure]
        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public bool IsPole()
        {
            return Lat == -90 || Lat == 90;
        }

        [Pure]
        public override string ToString()
        {
            return "[" + Lon + ", " + Lat + "]";
        }

        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public override bool Equals(object obj)
        {
            return obj is Coordinate c &&
                   c.Lat == Lat && c.Lon == Lon;
        }

        public override int GetHashCode()
        {
            return 7919 * Lat.GetHashCode() + Lon.GetHashCode();
        }

        /// <summary>
        /// Returns a new coordinate which wraps the longitude onto the regular -180 -> 180 range
        /// </summary>
        /// <returns></returns>
        [Pure]
        public Coordinate AsWrappedCoordinate()
        {
            var lon = Lon;
            if (lon > 180)
            {
                lon -= 360;
            }

            if (lon < -180)
            {
                lon += 180;
            }

            return new Coordinate(lon, Lat);
        }
    }
}