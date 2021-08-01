using Plugin.SimpleAudioPlayer;
using System.Collections.Generic;
using System.Linq;

namespace GeoGame.Models.Battles
{
    public abstract class WeaponBase
    {
        #region Constructors

        public WeaponBase(MovingObjectBase parent)
        {
            this.Parent = parent;
        }

        public double DeltaTime { get; set; }

        #endregion Constructors

        #region Properties

        public ISimpleAudioPlayer BulletFiredSound { get; set; } = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
        public List<BulletBase> Bullets { get; set; } = new List<BulletBase>();

        /// <summary>
        /// Max amount of bullets that can appear on screen at one time
        /// </summary>
        public int BulletsAmount { get; set; }

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

                    toFire.PosX = this.Parent.PosX + this.Parent.MainSprite.Width / 2;
                    toFire.PosY = this.Parent.PosY + (this.Parent.IsPlayer ? -this.Parent.MainSprite.Height : 0);
                    toFire.Fired = true;
                }
            }
        }

        #endregion Methods
    }
}