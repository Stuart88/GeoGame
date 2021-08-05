using GeoGame.Views;
using Xamarin.Forms;

namespace GeoGame
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        #region Constructors

        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(MainMap), typeof(MainMap));
        }

        #endregion Constructors

        //private async void OnMenuItemClicked(object sender, EventArgs e)
        //{
        //    await Shell.Current.GoToAsync("//MainMap");
        //}
    }
}