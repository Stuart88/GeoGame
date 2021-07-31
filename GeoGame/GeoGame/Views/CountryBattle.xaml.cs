using GeoGame.Models.Battles;
using GeoGame.Models.Geo;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static GeoGame.Data.BattlesData;

namespace GeoGame.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CountryBattle : ContentPage
    {
        #region Fields

        private const double _fpsWanted = 30.0;
        private readonly Stopwatch _stopWatch = new Stopwatch();
        private SKColor _fillColor;
        private double _fpsAverage = 0.0;
        private int _fpsCount = 0;
        private uint _frameCount = 0;
        private bool _movingLeft = false;
        private bool _movingRight = false;
        private bool _pageActive;
        private bool _playerReady = false;

        #endregion Fields

        #region Constructors

        public CountryBattle(Country country, List<PopulatedPlace> places)
        {
            InitializeComponent();
            this.Country = country;
            this.Places = places;

            countryNameLabel.Text = $"Fighting {this.Country.Name}!";
        }

        #endregion Constructors

        #region Properties

        private Country Country { get; set; }
        private List<PopulatedPlace> Places { get; set; }
        private Player Player { get; set; }

        #endregion Properties

        #region Methods

        private float _accel = 400;
        private float _baseAccel = 400;
        private float _jerk = 500;

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Init();
        }

        protected override void OnDisappearing()
        {
            _pageActive = false;

            base.OnDisappearing();
        }

        private void Init()
        {
            var ms = 1000.0 / _fpsWanted;
            var ts = TimeSpan.FromMilliseconds(ms);

            // Create a timer that triggers roughly every 1/30 seconds
            Device.StartTimer(ts, TimerLoop);

            _pageActive = true;
        }

        private void InitPlayer()
        {
            this.Player = new Player();
            this.Player.Width = canvasView.CanvasSize.Width / 15;
            this.Player.Height = this.Player.Width * 2f;
            this.Player.PosX = (canvasView.CanvasSize.Width - this.Player.Width) / 2;
            this.Player.Weapon = new Blaster(this.Player);
            this.Player.BaseVelX = 500;
            this._playerReady = true;
        }

        private void LeftButton_Pressed(object sender, EventArgs e)
        {
            this.Player.VelX = -this.Player.BaseVelX;
            _movingLeft = true;
            this.Player.Direction = Data.BattlesData.SpriteDirection.LeftMax;
        }

        private void LeftRightButton_Released(object sender, EventArgs e)
        {
            _movingRight = false;
            _movingLeft = false;
            _accel = _baseAccel;
            this.Player.VelX = 0;
            this.Player.Direction = Data.BattlesData.SpriteDirection.Centre;
        }

        private void MiddleButton_Pressed(object sender, EventArgs e)
        {
        }

        private void MovePlayer(SpriteDirection direction, float dt)
        {
            float vMax = 1500;
            _accel += dt * _jerk; // increasing acceleration. For nicer and more noticable increase in speed while moving left/right

            if (direction == SpriteDirection.Left && _movingLeft)
            {
                if (Math.Abs(this.Player.VelX) < vMax)
                    this.Player.VelX -= dt * _accel;

                if (this.Player.PosX > 0)
                {
                    this.Player.PosX += dt * this.Player.VelX;
                }
                else
                {
                    this.Player.VelX = 0;
                    _movingLeft = false;
                }
            }
            else if (direction == SpriteDirection.Right && _movingRight)
            {
                if (Math.Abs(this.Player.VelX) < vMax)
                    this.Player.VelX += dt * _accel;

                if (this.Player.PosX + this.Player.Width < canvasView.CanvasSize.Width)
                {
                    this.Player.PosX += dt * this.Player.VelX;
                }
                else
                {
                    this.Player.VelX = 0;
                    _movingRight = false;
                }
            }

            fpsLabel.Text = $"Vel: {this.Player.VelX.ToString("N3", CultureInfo.InvariantCulture)}, Accel: {_accel.ToString("N3", CultureInfo.InvariantCulture)}";
        }

        private void OnPainting(object sender, SKPaintSurfaceEventArgs e)
        {
            var surface = e.Surface;
            var canvas = surface.Canvas;
            canvas.Clear(_fillColor);

            if (!this._playerReady)
                InitPlayer();

            // Draw a circle
            using (SKPaint skPaint = new SKPaint())
            {
                skPaint.Style = SKPaintStyle.Fill;
                skPaint.IsAntialias = true;
                skPaint.Color = SKColors.Blue;
                skPaint.StrokeWidth = 10;

                //canvas.DrawCircle((float)x, (float)y, r, skPaint);
                this.Player.DrawPlayer(ref canvas, skPaint, canvasView.CanvasSize);
            }
        }

        private void RightButton_Pressed(object sender, EventArgs e)
        {
            _movingRight = true;
            this.Player.VelX = this.Player.BaseVelX;
            this.Player.Direction = Data.BattlesData.SpriteDirection.RightMax;
        }

        private bool TimerLoop()
        {
            if (!this._playerReady)  // Can't run any logic until player ready!
                return true;

            // Get the elapsed time from the stopwatch because the 1/30 timer interval is not accurate and can be off by 2 ms
            float dt = (float)_stopWatch.Elapsed.TotalSeconds;

            this.Player.Weapon.DeltaTime += dt;
            if (this.Player.Weapon.DeltaTime >= this.Player.Weapon.FireRate)
                this.Player.Weapon.FireWeapon();

            // Restart the time measurement for the next time this method is called
            _stopWatch.Restart();

            if (_movingRight || _movingLeft)
            {
                MovePlayer(_movingLeft ? SpriteDirection.Left : SpriteDirection.Right, dt);
            }

            // Calculate current fps
            var fps = dt > 0 ? 1.0 / dt : 0;

            // When the fps is too low reduce the load by skipping the frame
            if (fps < _fpsWanted / 2)
                return _pageActive;

            // Calculate an averaged fps
            _fpsAverage += fps;
            _fpsCount++;

            if (_fpsCount == 20)
            {
                fps = _fpsAverage / _fpsCount;
                //fpsLabel.Text = fps.ToString("N3", CultureInfo.InvariantCulture);

                _fpsCount = 0;
                _fpsAverage = 0.0;
            }

            // Trigger the redrawing of the view
            canvasView.InvalidateSurface();

            return _pageActive;
        }

        #endregion Methods
    }
}