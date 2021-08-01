using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace GeoGame.Models.Battles
{
    public abstract class MovingObjectBase
    {
        #region Properties

        public float BaseVelX { get; set; }
        public float BaseVelY { get; set; }
        public float Height { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float VelX { get; set; }
        public float VelY { get; set; }
        public float Width { get; set; }
        public bool IsDead { get; set; }
        public int Health { get; set; }
        public SKBitmap MainSprite { get; set; }
        public SKBitmap SpriteSheet { get; set; }
        public WeaponBase Weapon { get; set; }
        
        /// <summary>
        /// Object currently in game, 'on screen' (though sometimes might be slightly off screen!)
        /// </summary>
        public bool Active { get; set; }
        public bool IsPlayer => this is Player;

        #endregion Properties

        #region Methods

        public abstract void Draw(ref SKCanvas canvas, SKPaint skPaint, SKSize canvasSize);
        public abstract void Move(float dt, SKCanvasView canvasView);

        #endregion Methods
    }
}