using SimulationFramework;
using SimulationFramework.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace viylouuInc_Launcher
{
    internal class BaskGame
    {
        ITexture gsc = null;

        bool started = false;

        public void Update()
        {
            if (!started)
            {
                if (gsc != null) { gsc.Dispose(); }
                gsc = Graphics.LoadTexture(@"Assets\Sprites\Bask Assets\GreenCar.png");

                started = true;
            }
            else
            {
                ICanvas canv = Graphics.GetOutputCanvas();

                canv.Clear(Color.Black);

                canv.DrawTexture(gsc);
            }
        }
    }
}
