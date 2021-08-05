namespace GeoGame.Models.Enums
{
    public enum EnemyDifficulty
    {
        /// <summary>
        /// Use this when difficulty settings are not needed, e.g. when assigning something to player rather than enemy
        /// </summary>
        IsPlayer,

        Easy,
        Medium,
        Hard,
        Insane
    }
}