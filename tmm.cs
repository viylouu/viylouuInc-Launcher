using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace viylouuInc_Launcher
{
    internal class tmm
    {
        public class tile
        {
            public string ssSource { get; set; } //spritesheet source
            public int sssid { get; set; } //spritesheet source id

            public Vector2 sssp { get; set; } //spritesheet sprite pos
            public Vector2 ssss { get; set; } //spritesheet sprite scale

            public bool solid { get; set; } //using collisions
        }

        public class tilemap
        {
            public tile[,] matrix { get; set; }
        }
    }
}
