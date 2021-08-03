using GeoGame.Models.Geo;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeoGame.ViewModels
{
    public class MainMapViewModel : BaseViewModel
    {
        Country selectedCountry = new Country();
        public Country SelectedCountry
        {
            get { return selectedCountry; }
            set { 
                SetProperty(ref selectedCountry, value); 
            }
        }
    }
}
