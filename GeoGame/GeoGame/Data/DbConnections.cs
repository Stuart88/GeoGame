using System;
using System.IO;

namespace GeoGame.Data
{
    public enum DataItem
    {
        BoundaryLines,
        PopulatedPlaces,
        StatesProvinces,
        Countries
    }

    public static class DbConnections
    {
        #region Fields

        public const SQLite.SQLiteOpenFlags Flags =
        // open the database in read/write mode
        SQLite.SQLiteOpenFlags.ReadWrite |
        // create the database if it doesn't exist
        //SQLite.SQLiteOpenFlags.Create |
        // enable multi-threaded database access
        SQLite.SQLiteOpenFlags.SharedCache;

        #endregion Fields

        #region Methods

        public static string DatabasePath(DataItem item)
        {
            return item switch
            {
                DataItem.PopulatedPlaces => Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, DbNames.PopulatedPlacesDb),
                DataItem.Countries => Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, DbNames.CountriesDb),
                _ => throw new Exception("Database not found!")
            };
        }

        #endregion Methods
    }

    public static class DbNames
    {
        #region Fields

        public static readonly string CountriesDb = "CountriesSmall.sqlite";
        public static readonly string PopulatedPlacesDb = "PopulatedPlaces.sqlite";

        #endregion Fields
    }
}