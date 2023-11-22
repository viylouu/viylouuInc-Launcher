using ImGuiNET;
using SimulationFramework;
using SimulationFramework.Drawing;
using SimulationFramework.Input;
using System.Numerics;

namespace viylouuInc_Launcher
{
    internal class LiskGame
    {
        public bool started = false;

        public bool men = false;

        ITexture toronoiTest = null;

        public Random r = new Random();

        public bool editor = false;

        Vector2 editCam = new Vector2(0, 0);
        float editZoom = 1;

        Vector2 lmp = new Vector2(0, 0);

        public void Update()
        {
            if (!started)
            {
                men = false;
                editor = false;
                if (toronoiTest != null) { toronoiTest.Dispose(); }
                toronoiTest = genToronoi(10000000, new Vector2(512, 512), Color.White, Color.Black);

                started = true;
            }
            else
            {
                if (Keyboard.IsKeyPressed(Key.Esc))
                { men = !men; }

                if (Keyboard.IsKeyPressed(Key.M))
                { editor = !editor; }

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

                if (editor)
                {
                    for (int x = 0; x < 100; x++)
                    {
                        for (int y = 0; y < 100; y++)
                        {
                            canv.Fill(Color.Gray);
                            canv.DrawRect(new Vector2((x*10*4 + editCam.X) * editZoom, (canv.Height - y*10* 4 + editCam.Y) * editZoom), new Vector2(10 * editZoom, 10 * editZoom), Alignment.Center);
                        }
                    }

                    if (Mouse.IsButtonDown(MouseButton.Middle))
                    { editCam += (Mouse.Position - lmp) / editZoom; }

                    if (Keyboard.IsKeyDown(Key.LeftControl))
                    { editZoom += Mouse.ScrollWheelDelta / 50; }
                }

                canv.DrawTexture(toronoiTest, new Vector2(canv.Width / 2, canv.Height / 2), new Vector2(canv.Height / 3, canv.Height / 3), Alignment.Center);
                canv.DrawTexture(toronoiTest, new Vector2(canv.Width / 2, canv.Height / 2 + canv.Height / 3), new Vector2(canv.Height / 3, canv.Height / 3), Alignment.Center);
                canv.DrawTexture(toronoiTest, new Vector2(canv.Width / 2, canv.Height / 2 - canv.Height / 3), new Vector2(canv.Height / 3, canv.Height / 3), Alignment.Center);
                canv.DrawTexture(toronoiTest, new Vector2(canv.Width / 2 - canv.Height / 3, canv.Height / 2), new Vector2(canv.Height / 3, canv.Height / 3), Alignment.Center);
                canv.DrawTexture(toronoiTest, new Vector2(canv.Width / 2 - canv.Height / 3, canv.Height / 2 + canv.Height / 3), new Vector2(canv.Height / 3, canv.Height / 3), Alignment.Center);
                canv.DrawTexture(toronoiTest, new Vector2(canv.Width / 2 - canv.Height / 3, canv.Height / 2 - canv.Height / 3), new Vector2(canv.Height / 3, canv.Height / 3), Alignment.Center);
                canv.DrawTexture(toronoiTest, new Vector2(canv.Width / 2 + canv.Height / 3, canv.Height / 2), new Vector2(canv.Height / 3, canv.Height / 3), Alignment.Center);
                canv.DrawTexture(toronoiTest, new Vector2(canv.Width / 2 + canv.Height / 3, canv.Height / 2 + canv.Height / 3), new Vector2(canv.Height / 3, canv.Height / 3), Alignment.Center);
                canv.DrawTexture(toronoiTest, new Vector2(canv.Width / 2 + canv.Height / 3, canv.Height / 2 - canv.Height / 3), new Vector2(canv.Height / 3, canv.Height / 3), Alignment.Center);

                lmp = Mouse.Position;
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

            int dir = -Program.framesUntilGameStarted;

            for (int i = 0; i < iterations; i++)
            {
                if (currentPos.X >= 0 && currentPos.X <= size.X && currentPos.Y >= 0 && currentPos.Y <= size.Y)
                {
                    if (blendVals[(int)currentPos.X, (int)currentPos.Y] < 255)
                    { blendVals[(int)currentPos.X, (int)currentPos.Y] += 1; }
                }

                dir += r.Next(0, 360);

                currentPos += Angle.ToVector(dir * (float.Pi / 180));

                currentPos = new Vector2(currentPos.X % size.X, currentPos.Y % size.Y);

                if (currentPos.X <= 0) currentPos.X = size.X - 1;

                if (currentPos.Y <= 0) currentPos.Y = size.Y - 1;
            }

            for (int x = 0; x < size.X; x++)
            {
                for (int y = 0; y < size.Y; y++)
                {
                    noiseTex.GetPixel(x, y) = Color.Lerp(bgcol, paintcol, (float)blendVals[x, y] / 255f);
                }
            }

            noiseTex.ApplyChanges();
            return noiseTex;
        }
    }
}
