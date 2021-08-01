using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace GeoGame.Models.Battles
{
    public abstract class MovingObjectBase
    {
        #region Properties

        /// <summary>
        /// Object currently in game, 'on screen' (though sometimes might be slightly off screen!)
        /// </summary>
        public bool Active { get; set; }

        public float BaseVelX { get; set; }
        public float BaseVelY { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public float Height { get; set; }
        public bool HitByBullet { get; set; }
        public SKBitmap HitSprite { get; set; }
        public SKBitmap HitSpriteSheet { get; set; }
        public bool IsDead { get; set; }
        public bool IsPlayer => this is Player;
        public SKBitmap MainSprite { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public SKBitmap SpriteSheet { get; set; }
        public float VelX { get; set; }
        public float VelY { get; set; }
        public WeaponBase Weapon { get; set; }
        public float Width { get; set; }

        #endregion Properties

        #region Methods

        public void CheckCollisionWithBullet(BulletBase bullet)
        {
            if (bullet.PosX >= this.PosX && bullet.PosX <= this.PosX + this.MainSprite.Width // *1.25 because some bullets seems to pass through
                && bullet.PosY >= this.PosY  && bullet.PosY <= this.PosY + this.MainSprite.Height)
            {
                this.Health -= bullet.HitDamage;
                this.HitByBullet = true; // will draw hit sprite on next run
                bullet.Fired = false; // will not draw again

                if (this.Health <= 0)
                {
                    this.IsDead = true;
                    this.Active = false;
                }
            }
        }

        public abstract void Draw(ref SKCanvas canvas, SKPaint skPaint, SKSize canvasSize);

        public abstract void Move(float dt, SKCanvasView canvasView);

        #endregion Methods
    }
}