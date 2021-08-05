using SkiaSharp;
using System;
using System.IO;
using System.Reflection;

namespace GeoGame.Extensions
{
    public static class BitmapExtensions
    {
        #region Methods

        public static SKBitmap LoadBitmapResource(Type type, string resourceID)
        {
            Assembly assembly = type.GetTypeInfo().Assembly;
            var hello = assembly.GetManifestResourceNames();

            using (Stream stream = assembly.GetManifestResourceStream(resourceID))
            {
                return SKBitmap.Decode(stream);
            }
        }

        #endregion Methods
    }
}