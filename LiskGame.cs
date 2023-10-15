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
        public bool mapEditor = false;

        public int pixSize = 8;

        public int distBetwLines = 10;

        public wallObj[,] mapMat = new wallObj[1, 1];
        public int maMaSX = 1;
        public int maMaSY = 1;

        public bool started = false;

        public void Update()
        {
            if (!started)
            {
                maMaSX = Window.Width / pixSize / distBetwLines;
                maMaSY = Window.Height / pixSize / distBetwLines;

                mapMat = new wallObj[maMaSX, maMaSY];

                started = true;
            }
            else
            {
                ICanvas canv = Graphics.GetOutputCanvas();

                canv.Clear(new Color(71, 64, 92));

                if (Keyboard.IsKeyPressed(Key.M))
                { mapEditor = !mapEditor; }

                if (mapEditor)
                {
                    canv.Fill(new Color(122, 103, 143));

                    for (int x = 0; x < Window.Width / pixSize / distBetwLines + 1; x++)
                    {
                        canv.DrawRect(new Vector2(x * pixSize * distBetwLines, Window.Height / 2), new Vector2(pixSize, Window.Height), Alignment.Center);
                    }

                    for (int y = 0; y < Window.Height / pixSize / distBetwLines + 1; y++)
                    {
                        canv.DrawRect(new Vector2(Window.Width / 2, y * pixSize * distBetwLines), new Vector2(Window.Width, pixSize), Alignment.Center);
                    }

                    canv.Fill(new Color(255, 255, 255, 255));

                    int pixdivdist = pixSize * distBetwLines;

                    canv.DrawRect(

                        new Vector2(
                                (float)Math.Round(Mouse.Position.X / pixdivdist) * pixdivdist,
                                (float)Math.Round(Mouse.Position.Y / pixdivdist) * pixdivdist
                            ),

                        new Vector2(pixSize, pixSize), Alignment.Center
                    );

                    if (Mouse.IsButtonPressed(MouseButton.Left))
                    {
                        wallObj obj = new wallObj();

                        obj.connect = false;
                        obj.connectorAmt = 0;

                        obj.connectorXs = new int[] { 0 };
                        obj.connectorYs = new int[] { 0 };

                        mapMat[(int)Math.Round(Mouse.Position.X / pixdivdist), (int)Math.Round(Mouse.Position.Y / pixdivdist)] = obj;
                    }

                    for (int x = 0; x < maMaSX; x++)
                    {
                        for (int y = 0; y < maMaSY; y++)
                        {
                            if (mapMat[x, y] != null)
                            { canv.DrawRect(new Vector2(x * pixdivdist, y * pixdivdist), new Vector2(pixSize, pixSize), Alignment.Center); }
                        }
                    }
                }
            }
        }

        public class wallObj
        {
            public bool connect { get; set; }
            public int connectorAmt { get; set; }
            public int[] connectorXs { get; set; }
            public int[] connectorYs { get; set; }
        }
    }
}
