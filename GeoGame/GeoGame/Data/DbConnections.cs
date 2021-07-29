using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GeoGame.Data
{
    public enum DataItem
    {
        BoundaryLines,
        PopulatedPlaces,
        StatesProvinces,
        Countries
    }

    public static class DbNames
    {
        public static readonly string PopulatedPlacesDb = "PopulatedPlaces.sqlite";
        public static readonly string CountriesDb = "CountriesSmall.sqlite";
    }

    public static class DbConnections
    {
       

        public const SQLite.SQLiteOpenFlags Flags =
        // open the database in read/write mode
        SQLite.SQLiteOpenFlags.ReadWrite |
        // create the database if it doesn't exist
        //SQLite.SQLiteOpenFlags.Create |
        // enable multi-threaded database access
        SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath(DataItem item)
        {
            return item switch
            {
                DataItem.PopulatedPlaces => Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, DbNames.PopulatedPlacesDb),
                DataItem.Countries => Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, DbNames.CountriesDb),
                _ => throw new Exception("Database not found!")
            };
        }

    }
}
