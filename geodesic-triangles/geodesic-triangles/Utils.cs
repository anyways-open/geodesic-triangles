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


        public static string ToGeoJson(this IEnumerable<(IEnumerable<Coordinate> l, string fill)> lines)
        {
            var linesJson = new List<string>();
            foreach (var (line, fill) in lines)
            {
                var coordinates = "[" + string.Join(", ", line.Select(c => c.AsWrappedCoordinate())) + "]";
                linesJson.Add(
                    " { \"type\": \"Feature\", \"properties\": {\"fill-opacity\": 0.1,\"stroke-width\":0, \"fill\": \"" + fill +
                    "\"}, \"geometry\": { \"type\": \"Polygon\", \"coordinates\": ["
                    + coordinates + "]} }");
            }

            return "{ \"type\": \"FeatureCollection\", \"features\": [" + string.Join(", ", linesJson) + "]}";
        }

        public static string ToGeoJson(params (Triangle timothea, string fillColour)[] vars)
        {
            return vars.Select(v => (v.timothea.AsLineString(), v.fillColour)).ToGeoJson();
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