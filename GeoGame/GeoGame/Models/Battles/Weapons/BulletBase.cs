using GeoGame.Interfaces;
using GeoGame.Models.Enums;
using SkiaSharp;

namespace GeoGame.Models.Battles.Weapons
{
    public abstract class BulletBase : IDifficulty
    {
        #region Constructors

        public BulletBase(WeaponBase weapon)
        {
            this.Weapon = weapon;
            this.Difficulty = this.Weapon.Parent.Difficulty;
            this.PosX = weapon.Parent.PosX;
            this.PosY = weapon.Parent.PosY;
            this.HitDamage = 10; // general value. Reassign elsewhere if needed

            switch (this.Difficulty)
            {
                case Enums.EnemyDifficulty.Easy: InitEasy(); break;
                case Enums.EnemyDifficulty.Medium: InitMedium(); break;
                case Enums.EnemyDifficulty.Hard: InitHard(); break;
                case Enums.EnemyDifficulty.Insane: InitInsane(); break;
                case Enums.EnemyDifficulty.IsPlayer: InitPlayer(); break;
            }
        }

        #endregion Constructors

        #region Properties

        public EnemyDifficulty Difficulty { get; set; }

        /// <summary>
        /// Angle at which bullet should move  (+/- pi relative to vertical).
        /// </summary>
        public virtual float FireAngle { get; set; }

        public bool Fired { get; set; }
        public int HitDamage { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public SKBitmap Sprite { get; set; }
        public float VelX { get; set; }
        public float VelY { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public WeaponBase Weapon { get; set; }

        #endregion Properties

        #region Methods

        public void CheckStillInView(SKSize canvasSize)
        {
            if (this.PosX < 0 || this.PosX + this.Width > canvasSize.Width || this.PosY < 0 || this.PosY - this.Height > canvasSize.Height)
            {
                this.Fired = false;
            }
        }

        public abstract void InitEasy();

        public abstract void InitHard();

        public abstract void InitInsane();

        public abstract void InitMedium();

        public abstract void InitPlayer();

        /// <summary>
        /// For bullets where Vx or Vy might have some function to vary movement from base values
        /// </summary>
        public abstract void SetVxVy(float dt, float totalT);

        public void Move(float dt)
        {
            this.PosX += dt * this.VelX;
            this.PosY += dt * this.VelY;
        }

        #endregion Methods
    }
}