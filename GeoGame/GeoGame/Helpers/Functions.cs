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

        private static int[] PlusMinusVals = new int[2] { -1, 1 };

        /// <summary>
        /// Returns random value of +/- 1
        /// </summary>
        /// <returns></returns>
        public static int RandomSign(this Random r)
        {
            return PlusMinusVals[r.Next(0, 2)];
        }
    }
}
