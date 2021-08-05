using SkiaSharp;

namespace GeoGame.Models.Battles.Weapons
{
    public class BlasterBullet : BulletBase
    {
        #region Constructors

        public BlasterBullet(WeaponBase weapon) : base(weapon)
        {
            this.Width = weapon.Parent.Width / 4;
            this.Height = this.Width;
            this.FireAngle = 0;
            this.Sprite = weapon.Parent.IsPlayer ? Sprites.PlayerBlasterSprite : Sprites.EnemyBlasterSprite;
            this.OnMove += BulletMovementFunctions.BasicStraightVertical;
        }

        #endregion Constructors

        #region Methods

        public override void InitEasy()
        {
            this.VelY = 300f;
        }

        public override void InitInsane()
        {
            this.VelY = 400f;
        }
        public override void InitHard()
        {
            this.VelY = 500f;
        }

        public override void InitMedium()
        {
            this.VelY = 350f;
        }

        public override void InitPlayer()
        {
            this.VelY = -800f;
        }

        public override void Draw(ref SKCanvas canvas, SKSize canvasSize)
        {
            this.CheckStillInView(canvasSize);
            if (this.Fired)
                canvas.DrawBitmap(this.Sprite, this.PosX, this.PosY);
        }



        #endregion Methods
    }
}