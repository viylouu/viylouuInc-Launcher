using SimulationFramework.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace viylouuInc_Launcher
{
    internal class TinkGame
    {
        public bool started = false;

        public void Update()
        {
            if (!started)
            {
                started = true;
            }
            else
            {
                ICanvas canv = Graphics.GetOutputCanvas();
            }
        }

        public class ssData
        { 
            public Vector2[] topLefts { get; set; }

            public Vector2[] scales { get; set; }
        }
    }
}
