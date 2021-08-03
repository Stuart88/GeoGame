﻿using GeoGame.Data;
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

                foreach (var c in this.Countries)
                {
                    DrawGeometries(c, c.Geometry);
                }

                // Get country the player is currently on

                var currentCountry = this.Countries.FirstOrDefault(c => !Game.GameData.CountriesDefeatedIds.Contains(c.Id));

                this.Map.Opacity = 1;

                PanMapToCountry(currentCountry);


                this.HideSpinner();
            }));
        }

        private void PanMapToCountry(Country c)
        {
            NetTopologySuite.Geometries.Geometry geom = GetLargestGeometry(c);

            Distance d = MaxDistanceAcrossCountry(geom.EnvelopeInternal);

            Position mapPos = new Position(geom.Envelope.Centroid.Y, geom.Envelope.Centroid.Coordinate.X);

            //var mapZoom = Xamarin.Essentials.Location.CalculateDistance()
            MapSpan span = MapSpan.FromCenterAndRadius(mapPos, d);

            this.Map.MoveToRegion(span);
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
        private void HideSpinner()
        {
            this.Map.IsEnabled = true;
            this.Map.Opacity = 1;
            LoadingSpinner.IsRunning = false;
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
                var thisGeom = g.GetGeometryN(i);
                if (thisGeom.Area > area)
                    largestGeo = thisGeom;
            }

            return largestGeo;
        }

        private void InitMap()
        {
            
            this.Map = new CustomMap()
            {
                MapType = MapType.Street,
                HasScrollEnabled = false,
                HasZoomEnabled = false,
                IsShowingUser = false,
                TrafficEnabled = false,
                MoveToLastRegionOnLayoutChange = false,
                Opacity = 0
            };

            this.Map.MapClicked += Map_MapClicked;

            this.MapContentArea.Content = this.Map;

            //Gets countries ordered by population (smallest to largest)
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