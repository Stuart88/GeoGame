using System.ComponentModel.DataAnnotations;

namespace GeoGame.Models.Battles.Weapons
{
    public enum WeaponsEnum
    {
        [Display(Name = "Slow Blaster")]
        SlowBlaster,

        [Display(Name = "Fast Blaster")]
        FastBlaster,

        [Display(Name = "Star Blaster")]
        StarBlaster,

        [Display(Name = "Spreadblaster")]
        SpreadBlaster,

        [Display(Name = "Hornet blaster")]
        HornetBlaster,
    }
}