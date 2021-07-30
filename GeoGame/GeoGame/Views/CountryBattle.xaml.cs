using GeoGame.Models.Geo;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GeoGame.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CountryBattle : ContentPage
    {
        private bool _pageActive;
        private readonly Stopwatch _stopWatch = new Stopwatch();
        private double _fpsAverage = 0.0;
        private const double _fpsWanted = 30.0;
        private int _fpsCount = 0;
        private SKColor _fillColor;
        private uint _frameCount = 0;
        private double x;
        private double y;
        private double vx;
        private double vy;
        private int r = 50;

        private Country Country { get; set; }
        private List<PopulatedPlace> Places { get; set; }

        public CountryBattle(Country country, List<PopulatedPlace> places)
        {
            InitializeComponent();
            this.Country = country;
            this.Places = places;

            countryNameLabel.Text = $"Fighting {this.Country.Name}!";
        }

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

            // Initialize position
            x = 300.0;
            y = 200.0;

            // Initialize velocity
            vx = 100.0;
            vy = 200.0;

            _pageActive = true;
        }

        private bool TimerLoop()
        {
            // Get the elapsed time from the stopwatch because the 1/30 timer interval is not accurate and can be off by 2 ms
            var dt = _stopWatch.Elapsed.TotalSeconds;

            // Restart the time measurement for the next time this method is called
            _stopWatch.Restart();

            var width = canvasView.CanvasSize.Width - r;
            var height = canvasView.CanvasSize.Height - r;

            // Update position based on velocity and the delta time
            x += dt * vx;
            y += dt * vy;

            // Check collision with side of the screen and reverse velocity
            // We also use the velocity component to see if the circle is moving in the direction of the boundary
            // Otherwise it could try to reverse the direction again while the circle is moving away from the wall.
            if ((x < r && vx < 0) || (x > width && vx > 0))
                vx = -vx;

            if ((y < r && vy < 0) || (y > height && vy > 0))
                vy = -vy;

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
                fpsLabel.Text = fps.ToString("N3", CultureInfo.InvariantCulture);

                _fpsCount = 0;
                _fpsAverage = 0.0;
            }

            // Trigger the redrawing of the view
            canvasView.InvalidateSurface();

            return _pageActive;
        }

        private void OnPainting(object sender, SKPaintSurfaceEventArgs e)
        {
            var surface = e.Surface;
            var canvas = surface.Canvas;

            canvas.Clear(_fillColor);

            // Draw a circle
            using (SKPaint skPaint = new SKPaint())
            {
                skPaint.Style = SKPaintStyle.Fill;
                skPaint.IsAntialias = true;
                skPaint.Color = SKColors.Blue;
                skPaint.StrokeWidth = 10;

                canvas.DrawCircle((float)x, (float)y, r, skPaint);
            }
        }
    }
}