using GeoGame.Extensions;
using SkiaSharp;
using System;
using Xamarin.Forms;
using static GeoGame.Data.BattlesData;

namespace GeoGame.Models.Battles
{
    public class Player
    {
        #region Constructors

        public Player()
        {
        }

        #endregion Constructors

        #region Properties

        public float BaseVelX { get; set; }
        public float BaseVelY { get; set; }
        public float VelX { get; set; }
        public float VelY { get; set; }
        public SpriteDirection Direction { get; set; } = SpriteDirection.Centre;
        public float Height { get; set; } = 70f;
        public float PosX { get; set; }
        public float PosY { get; set; }
        public SKBitmap ShipCentre { get; set; } = BitmapExtensions.LoadBitmapResource(typeof(Player), "GeoGame.Resources.Sprites.shipCentre.png");
        public SKBitmap ShipLeft { get; set; } = BitmapExtensions.LoadBitmapResource(typeof(Player), "GeoGame.Resources.Sprites.shipLeft.png");
        public SKBitmap ShipLeftMax { get; set; } = BitmapExtensions.LoadBitmapResource(typeof(Player), "GeoGame.Resources.Sprites.shipLeftMax.png");
        public SKBitmap ShipRight { get; set; } = BitmapExtensions.LoadBitmapResource(typeof(Player), "GeoGame.Resources.Sprites.shipRight.png");
        public SKBitmap ShipRightMax { get; set; } = BitmapExtensions.LoadBitmapResource(typeof(Player), "GeoGame.Resources.Sprites.shipRightMax.png");
        public SKBitmap SpriteSheet { get; set; }
        public WeaponBase Weapon { get; set; }
        public float Width { get; set; } = 30f;

        #endregion Properties

        #region Methods

        public void ChangeWeapon(WeaponBase weapon)
        {
            this.Weapon.IsActive = false; // Stop device timer (stop firing)
            this.Weapon = weapon;
            Device.StartTimer(TimeSpan.FromMilliseconds(this.Weapon.FireRate), () =>
            {
                this.Weapon.FireWeapon();
                return this.Weapon.IsActive;
            });
        }

        public void DrawPlayer(ref SKCanvas canvas, SKPaint skPaint, SKSize canvasSize)
        {
            //this.PosX = this.PosX;
            this.PosY = canvasSize.Height * (1 - 0.01f);

            SKRect drawRect = new SKRect(this.PosX, this.PosY - this.Height, this.PosX + this.Width, this.PosY);

            switch (this.Direction)
            {
                case SpriteDirection.Centre:
                    canvas.DrawBitmap(this.ShipCentre, drawRect, skPaint);
                    break;

                case SpriteDirection.Left:
                    canvas.DrawBitmap(this.ShipLeft, drawRect, skPaint);
                    break;

                case SpriteDirection.Right:
                    canvas.DrawBitmap(this.ShipRight, drawRect, skPaint);
                    break;

                case SpriteDirection.LeftMax:
                    canvas.DrawBitmap(this.ShipLeftMax, drawRect, skPaint);
                    break;

                case SpriteDirection.RightMax:
                    canvas.DrawBitmap(this.ShipRightMax, drawRect, skPaint);
                    break;
            }

            //Draw bullets

            foreach (var b in this.Weapon.Bullets)
            {
                b.Move();
                b.CheckStillInView(canvasSize);
                if (b.Fired)
                    canvas.DrawBitmap(b.Sprite, b.PosX, b.PosY);
            }
        }

        #endregion Methods
    }
}