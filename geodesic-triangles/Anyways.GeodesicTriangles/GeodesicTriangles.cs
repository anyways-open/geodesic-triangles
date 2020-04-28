using System;
using System.Collections.Generic;
using System.Linq;
using Anyways.GeodesicTriangles.Internal;

namespace Anyways.GeodesicTriangles
{
    /// <summary>
    /// Geodesic triangles: main interface
    /// </summary>
    public static class GeodesicTriangles
    {
        public static ulong TriangleId(this (double lon, double lat) coordinate, int precision = 30)
        {
            if (precision > 30)
            {
                throw new ArgumentException("precision to big: at most 30 precision can be put into an int32");
            }

            return coordinate.GetId(precision).EncodeLong();
        }

        public static uint TriangleId32(this (double lon, double lat) coordinate, int precision = 14)
        {
            if (precision > 14)
            {
                throw new ArgumentException(
                    "precision to big: use TriangleId() instead. At most 14 precision can be put into an int32");
            }

            return coordinate.GetId(precision).EncodeInt();
        }

        public static (double lon, double lat) TriangleCenterPoint(this uint id)
        {
            return TriangleCenterPoint((ulong) id);
        }

        public static (double lon, double lat) TriangleCenterPoint(this ulong id)
        {
            return QtmToWgs.GenerateCenterPoint(id.Decode()).ToRadian().ToDegrees().AsTuple();
        }

        public static IEnumerable<(double lon, double lat)> PolygonAround(this (double lon, double lat) c,
            int precision = 30)
        {
            return PolygonAround(c.GetId(precision));
        }

        public static IEnumerable<(double lon, double lat)> PolygonForOctant(this int octant)
        {
            return PolygonAround(new[] {octant});
        }

        public static IEnumerable<(double lon, double lat)> PolygonAround(this ulong id)
        {
            return PolygonAround(id.Decode());
        }

        public static IEnumerable<(double lon, double lat)> PolygonAround(this uint id)
        {
            return PolygonAround(id.Decode());
        }

        private static IEnumerable<(double lon, double lat)> PolygonAround(this int[] id)
        {
            var tr = QtmToWgs.GenerateTriangleZot(id);

            var pole = tr.pole.ToRadian().ToDegrees();
            var a = tr.east.ToRadian().ToDegrees();
            var b = tr.west.ToRadian().ToDegrees();

            var coordinates = new List<(double lon, double lat)>();
            if (pole.IsPole())
            {
                coordinates.Add((a.Lon, pole.Lat));
                coordinates.Add(a.AsTuple());
                coordinates.Add(b.AsTuple());
                coordinates.Add((b.Lon, pole.Lat));
            }
            else
            {
                coordinates.Add(pole.AsTuple());
                coordinates.Add(a.AsTuple());
                coordinates.Add(b.AsTuple());
            }

            return coordinates;
        }

        /// <summary>
        /// This method is mostly meant to give some insight into the process, but should not be used in production.
        /// It is excellent to debug with though! This output can be used in a website such as geojson.io or QGIS
        /// </summary>
        /// <returns></returns>
        public static string AsGeoJson(this IEnumerable<(double lon, double lat)> polygon, string colour = "#ff0000")
        {
            return polygon.ToGeoJson(colour);
        }
    }
}