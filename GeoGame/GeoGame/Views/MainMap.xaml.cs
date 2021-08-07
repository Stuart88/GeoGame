using GeoGame.Data;
using GeoGame.Interfaces;
using GeoGame.Models.Geo;
using GeoGame.Models.Mapping;
using GeoGame.ViewModels;
using Plugin.SimpleAudioPlayer;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace GeoGame.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainMap : ContentPage, IMessageService
    {
        #region Fields

        private Random _rand = new Random();
        private string _stateProvinceStr = "StateProvince";

        #endregion Fields

        #region Constructors

        public MainMap()
        {
            this.BindingContext = new MainMapViewModel();
            InitializeComponent();
            SubscribeToMessages();
            InitMusic();
            InitMap();
        }

        #endregion Constructors

        #region Properties

        public ISimpleAudioPlayer Music { get; set; } = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
        private DbContexts Contexts { get; set; }
        private List<Country> Countries { get; set; }
        private int FightButtonFlashCount { get; set; } = 0;
        private MainMapViewModel GetViewModel => (MainMapViewModel)this.BindingContext;

        //private Country SelectedCountry { get; set; }
        private CustomMap Map { get; set; }

        #endregion Properties

        #region Methods

        public void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<IMessageService, Country>(this, Data.MessagingCenterMessages.OpenCountryBattle, async (sender, data) =>
            {
                await Navigation.PushModalAsync(new CountryBattle(data, this.Countries), false);
            });

            MessagingCenter.Subscribe<IMessageService, Country>(this, Data.MessagingCenterMessages.CountryClicked, (sender, c) =>
            {
                this.GetViewModel.SelectedCountry = c;
                PanMapToCountry(c);
            });

            MessagingCenter.Subscribe<IMessageService, object>(this, Data.MessagingCenterMessages.LostCountryBattle, async (sender, obj) =>
            {
                await Navigation.PopModalAsync();

                await this.DisplayAlert("DEAD", "You died!", "Okay :(");
            });

            MessagingCenter.Subscribe<IMessageService, Country>(this, Data.MessagingCenterMessages.WonCountryBattle, async (sender, country) =>
            {
                await Navigation.PopModalAsync();

                if (Game.GameData.CountriesDefeatedIds.Contains(country.Id)) // This was a replay battle. Don't need to do anything here.
                    return;

                if (Game.GameData.CountriesDefeatedIds.Count == this.Countries.Count) // Game already completed
                    return;

                Game.GameData.CountriesDefeatedIds.Add(country.Id);

                if (Game.GameData.CountriesDefeatedIds.Count == 20)
                {
                    await this.DisplayAlert("NEW WEAPON!", "Star Blaster is available", "COOL!");
                    Game.GameData.AvailableWeapons.Add(Models.Battles.Weapons.WeaponsEnum.StarBlaster);
                }
                if (Game.GameData.CountriesDefeatedIds.Count == 50)
                {
                    await this.DisplayAlert("NEW WEAPON!", "Fast Blaster is available", "COOL!");
                    Game.GameData.AvailableWeapons.Add(Models.Battles.Weapons.WeaponsEnum.FastBlaster);
                }
                if (Game.GameData.CountriesDefeatedIds.Count == 100)
                {
                    await this.DisplayAlert("NEW WEAPON!", "Spread Blaster is available", "COOL!");
                    Game.GameData.AvailableWeapons.Add(Models.Battles.Weapons.WeaponsEnum.SpreadBlaster);
                }
                if (Game.GameData.CountriesDefeatedIds.Count == 150)
                {
                    await this.DisplayAlert("NEW WEAPON!", "Hornet Blaster is available", "COOL!");
                    Game.GameData.AvailableWeapons.Add(Models.Battles.Weapons.WeaponsEnum.HornetBlaster);
                }

                Game.SaveGame();

                Country nextCountry = GetNextCountryToBattle();

                if (nextCountry == null)
                {
                    // GAME FINISHED?!
                    await this.DisplayAlert("WINNER!", "YOU FINISHED THE GAME!", "Yay :)");
                }
                else
                {
                    this.GetViewModel.SelectedCountry = nextCountry;
                    //this.UpdateLabels();
                    this.PanMapToCountry(this.GetViewModel.SelectedCountry);
                }
            });
        }

        protected override void OnAppearing()
        {
            if (this.Countries != null) // this is 'reappearring'
                this.Music.Play();

            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            this.Music.Stop();
            base.OnDisappearing();
        }

        private void AddCountryPolygonToMap(Country c, NetTopologySuite.Geometries.Geometry geo)
        {
            Polygon poly = new Polygon()
            {
                BindingContext = new Tuple<Country, NetTopologySuite.Geometries.Geometry>(c, geo)
            };

            this.Map.MapElements.Add(poly);
        }

        private async void BeginBattleBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new CountryBattle(this.GetViewModel.SelectedCountry, this.Countries), false);
        }

        private void CenterOnCountryBtn_Clicked(object sender, EventArgs e)
        {
            this.PanMapToCountry(this.GetViewModel.SelectedCountry);

            MessagingCenter.Send<IMessageService, Country>(this, MessagingCenterMessages.FlashPolygon, this.GetViewModel.SelectedCountry);
        }

        private void DrawGeometries(Country c, NetTopologySuite.Geometries.Geometry g)
        {
            for (int i = 0; i < g.NumGeometries; i++)
            {
                NetTopologySuite.Geometries.Geometry geo = g.GetGeometryN(i);

                if (geo.NumGeometries > 1)
                    DrawGeometries(c, geo); // Recursive. Some countries are a set of multiple polygons (island chains etc)

                AddCountryPolygonToMap(c, geo);
            }
        }

        /// <summary>
        /// Loads country data and moves map to correct country
        /// </summary>
        private void GetCountryData()
        {
            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(new Action(async () =>
            {
                this.ShowSpinner();

                this.Contexts = await DbContexts.Instance;
                this.Countries = await this.Contexts.GetAllCountries();

                int index = 1;
                foreach (Country c in this.Countries)
                {
                    c.IndexNumber = index++;
                    DrawGeometries(c, c.Geometry);
                }

                MessagingCenter.Send<IMessageService, List<Country>>(this, Data.MessagingCenterMessages.GotCountries, this.Countries);

                // Get country the player is currently on

                this.GetViewModel.SelectedCountry = GetNextCountryToBattle();

                this.Map.Opacity = 1;

                this.Music.Play();

                this.HideSpinner();

                PanMapToCountry(this.GetViewModel.SelectedCountry);
            }));
        }

        /// <summary>
        /// Gets largest geometry for country (some countries are made of several geometries, e.g. island chains etc)
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private NetTopologySuite.Geometries.Geometry GetLargestGeometry(Country c)
        {
            NetTopologySuite.Geometries.Geometry g = c.Geometry;
            double area = g.Area;
            NetTopologySuite.Geometries.Geometry largestGeo = g;

            for (int i = 0; i < g.NumGeometries; i++)
            {
                NetTopologySuite.Geometries.Geometry thisGeom = g.GetGeometryN(i);
                if (thisGeom.Area > area)
                    largestGeo = thisGeom;
            }

            return largestGeo;
        }

        private Country GetNextCountryToBattle()
        {
            return this.Countries.FirstOrDefault(c => !Game.GameData.CountriesDefeatedIds.Contains(c.Id));
        }

        private void HideSpinner()
        {
            this.Map.IsEnabled = true;
            this.Map.Opacity = 1;
            LoadingSpinner.IsRunning = false;
        }

        private void InitMap()
        {
            this.Map = new CustomMap()
            {
                MapType = MapType.Street,
                HasScrollEnabled = true,
                HasZoomEnabled = true,
                IsShowingUser = false,
                TrafficEnabled = false,
                MoveToLastRegionOnLayoutChange = false,
                Opacity = 0
            };

            this.MapContentArea.Content = this.Map;

            //Gets countries ordered by population (smallest to largest)
            this.GetCountryData();
        }

        private void InitMusic()
        {
            int songNum = _rand.Next(1, 6);
            this.Music.Load(Helpers.Functions.GetStreamFromFile($"Resources.Music.Map.{songNum}.mp3"));
            this.Music.Volume = 0.8;
            this.Music.PlaybackEnded += (s, e) => { this.Music.Play(); };
        }

        private Distance MaxDistanceAcrossCountry(NetTopologySuite.Geometries.Envelope env)
        {
            //horizontal
            double d1 = Location.CalculateDistance(new Location(env.MinX, env.MinY), new Location(env.MaxX, env.MinY), DistanceUnits.Kilometers);
            //vertical
            double d2 = Location.CalculateDistance(new Location(env.MinX, env.MinY), new Location(env.MinX, env.MaxY), DistanceUnits.Kilometers);
            //diagonal
            double d3 = Location.CalculateDistance(new Location(env.MinX, env.MinY), new Location(env.MaxX, env.MaxY), DistanceUnits.Kilometers);

            double[] vals = new double[] { d1, d2, d3 };

            double max = vals.Max();

            return Distance.FromKilometers(max);
        }

        private void NextBattleBtn_Clicked(object sender, EventArgs e)
        {
            var c = this.GetNextCountryToBattle();

            this.PanMapToCountry(c);

            this.GetViewModel.SelectedCountry = c;
            
            Device.StartTimer(TimeSpan.FromMilliseconds(300), () =>
            {
                // Simulate a kind of flashing effect to point the user to press the 'Fight' button after they press 'Next Battle'

                this.BeginBattleBtn.IsEnabled = !this.BeginBattleBtn.IsEnabled;
                this.BeginBattleBtn.Opacity = this.BeginBattleBtn.IsEnabled ? 1 : 0.7;

                this.FightButtonFlashCount++;

                if (this.FightButtonFlashCount == 5)
                {
                    this.BeginBattleBtn.IsEnabled = true;
                    this.BeginBattleBtn.Opacity = 1;
                    this.FightButtonFlashCount = 0;
                    return false;
                }

                return true;
            });
        }

        private void NextCountryBtn_Clicked(object sender, EventArgs e)
        {
            int i = this.Countries.IndexOf(this.GetViewModel.SelectedCountry);

            if (i == this.Countries.Count - 1) // already at last country
                return;

            this.GetViewModel.SelectedCountry = this.Countries[i + 1];

            this.PanMapToCountry(this.GetViewModel.SelectedCountry);
        }

        private void PanMapToCountry(Country c)
        {
            MessagingCenter.Send<IMessageService, Country>(this, Data.MessagingCenterMessages.HighlightCountry, c);

            NetTopologySuite.Geometries.Geometry geom = GetLargestGeometry(c);

            Distance d = MaxDistanceAcrossCountry(geom.EnvelopeInternal);

            Position mapPos = new Position(geom.Envelope.Centroid.Y, geom.Envelope.Centroid.Coordinate.X);

            MapSpan span = MapSpan.FromCenterAndRadius(mapPos, d);

            if (Data.Game.GameData.CountriesDefeatedIds.Contains(c.Id) || this.GetNextCountryToBattle().Id == c.Id)
            {
                // Can fight
                BeginBattleBtn.IsEnabled = true;
                BeginBattleBtn.Opacity = 1;
            }
            else
            {
                // Cannot fight!
                BeginBattleBtn.IsEnabled = false;
                BeginBattleBtn.Opacity = 0.7;
            }

            this.Map.MoveToRegion(span);
        }

        private void PreviousCountryBtn_Clicked(object sender, EventArgs e)
        {
            int i = this.Countries.IndexOf(this.GetViewModel.SelectedCountry);

            if (i == 0) // Already at start of list
                return;

            this.GetViewModel.SelectedCountry = this.Countries[i - 1];

            this.PanMapToCountry(this.GetViewModel.SelectedCountry);
        }

        private void ShowSpinner()
        {
            this.Map.IsEnabled = false;
            this.Map.Opacity = 0.6;
            LoadingSpinner.IsRunning = true;
        }

        #endregion Methods
    }
}