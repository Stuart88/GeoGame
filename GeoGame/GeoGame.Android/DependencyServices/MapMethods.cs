using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GeoGame.Data;
using GeoGame.Interfaces;
using GeoGame.Models.Geo;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoGame.Droid.DependencyServices
{
    public class MapMethods : IMapMethods
    {
        public void SetMapTheme(MapEnums.MapTheme theme)
        {
            throw new NotImplementedException();
        }
    }
}