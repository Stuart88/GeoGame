namespace GeoGame.Models.Battles.Weapons
{
    public class HornetBlasterBullet : FastBlasterBullet
    {
        #region Constructors

        public HornetBlasterBullet(WeaponBase weapon) : base(weapon)
        {
            this.Width = weapon.Parent.IsPlayer ? weapon.Parent.Width : weapon.Parent.Width / 3;
            this.Height = this.Width;
        }

        #endregion Constructors

        #region Methods

        public override void PostInit()
        {
            base.PostInit();

            this.Sprite = this.Weapon.Parent.IsPlayer ? Sprites.PlayerBeeBlasterSprite : Sprites.EnemyBeeBlasterSprite;
            this.HitDamage = 50;
        }

        #endregion Methods
    }
}