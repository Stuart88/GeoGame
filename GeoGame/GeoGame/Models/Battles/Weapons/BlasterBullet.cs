namespace GeoGame.Models.Battles.Weapons
{
    public class BlasterBullet : BulletBase
    {
        #region Constructors

        public BlasterBullet(WeaponBase weapon) : base(weapon)
        {
            this.Width = weapon.Parent.Width / 4;
            this.Height = this.Width;
            this.FireAngle = 0;
            this.Sprite = weapon.Parent.IsPlayer ? Sprites.PlayerBlasterSprite : Sprites.EnemyBlasterSprite;
        }

        #endregion Constructors

        #region Methods

        public override void InitEasy()
        {
            this.VelY = 300f;
        }

        public override void InitInsane()
        {
            this.VelY = 400f;
        }
        public override void InitHard()
        {
            this.VelY = 500f;
        }

        public override void InitMedium()
        {
            this.VelY = 350f;
        }

        public override void InitPlayer()
        {
            this.VelY = -800f;
        }

        public override void SetVxVy(float dt, float totalT)
        {
            // plain linear motion
        }



        #endregion Methods
    }
}