namespace GeoGame.Models.Battles.Weapons
{
    public class FastBlasterBullet : BulletBase
    {
        #region Constructors

        public FastBlasterBullet(WeaponBase weapon) : base(weapon)
        {
            this.Width = weapon.Parent is Player ? weapon.Parent.Width / 2 : weapon.Parent.Width / 4;
            this.Height = this.Width;
        }

        #endregion Constructors

        #region Methods

        public override void InitEasy()
        {
            this.VelX = 100f;
            this.VelY = 300f;
        }

        public override void InitHard()
        {
            this.VelX = 150f;
            this.VelY = 400f;
        }

        public override void InitInsane()
        {
            this.VelX = 200f;
            this.VelY = 500f;
        }

        public override void InitMedium()
        {
            this.VelX = 125f;
            this.VelY = 350f;
        }

        public override void InitPlayer()
        {
            this.VelX = 400f;
            this.VelY = -800f;
        }

        public override void PostInit()
        {
            base.PostInit();

            this.Sprite = this.Weapon.Parent.Difficulty switch
            {
                Enums.DifficultyLevel.IsPlayer => Sprites.OrbYellow,
                _ => Sprites.OrbRed,
            };

            this.HitDamage = 5;
        }

        #endregion Methods
    }
}