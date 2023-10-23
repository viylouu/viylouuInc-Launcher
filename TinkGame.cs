using SimulationFramework;
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

        public ITexture props = null;

        public int pixSize = 6;

        public void Update()
        {
            if (!started)
            {
                props = Graphics.LoadTexture(@"Assets\Sprites\Tink Assets\Woods-Overworld\Props.png");

                started = true;
            }
            else
            {
                ICanvas canv = Graphics.GetOutputCanvas();

                canv.Clear(Color.Black);

                //draws test plant
                canv.DrawTexture(
                    props,
                    new Rectangle(
                        new Vector2(
                            0,
                            16
                        ),
                        new Vector2(
                            16,
                            16
                        ),
                        Alignment.TopLeft
                    ),
                    new Rectangle(
                        new Vector2(
                            Window.Width / 2,
                            Window.Height / 2
                        ),
                        new Vector2(
                            16 * pixSize,
                            16 * pixSize
                        ),
                        Alignment.Center
                    )
                );
            }
        }
    }
}
