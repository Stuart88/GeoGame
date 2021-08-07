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
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps.Android;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
[assembly: Dependency(typeof(CustomMapRenderer))]

namespace GeoGame.Droid.CustomRenderers
{
    public class CustomMapRenderer : MapRenderer, GoogleMap.IInfoWindowAdapter, IMessageService
    {
        #region Constructors

        public CustomMapRenderer(Context context) : base(context)
        {
        }

        #endregion Constructors

        #region Properties

        private List<(Country, Android.Gms.Maps.Model.Polygon)> CountryPolygons { get; set; } = new List<(Country, Android.Gms.Maps.Model.Polygon)>();

        /// <summary>
        /// Populated places cache, data can be reused when going to battle page
        /// </summary>
        private List<(Circle marker, PopulatedPlace place)> PopulatedPlaceMarkers { get; set; } = new List<(Circle, PopulatedPlace)>();

        /// <summary>
        /// Ppolygons cache for reuse
        /// </summary>
        private List<Android.Gms.Maps.Model.Polygon> SelectedPolygons { get; set; } = new List<Android.Gms.Maps.Model.Polygon>();

        private Data.MapEnums.MapTheme SelectedTheme { get; set; }
        private Marker ShowingMarker { get; set; }

        #endregion Properties

        #region Methods


        public Android.Views.View GetInfoContents(Marker marker)
        {
            return null; // Default rendering
        }

        public Android.Views.View GetInfoWindow(Marker marker)
        {
            return null; // Default info window
        }

        public async void HighlightCountry(Country c)
        {
            if (this.CountryPolygons.Any(p => p.Item1.Id == c.Id))
            {
                var poly = this.CountryPolygons.FirstOrDefault(p => p.Item1.Id == c.Id);
                await this.HighlightPolygon(poly.Item2);
            }
        }

        private int PolygonFlashCount { get; set; } = 0;

        private void FlashFocusedPolygon(Country c)
        {
            if (this.PolygonFlashCount > 0) // already in flashing proces
                return;

            Device.StartTimer(TimeSpan.FromMilliseconds(300), () =>
            {
                // Simulate a kind of flashing effect to point the user to press the 'Fight' button after they press 'Next Battle'
                if (this.CountryPolygons.Any(p => p.Item1.Id == c.Id))
                {
                    var poly = this.CountryPolygons.FirstOrDefault(p => p.Item1.Id == c.Id);
                    var stroke = new Android.Graphics.Color(poly.Item2.StrokeColor).ToSystemColor();

                    if (this.PolygonFlashCount % 2 == 0)
                        _ = this.HighlightPolygon(poly.Item2, true, System.Drawing.Color.Transparent, stroke);
                    else
                        _ = this.HighlightPolygon(poly.Item2, true);

                    this.PolygonFlashCount++;

                    if (this.PolygonFlashCount == 5)
                    {
                        _ = this.HighlightPolygon(poly.Item2, true);
                        this.PolygonFlashCount = 0;
                        return false;
                    }

                    return true;

                }
                else
                {
                    return false;
                }

            });
        }

        public void SetMapTheme(Data.MapEnums.MapTheme theme)
        {
            if (NativeMap == null)
                return;

            this.SelectedTheme = theme;

            NativeMap.SetMapStyle(new MapStyleOptions(Data.Data.GetThemeStyleString(theme)));

            Data.Game.GameData.MapTheme = theme;

            Data.Game.SaveGame();
        }

        public void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<IMessageService, Country>(this, Data.MessagingCenterMessages.HighlightCountry, (sender, c) =>
            {
                this.HighlightCountry(c);
            });

            MessagingCenter.Subscribe<IMessageService, Data.MapEnums.MapTheme>(this, Data.MessagingCenterMessages.SetMapTheme, (sender, theme) =>
            {
                this.SetMapTheme(theme);
            });
            
            MessagingCenter.Subscribe<IMessageService, Country>(this, Data.MessagingCenterMessages.FlashPolygon, (sender, c) =>
            {
                this.FlashFocusedPolygon(c);
            });
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

            AssignDefaultPolyOptions(poly, Data.Game.GameData.CountriesDefeatedIds.Contains(data.Item1.Id));

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
            NativeMap.SetMapStyle(new MapStyleOptions(GeoGame.Data.Data.GetThemeStyleString(Data.Game.GameData.MapTheme)));
            NativeMap.InfoWindowClick += OnInfoWindowClick;
            NativeMap.SetInfoWindowAdapter(this);
            NativeMap.PolygonClick += NativeMap_PolygonClick;
            SubscribeToMessages();
        }

