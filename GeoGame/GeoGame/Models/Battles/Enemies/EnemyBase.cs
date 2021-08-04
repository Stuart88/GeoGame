using GeoGame.Interfaces;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;

namespace GeoGame.Models.Battles.Enemies
{
    public abstract class EnemyBase : MovingObjectBase, IDifficulty
    {
        #region Constructors

        public Random Rand { get; set; } = new Random();

        public EnemyBase(Enums.EnemyDifficulty difficulty, MoveAction onMove, SKCanvasView canvasView) : base(difficulty)
        {
            this.MainSprite = new SKBitmap();
            this.HitSprite = new SKBitmap();
            
            this.OnMove += onMove;
            
            switch (this.Difficulty)
            {
                case Enums.EnemyDifficulty.Easy: InitEasy(); break;
                case Enums.EnemyDifficulty.Medium: InitMedium(); break;
                case Enums.EnemyDifficulty.Hard: InitHard(); break;
                case Enums.EnemyDifficulty.Insane: InitInsane(); break;
                case Enums.EnemyDifficulty.IsPlayer: InitPlayer(); break;
            }
        }

        public float ResetPosYToTop()
        {
            this.PosY = this.Rand.Next((int)-this.Height - 20, (int)-this.Height); // off top of screen
            this.BasePosY = this.PosY;
            return this.PosY;
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

        public abstract void InitEasy();

        public abstract void InitHard();

        public abstract void InitInsane();

        public abstract void InitMedium();

        public void InitPlayer()
        {
            //Just here for interface implementation...
        }

        #endregion Methods
    }
}