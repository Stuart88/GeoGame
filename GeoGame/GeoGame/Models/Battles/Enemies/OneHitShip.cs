using GeoGame.Helpers;
using GeoGame.Models.Battles.Weapons;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeoGame.Models.Battles.Enemies
{
    public class OneHitShip : EnemyBase
    {
        public OneHitShip(Enums.EnemyDifficulty difficulty, MoveAction onMove, SKCanvasView canvasView) : base(difficulty, onMove, canvasView)
        {
            this.Width = canvasView.CanvasSize.Width / 10;
            this.Height = this.Width;
            this.Health = 1;
            this.MaxHealth = this.Health;
            this.DirectionSignX = this.Rand.RandomSign();
            
            this.BaseVelY = 40;
            
            this.VelX = this.Rand.Next(150, 251);
            this.VelY = this.Rand.Next(35, 45);
            
            this.PosX = this.Rand.Next(0, (int)(canvasView.CanvasSize.Width - this.Width));
            this.PosY = this.Rand.Next((int)-this.Height - 20, (int)-this.Height); // off top of screen
            
            this.BasePosX = this.PosX;
            this.BasePosY = this.PosY;
            
            this.Weapon = new SlowBlaster(this);

            this.AssignMainSprite(9, 3);

        }

        public override void InitEasy()
        {
            this.BaseVelX = this.Rand.Next(50, 101);

            this.SpriteSheet = Sprites.EnemyHitSpriteSheetEasy;
            this.HitSpriteSheet = Sprites.EnemyHitSpriteSheetEasy;
        }
        public override void InitMedium()
        {
            this.BaseVelX = this.Rand.Next(75, 126);

            this.SpriteSheet = Sprites.EnemySpriteSheetMedium;
            this.HitSpriteSheet = Sprites.EnemyHitSpriteSheetMedium;
        }

        public override void InitHard()
        {
            this.BaseVelX = this.Rand.Next(100, 151);

            this.SpriteSheet = Sprites.EnemySpriteSheetHard;
            this.HitSpriteSheet = Sprites.EnemyHitSpriteSheetMedium;
        }

        public override void InitInsane()
        {
            this.BaseVelX = this.Rand.Next(125, 176);

            this.SpriteSheet = Sprites.EnemySpriteSheetInsane;
            this.HitSpriteSheet = Sprites.EnemyHitSpriteSheetInsane;
        }

    }
}
