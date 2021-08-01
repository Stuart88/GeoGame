using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeoGame.Models.Battles
{
    public abstract class BulletBase
    {
        public BulletBase(WeaponBase weapon)
        {
            this.Weapon = weapon;
            this.PosX = weapon.Parent.PosX;
            this.PosY = weapon.Parent.PosY;
            this.HitDamage = 10; // general value. Reassign elsewhere if needed
        }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float VelX { get; set; }
        public float VelY { get; set; }
        public int HitDamage { get; set; }
        public bool Fired { get; set; }
        public WeaponBase Weapon { get; set; }
        public SKBitmap Sprite { get; set; }

        /// <summary>
        /// Angle at which bullet should move  (+/- pi relative to vertical).
        /// </summary>
        public virtual float FireAngle { get; set; }

        public abstract void Move();
        public void CheckStillInView(SKSize canvasSize)
        {
            if(this.PosX < 0 || this.PosX > canvasSize.Width || this.PosY < 0 || this.PosY > canvasSize.Height)
            {
                this.Fired = false;
            }
        }
    }
}
