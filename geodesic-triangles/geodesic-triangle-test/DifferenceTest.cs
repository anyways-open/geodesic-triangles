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
                    var c = new Coordinate(lon / 100, lat / 100);
                    var qtm = c.GenerateId(20);
                    var c0 = qtm.GetCenterPoint();

                    var diff = 
                        Utils.DistanceEstimateInMeter(c, c0);
                    Assert.True(diff < 100, "Diff to big: c: "+c+", c0: "+c0+" diff: "+diff);

                }
            }
            
            
            
        }
        
        
    }
}