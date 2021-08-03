using GeoGame.Extensions;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeoGame.Models.Battles
{
    public static class Sprites
    {
        public static readonly SKBitmap PlayerSpriteCentre = BitmapExtensions.LoadBitmapResource(typeof(Enemy), "GeoGame.Resources.Sprites.shipCentre.png");
        public static readonly SKBitmap PlayerSpriteLeft = BitmapExtensions.LoadBitmapResource(typeof(Enemy), "GeoGame.Resources.Sprites.shipLeft.png");
        public static readonly SKBitmap PlayerSpriteMaxLeft = BitmapExtensions.LoadBitmapResource(typeof(Enemy), "GeoGame.Resources.Sprites.shipLeftMax.png");
        public static readonly SKBitmap PlayerSpriteRight = BitmapExtensions.LoadBitmapResource(typeof(Enemy), "GeoGame.Resources.Sprites.shipRight.png");
        public static readonly SKBitmap PlayerSpriteMaxRight = BitmapExtensions.LoadBitmapResource(typeof(Enemy), "GeoGame.Resources.Sprites.shipRightMax.png");
        public static readonly SKBitmap EnemySpriteSheet = BitmapExtensions.LoadBitmapResource(typeof(Enemy), "GeoGame.Resources.Sprites.enemySprites.png");
        public static readonly SKBitmap EnemyHitSpriteSheet = BitmapExtensions.LoadBitmapResource(typeof(Enemy), "GeoGame.Resources.Sprites.enemySpritesHit.png");
        public static readonly SKBitmap PlayerBlasterSprite = BitmapExtensions.LoadBitmapResource(typeof(Enemy), "GeoGame.Resources.Sprites.shipBlaster.png");
        public static readonly SKBitmap EnemyBlasterSprite = BitmapExtensions.LoadBitmapResource(typeof(Enemy), "GeoGame.Resources.Sprites.enemyBlaster.png");
    }
}
