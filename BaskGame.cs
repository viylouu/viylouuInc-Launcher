using SimulationFramework;
using SimulationFramework.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using SimulationFramework.Input;
using ImGuiNET;

namespace viylouuInc_Launcher
{
    internal class BaskGame
    {
        ITexture[] cars = null;
        int[] carLs = null;
        float[] dirs = null;

        bool started = false;

        bool men = false;

        float delta = 0;

        bool debug = false;

        IFont font = null;

        public void Update()
        {
            if (!started)
            {
                if (cars != null) { for (int i = 0; i < cars.Length; i++) { cars[i].Dispose(); } }

                cars = new ITexture[] 
                {
                    Graphics.LoadTexture(@"Assets\Sprites\Bask Assets\BlueCar.png"),
                    Graphics.LoadTexture(@"Assets\Sprites\Bask Assets\PurpleCar.png"),
                    Graphics.LoadTexture(@"Assets\Sprites\Bask Assets\GreenCar.png"),
                    Graphics.LoadTexture(@"Assets\Sprites\Bask Assets\GreenBigCar.png"),
                    Graphics.LoadTexture(@"Assets\Sprites\Bask Assets\RedCar.png"),
                    Graphics.LoadTexture(@"Assets\Sprites\Bask Assets\YellowCar.png"),
                    Graphics.LoadTexture(@"Assets\Sprites\Bask Assets\BrownMotorcycle.png"),
                    Graphics.LoadTexture(@"Assets\Sprites\Bask Assets\RedMotorcycle.png"),
                    Graphics.LoadTexture(@"Assets\Sprites\Bask Assets\WhiteMotorcycle.png")
                };

                carLs = new int[]
                { 9, 8, 7, 10, 8, 12, 10, 10, 10 };

                dirs = new float[]
                { 
                    0, 
                    360 / cars.Length, 
                    360 / cars.Length * 2, 
                    360 / cars.Length * 3, 
                    360 / cars.Length * 4, 
                    360 / cars.Length * 5, 
                    360 / cars.Length * 6, 
                    360 / cars.Length * 7, 
                    360 / cars.Length * 8 
                };

                men = false;

                font = Graphics.LoadFont(@"Assets\Fonts\MatchupPro.ttf");

                Simulation.SetFixedResolution(320, 180, Color.Black, false, false, true);

                started = true;
            }
            else
            {
                ICanvas canv = Graphics.GetOutputCanvas();

                canv.Clear(Color.Black);

                for (int i2 = 0; i2 < cars.Length; i2++)
                {
                    for (int i = 0; i < carLs[i2]; i++)
                    {
                        canv.Translate(canv.Width / 2 + (i2 * 18 - ((cars.Length - 1) * 18 / 2)), canv.Height / 2 - i);
                        canv.Rotate((dirs[i2] - 90) * (float.Pi / 180f));

                        canv.DrawTexture(
                            cars[i2],
                            new Rectangle(
                                new Vector2(i * 16, 0),
                                new Vector2(16, 16),
                                Alignment.TopLeft
                            ),
                            new Rectangle(
                                new Vector2(0, 0),
                                new Vector2(16, 16),
                                Alignment.Center
                            )
                        );

                        canv.ResetState();
                    }

                    dirs[i2] += delta * 90;
                    dirs[i2] = dirs[i2] % 360;
                }

                if (Keyboard.IsKeyPressed(Key.Esc))
                { men = !men; }

                if (Keyboard.IsKeyPressed(Key.Tab))
                { debug = !debug; }

                canv.Font(font);
                canv.DrawText("Cars: " + cars.Length, 1, 1, Alignment.TopLeft);

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

                if (debug)
                {
                    ImGui.Begin("Debug");

                    for (int i = 0; i < cars.Length; i++)
                    {
                        ImGui.Text("Dir ( " + dirs[i] + " )");
                    }

                    ImGui.End();
                }

                delta = Time.DeltaTime;
            }
        }
    }
}
