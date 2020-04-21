using System;

namespace geodesic_triangles
{
    public class RadianCoordinate
    {
        public readonly double Lon;
        public readonly double Lat;

        public RadianCoordinate(double lon, double lat)
        {
            Lon = lon;
            Lat = lat;
        }

        public Coordinate ToDegrees()
        {
            return new Coordinate(Lon * 180 / Math.PI, Lat * 180 / Math.PI);
        }
    }
}