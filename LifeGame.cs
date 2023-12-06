using SimulationFramework;
using SimulationFramework.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace viylouuInc_Launcher
{
    internal class LifeGame
    {
        bool started = false;

        bool simstarted = false;

        Random r = new Random();

        Vector2[] anPs = null;

        int anAmt = 100; 

        public void Update() 
        {
            if (!started)
            {
                Simulation.SetFixedResolution(320, 180, Color.Black, false, false, false);

                started = true;
            }
            else
            {
                ICanvas canv = Graphics.GetOutputCanvas();

                canv.Clear(Color.Black);

                if (!simstarted)
                {
                    ImGui.Begin("Simulation Settings");

                    ImGui.SetWindowPos(Vector2.Zero);
                    ImGui.SetWindowSize(new Vector2(Window.Width, Window.Height));

                    ImGui.SliderInt("Animal Amount", ref anAmt, 2, 10000);

                    ImGui.End();
                }
            }
        }
    }
}
