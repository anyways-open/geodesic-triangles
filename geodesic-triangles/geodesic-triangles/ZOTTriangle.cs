using System;

namespace geodesic_triangles
{
    public class ZOTCoordinate
    {
        public readonly double Px;
        public readonly double Py;

        /// <summary>
        /// A coordinate in ZOT-space (zenithal orthogonal projection)
        /// </summary>
        /// <param name="lon"></param>
        /// <param name="lat"></param>
        /// <exception cref="ArgumentException"></exception>
        public ZOTCoordinate(double px, double py)
        {
            Px = px;
            Py = py;
        }
    }

    public static class CoordinateExtensions
    {
        private static int[] flops = new[] {1, 1, -1, -1, -1, 1, 1, -1};

        public static ZOTCoordinate ToZot(this RadianCoordinate c)
        {
            // See https://cartogis.org/docs/proceedings/archive/auto-carto-10/pdf/zenithial-orthotriangular-projection.pdf
            // Page 6
            // I too don't exactly now what is going on, I just copied the paper
            // We use Diam = 1 to have a unit square to work on
            var s = 1 / Math.PI;
            var p2 = Math.PI / 2;

            var org = (int) ((p2 - c.Lat) / p2);
            var oct = (int) ((org + 1) * (c.Lon / p2));

            var x1 = 2 - (oct + org - 1) % 2;
            var x2 = 3 - x1;

            var hs = 1 - 2 * org;
            var rx1 = s * flops[oct];
            var rx2 = -s * hs * flops[9 - oct];
            var cx1 = -org * rx1;
            var cx2 = -org * rx2;

            var clp = p2 - (hs * c.Lat); // abs colatitude of point
            var olp = clp * (c.Lon * p2) / p2; // lon offset
            var px = cx1 + (rx1 * Math.Abs(clp - olp));
            var py = cx2 + (rx2 * olp);
            
            return new ZOTCoordinate(px, py);
        }
    }
}