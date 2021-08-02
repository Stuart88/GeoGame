﻿using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeoGame.Models.Battles
{
    public static class MovementFunctions
    {
        public static void BasicLinear(MovingObjectBase o, float dt, float totalT, SKCanvasView canvasView)
        {
            o.PosX += dt * o.VelX;
            o.PosY += dt * o.VelY;

            if (o.PosX <= 0)
            {
                o.PosX = 1;
                o.VelX = -o.VelX;
            }
            if (o.PosX >= canvasView.CanvasSize.Width - o.Width)
            {
                o.PosX = canvasView.CanvasSize.Width - o.Width - 1;
                o.VelX = -o.VelX;
            }
        }

        /// <summary>
        /// Full sinusoidal motion from side to side
        /// </summary>
        /// <param name="o"></param>
        /// <param name="dt"></param>
        /// <param name="totalT"></param>
        /// <param name="canvasView"></param>
        public static void SinusoidalLeftRightFull(MovingObjectBase o, float dt, float totalT, SKCanvasView canvasView)
        {
            float w = canvasView.CanvasSize.Width - o.Width;

            if(o.SinePhaseX == 0 || float.IsNaN(o.SinePhaseX))
            {
                o.SinePhaseX = (float)Math.Asin((2 * o.PosX / w) - 1);
            }

            o.PosX = (1 +  (float)Math.Sin((o.DirectionSignX * totalT * Math.PI / 2) + o.SinePhaseX)) * w / 2;
            o.PosY += dt * o.VelY;

        }
    }
}
