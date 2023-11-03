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

        public Random r = new Random();

        public void Update()
        {
            if (!started)
            {
                toronoiTest = genToronoi(1000, new Vector2(128, 128), Color.White, Color.Black);

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

                //canv.DrawTexture(toronoiTest, new Vector2(Window.Width / 2, Window.Height / 2), new Vector2(Window.Height, Window.Height), Alignment.Center);
            }
        }

        public float getAngle(Vector2 p1, Vector2 p2)
        {
            return (float)Math.Atan2(p1.X - p2.X, p1.Y - p2.Y);
        }

        public ITexture genToronoi(int iterations, Vector2 size, Color bgcol, Color paintcol)
        {
            ITexture noiseTex = Graphics.CreateTexture((int)size.X, (int)size.Y, TextureOptions.None);
            int[,] blendVals = new int[(int)size.X, (int)size.Y];


            Vector2 currentPos = new Vector2(size.X / 2, size.Y / 2);

            for (int i = 0; i < iterations; i++)
            {
                if (currentPos.X >= 0 && currentPos.X <= size.X && currentPos.Y >= 0 && currentPos.Y <= size.Y)
                {
                    if (blendVals[(int)currentPos.X, (int)currentPos.Y] < 255)
                    { blendVals[(int)currentPos.X, (int)currentPos.Y] += 1; }
                }

                int dir = r.Next(0, 7);

                currentPos += Angle.ToVector(dir * 45 * (float.Pi / 180));

                currentPos = new Vector2(currentPos.X % size.X, currentPos.Y % size.Y);
            }

            for (int x = 0; x < size.X; x++)
            {
                for (int y = 0; y < size.Y; y++)
                {
                    blendVals[x, y] = 0;
                    noiseTex.GetPixel(x, y) = Color.Lerp(bgcol, paintcol, (float)blendVals[x, y] / 255f);
                }
            }

            noiseTex.ApplyChanges();
            return noiseTex;
        }
    }
}
