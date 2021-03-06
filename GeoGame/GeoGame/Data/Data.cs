using GeoGame.Models.Battles.Weapons;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static GeoGame.Data.MapEnums;

namespace GeoGame.Data
{
    public static class MapEnums
    {
        #region Enums

        public enum ElementType
        {
            Country,
            PopulatedPlace
        }

        public enum MapTheme
        {
            Standard,
            Silver,
            Retro,
            Dark,
            Night,
            Aubergine
        }

        #endregion Enums
    }

    public static class Game
    {
        #region Fields

        public static readonly string SaveGameFile = Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, "GameData.json");
        public static GameData GameData = new GameData();

        #endregion Fields

        #region Methods

        public static int GetCurrentPlayerHealth()
        {
            return 100 + 5 * (GameData.CountriesDefeatedIds.Count - 1);
        }

        public static void LoadGame()
        {
            if (File.Exists(SaveGameFile))
            {
                using (var reader = new StreamReader(File.Open(SaveGameFile, FileMode.Open)))
                {
                    var jsonData = reader.ReadToEnd();
                    GameData = JsonConvert.DeserializeObject<GameData>(jsonData);
                }
            }
        }

        public static void SaveGame()
        {
            GameData.CountriesDefeatedIds = GameData.CountriesDefeatedIds.Distinct().ToList(); // somehow '0' was being added multiple times..?
            string json = JsonConvert.SerializeObject(GameData);

            File.WriteAllText(SaveGameFile, json);
        }

