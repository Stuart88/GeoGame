using GeoGame.Extensions;

namespace GeoGame.Models.Battles
{
    public class BlasterBullet : BulletBase
    {
        #region Constructors

        public BlasterBullet(WeaponBase weapon) : base(weapon)
        {
            this.FireAngle = 0;
            this.VelY =  weapon.Parent.IsPlayer ? -20f : 20f;
            this.Sprite = weapon.Parent.IsPlayer
                ? BitmapExtensions.LoadBitmapResource(typeof(BlasterBullet), "GeoGame.Resources.Sprites.shipBlaster.png")
                : BitmapExtensions.LoadBitmapResource(typeof(BlasterBullet), "GeoGame.Resources.Sprites.enemyBlaster.png");
        }

        #endregion Constructors

        #region Methods

        public override void Move()
        {
            this.PosY += VelY;
        }

        #endregion Methods
    }
}