using GeoGame.Interfaces;
using GeoGame.Models.Geo;
using GeoGame.Views;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xamarin.Forms;

namespace GeoGame
{
    public partial class AppShell : Xamarin.Forms.Shell, IMessageService
    {
        #region Constructors

        public AppShell()
        {
            InitializeComponent();
            
            ShellHeaderImage.Source = ImageSource.FromResource("GeoGame.Resources.Sprites.shipRightMax.png", typeof(AppShell).GetTypeInfo().Assembly);
            
            Routing.RegisterRoute(nameof(MainMap), typeof(MainMap));
            PropertyChanged += AppShell_PropertyChanged;
            this.SubscribeToMessages();

            this.MapStylePicker.SelectedItem = Data.Game.GameData.MapTheme switch
            {
                Data.MapEnums.MapTheme.Standard => "Standard",
                Data.MapEnums.MapTheme.Silver => "Silver",
                Data.MapEnums.MapTheme.Retro => "Retro",
                Data.MapEnums.MapTheme.Dark => "Dark",
                Data.MapEnums.MapTheme.Night => "Night",
                Data.MapEnums.MapTheme.Aubergine => "Aubergine",
                _ => ""
            };

        }

        #endregion Constructors

        #region Methods

        public void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<IMessageService, List<Country>>(this, Data.MessagingCenterMessages.GotCountries, (sender, c) =>
            {
                this.PopulateCountriesList(c);
            });
        }

        private void AppShell_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("FlyoutIsPresented"))
            {
                if (FlyoutIsPresented)
                {
                    this.ShipHealthLabel.Text = $"Ship Health: {Data.Game.GetCurrentPlayerHealth()}";
                    this.CountriesDefeatedLabel.Text = $"Defeated: {Data.Game.GameData.CountriesDefeatedIds.Count} / 176 Nations";
                }
            }
        }

        private void Picker_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            string item = MapStylePicker.SelectedItem as string;

            Data.MapEnums.MapTheme theme = item switch
            {
                "Standard" => Data.MapEnums.MapTheme.Standard,
                "Silver" => Data.MapEnums.MapTheme.Silver,
                "Retro" => Data.MapEnums.MapTheme.Retro,
                "Dark" => Data.MapEnums.MapTheme.Dark,
                "Night" => Data.MapEnums.MapTheme.Night,
                "Aubergine" => Data.MapEnums.MapTheme.Aubergine,
                _ => Data.MapEnums.MapTheme.Aubergine
            };

            MessagingCenter.Send<IMessageService, Data.MapEnums.MapTheme>(this, Data.MessagingCenterMessages.SetMapTheme, theme);
        }

        private void PopulateCountriesList(List<Country> countries)
        {
            foreach (var c in countries)
            {
                bool isDefeated = Data.Game.GameData.CountriesDefeatedIds.Contains(c.Id);
                Grid addingGrid = new Grid()
                {
                    Margin = 2,
                    BackgroundColor = Color.Transparent,
                    HorizontalOptions = LayoutOptions.StartAndExpand
                };

                addingGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                addingGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                addingGrid.RowDefinitions.Add(new RowDefinition { Height = 1 });
                addingGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
                addingGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                Label name = new Label
                {
                    Text = $"{c.IndexNumber}. " + c.Name + (isDefeated ? " (DEFEATED)" : ""),
                    TextColor = isDefeated ? Color.DarkOrange : Color.Blue,
                    FontSize = 15,
                };
                Label population = new Label
                {
                    Text = $"Population: {c.Population}",
                    TextColor = isDefeated ? Color.DarkOrange : Color.Blue,
                    FontSize = 15,
                };
                Button goToBtn = new Button
                {
                    Text = "▶",
                    VerticalOptions = LayoutOptions.End,
                    BackgroundColor = Color.Orange
                };
                goToBtn.Clicked += (s, e) =>
                {
                    MessagingCenter.Send<IMessageService, Country>(this, Data.MessagingCenterMessages.HighlightCountry, c);
                    MessagingCenter.Send<IMessageService, Country>(this, Data.MessagingCenterMessages.CountryClicked, c);
                    this.FlyoutIsPresented = false;
                };

                Frame frame = new Frame { BackgroundColor = Color.LightGray, HeightRequest = 1 };

                addingGrid.Children.Add(name);
                addingGrid.Children.Add(population);
                addingGrid.Children.Add(goToBtn);
                addingGrid.Children.Add(frame);

                Grid.SetRow(name, 0);
                Grid.SetRow(population, 1);
                Grid.SetRow(goToBtn, 0);
                Grid.SetColumn(goToBtn, 1);
                Grid.SetRowSpan(goToBtn, 2);
                Grid.SetRow(frame, 2);
                Grid.SetColumn(frame, 0);
                Grid.SetColumnSpan(frame, 2);

                this.CountriesListStack.Children.Add(addingGrid);
            }
        }

        #endregion Methods

        //private async void OnMenuItemClicked(object sender, EventArgs e)
        //{
        //    await Shell.Current.GoToAsync("//MainMap");
        //}
    }
}