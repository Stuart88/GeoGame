using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeoGame.Models.Battles.Weapons
{
    public class HornetBlasterBullet : BlasterBullet
    {

        public HornetBlasterBullet(WeaponBase weapon) : base(weapon)
        {
            this.Width = weapon.Parent.Width;
            this.Height = this.Width;
        }

    }
}
