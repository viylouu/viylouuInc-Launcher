using SimulationFramework;
using SimulationFramework.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using System.ComponentModel;
using System.Net;
using System.Xml.Linq;
using System.Diagnostics;
using Silk.NET.Input.Extensions;

namespace viylouuInc_Launcher
{
    internal class LifeGame
    {
        bool started = false;

        FastNoiseLite noise = new FastNoiseLite();

        bool simstarted = false;

        Random r = new Random();

        Vector2[] anPs = null;

        int anAmt = 100;

        int worldSize = 100;

        int islAmt = 5;

        float[,] map = null;

        ITexture gradiant = null;

        Color water = new Color(76, 132, 217);
        Color sand = new Color(240, 181, 117);
        Color grass = new Color(77, 184, 84);
        Color snow = new Color(247, 251, 255);

        int sandHeight = 25;
        int grassHeight = 120;
        int snowHeight = 210;

        ITexture basemap = null;

        public void Update()
        {
            if (!started) {
                Simulation.SetFixedResolution(1920, 1080, Color.Black, false, false, false);

                started = true;
            }
            else {
                ICanvas canv = Graphics.GetOutputCanvas();

                canv.Clear(water);

                if (!simstarted) {
                    ImGui.Begin("Simulation Settings");

                    ImGui.SetWindowPos(Vector2.Zero);
                    ImGui.SetWindowSize(new Vector2(Window.Width, Window.Height));

                    ImGui.SliderInt("Animal Amount", ref anAmt, 2, 10000);

                    ImGui.SliderInt("World Size", ref worldSize, 16, 8192);

                    ImGui.SliderInt("Island Mult", ref islAmt, 1, 50);

                    if (ImGui.Button("Start")) {
                        simstarted = true;

                        GenWorld(worldSize, 5, r.Next(0, 10000), islAmt);
                    }

                    ImGui.End();

                    return;
                }

                canv.DrawTexture(basemap, new Vector2(canv.Width / 2, canv.Height / 2), Vector2.One * canv.Height, Alignment.Center);
            }
        }

        void GenWorld(int size, float scale, int seed, float islandAmt) {
            map = new float[size, size];
            noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            noise.SetSeed(seed);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            basemap = Graphics.CreateTexture(size, size);

            for (int x = 0; x < size; x++) {
                for (int y = 0; y < size; y++) {
                    map[x, y] = noise.GetNoise(x * scale, y * scale);
                    map[x, y] *= 255f;
                    map[x, y] *= islandAmt;
                    map[x, y] = (float)Math.Floor(Math.Clamp(map[x, y], 0, 255));
                    map[x, y] /= 255f;
                    map[x, y] *= ((size / 2f) - dist(x, size / 2f, y, size / 2f)) / (size / 2f);

                    if (map[x, y] >= sandHeight / 255f)
                    {
                        if (map[x, y] >= sandHeight / 255f && map[x, y] < grassHeight / 255f)
                        {
                            basemap.GetPixel(x, y) = sand;
                        }
                        else if (map[x, y] >= grassHeight / 255f && map[x, y] < snowHeight / 255f)
                        {
                            basemap.GetPixel(x, y) = grass;
                        }
                        else if (map[x, y] >= snowHeight / 255f)
                        {
                            basemap.GetPixel(x, y) = snow;
                        }
                    }
                }
            }

            basemap.ApplyChanges();
        }

        float sqr(float inp) {
            return inp * inp;
        }

        float dist(float x1, float x2, float y1, float y2) {
            return (float)Math.Sqrt(sqr(x2 - x1) + sqr(y2 - y1));
        }
    }
}
