using GeoGame.Models.Battles;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeoGame.ViewModels
{
    public class CountryBattleViewModel : BaseViewModel
    {
        string selectedWeaponName;
        public string SelectedWeaponName
        {
            get { return selectedWeaponName; }
            set { SetProperty(ref selectedWeaponName, value); }
        }
    }
}
