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
                case Enums.EnemyDifficulty.Easy: InitEasy(); break;
                case Enums.EnemyDifficulty.Medium: InitMedium(); break;
                case Enums.EnemyDifficulty.Hard: InitHard(); break;
                case Enums.EnemyDifficulty.Insane: InitInsane(); break;
                case Enums.EnemyDifficulty.IsPlayer: InitPlayer(); break;
            }

            this.BaseVelX = this.VelX;
            this.BaseVelY = this.VelY;

            PostInit();
        }

        #endregion Constructors

        #region Properties

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
            if (this.Weapon.Parent is Player)
            {
                switch (this.Weapon.WeaponNameEnum)
                {
                    case WeaponsEnum.StarBlaster:

                        break;

                    case WeaponsEnum.HornetBlaster:

                        break;

                    case WeaponsEnum.SlowBlaster:

                        break;

                    case WeaponsEnum.FastBlaster:

                        break;

                    default:

                        break;
                }
            }
            if (this.Weapon.Parent is Enemies.EnemyBase)
            {
                switch (this.Weapon.WeaponNameEnum)
                {
                    default:
                        this.Sprite = Sprites.EnemyBlasterSprite;
                        break;
                }
            }
        }

        #endregion Methods
    }
}