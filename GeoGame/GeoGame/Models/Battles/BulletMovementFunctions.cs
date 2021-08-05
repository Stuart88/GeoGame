using GeoGame.Models.Battles.Weapons;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeoGame.Models.Battles
{
    public static class BulletMovementFunctions
    {
        public static void BasicStraightVertical(MovingObjectBase b, float dt, float totalT, SKCanvasView canvasView)
        {
            b.PosY += dt * b.VelY;
        }
       
    }
}
