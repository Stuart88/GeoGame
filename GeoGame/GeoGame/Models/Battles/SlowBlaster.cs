namespace GeoGame.Models.Battles
{
    public class SlowBlaster : WeaponBase
    {
        #region Constructors

        public SlowBlaster(MovingObjectBase parent) : base(parent)
        {
            if (this.Parent.IsPlayer)
            {
                this.BulletFiredSound.Load(Helpers.Functions.GetStreamFromFile("Resources.Sounds.blasterBullet.wav"));
                this.BulletFiredSound.Volume = 0.1;
            }

            this.BulletsAmount = parent.IsPlayer ? 20 : 10;
            this.FireRate = parent.IsPlayer ? 0.400d : 1.500d;

            for (int i = 0; i < this.BulletsAmount; i++)
            {
                this.Bullets.Add(new SlowBlasterBullet(this));
            }
        }

        #endregion Constructors

        #region Methods  

        public override void FireWeapon(float dt)
        {
            base.FireWeapon(dt);
        }

        #endregion Methods
    }
}