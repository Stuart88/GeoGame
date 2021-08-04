using GeoGame.Interfaces;
using Plugin.SimpleAudioPlayer;
using System.Collections.Generic;
using System.Linq;

namespace GeoGame.Models.Battles.Weapons
{
    public abstract class WeaponBase : IDifficulty
    {
        #region Constructors

        public WeaponBase(MovingObjectBase parent)
        {
            this.Parent = parent;
            this.Difficulty = this.Parent.Difficulty;

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

        public ISimpleAudioPlayer BulletFiredSound { get; set; } = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
        public List<BulletBase> Bullets { get; set; } = new List<BulletBase>();

        /// <summary>
        /// Max amount of bullets that can appear on screen at one time
        /// </summary>
        public int BulletsAmount { get; set; }

        public double DeltaTime { get; set; }
        public Enums.EnemyDifficulty Difficulty { get; set; }

        /// <summary>
        /// After how many seconds should the Fire method be called again (for automatic firing)
        /// </summary>
        public double FireRate { get; set; }

        public MovingObjectBase Parent { get; set; }

        #endregion Properties

        #region Methods

        public virtual void FireWeapon(float dt)
        {
            this.DeltaTime += dt;

            if (this.DeltaTime >= this.FireRate)
            {
                this.DeltaTime = 0;

                var toFire = this.Bullets.FirstOrDefault(b => !b.Fired);

                if (toFire != null)
                {
                    if (this.Parent.IsPlayer)
                        this.BulletFiredSound.Play();

                    toFire.PosX = this.Parent.PosX + this.Parent.Width / 2 - toFire.Width / 2;
                    toFire.PosY = this.Parent.PosY + (this.Parent.IsPlayer ? -this.Parent.Height : 0);
                    toFire.Fired = true;
                }
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