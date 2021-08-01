using GeoGame.Models.Battles;
using GeoGame.Models.Geo;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private bool _objectsReady = false;
        private bool _pageActive;
        private Random _rand = new Random();
        private int MaxActiveEnemies { get; set; }

        #endregion Fields

        #region Constructors

        public CountryBattle(Country country, List<PopulatedPlace> places)
        {
            InitializeComponent();
            this.Country = country;
            this.Places = places;
            this.IsSmallCountry = this.Country.Population < this.PopulationScaler;

            countryNameLabel.Text = $"Fighting {this.Country.Name}!";
        }

        #endregion Constructors

        #region Properties

        private Country Country { get; set; }
        private List<Enemy> Enemies { get; set; }
        private bool GameWon { get; set; }
        private List<PopulatedPlace> Places { get; set; }
        private Player Player { get; set; }

        #endregion Properties

        #region Methods

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

        private void CheckCollisions()
        {
            foreach (var e in this.Enemies.Where(i => i.Active))
            {
                foreach (var b in this.Player.Weapon.Bullets.Where(i => i.Fired))
                    e.CheckCollisionWithBullet(b);

                foreach (var b in e.Weapon.Bullets.Where(i => i.Fired))
                    this.Player.CheckCollisionWithBullet(b);
            }

            // Now check if any new enemies need to be added, if 

            int activeEnemiesCount = this.Enemies.Where(e => e.Active).Count();

            if (activeEnemiesCount < this.MaxActiveEnemies)
            {
                //Get next available non-dead enemy
                var toActivate = this.Enemies.FirstOrDefault(e => !e.IsDead && !e.Active);

                if (toActivate != null)
                {
                    toActivate.Active = true;
                    activeEnemiesCount++; // need to increment this so the check below does need show 0 too early
                }
            }

            if(activeEnemiesCount == 0)
            {
                this.GameWon = true;
                this._pageActive = false;
                _ = DisplayAlert("Game Over", "You win!", "OK");
            }
        }

        private void DrawObjects(SKCanvas canvas, SKPaint skPaint)
        {
            this.Player.Draw(ref canvas, skPaint, canvasView.CanvasSize);

            foreach (var en in this.Enemies.Where(e => e.Active && !e.IsDead))
                en.Draw(ref canvas, skPaint, canvasView.CanvasSize);

           
        }

        private void FireWeapons(float dt)
        {
            this.Player.Weapon.FireWeapon(dt);

            foreach (var e in this.Enemies.Where(e => e.Active && !e.IsDead))
            {
                e.Weapon.FireWeapon(dt + dt * (-1 + 2 * (float)_rand.NextDouble())); // Add a +/- 1dt variation, so all enemies don't fire in unison
            }
        }

        private void Init()
        {
            var ms = 1000.0 / _fpsWanted;
            var ts = TimeSpan.FromMilliseconds(ms);

            // Create a timer that triggers roughly every 1/30 seconds
            Device.StartTimer(ts, TimerLoop);

            _pageActive = true;
        }

        private int PopulationScaler { get; } = 1000000;

        private bool IsSmallCountry { get; }
        private int EnemyCount { get; set; }

        private void InitEnemies()
        {
            this.Enemies = new List<Enemy>();

            this.EnemyCount = this.IsSmallCountry ? 1 : this.Country.Population / this.PopulationScaler; // e.g. UK will have 64, China will have 1379.

            this.MaxActiveEnemies = (this.EnemyCount / 4) >= 50 ? 50 : (this.EnemyCount / 4);
            if (this.MaxActiveEnemies == 0)
                this.MaxActiveEnemies = 1;

            this.PopulationKillIncrementSize = this.Country.Population / this.EnemyCount;

            int activesAdded = 0;

            for (int i = 0; i < this.EnemyCount; i++)
            {
                Enemy e = new Enemy();
                e.Width = canvasView.CanvasSize.Width / 15;
                e.Health = 40;
                e.Height = e.Width * 1.5f;
                e.BaseVelX = 200; // Not currently used
                e.BaseVelY = 40;  // Not currently used
                e.VelX = _rand.Next(150, 251);
                e.VelY = _rand.Next(35, 45);
                e.PosX = _rand.Next(0, (int)(canvasView.CanvasSize.Width - e.Width));
                e.PosY = _rand.Next((int)-e.Height - 20, (int)-e.Height); // off top of screen

                e.Weapon = new Blaster(e);

                if (activesAdded++ < this.MaxActiveEnemies)
                    e.Active = true;

                e.AssignMainSprite(_rand.Next(0, 10), _rand.Next(0, 10));

                this.Enemies.Add(e);
            }
        }

        private void InitPlayer()
        {
            this.Player = new Player();
            this.Player.MaxHealth = 100;
            this.Player.Health = 100;
            this.Player.Width = canvasView.CanvasSize.Width / 15;
            this.Player.Height = this.Player.Width * 2f;
            this.Player.PosX = (canvasView.CanvasSize.Width - this.Player.Width) / 2;
            this.Player.Weapon = new Blaster(this.Player);
            this.Player.BaseVelX = 500;
        }

        private void LeftButton_Pressed(object sender, EventArgs e)
        {
            this.Player.MovingLeft = true;
            this.Player.VelX = -this.Player.BaseVelX;
            this.Player.Direction = Data.BattlesData.SpriteDirection.LeftMax;
        }

        private void RightButton_Released(object sender, EventArgs e)
        {
            this.Player.MovingRight = false;
            this.Player.AccelRight = this.Player.BaseAccelRight;
            this.Player.VelX = 0;
            this.Player.Direction = SpriteDirection.Centre;
        }

        private void LeftButton_Released(object sender, EventArgs e)
        {
            this.Player.MovingLeft = false;
            this.Player.AccelLeft = this.Player.BaseAccelLeft;
            this.Player.VelX = 0;
            this.Player.Direction = SpriteDirection.Centre;
        }

        private void MiddleButton_Pressed(object sender, EventArgs e)
        {
        }

        private void MoveObjects(float dt)
        {
            this.Player.Move(dt, canvasView);

            foreach (var e in this.Enemies.Where(e => e.Active && !e.IsDead))
            {
                e.Move(dt, canvasView);
            }
        }

        private void OnPainting(object sender, SKPaintSurfaceEventArgs e)
        {
            var surface = e.Surface;
            var canvas = surface.Canvas;
            canvas.Clear(_fillColor);

            if (!this._objectsReady)
            {
                InitPlayer();
                InitEnemies();
                this._objectsReady = true;
            }

            // Draw a circle
            using (SKPaint skPaint = new SKPaint())
            {
                DrawObjects(canvas, skPaint);
            }
        }

        private void RightButton_Pressed(object sender, EventArgs e)
        {
            this.Player.MovingRight = true;
            this.Player.VelX = this.Player.BaseVelX;
            this.Player.Direction = SpriteDirection.RightMax;
        }

        private bool TimerLoop()
        {
            if (!this._objectsReady)  // Can't run any logic until game objects ready! (player, enemies etc)
                return true;

            // Get the elapsed time from the stopwatch because the 1/30 timer interval is not accurate and can be off by 2 ms
            float dt = (float)_stopWatch.Elapsed.TotalSeconds;

            FireWeapons(dt);

            // Restart the time measurement for the next time this method is called
            _stopWatch.Restart();

            MoveObjects(dt);

            CheckCollisions();

            UpdateScreenInformation();

            // Trigger the redrawing of the view
            canvasView.InvalidateSurface();

            return _pageActive;
        }

        /// <summary>
        /// Update labels, health, progress bar, etc
        /// </summary>
        private void UpdateScreenInformation()
        {
            // Progress bar starts full then progresses down to 0
            int deadCount = this.Enemies.Count(e => e.IsDead);

            double killsProgress = deadCount == this.EnemyCount
                ? 0
                : ((double)(this.Country.Population - (deadCount * this.PopulationKillIncrementSize)) / this.Country.Population);
            
            BattleStateProgress.ProgressTo(killsProgress, 100, Easing.CubicInOut);

            double healthBarProgress = (double)this.Player.Health / this.Player.MaxHealth;
            HealthBar.ProgressTo(healthBarProgress, 200, Easing.CubicInOut);

            if (healthBarProgress >= 0.5)
                HealthBar.ProgressColor = Color.LawnGreen;
            else if(healthBarProgress >= 0.2)
                HealthBar.ProgressColor = Color.OrangeRed;
            else
                HealthBar.ProgressColor = Color.Red;
        }

        private int PopulationKillIncrementSize { get; set; }

        #endregion Methods
    }
}