namespace GeoGame.Models.Battles.Weapons
{
    public class BlasterBullet : BulletBase
    {
        #region Constructors

        public BlasterBullet(WeaponBase weapon) : base(weapon)
        {
            this.FireAngle = 0;
            this.Sprite = weapon.Parent.IsPlayer ? Sprites.PlayerBlasterSprite : Sprites.EnemyBlasterSprite;
        }

        #endregion Constructors

        #region Methods

        public override void InitEasy()
        {
            this.VelY = 20f;
        }

        public override void InitInsane()
        {
            this.VelY = 40f;
        }
        public override void InitHard()
        {
            this.VelY = 30f;
        }

        public override void InitMedium()
        {
            this.VelY = 25f;
        }

        public override void InitPlayer()
        {
            this.VelY = -30f;
        }

        public override void Move()
        {
            this.PosY += VelY;
        }

        #endregion Methods
    }
}