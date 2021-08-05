namespace GeoGame.ViewModels
{
    public class CountryBattleViewModel : BaseViewModel
    {
        #region Fields

        private string selectedWeaponName;

        #endregion Fields

        #region Properties

        public string SelectedWeaponName
        {
            get { return selectedWeaponName; }
            set { SetProperty(ref selectedWeaponName, value); }
        }

        #endregion Properties
    }
}