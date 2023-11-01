using ImGuiNET;
using SimulationFramework;
using SimulationFramework.Drawing;
using SimulationFramework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace viylouuInc_Launcher
{
    internal class LiskGame
    {
        public bool started = false;

        public bool men = false;

        public void Update()
        {
            if (!started)
            {
                started = true;
            }
            else
            {
                if (Keyboard.IsKeyPressed(Key.Esc))
                { men = !men; }

                if (men)
                {
                    ImGui.Begin("Menu");

                    if (ImGui.Button("Quit Game"))
                    {
                        Program.gameStarted = false;
                        started = false;
                    }

                    if (ImGui.Button("Quit Launcher"))
                    {
                        Environment.Exit(0);
                    }

                    ImGui.End();
                }

                ICanvas canv = Graphics.GetOutputCanvas();

                canv.Clear(new Color(0, 0, 0));
            }
        }

        public float getAngle(Vector2 p1, Vector2 p2)
        {
            return (float)Math.Atan2(p1.X - p2.X, p1.Y - p2.Y);
        }
    }
}
