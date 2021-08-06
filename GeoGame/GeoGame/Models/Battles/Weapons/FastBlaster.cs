﻿namespace GeoGame.Models.Battles.Weapons
{
    public class FastBlaster : WeaponBase
    {
        #region Constructors

        public FastBlaster(MovingObjectBase parent) : base(parent)
        {
            this.WeaponNameEnum = WeaponsEnum.FastBlaster;

            for (int i = 0; i < this.BulletsAmount; i++)
            {
                this.Bullets.Add(new FastBlasterBullet(this));
            }

            this.OnBulletMove += BulletMovementFunctions.BasicStraightVertical;
        }

        #endregion Constructors

        #region Methods

        public override void FireWeapon(float dt)
        {
            base.FireWeapon(dt);
        }

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
            this.BulletFiredSound.Volume = 0.1;

            this.BulletsAmount = 100;
            this.FireRate = 0.100d;
        }

        protected override void PostInit()
        {
            base.PostInit();

            if (this.Parent is Player)
            {
                this.BulletFiredSound.Load(Helpers.Functions.GetStreamFromFile("Resources.Sounds.pulse.wav"));
                this.BulletFiredSound.Volume = 0.9;
            }
        }

        #endregion Methods
    }
}