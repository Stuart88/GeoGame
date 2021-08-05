using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using System.IO;

namespace GeoGame.Droid
{
    [Activity(Label = "GeoGame", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        #region Fields

        private const int RequestLocationId = 0;

        private readonly string[] LocationPermissions =
            {
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation
        };

        #endregion Fields

        #region Methods

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == RequestLocationId)
            {
                if ((grantResults.Length == 1) && (grantResults[0] == (int)Permission.Granted))
                {
                    // Permissions granted - display a message.
                }
                else
                {
                    // Permissions denied - display a message.
                }
            }
            else
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Xamarin.FormsMaps.Init(this, savedInstanceState);

            CheckAndLoadDatabases();

            LoadApplication(new App());
        }

        protected override void OnStart()
        {
            base.OnStart();

            if ((int)Build.VERSION.SdkInt >= 23)
            {
                if (CheckSelfPermission(Manifest.Permission.AccessFineLocation) != Permission.Granted)
                {
                    RequestPermissions(LocationPermissions, RequestLocationId);
                }
                else
                {
                    // Permissions already granted - display a message.
                }
            }
        }

        private void CheckAndLoadDatabases()
        {
            string populatedPlacesDbPath = Data.DbConnections.DatabasePath(Data.DataItem.PopulatedPlaces);
            if (!File.Exists(populatedPlacesDbPath))
            {
                Stream input = Assets.Open(Data.DbNames.PopulatedPlacesDb);
                using (var memoryStream = new MemoryStream())
                {
                    input.CopyTo(memoryStream);
                    File.WriteAllBytes(populatedPlacesDbPath, memoryStream.ToArray());
                }
            }

            string countriesDbPath = Data.DbConnections.DatabasePath(Data.DataItem.Countries);
            if (!File.Exists(countriesDbPath))
            {
                Stream input = Assets.Open(Data.DbNames.CountriesDb);
                using (var memoryStream = new MemoryStream())
                {
                    input.CopyTo(memoryStream);
                    File.WriteAllBytes(countriesDbPath, memoryStream.ToArray());
                }
            }
        }

        #endregion Methods
    }
}