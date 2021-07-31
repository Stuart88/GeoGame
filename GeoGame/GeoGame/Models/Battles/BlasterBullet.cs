using GeoGame.Extensions;

namespace GeoGame.Models.Battles
{
    public class BlasterBullet : BulletBase
    {
        #region Constructors

        public BlasterBullet(WeaponBase weapon) : base(weapon)
        {
            this.FireAngle = 0;
            this.VelY = 20f;
            this.Sprite = BitmapExtensions.LoadBitmapResource(typeof(BlasterBullet), "GeoGame.Resources.Sprites.shipBlaster.png");
        }

        #endregion Constructors

        #region Methods

        public override void Move()
        {
            this.PosY -= VelY; // Bottom of screen is max, and top of screen is 0
        }

        #endregion Methods
    }
}