using GeoGame.Helpers;
using GeoGame.Interfaces;
using Plugin.SimpleAudioPlayer;
using SkiaSharp.Views.Forms;
using System.Collections.Generic;
using System.Linq;

namespace GeoGame.Models.Battles.Weapons
{
    public delegate void BulletMoveAction(BulletBase b, float dt, float totalT, SKCanvasView canvasView);

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

            PostInit();
        }

        #endregion Constructors

        #region Events

        public event BulletMoveAction OnBulletMove;

        #endregion Events

        #region Properties

        public ISimpleAudioPlayer BulletFiredSound { get; set; } = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
        public List<BulletBase> Bullets { get; set; } = new List<BulletBase>();

        /// <summary>
        /// Max amount of bullets that can appear on screen at one time
        /// </summary>
        public int BulletsAmount { get; set; }

        /// <summary>
        /// If count == BulletsPerShot, reset! Useful in BulletMovementFunctions
        /// </summary>
        public int BulletsMovementProcessedCount { get; set; } = 0;

        /// <summary>
        /// Number of bullets fired in one shot
        /// </summary>
        public int BulletsPerShot { get; set; } = 1;

        /// <summary>
        /// After how many shots should BulletMovementFunction reset (if  needed)
        /// </summary>
        public int BulletsPerShotCycle { get; set; } = 1;

        public int CurrentBulletShot { get; set; } = 0;
        public double DeltaTime { get; set; }

        public Enums.EnemyDifficulty Difficulty { get; set; }

        /// <summary>
        /// After how many seconds should the Fire method be called again (for automatic firing)
        /// </summary>
        public double FireRate { get; set; }

        public MovingObjectBase Parent { get; set; }

        public string WeaponName => this.WeaponNameEnum.GetDisplayName();

        public WeaponsEnum WeaponNameEnum { get; set; }

        #endregion Properties

        #region Methods

        public virtual void FireWeapon(float dt)
        {
            this.DeltaTime += dt;

            if (this.DeltaTime >= this.FireRate)
            {
                this.DeltaTime = 0;

                List<BulletBase> toFire = this.Bullets.Where(b => !b.Fired).Take(this.BulletsPerShot).ToList();

                AssignShotId(toFire);

                bool playFireSound = true;

                foreach (var b in toFire)
                {
                    if (this.Parent.IsPlayer)
                    {
                        // only play sound on first run of loop!
                        if (playFireSound)
                        {
                            if (this is HornetBlaster)
                            {
                                //only play sound for every 2rd bee, otherwise it sounds crap
                                if (b.Id % 2 == 0)
                                    this.BulletFiredSound.Play();
                            }
                            else
                            {
                                this.BulletFiredSound.Play();
                            }
                        }
                        playFireSound = false;
                    }

                    b.PosX = this.Parent.PosX + this.Parent.Width / 2 - b.Width / 2;
                    b.PosY = this.Parent.PosY + (this.Parent.IsPlayer ? -this.Parent.Height : 0);
                    b.Fired = true;
                }
            }
        }

        public abstract void InitEasy();

        public abstract void InitHard();

        public abstract void InitInsane();

        public abstract void InitMedium();

        public abstract void InitPlayer();

        public virtual void MoveBullets(float dt, float totalT, SKCanvasView canvasView)
        {
            foreach (var b in this.Bullets.Where(b => b.Fired))
            {
                b.Weapon.OnBulletMove?.Invoke(b, dt, totalT, canvasView);
            }
        }

        protected virtual void PostInit()
        {
        }

        /// <summary>
        /// Assigns ShotID to bullets, such that they can be handled specifically in BulletMovement function, if needed
        /// </summary>
        /// <param name="bullets"></param>
        private void AssignShotId(List<BulletBase> bullets)
        {
            for (int i = 0; i < bullets.Count(); i++)
            {
                bullets[i].ShotId = i;
            }
        }

        #endregion Methods
    }
}