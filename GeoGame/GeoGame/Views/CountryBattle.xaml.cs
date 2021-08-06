using GeoGame.Extensions;
using GeoGame.Helpers;
using GeoGame.Interfaces;
using GeoGame.Models.Battles;
using GeoGame.Models.Battles.Enemies;
using GeoGame.Models.Battles.Weapons;
using GeoGame.Models.Geo;
using GeoGame.ViewModels;
using Plugin.SimpleAudioPlayer;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static GeoGame.Data.BattlesData;

namespace GeoGame.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CountryBattle : ContentPage, IMessageService
    {
        #region Fields

        private const double _fpsWanted = 30.0;
        private readonly Stopwatch _stopWatch = new Stopwatch();

        /// <summary>
        /// Continuous timer. Useful for trig functions in object movement
        /// </summary>
        private readonly Stopwatch _totalTime = new Stopwatch();

        private SKColor _fillColor;
        private bool _objectsReady = false;
        private bool _pageActive;
        private Random _rand = new Random();

        #endregion Fields

        #region Constructors

        public CountryBattle(Country country, List<Country> allCountries)
        {
            this.BindingContext = new CountryBattleViewModel();
            InitializeComponent();
            InitWeaponButtons();
            SubscribeToMessages();
            this.Country = country;
            this.AllCountries = allCountries;
            this.IsSmallCountry = this.Country.Population < this.PopulationScaler;
            _totalTime.Start();

            countryNameLabel.Text = $"Fighting {this.Country.Name}!";
        }

        #endregion Constructors

        #region Properties

        public List<Country> AllCountries { get; set; }

        public ISimpleAudioPlayer BattleMusic { get; set; } = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();

        public LevelData LevelData { get; set; }

        private Country Country { get; set; }

        private bool GameWon { get; set; }

        private bool IsSmallCountry { get; }

        private SKRect ParalaxDestRect { get; set; }

        private Player Player { get; set; } = new Player();

        private int PopulationScaler { get; } = 1000000;

        private float ScreenRatio { get; set; }

        private SKBitmap StarsMid { get; set; } = BitmapExtensions.LoadBitmapResource(typeof(EnemyBase), "GeoGame.Resources.Backgrounds.Stars.starsMid.png");

        private float StarsMidShift { get; set; }

        private SKRect StarsMidSrcRect { get; set; }

        private SKBitmap StarsSmall { get; set; } = BitmapExtensions.LoadBitmapResource(typeof(EnemyBase), "GeoGame.Resources.Backgrounds.Stars.starsSmall.png");

        private float StarsSmallShift { get; set; }

        private SKRect StarsSmallSrcRect { get; set; }

        #endregion Properties

        #region Methods

        public void SubscribeToMessages()
        {
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            InitMusic();

            InitGame();
        }

        protected override void OnDisappearing()
        {
            _pageActive = false;
            this.BattleMusic.Stop();
            base.OnDisappearing();
        }

        private void ChangePlayerWeapon(WeaponsEnum nextWeapon)
        {
            ((CountryBattleViewModel)this.BindingContext).SelectedWeaponName = nextWeapon.GetDisplayName();
            this.Player.ChangeWeapon(nextWeapon);
        }

        private void CheckCollisions()
        {
            foreach (var e in this.LevelData.Enemies.Where(i => i.Active || i.Weapon.Bullets.Any(b => b.Fired)))
            {
                if (e.Active)
                {
                    foreach (var b in this.Player.ActiveBullets)
                        e.CheckCollisionWithBullet(b);
                }

                foreach (var b in e.Weapon.Bullets.Where(i => i.Fired))
                {
                    this.Player.CheckCollisionWithBullet(b);

                    foreach (var b_player in this.Player.ActiveBullets)
                        b_player.CheckBulletOnBulletCollision(b);
                }
            }

            // Now check if any new enemies need to be added, if

            int activeEnemiesCount = this.LevelData.Enemies.Where(e => e.Active).Count();

            if (activeEnemiesCount < this.LevelData.MaxActiveEnemies)
            {
                //Get next available non-dead enemy
                var toActivate = this.LevelData.Enemies.FirstOrDefault(e => !e.IsDead && !e.Active);

                if (toActivate != null)
                {
                    toActivate.Active = true;
                    activeEnemiesCount++; // need to increment this so the check below does need show 0 too early
                }
            }

            if (activeEnemiesCount == 0)
            {
                this.GameWon = true;
                this._pageActive = false;
                this.OnGameWon();
            }

            if(this.Player.Health <= 0)
            {
                this._pageActive = false;
                this.OnGameLost();
            }
        }

        private void DoStarsParallax(float dt)
        {
            this.StarsSmallSrcRect = new SKRect(0, this.StarsSmall.Height / 2 - this.StarsSmallShift, this.StarsSmall.Width / 2 * this.ScreenRatio, this.StarsSmall.Height - this.StarsSmallShift);
            this.StarsMidSrcRect = new SKRect(0, this.StarsMid.Height / 2 - this.StarsMidShift, this.StarsMid.Width / 2 * this.ScreenRatio, this.StarsMid.Height - this.StarsMidShift);

            this.StarsSmallShift += 90 * dt;
            if (this.StarsSmallShift > this.StarsSmall.Height / 2)
                this.StarsSmallShift = 0;

            this.StarsMidShift += 80 * dt;
            if (this.StarsMidShift > this.StarsMid.Height / 2)
                this.StarsMidShift = 0;
        }

        private void DrawObjects(SKCanvas canvas, SKPaint skPaint)
        {
             this.ScreenRatio = canvasView.CanvasSize.Width / canvasView.CanvasSize.Height;
            // Can be 0 or NaN if initialisation happens early.
            if (this.ScreenRatio == 0 || this.ScreenRatio == float.NaN)
                return;
            this.StarsSmallSrcRect = new SKRect(0, 0, this.StarsSmall.Width / 2 * this.ScreenRatio, this.StarsSmall.Height / 2);
            this.StarsMidSrcRect = new SKRect(0, 0, this.StarsMid.Width / 2 * this.ScreenRatio, this.StarsMid.Height / 2);
            this.ParalaxDestRect = new SKRect(0, 0, canvasView.CanvasSize.Width, canvasView.CanvasSize.Height);

            canvas.DrawBitmap(this.StarsMid, this.StarsMidSrcRect, this.ParalaxDestRect, skPaint);
            canvas.DrawBitmap(this.StarsSmall, this.StarsSmallSrcRect, this.ParalaxDestRect, skPaint);

            // annoyingly need to force set this here because it's being initalised as a smaller value?! Seems canvas dimesions are late to init,
            // presumably cos of some XAML bullshit because this error only popped up after I changed a shit ton of XAML. Fucking annoying.
            this.Player.PosY = canvasView.CanvasSize.GetPlayerPosY();
            this.Player.Draw(ref canvas, canvasView.CanvasSize);
            this.Player.DrawBullets(ref canvas, canvasView.CanvasSize);

            foreach (var en in this.LevelData.Enemies)
            {
                en.Draw(ref canvas, canvasView.CanvasSize);
                en.DrawBullets(ref canvas, canvasView.CanvasSize);
            }
        }

        private void FastBlasterBtn_Clicked(object sender, EventArgs e)
        {
            this.ChangePlayerWeapon(WeaponsEnum.FastBlaster);
        }

        private void FireWeapons(float dt)
        {
            this.Player.Weapon.FireWeapon(dt);

            foreach (var e in this.LevelData.Enemies.Where(e => e.Active && !e.IsDead))
            {
                e.Weapon.FireWeapon(dt + dt * (-1 + 2 * (float)_rand.NextDouble())); // Add a +/- 1dt variation, so all enemies don't fire in unison
            }
        }

        private void HornetBlasterBtn_Clicked(object sender, EventArgs e)
        {
            this.ChangePlayerWeapon(WeaponsEnum.HornetBlaster);
        }

        private void InitEnemies()
        {
            this.LevelData = new LevelData(canvasView, this.Country, this.AllCountries);
        }

        private void InitGame()
        {
            var ms = 1000.0 / _fpsWanted;
            var ts = TimeSpan.FromMilliseconds(ms);

            // Create a timer that triggers roughly every 1/30 seconds
            Device.StartTimer(ts, TimerLoop);

            _pageActive = true;
        }

        private void InitMusic()
        {
            int availableTracksCount = 15;
            int songNum = this.Country.Id % availableTracksCount + 1; // Modulus ranges from 0 - 14
            this.BattleMusic.Load(Functions.GetStreamFromFile($"Resources.Music.Battle.{songNum}.mp3"));
            this.BattleMusic.PlaybackEnded += (s, e) => { this.BattleMusic.Play(); };
            this.BattleMusic.Play();
        }

        private void InitPlayer()
        {
            this.Player.MaxHealth = 100 + 10 * (Data.Game.GameData.CountriesDefeatedIds.Count - 1); // health increases as game progresses
            this.Player.Health = this.Player.MaxHealth;
            this.Player.Width = canvasView.CanvasSize.Width / 15;
            this.Player.Height = this.Player.Width * 2f;
            this.Player.PosX = (canvasView.CanvasSize.Width - this.Player.Width) / 2;
            this.Player.PosY = canvasView.CanvasSize.Height * (1 - 0.01f);
            this.Player.BaseVelX = 500;
            foreach (var w in Data.Game.GameData.AvailableWeapons)
            {
                this.Player.AddWeapon(w);
            }
            ChangePlayerWeapon(WeaponsEnum.SlowBlaster);
        }

        private void InitWeaponButtons()
        {
            SlowBlasterBtn.Source = ImageSource.FromResource("GeoGame.Resources.Sprites.btnSlowBlaster.png", typeof(CountryBattle).GetTypeInfo().Assembly);
            FastBlasterBtn.Source = ImageSource.FromResource("GeoGame.Resources.Sprites.btnFastBlaster.png", typeof(CountryBattle).GetTypeInfo().Assembly);
            StarBlasterBtn.Source = ImageSource.FromResource("GeoGame.Resources.Sprites.btnStarBlaster.png", typeof(CountryBattle).GetTypeInfo().Assembly);
            SpreadBlasterBtn.Source = ImageSource.FromResource("GeoGame.Resources.Sprites.btnSpreadBlaster.png", typeof(CountryBattle).GetTypeInfo().Assembly);
            HornetBlasterBtn.Source = ImageSource.FromResource("GeoGame.Resources.Sprites.btnHornetBlaster.png", typeof(CountryBattle).GetTypeInfo().Assembly);

            if (Data.Game.GameData.AvailableWeapons.Contains(WeaponsEnum.SlowBlaster))
            {
                SlowBlasterBtn.IsEnabled = true;
                SlowBlasterBtn.Opacity = 1;
            }
            if (Data.Game.GameData.AvailableWeapons.Contains(WeaponsEnum.StarBlaster))
            {
                StarBlasterBtn.IsEnabled = true;
                StarBlasterBtn.Opacity = 1;
            }
            if (Data.Game.GameData.AvailableWeapons.Contains(WeaponsEnum.FastBlaster))
            {
                FastBlasterBtn.IsEnabled = true;
                FastBlasterBtn.Opacity = 1;
            }
            if (Data.Game.GameData.AvailableWeapons.Contains(WeaponsEnum.SpreadBlaster))
            {
                SpreadBlasterBtn.IsEnabled = true;
                SpreadBlasterBtn.Opacity = 1;
            }
            if (Data.Game.GameData.AvailableWeapons.Contains(WeaponsEnum.HornetBlaster))
            {
                HornetBlasterBtn.IsEnabled = true;
                HornetBlasterBtn.Opacity = 1;
            }
        }

        private void LeftButton_Pressed(object sender, EventArgs e)
        {
            this.Player.MovingLeft = true;
            this.Player.VelX = -this.Player.BaseVelX;
            this.Player.Direction = SpriteDirection.LeftMax;
        }

        private void LeftButton_Released(object sender, EventArgs e)
        {
            this.Player.MovingLeft = false;
            this.Player.AccelLeft = this.Player.BaseAccelLeft;
            this.Player.VelX = 0;
            this.Player.Direction = SpriteDirection.Centre;
        }

        private void OnGameWon()
        {
            MessagingCenter.Send<IMessageService, Country>(this, Data.MessagingCenterMessages.WonCountryBattle, this.Country);
        }

        private void OnGameLost()
        {
            MessagingCenter.Send<IMessageService, object>(this, Data.MessagingCenterMessages.LostCountryBattle, null);
        }

        private void OnPainting(object sender, SKPaintSurfaceEventArgs e)
        {
            if (!_pageActive)
                return;

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

        private void RightButton_Released(object sender, EventArgs e)
        {
            this.Player.MovingRight = false;
            this.Player.AccelRight = this.Player.BaseAccelRight;
            this.Player.VelX = 0;
            this.Player.Direction = SpriteDirection.Centre;
        }

        private void SlowBlasterBtn_Clicked(object sender, EventArgs e)
        {
            this.ChangePlayerWeapon(WeaponsEnum.SlowBlaster);
        }

        private void SpreadBlasterBtn_Clicked(object sender, EventArgs e)
        {
            this.ChangePlayerWeapon(WeaponsEnum.SpreadBlaster);
        }

        private void StarBlasterBtn_Clicked(object sender, EventArgs e)
        {
            this.ChangePlayerWeapon(WeaponsEnum.StarBlaster);
        }

        private bool TimerLoop()
        {
            if (!this._objectsReady)  // Can't run any logic until game objects ready! (player, enemies etc)
                return true;

            // Get the elapsed time from the stopwatch because the 1/30 timer interval is not accurate and can be off by 2 ms
            float dt = (float)_stopWatch.Elapsed.TotalSeconds;

            //Move paralax source
            DoStarsParallax(dt);

            FireWeapons(dt);

            // Restart the time measurement for the next time this method is called
            _stopWatch.Restart();

            Updatebjects(dt, (float)_totalTime.Elapsed.TotalSeconds);

            CheckCollisions();

            UpdateScreenInformation();

            // Trigger the redrawing of the view
            canvasView.InvalidateSurface();

            return _pageActive;
        }

        private void Updatebjects(float dt, float totalT)
        {
            this.Player.Update(dt, totalT, canvasView);

            foreach (var e in this.LevelData.Enemies)
            {
                e.Update(dt, totalT, canvasView);
            }
        }

        /// <summary>
        /// Update labels, health, progress bar, etc
        /// </summary>
        private void UpdateScreenInformation()
        {
            // Progress bar starts full then progresses down to 0
            int remainingEnemyHealth = this.LevelData.Enemies.Sum(e => e.Health);
            BattleStateProgress.ProgressTo((double)remainingEnemyHealth / this.LevelData.MaxEnemyHealth, 100, Easing.CubicInOut);

            double healthBarProgress = (double)this.Player.Health / this.Player.MaxHealth;
            HealthBar.ProgressTo(healthBarProgress, 200, Easing.CubicInOut);

            if (healthBarProgress >= 0.5)
                HealthBar.ProgressColor = Color.LawnGreen;
            else if (healthBarProgress >= 0.2)
                HealthBar.ProgressColor = Color.OrangeRed;
            else
                HealthBar.ProgressColor = Color.Red;
        }

        #endregion Methods
    }
}