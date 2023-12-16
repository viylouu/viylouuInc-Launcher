using SimulationFramework;
using SimulationFramework.Drawing;
using SimulationFramework.Input;
using System.ComponentModel.Design;
using System.Numerics;

namespace viylouuInc_Launcher
{
    internal class VelocGame
    {
        //floor tiles 1
        static tmm.tile f1tc = new tmm.tile()
        { ssSource = null, sssp = new Vector2(8, 0), ssss = new Vector2(8, 8), solid = true, sssid = 0 };

        static tmm.tile f1tl = new tmm.tile()
        { ssSource = null, sssp = new Vector2(0, 0), ssss = new Vector2(8, 8), solid = true, sssid = 0 };

        static tmm.tile f1tr = new tmm.tile()
        { ssSource = null, sssp = new Vector2(16, 0), ssss = new Vector2(8, 8), solid = true, sssid = 0 };

        static tmm.tile f1mc = new tmm.tile()
        { ssSource = null, sssp = new Vector2(8, 8), ssss = new Vector2(8, 8), solid = true, sssid = 0 };

        static tmm.tile f1ml = new tmm.tile()
        { ssSource = null, sssp = new Vector2(0, 8), ssss = new Vector2(8, 8), solid = true, sssid = 0 };

        static tmm.tile f1mr = new tmm.tile()
        { ssSource = null, sssp = new Vector2(16, 8), ssss = new Vector2(8, 8), solid = true, sssid = 0 };

        static tmm.tile f1bc = new tmm.tile()
        { ssSource = null, sssp = new Vector2(8, 16), ssss = new Vector2(8, 8), solid = true, sssid = 0 };

        static tmm.tile f1bl = new tmm.tile()
        { ssSource = null, sssp = new Vector2(0, 16), ssss = new Vector2(8, 8), solid = true, sssid = 0 };

        static tmm.tile f1br = new tmm.tile()
        { ssSource = null, sssp = new Vector2(16, 16), ssss = new Vector2(8, 8), solid = true, sssid = 0 };

        //wall tiles 1
        static tmm.tile w1tc = new tmm.tile()
        { ssSource = null, sssp = new Vector2(8 + 56, 0), ssss = new Vector2(8, 8), solid = false, sssid = 0 };

        static tmm.tile w1tl = new tmm.tile()
        { ssSource = null, sssp = new Vector2(0 + 56, 0), ssss = new Vector2(8, 8), solid = false, sssid = 0 };

        static tmm.tile w1tr = new tmm.tile()
        { ssSource = null, sssp = new Vector2(16 + 56, 0), ssss = new Vector2(8, 8), solid = false, sssid = 0 };

        static tmm.tile w1mc = new tmm.tile()
        { ssSource = null, sssp = new Vector2(8 + 56, 8), ssss = new Vector2(8, 8), solid = false, sssid = 0 };

        static tmm.tile w1ml = new tmm.tile()
        { ssSource = null, sssp = new Vector2(0 + 56, 8), ssss = new Vector2(8, 8), solid = false, sssid = 0 };

        static tmm.tile w1mr = new tmm.tile()
        { ssSource = null, sssp = new Vector2(16 + 56, 8), ssss = new Vector2(8, 8), solid = false, sssid = 0 };

        static tmm.tile w1bc = new tmm.tile()
        { ssSource = null, sssp = new Vector2(8 + 56, 16), ssss = new Vector2(8, 8), solid = false, sssid = 0 };

        static tmm.tile w1bl = new tmm.tile()
        { ssSource = null, sssp = new Vector2(0 + 56, 16), ssss = new Vector2(8, 8), solid = false, sssid = 0 };

        static tmm.tile w1br = new tmm.tile()
        { ssSource = null, sssp = new Vector2(16 + 56, 16), ssss = new Vector2(8, 8), solid = false, sssid = 0 };

        /*
         * info on the naming of tiles
         * 
         * the first letter is the type of tile
         * 
         * f being floor
         * w being wall
         * 
         * the second letter is the variation of the tile
         * see the "tiles.png" file in the veloc assets file
         * 
         * the third letter is the vertical orientation of the tile
         * 
         * t being top
         * m being middle
         * b being bottom
         * 
         * then the fourth letter is the horizontal orientation of the tile
         * 
         * l being left
         * c being center
         * r being right
         * 
         * that is the naming info so the names of tiles arent too long
        */

