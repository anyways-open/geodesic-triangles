using System;
using System.Collections.Generic;
using System.IO;
using geodesic_triangles;

namespace tests
{
    class Program
    {
        private static void Main(string[] args)
        {
           File.WriteAllText("test.geojson", (Utils.SplitAndGeojson(new List<Triangle>
            {
                QtmToWgs.GenerateTopTriangle(2).Split().poleClosest3
            }, 5)));
        }
    }
}