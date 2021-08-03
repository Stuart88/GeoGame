using SkiaSharp;
using System;

namespace GeoGame.Models.Battles
{
    public class Enemy : MovingObjectBase
    {
        #region Constructors

        public Enemy()
        {
            this.SpriteSheet = Sprites.EnemySpriteSheet;
            this.HitSpriteSheet = Sprites.EnemyHitSpriteSheet;
            this.MainSprite = new SKBitmap();
            this.HitSprite = new SKBitmap();

            this.Weapon = new Blaster(this);
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

        #endregion Methods
    }
}