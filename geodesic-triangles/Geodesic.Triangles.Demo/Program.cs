using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Anyways.GeodesicTriangles.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var data = File.ReadLines("FietsstallingenGent.csv").ToList();
            var coordinates = new List<((double lon, double lat), uint capacity)>();
            for (int i = 1; i < data.Count(); i++)
            {
                Console.WriteLine(data[i]);
                if(string.IsNullOrEmpty(data[i]))
                {
                    continue;
                }
                var line = data[i].Split(",");
                var c = (double.Parse(line[0]), double.Parse(line[1]));
                var capacity = 6u;
                if (!string.IsNullOrEmpty(line[2]))
                {
                    capacity = uint.Parse(line[2]);
                }

                coordinates.Add((c, capacity));
            }

            var hist = new Dictionary<ulong, uint>();
            foreach (var (c, capacity) in coordinates)
            {
                var id = c.TriangleId(16);
                if (!hist.ContainsKey(id))
                {
                    hist[id] = 0;
                }

                hist[id] += capacity;
            }

            var triangles = new List<(IEnumerable<(double lon, double lat)>, Dictionary<string, string>)>();

            var max = hist.Select(kv => kv.Value).Max();

            max /= 10;
            
            foreach (var (id, v) in hist)
            {
                Console.WriteLine(id + " " + v);
                var triangle = id.PolygonAround();
                var intensity = (double) v / max;
                var properties = new Dictionary<string, string>
                {
                    {"capacity",v.ToString()},
                    {"stroke", "#000000"},
                    {"stroke-width", "0.1"},
                    {"stroke-opacity", "1"},
                    {"fill", "#0000ff"},
                    {"fill-opacity", intensity.ToString()}
                };
                triangles.Add((triangle, properties));
            }

            var geojson = triangles.ToGeoJson();
            File.WriteAllText("Example.geojson", geojson);
        }
    }
}