using GeoGame.Models.Battles.Weapons;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using static GeoGame.Data.BattlesData;

namespace GeoGame.Models.Battles
{
    public class Player : MovingObjectBase
    {
        #region Constructors

        public Player() : base(Enums.EnemyDifficulty.IsPlayer)
        {
            this.Width = 30f;
            this.Height = 70f;
            this.MainSprite = Sprites.PlayerSpriteCentre;
            this.WeaponsList = new List<WeaponBase>
            {
                new SlowBlaster(this, BulletMovementFunctions.BasicStraightVertical, WeaponsEnum.SlowBlaster),
                new Blaster(this, BulletMovementFunctions.BasicStraightVertical, WeaponsEnum.FastBlaster),
                new Blaster(this, BulletMovementFunctions.AlternateDiagonal, WeaponsEnum.StarBlaster),
                new SlowBlaster(this, BulletMovementFunctions.SpreadShot, WeaponsEnum.SpreadBlaster),
                new HornetBlaster(this, BulletMovementFunctions.HornetShot, WeaponsEnum.HornetBlaster),
            };
        }

        #endregion Constructors

        #region Properties
        public List<BulletBase> ActiveBullets => this.WeaponsList.Where(w => w.Bullets.Any(b => b.Fired)).SelectMany(b => b.Bullets).ToList();

        public float AccelLeft { get; set; } = 400;
        public float AccelRight { get; set; } = 400;
        public float BaseAccelLeft { get; set; } = 400;
        public float BaseAccelRight { get; set; } = 400;
        public SpriteDirection Direction { get; set; } = SpriteDirection.Centre;
        public bool MovingLeft { get; set; }
        public bool MovingRight { get; set; }
        public SKBitmap ShipLeft { get; set; } = Sprites.PlayerSpriteLeft;
        public SKBitmap ShipLeftMax { get; set; } = Sprites.PlayerSpriteMaxLeft;
        public SKBitmap ShipRight { get; set; } = Sprites.PlayerSpriteRight;
        public SKBitmap ShipRightMax { get; set; } = Sprites.PlayerSpriteMaxRight;
        public List<WeaponBase> WeaponsList { get; }
        private float Jerk { get; set; } = 500;

        #endregion Properties

        #region Methods

        public void ChangeWeapon(WeaponsEnum weapon)
        {
            this.Weapon = this.WeaponsList.First(w => w.WeaponNameEnum == weapon);
        }

        public override void Draw(ref SKCanvas canvas, SKSize canvasSize)
        {
            SKRect drawRect = new SKRect(this.PosX, this.PosY - this.Height, this.PosX + this.Width, this.PosY);

            switch (this.Direction)
            {
                case SpriteDirection.Centre:
                    canvas.DrawBitmap(this.MainSprite, drawRect);
                    break;

                case SpriteDirection.Left:
                    canvas.DrawBitmap(this.ShipLeft, drawRect);
                    break;

                case SpriteDirection.Right:
                    canvas.DrawBitmap(this.ShipRight, drawRect);
                    break;

                case SpriteDirection.LeftMax:
                    canvas.DrawBitmap(this.ShipLeftMax, drawRect);
                    break;

                case SpriteDirection.RightMax:
                    canvas.DrawBitmap(this.ShipRightMax, drawRect);
                    break;
            }
        }

        public override void DrawBullets(ref SKCanvas canvas, SKSize canvasSize)
        {
            foreach (BulletBase b in this.ActiveBullets)
            {
                b.Draw(ref canvas, canvasSize);
            }
        }

        public override void Update(float dt, float totalT, SKCanvasView canvasView)
        {
            this.Move(dt, totalT, canvasView);

            foreach (var w in this.WeaponsList)
                w.MoveBullets(dt, totalT, canvasView);
        }

        internal override void Move(float dt, float totalT, SKCanvasView canvasView)
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