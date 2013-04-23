using ProjNet.Converters.WellKnownText;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using System.Windows;

namespace LibPipeline
{
    public enum CoordinateSystem { WGS84, ALBERS_CONIC_EQUAL_AREA };

    public class CoordinateConverter
    {
        private ICoordinateSystem FromCs;
        private ICoordinateSystem ToCs;
        private ICoordinateTransformation Transformation;

        public CoordinateConverter(CoordinateSystem fromCs, CoordinateSystem toCs)
        {
            this.FromCs = this.GetICoordinateSystem(fromCs);
            this.ToCs = this.GetICoordinateSystem(toCs);

            CoordinateTransformationFactory ctfac = new CoordinateTransformationFactory();
            this.Transformation = ctfac.CreateFromCoordinateSystems(this.FromCs, this.ToCs);
        }

        public Point Convert(Point point)
        {
            double[] input = new double[] { point.X, point.Y };
            double[] output = this.Transformation.MathTransform.Transform(input);
            return new Point { X = output[0], Y = output[1] };
        }

        private ICoordinateSystem GetICoordinateSystem(CoordinateSystem cs)
        {
            string wkt = "";

            if (cs == CoordinateSystem.WGS84)
            {
                wkt = "GEOGCS[\"GCS_WGS_1984\",DATUM[\"D_WGS_1984\",SPHEROID[\"WGS_1984\",6378137,298.257223563]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.0174532925199433]]";
            }
            else if (cs == CoordinateSystem.ALBERS_CONIC_EQUAL_AREA)
            {
                wkt = "PROJCS[\"Equal-Area Projection USGS (United States)\", GEOGCS [ \"NAD 83 (Continental US)\", DATUM [\"NAD 83 (Continental US)\", SPHEROID [\"GRS 80\", 6378137.000000, 298.257222]], PRIMEM [ \"Greenwich\", 0.000000 ], UNIT [\"Decimal Degree\", 0.01745329251994330]], PROJECTION [\"albers_conic_equal_area\"], PARAMETER [\"Standard_Parallel_1\", 29.500000], PARAMETER [\"Standard_Parallel_2\", 45.500000], PARAMETER [\"Central_Meridian\", -96.000000], PARAMETER [\"Latitude_Of_Origin\", 23.000000], PARAMETER [\"false_easting\", 0], PARAMETER [\"false_northing\", 0], UNIT [\"Meter\", 1.000000000000]]";
            }

            return CoordinateSystemWktReader.Parse(wkt) as ICoordinateSystem;
        }
    }
}