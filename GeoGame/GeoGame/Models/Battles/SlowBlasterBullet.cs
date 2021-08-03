using GeoGame.Extensions;

namespace GeoGame.Models.Battles
{
    public class SlowBlasterBullet : BulletBase
    {
        #region Constructors

        public SlowBlasterBullet(WeaponBase weapon) : base(weapon)
        {
            this.FireAngle = 0;
            this.VelY =  weapon.Parent.IsPlayer ? -30f : 20f;
            this.Sprite = weapon.Parent.IsPlayer ? Sprites.PlayerBlasterSprite : Sprites.EnemyBlasterSprite;
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