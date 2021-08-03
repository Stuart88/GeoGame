using System;
using System.Collections.Generic;
using System.Text;

namespace GeoGame.Interfaces
{
    public interface IMessageService
    {
        // Necessary for all areas sending messaged to each other to have unified type

        void SubscribeToMessages();
    }
}
