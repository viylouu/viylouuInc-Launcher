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

namespace viylouuInc_Launcher
{
    internal class TinkGame
    {
        public bool started = false;

        public ITexture props = null;

        public int pixSize = 6;

        public bool men = false;

        public void Update()
        {
            if (!started)
            {
                if (props != null) { props.Dispose(); }
                props = Graphics.LoadTexture(@"Assets\Sprites\Tink Assets\Woods-Overworld\Props.png");

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

                canv.Clear(Color.Black);

                //draws test plant
                canv.DrawTexture(
                    props,
                    new Rectangle(
                        new Vector2(
                            0,
                            16
                        ),
                        new Vector2(
                            16,
                            16
                        ),
                        Alignment.TopLeft
                    ),
                    new Rectangle(
                        new Vector2(
                            Window.Width / 2,
                            Window.Height / 2
                        ),
                        new Vector2(
                            16 * pixSize,
                            16 * pixSize
                        ),
                        Alignment.Center
                    )
                );
            }
        }
    }
}
