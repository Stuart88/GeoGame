using SQLite;
using Xamarin.Forms.Maps;

namespace GeoGame.Models.Geo
{
    public class StateProvince
    {
        [Column("ogc_fid")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// Name using the most standard letters
        /// </summary>
        [Column("type_en")]
        public string Type { get; set; }

        [Column("geonunit")]
        public string Country { get; set; }

        [Column("latitude")]
        public double Latitude { get; set; }

        [Column("longitude")]
        public double Longitude { get; set; }

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

        [Ignore]
        public Position Position => new Position(this.Latitude, this.Longitude);
    }
}
