using GeoGame.Models.Battles.Weapons;
using SkiaSharp.Views.Forms;
using static GeoGame.Models.Battles.Weapons.WeaponBase;

namespace GeoGame.Models.Battles.Enemies
{
    public class OneHitShip : EnemyBase
    {
        #region Constructors

        public OneHitShip(Enums.EnemyDifficulty difficulty, MoveAction onMove, BulletMoveAction onBulletMove, WeaponsEnum weaponType, SKCanvasView canvasView) : base(difficulty, onMove, canvasView)
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

            this.Weapon = new SlowBlaster(this, onBulletMove, weaponType);

            this.AssignMainSprite(9, 3);
        }

        #endregion Constructors

        #region Methods

        public override void InitEasy()
        {
            this.VelX = this.Rand.Next(50, 101);

            this.SpriteSheet = Sprites.EnemyHitSpriteSheetEasy;
            this.HitSpriteSheet = Sprites.EnemyHitSpriteSheetEasy;
        }

        public override void InitHard()
        {
            this.VelX = this.Rand.Next(100, 151);

            this.SpriteSheet = Sprites.EnemySpriteSheetHard;
            this.HitSpriteSheet = Sprites.EnemyHitSpriteSheetMedium;
        }

        public override void InitInsane()
        {
            this.VelX = this.Rand.Next(125, 176);

            this.SpriteSheet = Sprites.EnemySpriteSheetInsane;
            this.HitSpriteSheet = Sprites.EnemyHitSpriteSheetInsane;
        }

        public override void InitMedium()
        {
            this.VelX = this.Rand.Next(75, 126);

            this.SpriteSheet = Sprites.EnemySpriteSheetMedium;
            this.HitSpriteSheet = Sprites.EnemyHitSpriteSheetMedium;
        }

        #endregion Methods
    }
}