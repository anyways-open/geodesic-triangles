using System.Collections.Generic;
using System.Linq;

namespace Anyways.GeodesicTriangles.Internal
{
    internal static class Utils
    {
        public static string ToGeoJson(
            this IEnumerable<(IEnumerable<(double lon, double lat)> l, Dictionary<string, string> properties)> lines,
            Coordinate targetPoint = default(Coordinate))
        {
            var linesJson = new List<string>();
            foreach (var (line, properties) in lines)
            {
                var lineWrapped = line.ToList();
                if (!lineWrapped.First().Equals(lineWrapped.Last()))
                {
                    lineWrapped.Add(lineWrapped.First());
                }

                var coordinates = "[" + string.Join(", ",
                                      lineWrapped.Select(c => new Coordinate(c.lon, c.lat).AsWrappedCoordinate())) +
                                  "]";
                var propertiesString
                    = "{" + string.Join(", ", properties.Select(kv => $"\"{kv.Key}\": \"{kv.Value}\"")) + "}";
                linesJson.Add(
                    " { \"type\": \"Feature\", \"properties\": " + propertiesString +
                    ", \"geometry\": { \"type\": \"Polygon\", \"coordinates\": ["
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
    }
}