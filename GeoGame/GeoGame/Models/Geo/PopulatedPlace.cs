using SQLite;

namespace GeoGame.Models.Geo
{
    public class PopulatedPlace
    {
        #region Properties

        [Column("adm0name")]
        public string Country { get; set; }

        [Ignore]
        public NetTopologySuite.Geometries.Geometry Geometry { get; set; }

        [Column("ogc_fid")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// Name using the most standard letters
        /// </summary>
        [Column("nameascii")]
        public string NameASCII { get; set; }

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