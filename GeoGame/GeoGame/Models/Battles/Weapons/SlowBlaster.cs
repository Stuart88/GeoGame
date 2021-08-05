namespace GeoGame.Models.Battles.Weapons
{
    public class SlowBlaster : WeaponBase
    {
        #region Constructors

        public SlowBlaster(MovingObjectBase parent, BulletMoveAction onBulletMove, WeaponsEnum weaponType) : base(parent, onBulletMove, weaponType)
        {
            for (int i = 0; i < this.BulletsAmount; i++)
            {
                this.Bullets.Add(new BlasterBullet(this));
            }
        }

        #endregion Constructors

        #region Methods

        public override void FireWeapon(float dt)
        {
            base.FireWeapon(dt);
        }

        public override void InitEasy()
        {
            this.BulletsAmount = 10;
            this.FireRate = 2.00d;
        }

        public override void InitHard()
        {
            this.BulletsAmount = 15;
            this.FireRate = 1.6;
        }

        public override void InitInsane()
        {
            this.BulletsAmount = 20;
            this.FireRate = 1.4;
        }

        public override void InitMedium()
        {
            this.BulletsAmount = 10;
            this.FireRate = 1.8d;
        }

        public override void InitPlayer()
        {
            this.BulletsAmount = 20;
            this.FireRate = 0.400d;
        }

        #endregion Methods
    }
}