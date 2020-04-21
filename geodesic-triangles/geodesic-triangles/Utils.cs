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


        public static string DebugGenerateId(this Coordinate c, int precision)
        {
            var id = c.GenerateId(precision);

            var jsonInput = new List<(IEnumerable<Coordinate>, string)>();


            var startTriangle = QtmToWgs.GenerateTopTriangle(id[0]);
            jsonInput.Add((startTriangle.AsLineString(), "#000000"));


            for (int i = 1; i < id.Length; i++)
            {
                startTriangle = startTriangle.GetQuadrant(id[i]);
                jsonInput.Add((startTriangle.AsLineString(), "#00ff00"));
            }

            return ToGeoJson(jsonInput, c);
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

        public static string SplitAndGeojson(this List<Triangle> currentTriangles, int depth)
        {
            var jsonInput = new List<(IEnumerable<Coordinate>, string)>();

            for (int d = 0; d < depth; d++)
            {
                jsonInput.Clear();
                var newTriangles = new List<Triangle>();
                foreach (var triangle in currentTriangles)
                {
                    var (center0, minLonSplit, maxLonSplit, poleClosest3) = triangle.Split();
                    jsonInput.Add((center0.AsLineString(), "#000000"));
                    jsonInput.Add((minLonSplit.AsLineString(), "#ff0000"));
                    jsonInput.Add((poleClosest3.AsLineString(), "#00ff00"));
                    jsonInput.Add((maxLonSplit.AsLineString(), "#0000ff"));

                    newTriangles.Add(center0);
                    newTriangles.Add(minLonSplit);
                    newTriangles.Add(poleClosest3);
                    newTriangles.Add(maxLonSplit);
                }

                currentTriangles = newTriangles;
            }

            return ToGeoJson(jsonInput);
        }
    }
}