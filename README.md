# geodesic-triangles

Geodesic-Triangles is a small library, which uses fancy math to split the surface of the earth into triangles, where the triangles are roughly the same size.

Every triangle has an identifier, and for every point the identifier of the containing triangle can be quickly calculated.
For every identifier, the centerpoint or outline of the corresponding triangle can be quickly calculated too.

The input coordinates are WGS84 (which is the common format for web applications such as OpenStreetMap and many others).
(Note: WGS84 only supports latitudes between -85° and 85°, this library supports up to -90° and 90°).

## Using the library

Using the library is pretty simple:

```
import Anyways.GeodesicTriangles.GeodesicTriangles

....


    // Get the identifier of the triangle which contains the given point
    ulong id = (longitude, latitude).TriangleId();
    
    // Convert back to a coordinate by using the center point of the triangle
    var (longitude0, latitude0) = id.TriangleCenterPoint();

    // Get the outline of the triangle which is represented by this identifier
    var poly = id.PolygonAround()
    
    // Build a histogram in order to know which cells have more coordinates
    List<(double lon, double lat)> coordinates = ...
    var histogram = new Dictionary<ulong, uint>();
    foreach(var c in coordinates){
        var id = c.TriangleId(15); // A precision of 15 wil generate triangles which are sized like a moderate village
        if(!histogram.ContainsKey(id)) histogram[id] = 0;
        histogram[id]++;
    }

```

# Example

All the triangles of all precisions to encode a location in Belgium, Europe:

![BruggeTraingles](Examples/ExampleTriangles_Brugge.png]


## Underlying mathematics


_(Note: all coordinates are written as `(lon, lat)`)_

In orer to determine the ID of the triangle containing the given point, we progressively build an array of numbers, indicating the which part of the world we are working with.

### Octant

To get started, the surface of the world is split in eight parts, each called an **octant**.

Octant *1* lies between `(0,0)`, `(90,0)` and the north pole `(*,90)`; octant *2* lies between `(90,0)`, `(180,0)` and the north pole, octant *3* is between `(180,0)`,`(270,0)` and the north pole, and the *4*th octant (you guessed it) is between `(270,0)`, `(360,0)` and the north pole.

The southern hemisphere follows the same pattern, with octant *5* between `(0,0)`, `(90,0)` and the _south pole_, octant *6* between `(90,0)`, `(180,0)` and the south pole, ...

The octant number (between 1-8 inclusive; not zero-indexed) is the first number we keep track of. The octant coordinates is the first and biggest _triangle_ that the point falls in. (Note that this triangle contains a pole as angle, which makes it _look_ like a rectangle on a Mercator projected map! Another fun fact is that this triangle has _three_ perpendicular angles).


![BruggeTraingles](Examples/ExampleTriangles_Brugge0.png]


### Quandrants

When the octant is determined, this corresponding triangle is cut into four parts: a triangle at the top, a triangle on the left, one on the right and one in the middle. This can be done easily, as the coordinates describing these triangle are either the average of the points of the bigger triangle, or points from this bigger triangle.

Every triangle is assigned a number. We determine in which triangle the coordinate we want to encode falls; the corresponding number is appended to the array with the ID.
If this triangle is still to big for the intended use, it can be split into four parts again, repeating the process just described until the result is satisfactory.

![BruggeTraingles](Examples/ExampleTriangles_Brugge1.png]
![BruggeTraingles](Examples/ExampleTriangles_Brugge2.png]
![BruggeTraingles](Examples/ExampleTriangles_Brugge3.png]

### Encoding the ID

In the end, we end up with a list of numbers describing triangles, each one a quarter of the size of the previous, bigger triangle. This list of triangles will have the format of:

`[ number between 1 - 8, number between 0 - 3, number between 0 - 3, number between 0 - 3, ...]`

This can efficiently be encoded into a ulong or uint. The interested reader can refer directly to the source code to see the details of this encoding.

### Quadrants revisited

Even though the triangles containing the point can be easily determined, it is quite hard to determine if a point lies within the triange, as the lines of a triangle over the world is not straight but a curve.

In order to circumvent this (and to speed up the calculation), the points are converted into ZOT-space. This is a polar projection, but instead of having a unit circle representing the equator, a unit square is used (with the corners aligning with `0°`,`90°`,`180` and `270°` longitude). Calculating this for a point in the first octant is done as following (although heavily simplified):

1) Convert from WGS84 to polar, where the polar coordinate is `(longitude, distance from the pole)`, or simply `(longitude, 90° - latitude)`
1) Calculate the distance `d` between point on the equator with the same longitudefrom the pole in ZOT-space. In polar coordinates, a point on the equator is always 90° away. In ZOT-space, these points fall on the line between `(0,-90°)` and `(90°,0). 
2) Rescale the polar coordinate to `(longitude, (90° - latitude) * (d / 90°))`

At this point, calculating the quadrant from a triangle can be done with Manhatten distance. For more details, see the included papers.



## Approximate sizes per precision level

This table gives a rough indication of how big a triangle is on each precision level.
The numbers are rounded, but can differ slightly between the triangles.


Digits of precision | Size of triangle (km²) | Size estimate
--------------------+------------------------+------
1                   | 64 000 000 | 1/8 of the earth
2                   | 22 500 000 | a big continent
3                   | 4 250 000 | a small continent
4                   | 1 400 000 | a few big countries
5                   | 310 000 | a big country
6                   | 82 500 | a small country
7                   | 20 000 | 
8                   | 5 000 | a region containing multiple cities or a metropole as Paris or NY
9                   | 1 250 | a big city such as brussles
10                  | 	320 | a smaller city
11                  | 80 | a village or multiple city districts
12                  | 20 | a city district
13                  | 5 | a city quarter
14                  | 1.25 | quite few residential blocks
15                  | 0.30 | a pair of residential blocks
16                  | 0.08 | half a residential block
17                  | 20 000m² | around 10 packed houses
18                  | 5 000m² | a few houses
19                  | 1 200m² | 
20                  | 300m² | one house
21                  | 75m² | a room
22                  | 20m² | a very very tiny house
23                  | 5m² | 
24                  | 1.2m² | A garden table
25                  | 0.25m² | 


