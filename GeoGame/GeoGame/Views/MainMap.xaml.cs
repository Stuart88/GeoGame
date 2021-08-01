using GeoGame.Data;
using GeoGame.Interfaces;
using GeoGame.Models.Geo;
using GeoGame.Models.Mapping;
using GeoGame.ViewModels;
using Plugin.SimpleAudioPlayer;
using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace GeoGame.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainMap : ContentPage, IMessageService
    {
        #region Fields

        private string _stateProvinceStr = "StateProvince";

        #endregion Fields

        #region Constructors
        private Random _rand = new Random();
        public MainMap()
        {
            this.BindingContext = new MainMapViewModel();
            InitializeComponent();
            SubscribeToMessages();
            InitMusic();
            InitMap();
        }

        protected override void OnAppearing()
        {
            this.Music.Play();
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            this.Music.Stop();
            base.OnDisappearing();
        }

        private void InitMusic()
        {
            int songNum = _rand.Next(1, 6);
            this.Music.Load(Helpers.Functions.GetStreamFromFile($"Resources.Music.Map.{songNum}.mp3"));
        }

        #endregion Constructors

        #region Properties

        public ISimpleAudioPlayer Music { get; set; } = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
        private DbContexts Contexts { get; set; }
        private List<Country> Countries { get; set; }
        private CustomMap Map { get; set; }

        #endregion Properties

        #region Methods

        private void AddCountryPolygonToMap(Country c, NetTopologySuite.Geometries.Geometry geo)
        {
            Polygon poly = new Polygon()
            {
                BindingContext = new Tuple<Country, NetTopologySuite.Geometries.Geometry>(c, geo)
            };

            this.Map.MapElements.Add(poly);
        }

        private void CycleGeometries(Country c, NetTopologySuite.Geometries.Geometry g)
        {
            for (int i = 0; i < g.NumGeometries; i++)
            {
                NetTopologySuite.Geometries.Geometry geo = g.GetGeometryN(i);

                if (geo.NumGeometries > 1)
                    CycleGeometries(c, geo); // Recursive. Some countries are a set of multiple polygons (island chains etc)

                AddCountryPolygonToMap(c, geo);
            }
        }

        private void GetCountryData()
        {
            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(new Action(async () =>
            {
                this.ShowSpinner();

                this.Contexts = await DbContexts.Instance;
                this.Countries = await this.Contexts.GetAllCountries();

                foreach (var c in this.Countries)
                {
                    CycleGeometries(c, c.Geometry);
                }

                this.HideSpinner();
            }));
        }

        private void HideSpinner()
        {
            this.Map.IsEnabled = true;
            this.Map.Opacity = 1;
            LoadingSpinner.IsRunning = false;
        }

        private void InitMap()
        {
            //UK lat/long
            MapSpan span = MapSpan.FromCenterAndRadius(new Position(55.3781, 3.4360), new Distance(1000000));

            this.Map = new CustomMap(span)
            {
                MapType = MapType.Street,
                HasScrollEnabled = true,
                HasZoomEnabled = true,
                IsShowingUser = false,
                TrafficEnabled = false,
                MoveToLastRegionOnLayoutChange = false,
            };

            this.Map.MapClicked += Map_MapClicked;

            this.MapContentArea.Content = this.Map;

            this.GetCountryData();
        }

        private async void Map_MapClicked(object sender, MapClickedEventArgs e)
        {
            //this.ShowPopulatedPlacedForClickedCountry(e.Position);

            return;
        }

        private void ShowSpinner()
        {
            this.Map.IsEnabled = false;
            this.Map.Opacity = 0.6;
            LoadingSpinner.IsRunning = true;
        }

        private void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<IMessageService, (Country, List<PopulatedPlace>)>(this, Data.MessagingCenterMessages.OpenCountryBattle, async (sender, data) =>
            {
                await Navigation.PushModalAsync(new CountryBattle(data.Item1, data.Item2));
            });
        }

        #endregion Methods
    }
}