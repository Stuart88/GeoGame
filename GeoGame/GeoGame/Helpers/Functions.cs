using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace GeoGame.Helpers
{
    public static class Functions
    {
        public static Stream GetStreamFromFile(string filename)
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream("GeoGame." + filename);
            return stream;
        }
    }
}
