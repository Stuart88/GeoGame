namespace GeoGame.Interfaces
{
    public interface IMessageService
    {
        // Necessary for all areas sending messaged to each other to have unified type

        #region Methods

        void SubscribeToMessages();

        #endregion Methods
    }
}