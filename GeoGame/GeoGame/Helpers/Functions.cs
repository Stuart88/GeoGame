using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
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

        public static string GetDisplayName(this Enum enumValue)
        {
            string displayName;
            displayName = enumValue.GetType()
                .GetMember(enumValue.ToString())
                .FirstOrDefault()
                .GetCustomAttribute<DisplayAttribute>()?
                .GetName();
            if (String.IsNullOrEmpty(displayName))
            {
                displayName = enumValue.ToString();
            }
            return displayName;
        }

        public static T CycleNext<T>(this Enum enumValue) where T : Enum
        {
            List<T> vals = Enum.GetValues(typeof(T)).Cast<T>().ToList();

            var index = vals.IndexOf((T)enumValue);

            index++;

            if (index >= vals.Count)
                index = 0;

            return vals[index];
        }
    }
}
