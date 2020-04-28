using System;
using System.Collections.Generic;
using System.Linq;

namespace Anyways.GeodesicTriangles.Internal
{
    internal static class Utils
    {
        public static string ToGeoJson(this IEnumerable<(double lon, double lat)> line, string colour)
        {
            return new List<(IEnumerable<Coordinate>, string)> {(line.Select(c => new Coordinate(c.lon, c.lat)), colour)}.
                ToGeoJson();
        }

        public static string ToGeoJson(this IEnumerable<(IEnumerable<Coordinate> l, string fill)> lines,
            Coordinate targetPoint = default(Coordinate))
        {
            var linesJson = new List<string>();
            foreach (var (line, fill) in lines)
            {
                var lineWrapped = line.ToList();
                if(!lineWrapped.First().Equals(lineWrapped.Last()))
                {
                    lineWrapped.Add(lineWrapped.First());
                }
                var coordinates = "[" + string.Join(", ", lineWrapped.Select(c => c.AsWrappedCoordinate())) + "]";
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



    }
}