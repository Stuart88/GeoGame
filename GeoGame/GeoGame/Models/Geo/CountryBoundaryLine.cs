using SQLite;

namespace GeoGame.Models.Geo
{
    public class CountryBoundaryLine
    {
        [Column("ogc_fid")]
        public int Id { get; set; }

        [Column("adm0_left")]
        public string LeftBoundary { get; set; }

        [Column("adm0_right")]
        public string RightBoundary { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("featurecla")]
        public string Feature { get; set; }

        [Column("WKT_GEOMETRY")]
        public string WktGeometry
        {
            get
            {
                // Don't need this as it's just for processing into Geometry object from WKT string in db
                return null;
            }
            set
            {
                var reader = new NetTopologySuite.IO.WKTReader();

                this.Geometry = reader.Read(value);
            }
        }

        [Ignore]
        public NetTopologySuite.Geometries.Geometry Geometry { get; set; }


    }
}
