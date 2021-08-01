namespace GeoGame.Models.Battles
{
    public class Blaster : WeaponBase
    {
        #region Constructors

        public Blaster(MovingObjectBase parent) : base(parent)
        {
            this.BulletsAmount = parent.IsPlayer ? 100 : 20;
            this.FireRate = parent.IsPlayer ? 0.200d : 1.000d;

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