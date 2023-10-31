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

        public void Update()
        {
            if (!started)
            {
                started = true;
            }
            else
            {
                ICanvas canv = Graphics.GetOutputCanvas();

                canv.Clear(new Color(71, 64, 92));
            }
        }

        public float getAngle(Vector2 p1, Vector2 p2)
        {
            return (float)Math.Atan2(p1.X - p2.X, p1.Y - p2.Y);
        }
    }
}
