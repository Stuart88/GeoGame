using GeoGame.Models.Battles.Weapons;
using SkiaSharp.Views.Forms;

namespace GeoGame.Models.Battles.Enemies
{
    public class Attacker : EnemyBase
    {
        #region Constructors

        public Attacker(Enums.EnemyDifficulty difficulty, MoveAction onMove, WeaponsEnum weaponType, SKCanvasView canvasView) : base(difficulty, onMove, canvasView)
        {
            this.Width = canvasView.CanvasSize.Width / 9;
            this.Height = this.Width;

            this.MaxHealth = this.Health;

            this.BaseVelY = 50;
            this.BaseVelX = this.VelX; // VelX initialised via base constructor

            this.VelY = this.Rand.Next(35, 45);

            this.PosX = this.Rand.Next(0, (int)(canvasView.CanvasSize.Width - this.Width));
            this.PosY = this.ResetPosYToTop();

            this.BasePosX = this.PosX;
            this.BasePosY = this.PosY;

            this.SetWeapon(weaponType);

            this.AssignMainSprite(3, 4);
        }

        #endregion Constructors

        #region Methods

        public override void InitEasy()
        {
            base.InitEasy();
            this.Health = 40;
            this.VelX = this.Rand.Next(50, 101);
        }

        public override void InitHard()
        {
            base.InitHard();
            this.Health = 80;
            this.VelX = this.Rand.Next(100, 151);
        }

        public override void InitInsane()
        {
            base.InitInsane();
            this.Health = 100;
            this.VelX = this.Rand.Next(125, 176);
        }

        public override void InitMedium()
        {
            base.InitMedium();
            this.Health = 60;
            this.VelX = this.Rand.Next(75, 126);
        }

        #endregion Methods
    }
}