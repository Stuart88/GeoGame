using GeoGame.Models.Geo;

namespace GeoGame.ViewModels
{
    public class MainMapViewModel : BaseViewModel
    {
        #region Fields

        private Country selectedCountry = new Country();

        #endregion Fields

        #region Properties

        public Country SelectedCountry
        {
            get { return selectedCountry; }
            set { SetProperty(ref selectedCountry, value); }
        }

        #endregion Properties
    }
}