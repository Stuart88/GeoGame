using System;
using System.Collections.Generic;
using System.Text;

namespace GeoGame.Interfaces
{
    public interface IDifficulty
    {
        public abstract void InitEasy();

        public abstract void InitHard();

        public abstract void InitInsane();

        public abstract void InitMedium();

        public abstract void InitPlayer();
    }
}
