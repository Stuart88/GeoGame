namespace GeoGame.Models.Battles
{
    public class Blaster : WeaponBase
    {
        #region Constructors

        public Blaster(Player player) : base(player)
        {
            this.BulletsAmount = 100;
            this.FireRate = 0.100d;

            for (int i = 0; i < this.BulletsAmount; i++)
            {
                this.Bullets.Add(new BlasterBullet(this));
            }
        }

        #endregion Constructors

        #region Methods

        public override void FireWeapon()
        {
            base.FireWeapon();
        }

        #endregion Methods
    }
}