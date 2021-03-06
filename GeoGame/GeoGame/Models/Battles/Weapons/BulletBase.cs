using GeoGame.Interfaces;
using SkiaSharp;

namespace GeoGame.Models.Battles.Weapons
{
    public abstract class BulletBase : MovingObjectBase, IDifficulty
    {
        #region Constructors

        public BulletBase(WeaponBase weapon) : base(weapon.Difficulty)
        {
            this.Weapon = weapon;
            this.Difficulty = this.Weapon.Parent.Difficulty;
            this.PosX = weapon.Parent.PosX;
            this.PosY = weapon.Parent.PosY;
            this.HitDamage = 10; // general value. Reassign elsewhere if needed

            this.Id = this.Weapon.Bullets.Count + 1;

            // ID used as index for selection in some BulletMovementFunctions methods
            // ID starts at 1 because some BulletMovementFunctions methods hit divde by zero error if using the index in modulo operator

            switch (this.Difficulty)
            {
                case Enums.DifficultyLevel.Easy: InitEasy(); break;
                case Enums.DifficultyLevel.Medium: InitMedium(); break;
                case Enums.DifficultyLevel.Hard: InitHard(); break;
                case Enums.DifficultyLevel.Insane: InitInsane(); break;
                case Enums.DifficultyLevel.IsPlayer: InitPlayer(); break;
            }

            this.BaseVelX = this.VelX;
            this.BaseVelY = this.VelY;

            PostInit();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// If player bullet hits enemy bullet, destroy enemy bullet
        /// </summary>
        /// <param name="bullet"></param>
        public void CheckBulletOnBulletCollision(BulletBase enemyBullet)
        {
            if (enemyBullet.Weapon.Parent is Player)
                return;

            if (enemyBullet.PosX + enemyBullet.Width >= this.PosX && enemyBullet.PosX <= this.PosX + this.Width
                && enemyBullet.PosY + enemyBullet.Height <= this.PosY && enemyBullet.PosY >= this.PosY - this.Height && enemyBullet.Fired)
            {
                enemyBullet.Fired = false;
            }
        }

        /// <summary>
        /// Angle at which bullet should move  (+/- pi relative to vertical).
        /// </summary>
        public virtual float FireAngle { get; set; }

        public bool Fired { get; set; }

        public int HitDamage { get; set; }

        /// <summary>
        /// Useful for tracking bullet when part of a weapon that fires multiple bullets in one shot
        /// </summary>
        public int ShotId { get; set; }

        public SKBitmap Sprite { get; set; }

        #endregion Properties

        #region Methods

        public void CheckStillInView(SKSize canvasSize)
        {
            if (this.PosX < 0 || this.PosX + this.Width > canvasSize.Width || this.PosY < 0 || this.PosY - this.Height > canvasSize.Height)
            {
                this.Fired = false;
                this.MovementTime = 0;
            }
        }

        public override void Draw(ref SKCanvas canvas, SKSize canvasSize)
        {
            this.CheckStillInView(canvasSize);
            if (this.Fired)
            {
                SKRect destRect = new SKRect(this.PosX, this.PosY - this.Height, this.PosX + this.Width, this.PosY);
                canvas.DrawBitmap(this.Sprite, destRect);
            }
        }

        public abstract void InitEasy();

        public abstract void InitHard();

        public abstract void InitInsane();

        public abstract void InitMedium();

        public abstract void InitPlayer();

        public virtual void PostInit()
        {
        }

        #endregion Methods
    }
}