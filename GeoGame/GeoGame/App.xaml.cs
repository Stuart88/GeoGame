using Xamarin.Forms;

namespace GeoGame
{
    public partial class App : Application
    {
        #region Constructors

        public App()
        {
            InitializeComponent();

            Data.Game.LoadGame();

            MainPage = new AppShell();
        }

        #endregion Constructors

        #region Methods

        protected override void OnResume()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnStart()
        {
        }

        #endregion Methods
    }
}