using System;
using System.Collections.Generic;
using System.Linq;
using Geodesic.Triangles.Internal;

namespace Geodesic.Triangles
{
    /// <summary>
    ///
    /// This class contains all methods needed to convert a coordinate (in WGS84) onto an ID structured by QTM.
    /// Read the README.md for more details
    /// </summary>
    public static class GeodesicTriangles
    {
        /// <summary>
        /// Converts a coordinate into a QTM-id, with 31 (or less) quadrants precision.
        /// </summary>
        /// <param name="coordinate">The WGS84-coordinate, in (lon, lat) format</param>
        /// <param name="precision">How much digits are used. The number of bits used is (precision *2 ) + 2</param>
        /// <returns></returns>
        public static ulong TriangleId(this (double lon, double lat) coordinate, int precision = 30)
        {
            if (precision > 30)
            {
                throw new ArgumentException("precision to big: at most 30 precision can be put into an int64");
            }

            return coordinate.GetId(precision).EncodeLong();
        }

        /// <summary>
        /// Converts a coordinate into a QTM-id, with 15 (or less) quadrants precision.
        /// </summary>
        /// <param name="coordinate">The WGS84-coordinate, in (lon, lat) format</param>
        /// <param name="precision">How much digits are used. The number of bits used is (precision *2 ) + 2</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If precision > 31</exception>
        public static uint TriangleId32(this (double lon, double lat) coordinate, int precision = 15)
        {
            if (precision > 15)
            {
                throw new ArgumentException(
                    "precision to big: use TriangleId() instead. At most 15 precision can be put into an int32");
            }

            return coordinate.GetId(precision).EncodeInt();
        }

        /// <summary>
        /// Converts a QTM-id into a coordinate, by returning the center point of the represented triangle.
        /// When using a precision > 21, this should be less then a few meters away from the original coordinate
        /// </summary>
        public static (double lon, double lat) TriangleCenterPoint(this uint id)
        {
            return TriangleCenterPoint((ulong) id);
        }

        /// <summary>
        /// Converts a QTM-id into a coordinate, by returning the center point of the represented triangle.
        /// When using a precision > 21, this should be less then a few meters away from the original coordinate
        /// </summary>
        public static (double lon, double lat) TriangleCenterPoint(this ulong id)
        {
            return QtmToWgs.GenerateCenterPoint(id.Decode()).ToRadian().ToDegrees().AsTuple();
        }

        /// <summary>
        /// Gives the triangle that contains this point, for the given precision.
        /// Note that this polygon is always a triangle (as seen on the globe), but it might not look like one on a mercator projected map:
        /// If one of the angles is the north- or south pole, this point will be stretched out to a line on a mercator projected map,
        /// giving the triangle the appearance of a rectangle
        /// </summary>
        public static IEnumerable<(double lon, double lat)> PolygonAround(this (double lon, double lat) c,
            int precision = 30)
        {
            return PolygonAround(c.GetId(precision));
        }

        /// <summary>
        /// Gives the triangle that contains the given octant.
        /// Note that this polygon is always a triangle (as seen on the globe), but it'll have the appearance of a rectangle
        /// as either the north- or south pole will be contained as point.
        /// </summary>
        public static IEnumerable<(double lon, double lat)> PolygonForOctant(this int octant)
        {
            return PolygonAround(new[] {octant});
        }

        /// <summary>
        /// Gives the triangle that is represented by this identifier
        /// Note that this polygon is always a triangle (as seen on the globe), but it might not look like one on a mercator projected map:
        /// If one of the angles is the north- or south pole, this point will be stretched out to a line on a mercator projected map,
        /// giving the triangle the appearance of a rectangle
        /// </summary>
        public static IEnumerable<(double lon, double lat)> PolygonAround(this ulong id)
        {
            return PolygonAround(id.Decode());
        }

        /// <summary>
        /// Gives the triangle that contains this point, for the given precision.
        /// Note that this polygon is always a triangle (as seen on the globe), but it might not look like one on a mercator projected map:
        /// If one of the angles is the north- or south pole, this point will be stretched out to a line on a mercator projected map,
        /// giving the triangle the appearance of a rectangle
        /// </summary>
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

        public static ulong ParentTriangle(this ulong id)
        {
            var idArr = id.Decode();
            return idArr.EncodeLong(1);
        }
        
        /// <summary>
        /// Determines if the triangle 'container' contains the triangle 'containee'
        /// </summary>
        public static bool Contains(this ulong container, ulong containee)
        {
            if (container < containee)
            {
                // THe container is smaller then the containee
                return false;
            }

            var idContainer = container.Decode();
            var idContainee = containee.Decode();
            if (idContainer.Length < idContainee.Length)
            {
                return false;
            }

            for (int i = 0; i < idContainer.Length; i++)
            {
                if (idContainer[i] != idContainee[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// This method is mostly meant to give some insight into the process, but should not be used in production.
        /// It is excellent to debug with though! This output can be used in a website such as geojson.io or QGIS
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public static string ToGeoJson(
            this IEnumerable<(IEnumerable<(double lon, double lat)>, Dictionary<string, string> properties)> polygons)
        {
            return Utils.ToGeoJson(polygons);
        }

        /// <summary>
        /// This method is mostly meant to give some insight into the process, but should not be used in production.
        /// It is excellent to debug with though! This output can be used in a website such as geojson.io or QGIS
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public static string ToGeoJson(
            this IEnumerable<IEnumerable<(double lon, double lat)>> polygons)
        {
            return Utils.ToGeoJson(polygons.Select(poly => (poly, new Dictionary<string, string>())));
        }

        /// <summary>
        /// This method is mostly meant to give some insight into the process, but should not be used in production.
        /// It is excellent to debug with though! This output can be used in a website such as geojson.io or QGIS
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public static string ToGeoJson(
            this IEnumerable<(double lon, double lat)> polygon)
        {
            return ToGeoJson(new List<IEnumerable<(double, double)>> {polygon});
        }
    }
}