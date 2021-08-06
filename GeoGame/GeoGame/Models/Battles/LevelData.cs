using GeoGame.Helpers;
using GeoGame.Interfaces;
using GeoGame.Models.Battles.Enemies;
using GeoGame.Models.Battles.Weapons;
using GeoGame.Models.Enums;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGame.Models.Battles
{
    public class LevelData : IDifficulty
    {
        #region Fields

        private Random Rand = new Random();

        #endregion Fields

        #region Constructors

        public LevelData(SKCanvasView canvasView, Geo.Country country, List<Geo.Country> allCountries)
        {
            this.Canvas = canvasView;
            int countryCount = allCountries.Count;
            int currentCountryIndex = allCountries.IndexOf(country);
            DifficultyLevel diff;

            if (currentCountryIndex <= countryCount / 4)
                diff = DifficultyLevel.Easy;
            else if (currentCountryIndex <= countryCount / 2)
                diff = DifficultyLevel.Medium;
            else if (currentCountryIndex <= 3 * countryCount / 4)
                diff = DifficultyLevel.Hard;
            else
                diff = DifficultyLevel.Insane;

            this.DifficultyLevel = diff;

            this.OneHitShipBalance = (double)(countryCount - currentCountryIndex) / countryCount;
            this.DroneBalance = (double)(countryCount / 2 - currentCountryIndex) / countryCount;
            this.AttackerBalance = (double)(countryCount / 4 - currentCountryIndex) / countryCount;
            this.CountryIndex = currentCountryIndex;

            EnemyCount = this.CountryIndex < 5 ? 5 : this.CountryIndex;

            this.NormaliseBalances();

            switch (this.DifficultyLevel)
            {
                case DifficultyLevel.Easy: InitEasy(); break;
                case DifficultyLevel.Medium: InitMedium(); break;
                case DifficultyLevel.Hard: InitHard(); break;
                case DifficultyLevel.Insane: InitInsane(); break;
                case DifficultyLevel.IsPlayer: InitPlayer(); break;
            }

            this.MaxEnemyHealth = this.Enemies.Sum(e => e.MaxHealth);

            this.InitActiveEnemies();
            this.Enemies.Shuffle();
        }

        #endregion Constructors

        #region Properties

        public double AttackerBalance { get; set; }
        public int CountryIndex { get; set; }
        public double DroneBalance { get; set; }
        public List<EnemyBase> Enemies { get; set; } = new List<EnemyBase>();
        public int MaxActiveEnemies { get; set; }
        public int MaxEnemyHealth { get; set; }
        public double OneHitShipBalance { get; set; }
        private SKCanvasView Canvas { get; set; }
        private DifficultyLevel DifficultyLevel { get; set; }
        private int EnemyCount { get; set; }

        private List<MoveAction> MoveActions { get; } = new List<MoveAction>
        {
            MovementFunctions.BasicLinearLeftRight,
            MovementFunctions.LocalisedCircle,
            MovementFunctions.SinusoidalLeftRightFull,
            MovementFunctions.SinusoidalLeftRightLocal,
        };

        private int RawBalance => (int)(this.OneHitShipBalance + this.DroneBalance + this.AttackerBalance);

        #endregion Properties

        #region Methods

        public void InitEasy()
        {
            this.MaxActiveEnemies = 5;

            DifficultyLevel diff = DifficultyLevel.Easy;

            for (int i = 0; i < this.OneHitShipBalance; i++)
            {
                SetDifficultyBasedOnIncrement(ref diff, i);
                this.Enemies.Add(new OneHitShip(diff, GetRandomMoveAction(), WeaponsEnum.SlowBlaster, this.Canvas));
                diff = DifficultyLevel.Easy;
            }
            for (int i = 0; i < this.DroneBalance; i++)
            {
                SetDifficultyBasedOnIncrement(ref diff, i);
                this.Enemies.Add(new Drone(diff, GetRandomMoveAction(), WeaponsEnum.SlowBlaster, this.Canvas));
                diff = DifficultyLevel.Easy;
            }
            for (int i = 0; i < this.AttackerBalance; i++)
            {
                SetDifficultyBasedOnIncrement(ref diff, i);
                this.Enemies.Add(new Attacker(diff, GetRandomMoveAction(), WeaponsEnum.SpreadBlaster, this.Canvas));
                diff = DifficultyLevel.Easy;
            }
        }

        public void InitHard()
        {
            this.MaxActiveEnemies = 30;
        }

        public void InitInsane()
        {
            this.MaxActiveEnemies = 50;
        }

        public void InitMedium()
        {
            this.MaxActiveEnemies = 20;
        }

        public void InitPlayer()
        {
            // Here for interface implementation
            return;
        }

        private MoveAction GetRandomMoveAction()
        {
            MoveActions.Shuffle();
            return MoveActions[0];
        }

        private void InitActiveEnemies()
        {
            int activesAdded = 0;
            foreach (var e in this.Enemies)
            {
                if (activesAdded++ < this.MaxActiveEnemies)
                    e.Active = true;
            }
        }

        private void NormaliseBalances()
        {
            double weighting = (double)this.EnemyCount / this.RawBalance;

            this.OneHitShipBalance = Math.Round(weighting * this.OneHitShipBalance, 0, MidpointRounding.AwayFromZero);
            this.AttackerBalance = Math.Round(weighting * this.AttackerBalance, 0, MidpointRounding.AwayFromZero);
            this.DroneBalance = Math.Round(weighting * this.DroneBalance, 0, MidpointRounding.AwayFromZero);

            while (this.RawBalance < this.EnemyCount)
                this.DroneBalance++;

            this.EnemyCount = this.RawBalance;
        }

        private void SetDifficultyBasedOnIncrement(ref DifficultyLevel diff, int i)
        {
            if (i > 2)
                diff = DifficultyLevel.Medium;

            if (i > 4)
                diff = DifficultyLevel.Hard;

            if (i > 6)
                diff = this.DifficultyLevel;
        }

        #endregion Methods
    }
}