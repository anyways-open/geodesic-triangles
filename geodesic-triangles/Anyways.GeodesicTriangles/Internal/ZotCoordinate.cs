using System;
using System.Collections.Generic;
using System.Linq;

namespace Anyways.GeodesicTriangles.Internal
{
    internal class ZotCoordinate
    {
        public readonly double Px;
        public readonly double Py;
        public readonly bool SouthernHemisphere;

        /// <summary>
        /// A coordinate in ZOT-space (zenithal orthogonal projection)
        /// </summary>
        public ZotCoordinate(double px, double py, bool southernHemisphere)
        {
            Px = px;
            Py = py;
            SouthernHemisphere = southernHemisphere;
        }

        public override string ToString()
        {
            return $"ZOT: dx:{Px} dy:{Py}";
        }

        public ZotCoordinate SetHemisphere(bool hemi)
        {
            return new ZotCoordinate(Px, Py, hemi);
        }
    }

    internal static class CoordinateExtensions
    {

        public static IEnumerable<Coordinate> ToDegrees(this IEnumerable<ZotCoordinate> coors)
        {
            return coors.Select(zot => zot.ToRadian().ToDegrees());
        }
        
        public static IEnumerable<Coordinate> ToDegrees(this (ZotCoordinate a, ZotCoordinate b, ZotCoordinate c) coors)
        {
            return new[]
            {
                coors.a.ToRadian().ToDegrees(),
                coors.b.ToRadian().ToDegrees(),
                coors.c.ToRadian().ToDegrees()
            };
        }
        
        public static ZotCoordinate ToZot(this Coordinate c)
        {
            return ToZot(c.AsPositiveCoordinate().ToRadians());
        }

        public static ZotCoordinate InBetween(ZotCoordinate a, ZotCoordinate b)
        {
            if (a.SouthernHemisphere != b.SouthernHemisphere)
            {
                throw new ArgumentException("InBetween must have same hemispheres");
            }

            return new ZotCoordinate(
                (a.Px + b.Px) / 2,
                (a.Py + b.Py) / 2,
                a.SouthernHemisphere
            );
        }

        /// <summary>
        /// Calculates the scaling factor to rescale the distance from the pole
        /// For this, we consider a point (lat=  0, lon) (thus, on the equator)
        /// This point will always be 90° away from the pole
        ///
        /// However, depending on the longitude, to convert to ZOT, it might be closer, with the most extreme being lon=45° (+ n*90°)
        /// There, the distance to the pole is rescaled to sqrt(2)*90°
        /// </summary>
        private static double FactorForLon(double polarLon)
        {
            // The normal length is the unit circle
            var normalDistance = 1;

            while (polarLon < 0)
            {
                polarLon += 2 * Math.PI;
            }

            while (polarLon > Math.PI / 2)
            {
                // Fold (again)
                polarLon -= Math.PI / 2;
            }

            // Thus: the intersection coordinate is given by:
            var tan = Math.Tan(polarLon);
            var x = 1 / (1 + tan);
            var y = -tan / (1 + tan);

            // At last, we calculate the distance between the pole and the intersection point
            var actualDistance = Math.Sqrt(x * x + y * y);
            return actualDistance / normalDistance;
        }

        private static double Tolerance = 0.00001;
        private static double pi2 = Math.PI / 2;


        public static ZotCoordinate ToZot(this RadianCoordinate c)
        {
            // Lon will always be positive as radianCoordinate is made positive
            if (Math.Abs(c.Lon) < Tolerance ||
                Math.Abs(c.Lon - Math.PI) < Tolerance)
            {
                // Lon is 0 or 180°
                // NO scaling is needed
                return new ZotCoordinate(c.Lon, Math.Abs(c.Lat) - pi2, c.Lat < 0);
            }


            // The ZOT coordinate is a zenithal, orthogonal projection
            // THis means that the center of the coordinate space is the north pole
            // An example of a zenithal projection can be found here: https://casa.nrao.edu/aips2_docs/memos/107/node2.html


            // However, instead of having a circle, we want a square 
            // IN order to do this, we rescale every point
            // THe factor for this depends on the longitude: a longitude of 45°, 135°, -45° and 135° will have maximal rescaling
            // For this rescaling, consider all points of latitude = 0°
            // IN a normal zenithal projection, these would be the unit (outermost) circle of length 90°, which is always 90° away from the pole
            // For the first quadrant (lon = 0° to 90°)
            // We want to project these points onto the diagonal line between (x=0,y=-90) to (x=90,y=0)
            // FOr this, we calculate the intersection point of the line with angle 'longitude' and the aformentioned diagonal
            // This yields a new latitude. This new latitude can be used to calculate the distance to the pole
            // THe new distance divided by the actual distance gives us our rescaling factor for c


            // formula of the diagonal line: y = x - 1
            // formula of the line with slope lon:
            // y = tan(lon - 90) * x
            // => x - 1 = tan(lon -90) * x
            // => x = 1 / (1 - tan(lon-90)

            // This breaks on lon == 90


            var polarDistance = pi2 - Math.Abs(c.Lat);
            var polarLon = c.Lon;

            var factor = FactorForLon(polarLon);
            var rescaled = polarDistance * factor;

            var px = Math.Sin(polarLon) * rescaled;
            var py = Math.Cos(polarLon) * rescaled;

            return new ZotCoordinate(px, -py, c.Lat < 0);
        }

        public static RadianCoordinate ToRadian(this ZotCoordinate c)
        {
            var southern = c.SouthernHemisphere ? -1 : 1;
            if (Math.Abs(c.Px) < Tolerance && Math.Abs(c.Py) < Tolerance)
            {
                return new RadianCoordinate(0, southern * Math.PI / 2);
            }


            var rescaled = Math.Sqrt(c.Px * c.Px + c.Py * c.Py);

            var polarLon = Math.Acos(-c.Py / rescaled);
            if (c.Px < 0)
            {
                polarLon = -polarLon; // Polarlon will be negative here, so we mirror. The negative value is reserved for the southern hemisphere
            }


            var factor = FactorForLon(polarLon);
            var polarDistance = rescaled / factor;

            var lon = polarLon;
            var lat = pi2 - polarDistance;
            return new RadianCoordinate(lon, southern * lat);
        }
    }
}