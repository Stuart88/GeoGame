using GeoGame.Models.Battles;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeoGame.Extensions
{
    public static class SKSizeExtensions
    {
        public static float GetPlayerPosY(this SKSize s)
        {
            return s.Height * (1 - 0.01f);
        }
    }
}
