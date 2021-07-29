using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using GeoGame.Droid.CustomRenderers;
using GeoGame.Interfaces;
using GeoGame.Models.Geo;
using GeoGame.Models.Mapping;
using Java.Util;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps.Android;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
[assembly: Dependency(typeof(CustomMapRenderer))]

namespace GeoGame.Droid.CustomRenderers
{
    public class CustomMapRenderer : MapRenderer, GoogleMap.IInfoWindowAdapter, IMapMethods
    {
        #region Constructors

        public CustomMapRenderer(Context context) : base(context)
        {
        }

        #endregion Constructors

        #region Properties

        private List<Circle> PopulaedPlaceMarkers { get; set; } = new List<Circle>();
        private List<Android.Gms.Maps.Model.Polygon> SelectedPolygons { get; set; } = new List<Android.Gms.Maps.Model.Polygon>();
        private Marker ShowingMarker { get; set; }

        #endregion Properties

        #region Methods

        private List<(Country, Android.Gms.Maps.Model.Polygon)> CountryPolygons { get; set; } = new List<(Country, Android.Gms.Maps.Model.Polygon)>();

        private Data.MapEnums.MapTheme SelectedTheme { get; set; }

        public Android.Views.View GetInfoContents(Marker marker)
        {
            return null; // Default rendering
        }

        public Android.Views.View GetInfoWindow(Marker marker)
        {
            return null; // Default info window
        }

        public void SetMapTheme(Data.MapEnums.MapTheme theme)
        {
            this.SelectedTheme = theme;

            var selectedStyle = theme switch
            {
                Data.MapEnums.MapTheme.Standard => new MapStyleOptions(GeoGame.Data.MapData.PlainMapStandardStyle),
                Data.MapEnums.MapTheme.Silver => new MapStyleOptions(GeoGame.Data.MapData.PlainMapSilver_Style),
                Data.MapEnums.MapTheme.Retro => new MapStyleOptions(GeoGame.Data.MapData.PlainMapRetro_Style),
                Data.MapEnums.MapTheme.Dark => new MapStyleOptions(GeoGame.Data.MapData.PlainMapDark_Style),
                Data.MapEnums.MapTheme.Night => new MapStyleOptions(GeoGame.Data.MapData.PlainMapNight_Style),
                Data.MapEnums.MapTheme.Aubergine => new MapStyleOptions(GeoGame.Data.MapData.PlainMapAubergine_Style),
                _ => new MapStyleOptions(GeoGame.Data.MapData.PlainMapStandardStyle)
            };

            NativeMap.SetMapStyle(selectedStyle);
        }

        protected override PolygonOptions CreatePolygonOptions(Xamarin.Forms.Maps.Polygon polygon)
        {
            Tuple<Country, Geometry> data = (Tuple<Country, Geometry>)polygon.BindingContext;
            var country = data.Item1;
            var geo = data.Item2;

            PolygonOptions poly = new PolygonOptions();

            for (int i = 0; i < geo.NumGeometries; i++)
            {
                Geometry g = geo.GetGeometryN(i);

                if (g is NetTopologySuite.Geometries.Polygon p)
                {
                    LatLng[] outerRingPath = p.ExteriorRing.Coordinates.Select(c => new LatLng(c.Y, c.X)).ToArray();
                    poly.Add(outerRingPath);

                    if (p.Holes.Length > 0)
                    {
                        foreach (var h in p.Holes)
                        {
                            var holePath = h.Coordinates.Select(c => new LatLng(c.Y, c.X)).ToArray();
                            poly.AddHole(new ArrayList(holePath));
                        }
                    }
                }
            }

            AssignDefaultPolyOptions(poly);

            poly.InvokeZIndex(country.Id);

            var added = NativeMap.AddPolygon(poly);

            this.CountryPolygons.Add((country, added));

            return poly;
        }

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Xamarin.Forms.Maps.Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                NativeMap.InfoWindowClick -= OnInfoWindowClick;
            }

