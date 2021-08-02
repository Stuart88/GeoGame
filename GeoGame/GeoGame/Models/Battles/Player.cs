using GeoGame.Extensions;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using Xamarin.Forms;
using static GeoGame.Data.BattlesData;

namespace GeoGame.Models.Battles
{
    public class Player : MovingObjectBase
    {
        #region Constructors

        public Player()
        {
            this.Width = 30f;
            this.Height = 70f;
            this.MainSprite  = BitmapExtensions.LoadBitmapResource(typeof(Player), "GeoGame.Resources.Sprites.shipCentre.png");
        }

        #endregion Constructors

        #region Properties

        public SpriteDirection Direction { get; set; } = SpriteDirection.Centre;
        public SKBitmap ShipLeft { get; set; } = BitmapExtensions.LoadBitmapResource(typeof(Player), "GeoGame.Resources.Sprites.shipLeft.png");
        public SKBitmap ShipLeftMax { get; set; } = BitmapExtensions.LoadBitmapResource(typeof(Player), "GeoGame.Resources.Sprites.shipLeftMax.png");
        public SKBitmap ShipRight { get; set; } = BitmapExtensions.LoadBitmapResource(typeof(Player), "GeoGame.Resources.Sprites.shipRight.png");
        public SKBitmap ShipRightMax { get; set; } = BitmapExtensions.LoadBitmapResource(typeof(Player), "GeoGame.Resources.Sprites.shipRightMax.png");

        public bool MovingRight { get; set; }
        public bool MovingLeft { get; set; }
        #endregion Properties

        public float AccelLeft { get; set; } = 400;
        public float BaseAccelLeft { get; set; } = 400;
        public float AccelRight { get; set; } = 400;
        public float BaseAccelRight { get; set; } = 400;
        private float Jerk { get; set; } = 500;

        #region Methods

        public void ChangeWeapon(WeaponBase weapon)
        {
            this.Weapon = weapon;
        }

        public override void Draw(ref SKCanvas canvas, SKPaint skPaint, SKSize canvasSize)
        {
            //this.PosX = this.PosX;
            this.PosY = canvasSize.Height * (1 - 0.01f);

            SKRect drawRect = new SKRect(this.PosX, this.PosY - this.Height, this.PosX + this.Width, this.PosY);

            switch (this.Direction)
            {
                case SpriteDirection.Centre:
                    canvas.DrawBitmap(this.MainSprite, drawRect, skPaint);
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

        public override void Move(float dt, float totalT, SKCanvasView canvasView)
        {
            if (!this.MovingLeft && !this.MovingRight)
                return;

            SpriteDirection direction = this.MovingLeft ? SpriteDirection.Left : SpriteDirection.Right;
            float vMax = 1500;

            if (direction == SpriteDirection.Left && this.MovingLeft)
            {
                this.AccelLeft += dt * this.Jerk; // increasing acceleration. For nicer and more noticable increase in speed while moving left/right
                
                if (Math.Abs(this.VelX) < vMax)
                    this.VelX -= dt * this.AccelLeft;

                if (this.PosX > 0)
                {
                    this.PosX += dt * this.VelX;
                }
                else
                {
                    this.VelX = 0;
                    this.MovingLeft = false;
                    this.AccelLeft = this.BaseAccelLeft;
                }
            }
            else if (direction == SpriteDirection.Right && this.MovingRight)
            {
                this.AccelRight += dt * this.Jerk; // increasing acceleration. For nicer and more noticable increase in speed while moving left/right
                
                if (Math.Abs(this.VelX) < vMax)
                    this.VelX += dt * this.AccelRight;

                if (this.PosX + this.Width < canvasView.CanvasSize.Width)
                {
                    this.PosX += dt * this.VelX;
                }
                else
                {
                    this.VelX = 0;
                    this.MovingRight = false;
                    this.AccelRight = this.BaseAccelRight;
                }
            }

        }

        #endregion Methods
    }
}