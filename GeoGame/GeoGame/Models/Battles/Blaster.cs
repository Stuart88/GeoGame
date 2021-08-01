namespace GeoGame.Models.Battles
{
    public class Blaster : WeaponBase
    {
        #region Constructors

        public Blaster(MovingObjectBase parent) : base(parent)
        {
            if (this.Parent.IsPlayer)
            {
                this.BulletFiredSound.Load(Helpers.Functions.GetStreamFromFile("Resources.Sounds.blasterBullet.wav"));
                this.BulletFiredSound.Volume = 0.1;
            }

            this.BulletsAmount = parent.IsPlayer ? 100 : 20;
            this.FireRate = parent.IsPlayer ? 0.150d : 1.500d;

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

        #endregion Methods
    }
}