            if (e.NewElement != null)
            {
                var formsMap = (CustomMap)e.NewElement;
                //customPins = formsMap.CustomPins;
            }
        }

        protected override void OnMapReady(GoogleMap map)
        {
            base.OnMapReady(map);
            NativeMap.SetMapStyle(new MapStyleOptions(GeoGame.Data.MapData.PlainMapAubergine_Style));
            NativeMap.InfoWindowClick += OnInfoWindowClick;
            NativeMap.SetInfoWindowAdapter(this);
            NativeMap.MapClick += NativeMap_MapClick;
            NativeMap.PolygonClick += NativeMap_PolygonClick;
            NativeMap.MapLongClick += NativeMap_MapLongClick;
        }

        private async void AddPopulatedPlacesForCountry(Country country)
        {
            var db = await Data.DbContexts.Instance;

            //This gets all places belonging to country
            var places = await db.GetPopulatedPlacesForCountry(country.Name);

            ////So now filter only for places in the given polyong some countries are made up of more than one polygon
            //places = places.Where(p => poly.Points)

            foreach (var m in this.PopulaedPlaceMarkers)
            {
                m.Remove();
            }
            this.PopulaedPlaceMarkers.Clear();
            foreach (var p in places)
            {
                CircleOptions c = new CircleOptions();
                c.InvokeCenter(new LatLng(p.Geometry.Centroid.Coordinate.Y, p.Geometry.Centroid.Coordinate.X));
                c.InvokeRadius(20000);//20km
                c.InvokeFillColor(System.Drawing.Color.Yellow.ToPlatformColor());
                c.InvokeStrokeColor(System.Drawing.Color.Yellow.ToPlatformColor());
                c.InvokeStrokeWidth(2);
                c.InvokeZIndex(1000);//Need this to ensure Z-index higher than values wich are used for country IDs
                Circle added = NativeMap.AddCircle(c);

                this.PopulaedPlaceMarkers.Add(added);
            }
        }

        private void AssignDefaultPolyOptions(PolygonOptions poly)
        {
            poly.InvokeFillColor(System.Drawing.Color.Transparent.ToPlatformColor());
            poly.InvokeStrokeColor(System.Drawing.Color.LightGreen.ToPlatformColor());
            poly.InvokeStrokeWidth(4);
            poly.Clickable(true);
        }

        private PolygonOptions ClonePolygonWithDefaults(Android.Gms.Maps.Model.Polygon p)
        {
            PolygonOptions poly = new PolygonOptions();
            poly.Add(p.Points.ToArray());
            foreach (var h in p.Holes)
                poly.Holes.Add(h);

            poly.InvokeZIndex(p.ZIndex);

            AssignDefaultPolyOptions(poly);

            return poly;
        }

        private PolygonOptions ClonePolygonWithNewColours(Android.Gms.Maps.Model.Polygon p, System.Drawing.Color fill, System.Drawing.Color stroke, float strokeWidth)
        {
            PolygonOptions poly = new PolygonOptions();
            poly.Add(p.Points.ToArray());
            foreach (var h in p.Holes)
                poly.Holes.Add(h);

            poly.InvokeZIndex(p.ZIndex);

            poly.InvokeFillColor(fill.ToPlatformColor());
            poly.InvokeStrokeColor(stroke.ToPlatformColor());
            poly.InvokeStrokeWidth(strokeWidth);
            //poly.InvokeStrokePattern(new List<PatternItem>() { new PatternItem(0, (Java.Lang.Float)1), new PatternItem(2, (Java.Lang.Float)1) });
            poly.Clickable(true);

            return poly;
        }

        private LatLng GetPointsCentre(List<LatLng> visiblePoints)
        {
            double lat = visiblePoints.Sum(s => s.Latitude) / visiblePoints.Count;
            double lng = visiblePoints.Sum(s => s.Longitude) / visiblePoints.Count;
            return new LatLng(lat, lng);
        }

        private void NativeMap_MapClick(object sender, GoogleMap.MapClickEventArgs e)
        {
        }

        private void NativeMap_MapLongClick(object sender, GoogleMap.MapLongClickEventArgs e)
        {
            int themeVal = (int)this.SelectedTheme;

            themeVal++;

            int totalThemes = Enum.GetValues(typeof(Data.MapEnums.MapTheme)).Length;

            if (themeVal + 1 >= totalThemes)
                themeVal = 0;

            this.SetMapTheme((Data.MapEnums.MapTheme)themeVal);
        }

        private async void NativeMap_PolygonClick(object sender, GoogleMap.PolygonClickEventArgs e)
        {
            if (this.SelectedPolygons.Any(p => e.Polygon.ZIndex == p.ZIndex))
                return; // Already selected.

            // Previously clicked polygons redraw with defaults
            if (this.SelectedPolygons.Count > 0)
            {
                foreach (var p in this.SelectedPolygons.ToList())
                {
                    NativeMap.AddPolygon(ClonePolygonWithDefaults(p));
                    p.Remove();
                    this.SelectedPolygons.Remove(p);
                }
            }

            int countryId = (int)e.Polygon.ZIndex;
            var db = await Data.DbContexts.Instance;
            var country = await db.GetCountry(countryId);

            foreach (var c in this.CountryPolygons.Where(p => p.Item1.Id == countryId).ToList())
            {
                //Draw currently selected polygon with highlighting colours
                PolygonOptions newPoly = ClonePolygonWithNewColours(c.Item2, System.Drawing.Color.DarkRed, System.Drawing.Color.DarkRed, 9);
                Android.Gms.Maps.Model.Polygon addedPoly = NativeMap.AddPolygon(newPoly);
                this.SelectedPolygons.Add(addedPoly);

                // Remove the polygon that was just clicked, as it has now been replaced with hghlighted polygon
                this.CountryPolygons.Remove(c);
                c.Item2.Remove();

                //Save this new polygon for later as it will need to be redrawn with defaults after another is clicked
                this.CountryPolygons.Add((c.Item1, addedPoly));
            }

            var v = NativeMap.Projection.VisibleRegion;

            // get all country geometry points that are in the visible region
            List<LatLng> visiblePoints = country.Geometry.Coordinates.Select(s => new LatLng(s.Y, s.X)).Where(c => PointInVisibleRegion(v, c)).ToList();

            // Marker point will be the centre point of the visible region that the country is showing
            LatLng centroid = GetPointsCentre(visiblePoints);

            this.ShowingMarker?.Remove();

            //Show populated places for clicked country

            this.AddPopulatedPlacesForCountry(country);

            //Show info window for clicked country

            var infoMarker = new MarkerOptions();
            infoMarker.SetAlpha(0.0f);
            infoMarker.SetTitle($"This area belongs to {country.NameOfficial}");
            infoMarker.SetPosition(centroid);
            infoMarker.Anchor(0, 0);
            //infoMarker.SetIcon(null);
            Marker tempMarker = NativeMap.AddMarker(infoMarker);
            tempMarker.ShowInfoWindow();
            this.ShowingMarker = tempMarker;
        }

        private void OnInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
            //e.Marker.Remove();
            //this.ShowingMarker?.Remove();
        }

        private bool PointInVisibleRegion(VisibleRegion v, LatLng c)
        {
            return c.Latitude < v.FarLeft.Latitude && c.Latitude > v.NearLeft.Latitude
                 && c.Longitude > v.NearLeft.Longitude && c.Longitude < v.NearRight.Longitude;
        }

        #endregion Methods
    }
}