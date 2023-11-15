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
        ITexture gsc = null;
        int gscl = 7;

        bool started = false;

        int pixSize = 3;

        bool men = false;

        int dir = 45;

        public void Update()
        {
            if (!started)
            {
                if (gsc != null) { gsc.Dispose(); }
                gsc = Graphics.LoadTexture(@"Assets\Sprites\Bask Assets\GreenCar.png");

                men = false;

                started = true;
            }
            else
            {
                ICanvas canv = Graphics.GetOutputCanvas();

                canv.Clear(Color.Black);

                for (int i = 0; i < gscl; i++)
                {
                    //canv.Translate(canv.Width / 2, canv.Height / 2 - i * pixSize);
                    //canv.Rotate(dir * (float.Pi / 180f));

                    canv.DrawTexture(
                        gsc,
                        new Rectangle(
                            new Vector2(i * 16, 0),
                            new Vector2(16, 16),
                            Alignment.TopLeft
                        ),
                        new Rectangle(
                            new Vector2(canv.Width / 2, canv.Height / 2 - i * pixSize),
                            new Vector2(16 * pixSize, 16 * pixSize),
                            Alignment.Center
                        )
                    );
                }

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
            }
        }
    }
}
