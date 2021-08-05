using GeoGame.Models.Geo;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoGame.Data
{
    public class DbContexts
    {
        #region Fields

        public static readonly AsyncLazy<DbContexts> Instance = new AsyncLazy<DbContexts>(async () =>
        {
            var instance = new DbContexts();
            return instance;
        });

        private static SQLiteAsyncConnection CountriesDb;
        private static SQLiteAsyncConnection PopulatedPlacedDb;

        #endregion Fields

        #region Constructors

        public DbContexts()
        {
            PopulatedPlacedDb = new SQLiteAsyncConnection(DbConnections.DatabasePath(DataItem.PopulatedPlaces), DbConnections.Flags);
            CountriesDb = new SQLiteAsyncConnection(DbConnections.DatabasePath(DataItem.Countries), DbConnections.Flags);
        }

        #endregion Constructors

        #region Methods

        public Task<List<Country>> GetAllCountries()
        {
            return CountriesDb.QueryAsync<Country>("SELECT ogc_fid, admin, name_en, name_long, continent, region_un, subregion, pop_est, GEOMETRY FROM countriessmall ORDER BY pop_est ASC");
        }

        public Task<List<PopulatedPlace>> GetAllPopulatedPlaceData()
        {
            return PopulatedPlacedDb.QueryAsync<PopulatedPlace>("SELECT ogc_fid, name, nameascii, adm0name, GEOMETRY FROM populatedplaces");
        }

        public async Task<Country> GetCountry(int id)
        {
            var res = await CountriesDb.QueryAsync<Country>("SELECT ogc_fid, admin, name_en, name_long, continent, region_un, subregion, pop_est, GEOMETRY FROM countriessmall WHERE ogc_fid = ?", id);

            if (res.Count == 1)
                return res[0];
            else return null;
        }

        public Task<List<PopulatedPlace>> GetPopulatedPlacesForCountry(string countryName)
        {
            return PopulatedPlacedDb.QueryAsync<PopulatedPlace>("SELECT ogc_fid, name, nameascii, adm0name, GEOMETRY FROM populatedplaces WHERE adm0name = ?", countryName);
        }

        #endregion Methods
    }
}