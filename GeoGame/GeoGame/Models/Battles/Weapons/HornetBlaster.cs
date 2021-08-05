namespace GeoGame.Models.Battles.Weapons
{
    public class HornetBlaster : WeaponBase
    {
        #region Constructors

        public HornetBlaster(MovingObjectBase parent, BulletMoveAction onBulletMove, WeaponsEnum weaponType) : base(parent, onBulletMove, weaponType)
        {
            for (int i = 0; i < this.BulletsAmount; i++)
            {
                this.Bullets.Add(new HornetBlasterBullet(this));
            }
        }

        #endregion Constructors

        #region Methods

        public override void InitEasy()
        {
            this.BulletsAmount = 20;
            this.FireRate = 1.00d;
        }

        public override void InitHard()
        {
            this.BulletsAmount = 30;
            this.FireRate = 0.6d;
        }

        public override void InitInsane()
        {
            this.BulletsAmount = 35;
            this.FireRate = 0.4;
        }

        public override void InitMedium()
        {
            this.BulletsAmount = 25;
            this.FireRate = 0.8d;
        }

        public override void InitPlayer()
        {
            this.BulletsAmount = 100;
            this.FireRate = 0.150d;
        }

        #endregion Methods
    }
}