        private void AssignDefaultPolyOptions(PolygonOptions poly, bool isDefeated)
        {
            poly.InvokeStrokeWidth(4);

            if (isDefeated)
            {
                poly.InvokeStrokeColor(System.Drawing.Color.DarkOrange.ToPlatformColor());
                poly.InvokeFillColor(System.Drawing.Color.Orange.ToPlatformColor());
            }
            else
            {
                poly.InvokeStrokeColor(System.Drawing.Color.LightGreen.ToPlatformColor());
                poly.InvokeFillColor(System.Drawing.Color.Transparent.ToPlatformColor());
            }
            
            poly.Clickable(true);
        }

        private PolygonOptions ClonePolygonWithDefaults(Android.Gms.Maps.Model.Polygon p)
        {
            PolygonOptions poly = new PolygonOptions();
            poly.Add(p.Points.ToArray());
            foreach (var h in p.Holes)
                poly.Holes.Add(h);

            poly.InvokeZIndex(p.ZIndex);

            AssignDefaultPolyOptions(poly, Data.Game.GameData.CountriesDefeatedIds.Contains((int)p.ZIndex));

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

        private async Task HighlightPolygon(Android.Gms.Maps.Model.Polygon p, bool flashingAnimation = false, System.Drawing.Color? fill = null, System.Drawing.Color? stroke = null)
        {
            if (NativeMap == null)
                return;

            if (this.SelectedPolygons.Any(poly => poly.ZIndex == p.ZIndex) && !flashingAnimation)
                return; // Already selected.

            // Previously clicked polygons redraw with defaults
            if (this.SelectedPolygons.Count > 0 && !flashingAnimation)
            {
                foreach (var poly in this.SelectedPolygons.ToList())
                {
                    NativeMap.AddPolygon(ClonePolygonWithDefaults(poly));
                    poly.Remove();
                    this.SelectedPolygons.Remove(poly);
                }
            }

            int countryId = (int)p.ZIndex;
            var db = await Data.DbContexts.Instance;
            var country = await db.GetCountry(countryId);

            foreach (var c in this.CountryPolygons.Where(p => p.Item1.Id == countryId).ToList())
            {
                //Draw currently selected polygon with highlighting colours
                PolygonOptions newPoly = new PolygonOptions();
                
                if (flashingAnimation && fill.HasValue && stroke.HasValue)
                    newPoly = ClonePolygonWithNewColours(c.Item2, fill.Value, stroke.Value, 9);
                else
                    newPoly = ClonePolygonWithNewColours(c.Item2, System.Drawing.Color.LightBlue, System.Drawing.Color.Blue, 9); ;
                
                Android.Gms.Maps.Model.Polygon addedPoly = NativeMap.AddPolygon(newPoly);
                this.SelectedPolygons.Add(addedPoly);

                // Remove the polygon that was just clicked, as it has now been replaced with hghlighted polygon
                this.CountryPolygons.Remove(c);
                c.Item2.Remove();

                //Save this new polygon for later as it will need to be redrawn with defaults after another is clicked
                this.CountryPolygons.Add((c.Item1, addedPoly));
            }
        }


        private void NativeMap_MapLongClick(object sender, GoogleMap.MapLongClickEventArgs e)
        {
            int themeVal = (int)this.SelectedTheme;

            themeVal++;

            int totalThemes = Enum.GetValues(typeof(Data.MapEnums.MapTheme)).Length;

            if (themeVal >= totalThemes)
                themeVal = 0;

            this.SetMapTheme((Data.MapEnums.MapTheme)themeVal);
        }

        private async void NativeMap_PolygonClick(object sender, GoogleMap.PolygonClickEventArgs e)
        {
            await this.HighlightPolygon(e.Polygon);

            Country country = this.CountryPolygons.First(p => p.Item1.Id == e.Polygon.ZIndex).Item1;

            this.HighlightCountry(country);

            MessagingCenter.Send<IMessageService, Country>(this, Data.MessagingCenterMessages.CountryClicked, country);
        }

        private void OnInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
            Country country = this.CountryPolygons.First(p => this.SelectedPolygons.Any(poly => poly.ZIndex == p.Item1.Id)).Item1;
            List<PopulatedPlace> places = this.PopulatedPlaceMarkers.Select(p => p.place).ToList();

            MessagingCenter.Send<IMessageService, Country>(this, Data.MessagingCenterMessages.OpenCountryBattle, country);
        }

        private bool PointInVisibleRegion(VisibleRegion v, LatLng c)
        {
            return c.Latitude < v.FarLeft.Latitude && c.Latitude > v.NearLeft.Latitude
                 && c.Longitude > v.NearLeft.Longitude && c.Longitude < v.NearRight.Longitude;
        }

        #endregion Methods
    }
}