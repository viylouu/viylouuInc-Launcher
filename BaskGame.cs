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
using System.Reflection;

namespace viylouuInc_Launcher
{
    internal class BaskGame
    {
        ITexture[] cars = null;
        int[] carLs = null;
        float[] dirs = null;

        ITexture smoke = null;

        bool started = false;

        bool men = false;

        float delta = 0;

        bool debug = false;

        IFont font = null;

        Vector2[] particles = null;
        float[] particleTimeLeft = null;

        Vector2[] map = new Vector2[] { 
            new Vector2(0, 0),
            new Vector2(2, 20),
            new Vector2(18, 48),
            new Vector2(44, 57),
            new Vector2(84, 73),
            new Vector2(126, 80),
            new Vector2(175, 75),
            new Vector2(222, 62),
            new Vector2(240, 42),
            new Vector2(255, -1),
            new Vector2(241, -26),
            new Vector2(216, -46),
            new Vector2(202, -26),
            new Vector2(197, -4),
            new Vector2(190, 4),
            new Vector2(178, 1),
            new Vector2(170, -25),
            new Vector2(146, -43),
            new Vector2(99, -46),
            new Vector2(53, -37),
            new Vector2(22, -24)
        };

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

                smoke = Graphics.LoadTexture(@"Assets\Sprites\Bask Assets\Smoke.png");

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

                Simulation.SetFixedResolution(320, 180, Color.Black, false, false, false);

                makeSmoke(320, 180);

                ICanvas canv = Graphics.GetOutputCanvas();

                started = true;
            }
            else
            {
                ICanvas canv = Graphics.GetOutputCanvas();

                canv.Antialias(false);

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

                for (int i = 0; i < (particles == null? 0 : particles.Length); i++)
                {
                    int roundPartTimeLeft = (int)Math.Round(particleTimeLeft[i] * 4);

                    canv.DrawTexture(
                        smoke,
                        new Rectangle(
                            new Vector2(roundPartTimeLeft == 3 || roundPartTimeLeft == 1 || roundPartTimeLeft == 0 ? 8 : 0, roundPartTimeLeft == 2 || roundPartTimeLeft == 1 || roundPartTimeLeft == 0 ? 8 : 0),
                            new Vector2(8, 8),
                            Alignment.TopLeft
                        ),
                        new Rectangle(
                            new Vector2(particles[i].X, particles[i].Y),
                            new Vector2(8, 8),
                            Alignment.Center
                        )
                    );

                    particles[i] -= new Vector2(0, delta * 16);
                    particleTimeLeft[i] -= delta;

                    if (particleTimeLeft[i] < 0)
                    {
                        if (particles.Length > 1)
                        {
                            Vector2[] oldparticles = particles;
                            float[] oldparticleTimeLeft = particleTimeLeft;

                            particles = new Vector2[oldparticles.Length - 1];
                            particleTimeLeft = new float[oldparticleTimeLeft.Length - 1];

                            for (int i2 = 0; i2 < particles.Length; i2++)
                            {
                                particles[i2] = oldparticles[i2];
                                particleTimeLeft[i2] = oldparticleTimeLeft[i2];
                            }
                        }
                        else
                        {
                            particles = null;
                            particleTimeLeft = null;
                        }
                    }
                }

                canv.Fill(Color.White);
                canv.DrawPolygon(map, true);

                delta = Time.DeltaTime;
            }
        }

        void makeSmoke(float x, float y) 
        {
            if (particles != null)
            {
                Vector2[] oldparticles = particles;
                float[] oldparticleTimeLeft = particleTimeLeft;

                particles = new Vector2[oldparticles.Length + 1];
                particleTimeLeft = new float[oldparticleTimeLeft.Length + 1];

                particles[particles.Length - 1] = new Vector2(x, y);
                particleTimeLeft[particles.Length - 1] = 1;

                for (int i = 0; i < oldparticles.Length; i++)
                {
                    particles[i] = oldparticles[i];
                    particleTimeLeft[i] = oldparticleTimeLeft[i];
                }
            }
            else
            {
                particles = new Vector2[1];
                particleTimeLeft = new float[1];

                particles[0] = new Vector2(x, y);
                particleTimeLeft[0] = 1;
            }
        }
    }
}