        static tmm.tile[,] mat = new tmm.tile[,]
        {
            { w1tl, w1tc, w1tc, w1tc, w1tr },
            { w1ml, f1tl, f1tc, f1tr, w1mr },
            { w1ml, f1ml, f1mc, f1mr, w1mr },
            { w1ml, f1bl, f1bc, f1br, w1mr },
            { w1bl, w1bc, w1bc, w1bc, w1br }
        };

        tmm.tilemap tm = new tmm.tilemap() 
        { matrix = mat };

        ITexture tss = null; //tileset spritesheet
        ITexture ps = null; //player sprite

        bool started = false;

        Vector2 camp = Vector2.Zero; //camera pos

        Vector2 pp = new Vector2(16, -8); //player pos
        Vector2 pv = Vector2.Zero; //player vel

        float pw = 0.4f;

        public void Update()
        {
            if (!started) 
            {
                Simulation.SetFixedResolution(320, 180, Color.Black, false, false, false);

                tss = Graphics.LoadTexture(@"Assets\Sprites\Veloc Assets\tiles.png");

                ps = Graphics.LoadTexture(@"Assets\Sprites\Veloc Assets\char.png");

                started = true;
            }
            else
            {
                ICanvas canv = Graphics.GetOutputCanvas();

                canv.Clear(new Color(24, 20, 37));

                //player physics
                pv += new Vector2(0, pw) * (Time.DeltaTime * 30);

                pp += pv * (Time.DeltaTime * 30);

                for (int x = 0; x < tm.matrix.GetLength(0); x++)
                {
                    for (int y = 0; y < tm.matrix.GetLength(1); y++)
                    {
                        if (tm.matrix[x, y] != null)
                        {
                            bool xTouch = false;
                            bool collision = pTouch(x, y, ref xTouch);

                            if (tm.matrix[x, y].solid && collision)
                            {
                                // Adjust Y velocity for gravity
                                pv.Y = 0; // Set to 0 to stop falling (or adjust by your gravity value)

                                // Adjust X velocity for collisions on the X axis
                                if (xTouch)
                                {
                                    pv.X = 0; // Set to 0 or adjust based on your desired behavior
                                }
                            }
                        }
                    }
                }

                camp += (pp - new Vector2(canv.Width /2, canv.Height /2) - camp) / (5 / (Time.DeltaTime * 30));

                for (int x = 0; x < tm.matrix.GetLength(0); x++)
                { 
                    for (int y = 0; y < tm.matrix.GetLength(1); y++) 
                    {
                        if (tm.matrix[x, y] != null)
                        {
                            //draw tile
                            canv.DrawTexture(
                                tss,
                                new Rectangle(
                                    tm.matrix[x, y].sssp,
                                    tm.matrix[x, y].ssss,
                                    Alignment.TopLeft
                                ),
                                new Rectangle(
                                    new Vector2(y * tm.matrix[x, y].ssss.X - (int)camp.X, x * tm.matrix[x, y].ssss.Y - (int)camp.Y),
                                    tm.matrix[x, y].ssss,
                                    Alignment.Center
                                )
                            );
                        }
                    } 
                }

                canv.DrawTexture(
                    ps,
                    new Rectangle(
                        new Vector2((int)pp.X - (int)camp.X, (int)pp.Y - (int)camp.Y),
                        Vector2.One * 8,
                        Alignment.Center
                    )
                );

                canv.Fill(Color.White);
                canv.DrawText(pp.X + " " + pp.Y, Vector2.One * 5);
            }
        }

        bool pTouch(int tx, int ty, ref bool xt)
        {
            float playerLeft = pp.X - 8;
            float playerRight = pp.X + 8;
            float playerTop = pp.Y - 8;
            float playerBottom = pp.Y + 8;

            float tileLeft = tx;
            float tileRight = tx + 8;
            float tileTop = ty;
            float tileBottom = ty + 8;

            // Check for no collision
            if (playerRight < tileLeft || playerLeft > tileRight || playerBottom < tileTop || playerTop > tileBottom)
            {
                return false;
            }

            // Collision occurred
            return true;
        }
    }
}
