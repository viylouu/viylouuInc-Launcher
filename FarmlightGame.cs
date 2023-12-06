using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulationFramework;
using SimulationFramework.Drawing;
using SimulationFramework.Input;
using System.Numerics;

namespace viylouuInc_Launcher
{
    internal class FarmlightGame
    {
        bool started = false;

        Vector2 mouse = new Vector2(0, 0);

        public void Update()
        {
            if (!started)
            {
                Simulation.SetFixedResolution(320, 180, Color.Black, false, false, true);

                started = true;
            }
            else
            {
                ICanvas canv = Graphics.GetOutputCanvas();
                canv.Clear(Color.Black);

                canv.Fill(Color.White);
                canv.DrawRect(mouse, new Vector2(50, 50), Alignment.Center);

                mouse += new Vector2(
                    ( Mouse.Position.X - mouse.X ) / ( 2.154f / (Time.DeltaTime*30) ),
                    ( Mouse.Position.Y - mouse.Y ) / ( 2.154f / (Time.DeltaTime*30) )
                );
            }
        }
    }
}
