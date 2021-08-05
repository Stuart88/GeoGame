namespace GeoGame.Models.Battles.Weapons
{
    public class HornetBlasterBullet : BlasterBullet
    {
        #region Constructors

        public HornetBlasterBullet(WeaponBase weapon) : base(weapon)
        {
            this.Width = weapon.Parent.Width;
            this.Height = this.Width;
        }

        #endregion Constructors
    }
}