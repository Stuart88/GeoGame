using GeoGame.Models.Battles.Weapons;
using SkiaSharp.Views.Forms;

namespace GeoGame.Models.Battles.Enemies
{
    public class OneHitShip : EnemyBase
    {
        #region Constructors

        public OneHitShip(Enums.EnemyDifficulty difficulty, MoveAction onMove, WeaponsEnum weaponType, SKCanvasView canvasView) : base(difficulty, onMove, canvasView)
        {
            this.Width = canvasView.CanvasSize.Width / 10;
            this.Height = this.Width;

            this.Health = 1;
            this.MaxHealth = this.Health;

            this.BaseVelY = 40;
            this.BaseVelX = this.VelX; // VelX initialised via base constructor

            this.VelY = this.Rand.Next(35, 45);

            this.PosX = this.Rand.Next(0, (int)(canvasView.CanvasSize.Width - this.Width));
            this.PosY = this.ResetPosYToTop();

            this.BasePosX = this.PosX;
            this.BasePosY = this.PosY;

            this.Weapon = new SlowBlaster(this);

            this.AssignMainSprite(9, 3);
        }

        #endregion Constructors

        #region Methods

        public override void InitEasy()
        {
            base.InitEasy();
            this.VelX = this.Rand.Next(50, 101);
        }

        public override void InitHard()
        {
            base.InitHard();
            this.VelX = this.Rand.Next(100, 151);
        }

        public override void InitInsane()
        {
            base.InitInsane();
            this.VelX = this.Rand.Next(125, 176);
        }

        public override void InitMedium()
        {
            base.InitMedium();
            this.VelX = this.Rand.Next(75, 126);
        }

        #endregion Methods
    }
}