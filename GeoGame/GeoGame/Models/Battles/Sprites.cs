using GeoGame.Extensions;
using GeoGame.Models.Battles.Enemies;
using SkiaSharp;

namespace GeoGame.Models.Battles
{
    public static class Sprites
    {
        #region Fields

        public static readonly SKBitmap EnemyBeeBlasterSprite = BitmapExtensions.LoadBitmapResource(typeof(EnemyBase), "GeoGame.Resources.Sprites.enemyBee.png");
        public static readonly SKBitmap EnemyBlasterSprite = BitmapExtensions.LoadBitmapResource(typeof(EnemyBase), "GeoGame.Resources.Sprites.enemyBlaster.png");
        public static readonly SKBitmap EnemyHitSpriteSheetEasy = BitmapExtensions.LoadBitmapResource(typeof(EnemyBase), "GeoGame.Resources.Sprites.enemySpritesEasyHit.png");
        public static readonly SKBitmap EnemyHitSpriteSheetHard = BitmapExtensions.LoadBitmapResource(typeof(EnemyBase), "GeoGame.Resources.Sprites.enemySpritesHardHit.png");
        public static readonly SKBitmap EnemyHitSpriteSheetInsane = BitmapExtensions.LoadBitmapResource(typeof(EnemyBase), "GeoGame.Resources.Sprites.enemySpritesInsaneHit.png");
        public static readonly SKBitmap EnemyHitSpriteSheetMedium = BitmapExtensions.LoadBitmapResource(typeof(EnemyBase), "GeoGame.Resources.Sprites.enemySpritesMediumHit.png");
        public static readonly SKBitmap EnemySpriteSheetEasy = BitmapExtensions.LoadBitmapResource(typeof(EnemyBase), "GeoGame.Resources.Sprites.enemySpritesEasy.png");
        public static readonly SKBitmap EnemySpriteSheetHard = BitmapExtensions.LoadBitmapResource(typeof(EnemyBase), "GeoGame.Resources.Sprites.enemySpritesHard.png");
        public static readonly SKBitmap EnemySpriteSheetInsane = BitmapExtensions.LoadBitmapResource(typeof(EnemyBase), "GeoGame.Resources.Sprites.enemySpritesInsane.png");
        public static readonly SKBitmap EnemySpriteSheetMedium = BitmapExtensions.LoadBitmapResource(typeof(EnemyBase), "GeoGame.Resources.Sprites.enemySpritesMedium.png");
        public static readonly SKBitmap OrbYellow = BitmapExtensions.LoadBitmapResource(typeof(EnemyBase), "GeoGame.Resources.Sprites.orbYellow.png");
        public static readonly SKBitmap OrbGreen = BitmapExtensions.LoadBitmapResource(typeof(EnemyBase), "GeoGame.Resources.Sprites.orbGreen.png");
        public static readonly SKBitmap OrbRed = BitmapExtensions.LoadBitmapResource(typeof(EnemyBase), "GeoGame.Resources.Sprites.orbRed.png");
        public static readonly SKBitmap OrbBlue = BitmapExtensions.LoadBitmapResource(typeof(EnemyBase), "GeoGame.Resources.Sprites.orbBlue.png");
        public static readonly SKBitmap PlayerBeeBlasterSprite = BitmapExtensions.LoadBitmapResource(typeof(EnemyBase), "GeoGame.Resources.Sprites.bee.png");
        public static readonly SKBitmap PlayerBlasterSprite = BitmapExtensions.LoadBitmapResource(typeof(EnemyBase), "GeoGame.Resources.Sprites.shipBlaster.png");
        public static readonly SKBitmap PlayerSpriteCentre = BitmapExtensions.LoadBitmapResource(typeof(EnemyBase), "GeoGame.Resources.Sprites.shipCentre.png");
        public static readonly SKBitmap PlayerSpriteLeft = BitmapExtensions.LoadBitmapResource(typeof(EnemyBase), "GeoGame.Resources.Sprites.shipLeft.png");
        public static readonly SKBitmap PlayerSpriteMaxLeft = BitmapExtensions.LoadBitmapResource(typeof(EnemyBase), "GeoGame.Resources.Sprites.shipLeftMax.png");
        public static readonly SKBitmap PlayerSpriteMaxRight = BitmapExtensions.LoadBitmapResource(typeof(EnemyBase), "GeoGame.Resources.Sprites.shipRightMax.png");
        public static readonly SKBitmap PlayerSpriteRight = BitmapExtensions.LoadBitmapResource(typeof(EnemyBase), "GeoGame.Resources.Sprites.shipRight.png");
        public static readonly SKBitmap StarBlasterSprite = BitmapExtensions.LoadBitmapResource(typeof(EnemyBase), "GeoGame.Resources.Sprites.starBullet.png");

        #endregion Fields
    }
}