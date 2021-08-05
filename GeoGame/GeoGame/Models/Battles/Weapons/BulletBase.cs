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

        /// <summary>
        /// Angle at which bullet should move  (+/- pi relative to vertical).
        /// </summary>
        public virtual float FireAngle { get; set; }

        public bool Fired { get; set; }
        public int HitDamage { get; set; }
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

        public abstract void InitEasy();

        public abstract void InitHard();

        public abstract void InitInsane();

        public abstract void InitMedium();

        public abstract void InitPlayer();

        #endregion Methods
    }
}