using SkiaSharp.Views.Forms;
using System;

namespace GeoGame.Models.Battles
{
    public static class MovementFunctions
    {
        #region Methods

        public static void BasicLinearLeftRight(MovingObjectBase o, float dt, float totalT, SKCanvasView canvasView)
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

        public static void LocalisedCircle(MovingObjectBase o, float dt, float totalT, SKCanvasView canvasView)
        {
            var s = canvasView.CanvasSize;

            float amplitude = s.Width / 8;

            if (o.BasePosX + amplitude + o.Width > s.Width)
                o.BasePosX = s.Width - amplitude - o.Width;

            if (o.BasePosX - amplitude < 0)
                o.BasePosX = amplitude;

            o.PosX = o.BasePosX + amplitude * (float)Math.Sin(o.DirectionSignX * totalT * Math.PI);
            o.BasePosY += dt * o.VelY;
            o.PosY = o.BasePosY + amplitude * (float)Math.Cos(o.DirectionSignY * totalT * Math.PI);
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

            if (!o.SinePhaseX.HasValue)
            {
                o.SinePhaseX = (float)Math.Asin((2 * o.PosX / w) - 1);
            }

            o.PosX = (1 + (float)Math.Sin((o.DirectionSignX * totalT * Math.PI / 2) + o.SinePhaseX.Value)) * w / 2;
            o.PosY += dt * o.VelY;
        }

        /// <summary>
        /// Small sinusoidal motion localised around current X position
        /// </summary>
        /// <param name="o"></param>
        /// <param name="dt"></param>
        /// <param name="totalT"></param>
        /// <param name="canvasView"></param>
        public static void SinusoidalLeftRightLocal(MovingObjectBase o, float dt, float totalT, SKCanvasView canvasView)
        {
            var s = canvasView.CanvasSize;

            float amplitude = s.Width / 6;

            if (o.BasePosX + amplitude + o.Width > s.Width)
                o.BasePosX = s.Width - amplitude - o.Width;

            if (o.BasePosX - amplitude < 0)
                o.BasePosX = amplitude;

            o.PosX = o.BasePosX + amplitude * (float)Math.Sin(o.DirectionSignX * totalT * Math.PI / 2);
            o.PosY += dt * o.VelY;
        }

        #endregion Methods
    }
}