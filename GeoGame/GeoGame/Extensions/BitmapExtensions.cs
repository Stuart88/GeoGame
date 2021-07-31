using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace GeoGame.Extensions
{
    public static class BitmapExtensions
    {
        public static SKBitmap LoadBitmapResource(Type type, string resourceID)
        {
            Assembly assembly = type.GetTypeInfo().Assembly;
            var hello = assembly.GetManifestResourceNames();

            using (Stream stream = assembly.GetManifestResourceStream(resourceID))
            {
                return SKBitmap.Decode(stream);
            }
        }
    }
}
