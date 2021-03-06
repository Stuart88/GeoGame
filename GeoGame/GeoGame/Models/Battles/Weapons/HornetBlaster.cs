namespace GeoGame.Models.Battles.Weapons
{
    public class HornetBlaster : WeaponBase
    {
        #region Constructors

        public HornetBlaster(MovingObjectBase parent) : base(parent)
        {
            this.WeaponNameEnum = WeaponsEnum.HornetBlaster;

            for (int i = 0; i < this.BulletsAmount; i++)
            {
                this.Bullets.Add(new HornetBlasterBullet(this));
            }

            this.OnBulletMove += BulletMovementFunctions.HornetShot;
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

        protected override void PostInit()
        {
            base.PostInit();

            BulletsPerShotCycle = 7;
            BulletsPerShot = 1;
            BulletsAmount = this.BulletsAmount * 2;
            if (this.Parent is Player)
            {
                this.BulletFiredSound.Load(Helpers.Functions.GetStreamFromFile("Resources.Sounds.beeBlasterBullet.wav"));
                this.BulletFiredSound.Volume = 0.8;
            }
        }

        #endregion Methods
    }
}