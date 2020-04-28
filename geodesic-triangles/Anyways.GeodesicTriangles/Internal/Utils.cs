using System.Collections.Generic;
using System.Linq;

namespace Anyways.GeodesicTriangles.Internal
{
    internal static class Utils
    {
        public static string ToGeoJson(this IEnumerable<(double lon, double lat)> line, string colour)
        {
            var contents = new List<(IEnumerable<(double lon, double lat)> l, string fill)>
            {
                (line, colour)
            };
            return contents.ToGeoJson();
        }

        public static string ToGeoJson(this IEnumerable<(IEnumerable<(double lon, double lat)> l, string fill)> lines,
            Coordinate targetPoint = default(Coordinate))
        {
            var linesJson = new List<string>();
            foreach (var (line, fill) in lines)
            {
                var lineWrapped = line.ToList();
                if (!lineWrapped.First().Equals(lineWrapped.Last()))
                {
                    lineWrapped.Add(lineWrapped.First());
                }

                var coordinates = "[" + string.Join(", ",
                                      lineWrapped.Select(c => new Coordinate(c.lon, c.lat).AsWrappedCoordinate())) +
                                  "]";
                linesJson.Add(
                    " { \"type\": \"Feature\", \"properties\": {\"fill-opacity\": 0.05,\"stroke-width\":0.8,\"stroke-colour\":\"#000000\", \"fill\": \"" +
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
    }
}