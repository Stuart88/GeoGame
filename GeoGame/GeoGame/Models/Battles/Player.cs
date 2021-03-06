using GeoGame.Extensions;
using GeoGame.Models.Battles.Weapons;
using Plugin.SimpleAudioPlayer;
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

        public Player() : base(Enums.DifficultyLevel.IsPlayer)
        {
            this.Width = 30f;
            this.Height = 70f;
            this.MainSprite = Sprites.PlayerSpriteCentre;
            this.HitDamageSound.Load(Helpers.Functions.GetStreamFromFile("Resources.Sounds.shipHit.wav"));
            this.HitDamageSound.Volume = 0.4;
            this.WeaponsList = new List<WeaponBase>();
        }

        #endregion Constructors

        #region Properties

        public float AccelLeft { get; set; } = 400;

        public float AccelRight { get; set; } = 400;

        public List<BulletBase> ActiveBullets => this.WeaponsList.Where(w => w.Bullets.Any(b => b.Fired)).SelectMany(b => b.Bullets).ToList();

        public float BaseAccelLeft { get; set; } = 400;

        public float BaseAccelRight { get; set; } = 400;

        public SpriteDirection Direction { get; set; } = SpriteDirection.Centre;

        public ISimpleAudioPlayer HitDamageSound { get; set; } = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();

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

        public void AddWeapon(WeaponsEnum w)
        {
            WeaponBase toAdd = w switch
            {
                WeaponsEnum.SlowBlaster => new SlowBlaster(this),
                WeaponsEnum.FastBlaster => new FastBlaster(this),
                WeaponsEnum.StarBlaster => new StarBlaster(this),
                WeaponsEnum.SpreadBlaster => new SpreadBlaster(this),
                WeaponsEnum.HornetBlaster => new HornetBlaster(this),
            };
            this.WeaponsList.Add(toAdd);
        }

        public void ChangeWeapon(WeaponsEnum weapon)
        {
            this.Weapon = this.WeaponsList.First(w => w.WeaponNameEnum == weapon);
        }

        public override void Draw(ref SKCanvas canvas, SKSize canvasSize)
        {
            if (this.HitByBullet)
            {
                this.HitDamageSound.Play();
                this.HitByBullet = false;
            }
            SKRect drawRect = new SKRect(this.PosX, canvasSize.GetPlayerPosY() - this.Height, this.PosX + this.Width, canvasSize.GetPlayerPosY());

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