        #endregion Methods
    }

    public static class Data
    {
        #region Fields

        public static readonly string PlainMapAubergine_Style = "[ { \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#1d2c4d\" } ] }, { \"elementType\": \"labels\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#8ec3b9\" } ] }, { \"elementType\": \"labels.text.stroke\", \"stylers\": [ { \"color\": \"#1a3646\" } ] }, { \"featureType\": \"administrative\", \"elementType\": \"geometry\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"administrative.country\", \"elementType\": \"geometry.stroke\", \"stylers\": [ { \"color\": \"#4b6878\" } ] }, { \"featureType\": \"administrative.land_parcel\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#64779e\" } ] }, { \"featureType\": \"administrative.neighborhood\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"administrative.province\", \"elementType\": \"geometry.stroke\", \"stylers\": [ { \"color\": \"#4b6878\" } ] }, { \"featureType\": \"landscape.man_made\", \"elementType\": \"geometry.stroke\", \"stylers\": [ { \"color\": \"#334e87\" } ] }, { \"featureType\": \"landscape.natural\", \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#023e58\" } ] }, { \"featureType\": \"poi\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"poi\", \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#283d6a\" } ] }, { \"featureType\": \"poi\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#6f9ba5\" } ] }, { \"featureType\": \"poi\", \"elementType\": \"labels.text.stroke\", \"stylers\": [ { \"color\": \"#1d2c4d\" } ] }, { \"featureType\": \"poi.park\", \"elementType\": \"geometry.fill\", \"stylers\": [ { \"color\": \"#023e58\" } ] }, { \"featureType\": \"poi.park\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#3C7680\" } ] }, { \"featureType\": \"road\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"road\", \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#304a7d\" } ] }, { \"featureType\": \"road\", \"elementType\": \"labels.icon\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"road\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#98a5be\" } ] }, { \"featureType\": \"road\", \"elementType\": \"labels.text.stroke\", \"stylers\": [ { \"color\": \"#1d2c4d\" } ] }, { \"featureType\": \"road.highway\", \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#2c6675\" } ] }, { \"featureType\": \"road.highway\", \"elementType\": \"geometry.stroke\", \"stylers\": [ { \"color\": \"#255763\" } ] }, { \"featureType\": \"road.highway\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#b0d5ce\" } ] }, { \"featureType\": \"road.highway\", \"elementType\": \"labels.text.stroke\", \"stylers\": [ { \"color\": \"#023e58\" } ] }, { \"featureType\": \"transit\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"transit\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#98a5be\" } ] }, { \"featureType\": \"transit\", \"elementType\": \"labels.text.stroke\", \"stylers\": [ { \"color\": \"#1d2c4d\" } ] }, { \"featureType\": \"transit.line\", \"elementType\": \"geometry.fill\", \"stylers\": [ { \"color\": \"#283d6a\" } ] }, { \"featureType\": \"transit.station\", \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#3a4762\" } ] }, { \"featureType\": \"water\", \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#0e1626\" } ] }, { \"featureType\": \"water\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#4e6d70\" } ] } ]";
        public static readonly string PlainMapDark_Style = "[ { \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#212121\" } ] }, { \"elementType\": \"labels\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"elementType\": \"labels.icon\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#757575\" } ] }, { \"elementType\": \"labels.text.stroke\", \"stylers\": [ { \"color\": \"#212121\" } ] }, { \"featureType\": \"administrative\", \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#757575\" }, { \"visibility\": \"off\" } ] }, { \"featureType\": \"administrative.country\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#9e9e9e\" } ] }, { \"featureType\": \"administrative.land_parcel\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"administrative.locality\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#bdbdbd\" } ] }, { \"featureType\": \"administrative.neighborhood\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"poi\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"poi\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#757575\" } ] }, { \"featureType\": \"poi.park\", \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#181818\" } ] }, { \"featureType\": \"poi.park\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#616161\" } ] }, { \"featureType\": \"poi.park\", \"elementType\": \"labels.text.stroke\", \"stylers\": [ { \"color\": \"#1b1b1b\" } ] }, { \"featureType\": \"road\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"road\", \"elementType\": \"geometry.fill\", \"stylers\": [ { \"color\": \"#2c2c2c\" } ] }, { \"featureType\": \"road\", \"elementType\": \"labels.icon\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"road\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#8a8a8a\" } ] }, { \"featureType\": \"road.arterial\", \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#373737\" } ] }, { \"featureType\": \"road.highway\", \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#3c3c3c\" } ] }, { \"featureType\": \"road.highway.controlled_access\", \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#4e4e4e\" } ] }, { \"featureType\": \"road.local\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#616161\" } ] }, { \"featureType\": \"transit\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"transit\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#757575\" } ] }, { \"featureType\": \"water\", \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#000000\" } ] }, { \"featureType\": \"water\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#3d3d3d\" } ] } ]";
        public static readonly string PlainMapNight_Style = "[ { \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#242f3e\" } ] }, { \"elementType\": \"labels\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#746855\" } ] }, { \"elementType\": \"labels.text.stroke\", \"stylers\": [ { \"color\": \"#242f3e\" } ] }, { \"featureType\": \"administrative\", \"elementType\": \"geometry\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"administrative.locality\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#d59563\" } ] }, { \"featureType\": \"administrative.neighborhood\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"poi\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"poi\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#d59563\" } ] }, { \"featureType\": \"poi.park\", \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#263c3f\" } ] }, { \"featureType\": \"poi.park\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#6b9a76\" } ] }, { \"featureType\": \"road\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"road\", \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#38414e\" } ] }, { \"featureType\": \"road\", \"elementType\": \"geometry.stroke\", \"stylers\": [ { \"color\": \"#212a37\" } ] }, { \"featureType\": \"road\", \"elementType\": \"labels.icon\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"road\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#9ca5b3\" } ] }, { \"featureType\": \"road.highway\", \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#746855\" } ] }, { \"featureType\": \"road.highway\", \"elementType\": \"geometry.stroke\", \"stylers\": [ { \"color\": \"#1f2835\" } ] }, { \"featureType\": \"road.highway\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#f3d19c\" } ] }, { \"featureType\": \"transit\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"transit\", \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#2f3948\" } ] }, { \"featureType\": \"transit.station\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#d59563\" } ] }, { \"featureType\": \"water\", \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#17263c\" } ] }, { \"featureType\": \"water\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#515c6d\" } ] }, { \"featureType\": \"water\", \"elementType\": \"labels.text.stroke\", \"stylers\": [ { \"color\": \"#17263c\" } ] } ]";
        public static readonly string PlainMapRetro_Style = "[ { \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#ebe3cd\" } ] }, { \"elementType\": \"labels\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#523735\" } ] }, { \"elementType\": \"labels.text.stroke\", \"stylers\": [ { \"color\": \"#f5f1e6\" } ] }, { \"featureType\": \"administrative\", \"elementType\": \"geometry\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"administrative\", \"elementType\": \"geometry.stroke\", \"stylers\": [ { \"color\": \"#c9b2a6\" } ] }, { \"featureType\": \"administrative.land_parcel\", \"elementType\": \"geometry.stroke\", \"stylers\": [ { \"color\": \"#dcd2be\" } ] }, { \"featureType\": \"administrative.land_parcel\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#ae9e90\" } ] }, { \"featureType\": \"administrative.neighborhood\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"landscape.natural\", \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#dfd2ae\" } ] }, { \"featureType\": \"poi\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"poi\", \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#dfd2ae\" } ] }, { \"featureType\": \"poi\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#93817c\" } ] }, { \"featureType\": \"poi.park\", \"elementType\": \"geometry.fill\", \"stylers\": [ { \"color\": \"#a5b076\" } ] }, { \"featureType\": \"poi.park\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#447530\" } ] }, { \"featureType\": \"road\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"road\", \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#f5f1e6\" } ] }, { \"featureType\": \"road\", \"elementType\": \"labels.icon\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"road.arterial\", \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#fdfcf8\" } ] }, { \"featureType\": \"road.highway\", \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#f8c967\" } ] }, { \"featureType\": \"road.highway\", \"elementType\": \"geometry.stroke\", \"stylers\": [ { \"color\": \"#e9bc62\" } ] }, { \"featureType\": \"road.highway.controlled_access\", \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#e98d58\" } ] }, { \"featureType\": \"road.highway.controlled_access\", \"elementType\": \"geometry.stroke\", \"stylers\": [ { \"color\": \"#db8555\" } ] }, { \"featureType\": \"road.local\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#806b63\" } ] }, { \"featureType\": \"transit\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"transit.line\", \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#dfd2ae\" } ] }, { \"featureType\": \"transit.line\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#8f7d77\" } ] }, { \"featureType\": \"transit.line\", \"elementType\": \"labels.text.stroke\", \"stylers\": [ { \"color\": \"#ebe3cd\" } ] }, { \"featureType\": \"transit.station\", \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#dfd2ae\" } ] }, { \"featureType\": \"water\", \"elementType\": \"geometry.fill\", \"stylers\": [ { \"color\": \"#b9d3c2\" } ] }, { \"featureType\": \"water\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#92998d\" } ] } ]";
        public static readonly string PlainMapSilver_Style = "[ { \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#f5f5f5\" } ] }, { \"elementType\": \"labels\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"elementType\": \"labels.icon\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#616161\" } ] }, { \"elementType\": \"labels.text.stroke\", \"stylers\": [ { \"color\": \"#f5f5f5\" } ] }, { \"featureType\": \"administrative\", \"elementType\": \"geometry\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"administrative.land_parcel\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#bdbdbd\" } ] }, { \"featureType\": \"administrative.neighborhood\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"poi\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"poi\", \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#eeeeee\" } ] }, { \"featureType\": \"poi\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#757575\" } ] }, { \"featureType\": \"poi.park\", \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#e5e5e5\" } ] }, { \"featureType\": \"poi.park\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#9e9e9e\" } ] }, { \"featureType\": \"road\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"road\", \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#ffffff\" } ] }, { \"featureType\": \"road\", \"elementType\": \"labels.icon\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"road.arterial\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#757575\" } ] }, { \"featureType\": \"road.highway\", \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#dadada\" } ] }, { \"featureType\": \"road.highway\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#616161\" } ] }, { \"featureType\": \"road.local\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#9e9e9e\" } ] }, { \"featureType\": \"transit\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"transit.line\", \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#e5e5e5\" } ] }, { \"featureType\": \"transit.station\", \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#eeeeee\" } ] }, { \"featureType\": \"water\", \"elementType\": \"geometry\", \"stylers\": [ { \"color\": \"#c9c9c9\" } ] }, { \"featureType\": \"water\", \"elementType\": \"labels.text.fill\", \"stylers\": [ { \"color\": \"#9e9e9e\" } ] } ]";
        public static readonly string PlainMapStandardStyle = "[ { \"elementType\": \"labels\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"administrative\", \"elementType\": \"geometry\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"administrative.neighborhood\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"poi\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"road\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"road\", \"elementType\": \"labels.icon\", \"stylers\": [ { \"visibility\": \"off\" } ] }, { \"featureType\": \"transit\", \"stylers\": [ { \"visibility\": \"off\" } ] } ]";

        #endregion Fields

        public static string GetThemeStyleString(MapTheme theme)
        {
            return theme switch
            {
                MapTheme.Standard => PlainMapStandardStyle,
                MapTheme.Silver => PlainMapSilver_Style,
                MapTheme.Retro => PlainMapRetro_Style,
                MapTheme.Dark => PlainMapDark_Style,
                MapTheme.Night => PlainMapNight_Style,
                MapTheme.Aubergine => PlainMapAubergine_Style,
                _ => PlainMapStandardStyle
            };

        }
    }

    public static class MessagingCenterMessages
    {
        #region Fields

        public static readonly string HighlightCountry = "HighlightCountry";
        public static readonly string OpenCountryBattle = "OpenCountryBattle";
        public static readonly string WonCountryBattle = "WonCountryBattle";
        public static readonly string LostCountryBattle = "LostCountryBattle";
        public static readonly string CountryClicked = "CountryClicked";
        public static readonly string GotCountries = "GotCountries";
        public static readonly string SetMapTheme = "SetMapTheme";
        public static readonly string FlashPolygon = "FlashPolygon";

        #endregion Fields
    }

    public class GameData
    {
        #region Fields

        public List<int> CountriesDefeatedIds = new List<int>() { 0 };
        public List<WeaponsEnum> AvailableWeapons = new List<WeaponsEnum>() { WeaponsEnum.SlowBlaster };
        public MapEnums.MapTheme MapTheme = MapEnums.MapTheme.Aubergine;

        #endregion Fields

        // init with zero for creating savegame file
    }
}