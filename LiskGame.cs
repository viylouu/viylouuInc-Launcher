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

        public int lastClickX = 0;
        public int lastClickY = 0;
        public bool firstClick = true;

        public void Update()
        {
            if (!started)
            {
                maMaSX = Window.Width / pixSize / distBetwLines + 1;
                maMaSY = Window.Height / pixSize / distBetwLines + 1;

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

                        obj.connectorX = 0;
                        obj.connectorY = 0;

                        mapMat[(int)Math.Round(Mouse.Position.X / pixdivdist), (int)Math.Round(Mouse.Position.Y / pixdivdist)] = obj;

                        if (!firstClick)
                        {
                            mapMat[lastClickX, lastClickY].connect = true;

                            mapMat[lastClickX, lastClickY].connectorX = (int)Math.Round(Mouse.Position.X / pixdivdist);
                            mapMat[lastClickX, lastClickY].connectorY = (int)Math.Round(Mouse.Position.Y / pixdivdist);

                            lastClickX = (int)Math.Round(Mouse.Position.X / pixdivdist);
                            lastClickY = (int)Math.Round(Mouse.Position.Y / pixdivdist);
                        }
                        else
                        {
                            firstClick = false;

                            lastClickX = (int)Math.Round(Mouse.Position.X / pixdivdist);
                            lastClickY = (int)Math.Round(Mouse.Position.Y / pixdivdist);
                        }
                    }
                    else if (Mouse.IsButtonPressed(MouseButton.Right))
                    {
                        firstClick = true;

                        lastClickX = 0;
                        lastClickY = 0;
                    }

                    for (int x = 0; x < maMaSX; x++)
                    {
                        for (int y = 0; y < maMaSY; y++)
                        {
                            if (mapMat[x, y] != null)
                            {
                                if (mapMat[x, y].connect)
                                {
                                    canv.Fill(Color.LightGray);

                                    canv.StrokeWidth(pixSize);

                                    canv.DrawLine(x * pixdivdist, y * pixdivdist, mapMat[x, y].connectorX * pixdivdist, mapMat[x, y].connectorY * pixdivdist);
                                }

                                canv.Fill(Color.White);

                                canv.DrawRect(new Vector2(x * pixdivdist, y * pixdivdist), new Vector2(pixSize, pixSize), Alignment.Center);
                            }
                        }
                    }
                }
            }
        }

        public float getAngle(Vector2 p1, Vector2 p2)
        {
            return (float)Math.Atan2(p1.X - p2.X, p1.Y - p2.Y);
        }

        public class wallObj
        {
            public bool connect { get; set; }
            public int connectorAmt { get; set; }
            public int connectorX { get; set; }
            public int connectorY { get; set; }
        }
    }
}
