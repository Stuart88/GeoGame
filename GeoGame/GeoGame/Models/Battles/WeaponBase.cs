using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace GeoGame.Models.Battles
{
    public abstract class WeaponBase
    {
        #region Constructors

        public double DeltaTime { get; set; } 

        public WeaponBase(MovingObjectBase parent)
        {
            this.Parent = parent;
        }

        #endregion Constructors

        #region Properties

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
                    toFire.PosX = this.Parent.PosX + this.Parent.MainSprite.Width / 2;
                    toFire.PosY = this.Parent.PosY + (this.Parent.IsPlayer ? - this.Parent.Height : this.Parent.Height);
                    toFire.Fired = true;
                }
            }
        }

        #endregion Methods
    }
}