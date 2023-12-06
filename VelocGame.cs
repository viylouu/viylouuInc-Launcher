using SimulationFramework;
using SimulationFramework.Drawing;
using System.Numerics;

namespace viylouuInc_Launcher
{
    internal class VelocGame
    {
        static tmm.tile f1tc = new tmm.tile()
        {
            ssSource = @"Assets\Sprites\Veloc Assets\tiles.png",
            sssp = new Vector2(8, 0),
            ssss = new Vector2(8, 8),
            solid = true,
            sssid = 1
        };

        static tmm.tile f1tl = new tmm.tile()
        {
            ssSource = @"Assets\Sprites\Veloc Assets\tiles.png",
            sssp = new Vector2(0, 0),
            ssss = new Vector2(8, 8),
            solid = true,
            sssid = 1
        };

        static tmm.tile f1tr = new tmm.tile()
        {
            ssSource = @"Assets\Sprites\Veloc Assets\tiles.png",
            sssp = new Vector2(16, 0),
            ssss = new Vector2(8, 8),
            solid = true,
            sssid = 1
        };

        static tmm.tile f1mc = new tmm.tile()
        {
            ssSource = @"Assets\Sprites\Veloc Assets\tiles.png",
            sssp = new Vector2(8, 8),
            ssss = new Vector2(8, 8),
            solid = true,
            sssid = 1
        };

        static tmm.tile f1ml = new tmm.tile()
        {
            ssSource = @"Assets\Sprites\Veloc Assets\tiles.png",
            sssp = new Vector2(0, 8),
            ssss = new Vector2(8, 8),
            solid = true,
            sssid = 1
        };

        static tmm.tile f1mr = new tmm.tile()
        {
            ssSource = @"Assets\Sprites\Veloc Assets\tiles.png",
            sssp = new Vector2(16, 8),
            ssss = new Vector2(8, 8),
            solid = true,
            sssid = 1
        };

        static tmm.tile f1bc = new tmm.tile()
        {
            ssSource = @"Assets\Sprites\Veloc Assets\tiles.png",
            sssp = new Vector2(8, 16),
            ssss = new Vector2(8, 8),
            solid = true,
            sssid = 1
        };

        static tmm.tile f1bl = new tmm.tile()
        {
            ssSource = @"Assets\Sprites\Veloc Assets\tiles.png",
            sssp = new Vector2(0, 16),
            ssss = new Vector2(8, 8),
            solid = true,
            sssid = 1
        };

        static tmm.tile f1br = new tmm.tile()
        {
            ssSource = @"Assets\Sprites\Veloc Assets\tiles.png",
            sssp = new Vector2(16, 16),
            ssss = new Vector2(8, 8),
            solid = true,
            sssid = 1
        };

        static tmm.tile notl = tmm.noTile;

        static tmm.tile[] tiles = new tmm.tile[] //must have only tiles with new spritesheets
        { 
            notl, 
            f1tc
        };

        static tmm.tile[,] mat = new tmm.tile[,]
        {
            { notl, notl, notl, notl, notl },
            { notl, f1tl, f1tc, f1tr, notl },
            { notl, f1ml, f1mc, f1mr, notl },
            { notl, f1bl, f1bc, f1br, notl },
            { notl, notl, notl, notl, notl }
        };

        tmm.tilemap tm = new tmm.tilemap() 
        { matrix = mat };

        ITexture[] tmts = new ITexture[tiles.Length];

        bool started = false;

        public void Update()
        {
            if (!started) 
            {
                Simulation.SetFixedResolution(320, 180, Color.Black, false, false, false);

                for (int i = 0; i < tiles.Length; i++)
                { tmts[i] = Graphics.LoadTexture(tiles[i].ssSource); }

                started = true;
            }
            else
            {
                ICanvas canv = Graphics.GetOutputCanvas();

                canv.Clear(new Color(24, 20, 37));

                for (int x = 0; x < tm.matrix.GetLength(0); x++)
                { 
                    for (int y = 0; y < tm.matrix.GetLength(1); y++) 
                    {
                        canv.DrawTexture(
                            tmts[tm.matrix[x, y].sssid],
                            new Rectangle(
                                tm.matrix[x, y].sssp,
                                tm.matrix[x, y].ssss,
                                Alignment.TopLeft
                            ),
                            new Rectangle(
                                new Vector2(y * tm.matrix[x, y].ssss.X, x * tm.matrix[x, y].ssss.Y),
                                tm.matrix[x, y].ssss,
                                Alignment.Center
                            )
                        );
                    } 
                }
            }
        }
    }
}
