﻿using GeoGame.Models.Battles.Weapons;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeoGame.Models.Battles
{
    public static class BulletMovementFunctions
    {
        public static void BasicStraightVertical(BulletBase b, float dt, float totalT, SKCanvasView canvasView)
        {
            b.PosY += dt * b.BaseVelY;
        }

        /// <summary>
        /// Fires in a diagonal direction, alternating between left and right
        /// </summary>
        public static void AlternateDiagonal(BulletBase b, float dt, float totalT, SKCanvasView canvasView)
        {
            if (b.Id % 2 == 0)
                b.PosX += dt * b.BaseVelX;
            else
                b.PosX -= dt * b.BaseVelX;

            b.PosY += dt * b.BaseVelY;
        }

        public static void SpreadShot(BulletBase b, float dt, float totalT, SKCanvasView canvasView)
        {

            int spread = b.Weapon.BulletsPerShot;
            float divisor = 1f /spread;

            b.PosX += dt * divisor * (b.ShotId - spread / 2) * b.BaseVelX;
            
            b.PosY += dt * b.BaseVelY;
        }

        public static void HornetShot(BulletBase b, float dt, float totalT, SKCanvasView canvasView)
        {
            b.Weapon.CurrentBulletShot++;
            if (b.Weapon.CurrentBulletShot == b.Weapon.BulletsPerShotCycle)
                b.Weapon.CurrentBulletShot = 0;

            int spread = b.Weapon.BulletsPerShotCycle;
            float divisor = 5f / spread;

            b.PosX += dt * divisor * (b.Weapon.CurrentBulletShot - spread / 2) * b.BaseVelX;
            b.PosY += dt * b.BaseVelY;
        }


    }
}
