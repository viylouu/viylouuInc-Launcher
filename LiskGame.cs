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

        ITexture toronoiTest = null;

        public void Update()
        {
            if (!started)
            {
                toronoiTest = genToronoi(1000, new Vector2(100, 100), Color.White, Color.Black);

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

                canv.DrawTexture(toronoiTest, new Vector2(Window.Width / 2, Window.Height / 2), new Vector2(Window.Height, Window.Height), Alignment.Center);
            }
        }

        public float getAngle(Vector2 p1, Vector2 p2)
        {
            return (float)Math.Atan2(p1.X - p2.X, p1.Y - p2.Y);
        }

        public ITexture genToronoi(int iterations, Vector2 size, Color bgcol, Color paintcol)
        {
            ITexture noiseTex = Graphics.CreateTexture((int)size.X, (int)size.Y, TextureOptions.None);

            for (int x = 0; x < size.X; x++)
            {
                for (int y = 0; y < size.Y; y++)
                {
                    noiseTex.GetPixel(x, y) = bgcol;
                }
            }

            noiseTex.ApplyChanges();
            return noiseTex;
        }
    }
}
