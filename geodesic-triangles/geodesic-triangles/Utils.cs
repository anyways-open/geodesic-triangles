using System;
using System.Collections.Generic;
using System.Linq;

namespace geodesic_triangles
{
    public static class Utils
    {
        public static string ToGeoJson(this IEnumerable<Coordinate> line)
        {
            return new List<(IEnumerable<Coordinate>, string)> {(line, "#ffffff")}.ToGeoJson();
        }

        public static string ToGeoJson(
            this IEnumerable<((Coordinate a, Coordinate b, Coordinate c) coor, string fill)> triangles,
            Coordinate targetPoint = default(Coordinate))
        {
            var lines = triangles
                //   .Where(triangle => !(triangle.coor.a.IsNan() || triangle.coor.b.IsNan() || triangle.coor.c.IsNan()))
                .Select(triangle =>
                {
                    var coors = (IEnumerable<Coordinate>) new[]
                        {triangle.coor.a, triangle.coor.b, triangle.coor.c, triangle.coor.a};
                    if (triangle.coor.a.IsPole())
                    {
                        var poleB = new Coordinate(triangle.coor.b.Lon, triangle.coor.a.Lat);
                        var poleC = new Coordinate(triangle.coor.c.Lon, triangle.coor.a.Lat);

                        coors = (IEnumerable<Coordinate>) new[]
                        {
                            poleC,
                            poleB,
                            triangle.coor.b, triangle.coor.c, poleC
                        };
                    }

                    return (coors, triangle.fill);
                });
            return ToGeoJson(lines, targetPoint);
        }

        public static List<List<int>> GenerateField(int depth = 5)
        {
            var ids = new List<List<int>> {new List<int> {1}};

            for (int i = 1; i < depth; i++)
            {
                ids = ids.SelectMany(id =>
                    new[]
                    {
                        id.Append(0).ToList(),
                        id.Append(1).ToList(),
                        id.Append(2).ToList(),
                        id.Append(3).ToList()
                    }
                ).ToList();
            }

            return ids;
        }

        public static string ToGeoJson(this IEnumerable<(IEnumerable<Coordinate> l, string fill)> lines,
            Coordinate targetPoint = default(Coordinate))
        {
            var linesJson = new List<string>();
            foreach (var (line, fill) in lines)
            {
                var coordinates = "[" + string.Join(", ", line.Select(c => c.AsWrappedCoordinate())) + "]";
                linesJson.Add(
                    " { \"type\": \"Feature\", \"properties\": {\"fill-opacity\": 0.1,\"stroke-width\":0.5,\"stroke-colour\":\"#000000\", \"fill\": \"" +
                    fill +
                    "\"}, \"geometry\": { \"type\": \"Polygon\", \"coordinates\": ["
                    + coordinates + "]} }");
            }

            var targetPointGeojson = "";
            if (!Equals(targetPoint, default(Coordinate)))
            {
                var pre =
                    ",{\"type\": \"Feature\",\"properties\": {\"marker-color\": \"#ff0000\",\"marker-size\": \"medium\",                        \"marker-symbol\": \"\"                    },                    \"geometry\": {                        \"type\": \"Point\",                        \"coordinates\": ";

                var post = "}}";
                targetPointGeojson = pre + targetPoint + post;
            }


            return "{ \"type\": \"FeatureCollection\", \"features\": [" + string.Join(", ", linesJson) +
                   targetPointGeojson + "]}";
        }

        public static string ToGeoJson(params (Triangle timothea, string fillColour)[] vars)
        {
            return vars.Select(v => (v.timothea.AsLineString(), v.fillColour)).ToGeoJson();
        }


        private const double RadiusOfEarth = 6371000;

        /// <summary>
        /// Returns an estimate of the distance between the two given coordinates.
        /// Stolen from https://github.com/itinero/routing/blob/1764afc75db43a1459789592de175283f642123f/src/Itinero/LocalGeo/Coordinate.cs
        /// </summary>
        /// <remarks>Accuracy decreases with distance.</remarks>
        public static float DistanceEstimateInMeter(Coordinate c1, Coordinate c2)
        {
            var lat1Rad = c1.Lat / 180d * Math.PI;
            var lon1Rad = c1.Lon / 180d * Math.PI;
            var lat2Rad = c2.Lat / 180d * Math.PI;
            var lon2Rad = c2.Lon / 180d * Math.PI;

            var x = (lon2Rad - lon1Rad) * Math.Cos((lat1Rad + lat2Rad) / 2.0);
            var y = lat2Rad - lat1Rad;

            var m = Math.Sqrt(x * x + y * y) * RadiusOfEarth;

            return (float) m;
        }
    }
}