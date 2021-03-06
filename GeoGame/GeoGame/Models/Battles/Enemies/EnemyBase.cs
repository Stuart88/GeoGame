using GeoGame.Interfaces;
using GeoGame.Models.Battles.Weapons;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;

namespace GeoGame.Models.Battles.Enemies
{
    public abstract class EnemyBase : MovingObjectBase, IDifficulty
    {
        #region Constructors

        public EnemyBase(Enums.DifficultyLevel difficulty, MoveAction onMove, SKCanvasView canvasView) : base(difficulty)
        {
            this.MainSprite = new SKBitmap();
            this.HitSprite = new SKBitmap();

            this.OnMove += onMove;

            switch (this.Difficulty)
            {
                case Enums.DifficultyLevel.Easy: InitEasy(); break;
                case Enums.DifficultyLevel.Medium: InitMedium(); break;
                case Enums.DifficultyLevel.Hard: InitHard(); break;
                case Enums.DifficultyLevel.Insane: InitInsane(); break;
                case Enums.DifficultyLevel.IsPlayer: InitPlayer(); break;
            }
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Assigns sprite from 10x10 sprite sheet
        /// </summary>
        /// <param name="i">Between 0 and 9</param>
        /// <param name="j">Between 0 and 9</param>
        public void AssignMainSprite(int i, int j)
        {
            if (i > 9 || j > 9)
                throw new Exception("Range should be between 0 and 9!");

            int w = this.SpriteSheet.Width;
            int h = this.SpriteSheet.Height;

            int spriteW = w / 10;
            int spriteH = h / 10;

            var subsetRect = new SKRectI(i * spriteW, j * spriteH, spriteW * (i + 1), spriteH * (j + 1));

            this.SpriteSheet.ExtractSubset(this.MainSprite, subsetRect);
            this.HitSpriteSheet.ExtractSubset(this.HitSprite, subsetRect);
        }

        public override void Draw(ref SKCanvas canvas, SKSize canvasSize)
        {
            SKRect drawRect = new SKRect(this.PosX, this.PosY - this.Height, this.PosX + this.Width, this.PosY);

            if (this.Active && !this.IsDead && !this.HitByBullet)
            {
                canvas.DrawBitmap(this.MainSprite, drawRect);
            }
            else if (this.HitByBullet)
            {
                canvas.DrawBitmap(this.HitSprite, drawRect);
                this.HitByBullet = false;
            }
        }

        public virtual void InitEasy()
        {
            this.SpriteSheet = Sprites.EnemySpriteSheetEasy;
            this.HitSpriteSheet = Sprites.EnemyHitSpriteSheetEasy;
        }

        public virtual void InitHard()
        {
            this.SpriteSheet = Sprites.EnemySpriteSheetHard;
            this.HitSpriteSheet = Sprites.EnemyHitSpriteSheetHard;
        }

        public virtual void InitInsane()
        {
            this.SpriteSheet = Sprites.EnemySpriteSheetInsane;
            this.HitSpriteSheet = Sprites.EnemyHitSpriteSheetInsane;
        }

        public virtual void InitMedium()
        {
            this.SpriteSheet = Sprites.EnemySpriteSheetMedium;
            this.HitSpriteSheet = Sprites.EnemyHitSpriteSheetMedium;
        }

        public void InitPlayer()
        {
            //Just here for interface implementation...
        }

        public float ResetPosYToTop()
        {
            this.PosY = this.Rand.Next((int)-this.Height - 20, (int)-this.Height); // off top of screen
            this.BasePosY = this.PosY;
            this.MovementTime = 0;
            return this.PosY;
        }

        public void SetWeapon(Weapons.WeaponsEnum weapon)
        {
            this.Weapon = weapon switch
            {
                Weapons.WeaponsEnum.SlowBlaster => new SlowBlaster(this),
                Weapons.WeaponsEnum.FastBlaster => new FastBlaster(this),
                Weapons.WeaponsEnum.StarBlaster => new StarBlaster(this),
                Weapons.WeaponsEnum.SpreadBlaster => new SpreadBlaster(this),
                Weapons.WeaponsEnum.HornetBlaster => new HornetBlaster(this),
            };
        }

        public override void Update(float dt, float totalT, SKCanvasView canvasView)
        {
            if (this.Active && !this.IsDead)
                this.Move(dt, totalT, canvasView);

            this.Weapon.MoveBullets(dt, totalT, canvasView);
        }

        #endregion Methods
    }
}