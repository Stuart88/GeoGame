using GeoGame.Extensions;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeoGame.Models.Battles
{
    public class Enemy : MovingObjectBase
    {
        public Enemy()
        {
            this.SpriteSheet = BitmapExtensions.LoadBitmapResource(typeof(Enemy), "GeoGame.Resources.Sprites.enemySprites.png");
            this.HitSpriteSheet = BitmapExtensions.LoadBitmapResource(typeof(Enemy), "GeoGame.Resources.Sprites.enemySpritesHit.png");
            this.MainSprite = new SKBitmap();
            this.HitSprite = new SKBitmap();

            this.Weapon = new Blaster(this);
            
        }

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

            var subsetRect = new SKRectI(i * spriteW, j * spriteH, spriteW * (i + 1), spriteH *(j + 1));

            this.SpriteSheet.ExtractSubset(this.MainSprite, subsetRect);
            this.HitSpriteSheet.ExtractSubset(this.HitSprite, subsetRect);
        }

        public override void Draw(ref SKCanvas canvas, SKPaint skPaint, SKSize canvasSize)
        {

            SKRect drawRect = new SKRect(this.PosX, this.PosY - this.Height, this.PosX + this.Width, this.PosY);

            if (this.HitByBullet)
            {
                canvas.DrawBitmap(this.HitSprite, drawRect, skPaint);
                this.HitByBullet = false;
            }
            else
            {
                canvas.DrawBitmap(this.MainSprite, drawRect, skPaint);
            }

            //Draw bullets

            foreach (var b in this.Weapon.Bullets)
            {
                b.Move();
                b.CheckStillInView(canvasSize);
                if (b.Fired)
                    canvas.DrawBitmap(b.Sprite, b.PosX, b.PosY);
            }
        }

        //public override void Move(float dt, SKCanvasView canvasView)
        //{
        //    //this.PosX += dt * this.VelX;
        //    //this.PosY += dt * this.VelY;

        //    //if (this.PosX <= 0)
        //    //{
        //    //    this.PosX = 1;
        //    //    this.VelX = -this.VelX;
        //    //}
        //    //if(this.PosX >= canvasView.CanvasSize.Width - this.Width)
        //    //{
        //    //    this.PosX = canvasView.CanvasSize.Width - this.Width - 1;
        //    //    this.VelX = -this.VelX;
        //    //}
        //}
    }
}
