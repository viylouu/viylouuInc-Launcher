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
                props = Graphics.LoadTexture(@"Assets\Sprites\Tink Assets\Props.png");

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
                            propsD.topLefts[0].X, 
                            propsD.topLefts[0].Y
                        ), 
                        new Vector2(
                            propsD.scales[0].X, 
                            propsD.scales[0].Y)
                        ), 
                    new Rectangle(
                        new Vector2(
                            Window.Width / 2, 
                            Window.Height / 2
                        ), 
                        new Vector2(
                            propsD.scales[0].X * pixSize, 
                            propsD.scales[0].Y * pixSize
                        )
                    )
                );

            }
        }
    }
}
