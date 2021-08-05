namespace GeoGame.Interfaces
{
    public interface IDifficulty
    {
        #region Methods

        public abstract void InitEasy();

        public abstract void InitHard();

        public abstract void InitInsane();

        public abstract void InitMedium();

        public abstract void InitPlayer();

        #endregion Methods
    }
}