namespace geodesic_triangles
{
    public static class WgsToQtm
    {

        public static int DetermineOctant(this Coordinate c)
        {
            return (1 + (int) (c.Lon / 90)) - 4 * ((int) (c.Lat - 90) / 90);
        }
        
    }
}