﻿using GeoGame.Models.Battles.Weapons;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Linq;

namespace GeoGame.Models.Battles
{
    public abstract class MovingObjectBase
    {
        public MovingObjectBase(Enums.EnemyDifficulty difficulty)
        {
            this.Difficulty = difficulty;
        }

        #region Properties
        public Models.Enums.EnemyDifficulty Difficulty { get; set; }

        public delegate void MoveAction(MovingObjectBase o, float dt, float totalT, SKCanvasView canvasView);

        public event MoveAction OnMove;

        /// <summary>
        /// Object currently in game, 'on screen' (though sometimes might be slightly off screen!)
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// +/-1, useful for setting direction of movement
        /// </summary>
        public int DirectionSignX { get; set; }
        /// <summary>
        /// +/-1, useful for setting direction of movement
        /// </summary>
        public int DirectionSignY { get; set; }

        /// <summary>
        /// Useful for movement functions where original position is needed, e.g. in trig functions
        /// </summary>
        public float BasePosX { get; set; }

        public float BasePosY { get; set; }
        public float BaseVelX { get; set; }
        public float BaseVelY { get; set; }
        public int Health { get; set; }

        public float Height { get; set; }

        public bool HitByBullet { get; set; }

        public SKBitmap HitSprite { get; set; }

        public SKBitmap HitSpriteSheet { get; set; }

        public bool IsDead { get; set; }

        public bool IsPlayer => this is Player;

        public SKBitmap MainSprite { get; set; }

        public int MaxHealth { get; set; }

        public float PosX { get; set; }

        public float PosY { get; set; }

        /// <summary>
        /// Useful for trig movement functions where phase is needed to ensure all movements stays relative to BasePosX (start pos)
        /// </summary>
        public float? SinePhaseX { get; set; } = null;

        /// <summary>
        /// Useful for trig movement functions where phase is needed to ensure all movements stays relative to BasePosY (start pos)
        /// </summary>
        public float SinePhaseY { get; set; }

        public SKBitmap SpriteSheet { get; set; }
        public float VelX { get; set; }
        public float VelY { get; set; }
        public WeaponBase Weapon { get; set; }
        public float Width { get; set; }

        #endregion Properties

        #region Methods

        public void CheckCollisionWithBullet(BulletBase bullet)
        {
            if (bullet.PosX + bullet.Width >= this.PosX && bullet.PosX <= this.PosX + this.Width
                && bullet.PosY + bullet.Height <= this.PosY && bullet.PosY >= this.PosY - this.Height)
            {
                this.Health -= bullet.HitDamage;
                this.HitByBullet = true; // will draw hit sprite on next run
                bullet.Fired = false; // will not draw again

                if (this.Health <= 0)
                {
                    this.Health = 0;
                    this.IsDead = true;
                    this.Active = false;
                }
            }
        }

        public abstract void Draw(ref SKCanvas canvas, SKSize canvasSize);
        public virtual void DrawBullets(ref SKCanvas canvas, SKSize canvasSize)
        {
            foreach (var b in this.Weapon.Bullets.Where(b => b.Fired))
            {
                b.CheckStillInView(canvasSize);
                if (b.Fired)
                    canvas.DrawBitmap(b.Sprite, b.PosX, b.PosY);
            }
        }

        public virtual void MoveBullets(float dt, float totalT)
        {
            foreach (var b in this.Weapon.Bullets.Where(b => b.Fired))
            {
                b.SetVxVy(dt, totalT);
                b.Move(dt);
            }
        }

        public virtual void Move(float dt, float totalT, SKCanvasView canvasView)
        {
            this.OnMove?.Invoke(this, dt, totalT, canvasView);

            if(this is Enemies.EnemyBase)
            {
                var s = canvasView.CanvasSize;
                if (this.PosY - this.Height > s.Height)
                {
                    this.PosY = this.BasePosY;
                }
            }
        }

        #endregion Methods
    }
}