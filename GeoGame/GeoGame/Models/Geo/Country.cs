using SQLite;

namespace GeoGame.Models.Geo
{
    public class Country
    {
        #region Properties

        [Column("continent")]
        public string Continent { get; set; }

        [Ignore]
        public NetTopologySuite.Geometries.Geometry Geometry { get; set; }

        [Column("ogc_fid")]
        public int Id { get; set; }

        [Column("name_en")]
        public string Name { get; set; }

        [Column("name_long")]
        public string NameLong { get; set; }

        [Column("admin")]
        public string NameOfficial { get; set; }

        [Column("pop_est")]
        public int Population { get; set; }

        [Column("region_un")]
        public string Region { get; set; }

        [Column("subregion")]
        public string SubRegion { get; set; }

        [Column("GEOMETRY")]
        public byte[] WkbGeometry
        {
            get
            {
                // Don't need this as it's just for processing into Geometry object from WKT string in db
                return null;
            }
            set
            {
                var reader = new NetTopologySuite.IO.WKBReader();

                this.Geometry = reader.Read(value);
            }
        }

        #endregion Properties
    }
}