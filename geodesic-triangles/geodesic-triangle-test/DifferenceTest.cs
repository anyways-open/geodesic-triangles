using System;
using geodesic_triangles;
using Xunit;

namespace geodesic_triangle_test
{
    public class DifferenceTest
    {
        
        [Fact]
        public void ToQTM_FromQTM_DifferenceIsSmall()
        {

            for (int lon = -17000; lon < 17000; lon++)
            {
                for (int lat = -4500; lat < 8500; lat++)
                {
                    var c = new Coordinate((double) lon / 100, (double) lat / 100);
                    var qtm = c.GetId(20);

                   // var diff = 
                    //    Utils.DistanceEstimateInMeter(c, c0);
                //    Assert.True(diff < 100, "Diff to big: c: "+c+", c0: "+c0+" diff: "+diff);

                }
            }
            
            
            
        }
        
        
    }
}