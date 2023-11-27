using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SimulationFramework;
using SimulationFramework.Drawing;
using SimulationFramework.Desktop;

namespace viylouuInc_Launcher
{
    internal class LinkiGame
    {
        bool started = false;

        public Mesh cube = null;

        float near = 0.1f;
        static float fov = 90f;
        float fovRad = 1f / (float)Math.Tan(fov / 2 / 180f * Math.PI);

        Vector3 camP = Vector3.Zero;

        Mesh[] meshes = null;

        Tri[] sortedTris = null;

        public void Update()
        {
            if (!started)
            {
                Simulation.SetFixedResolution(320, 180, Color.Black, false, false, true);

                cube = new Mesh()
                {
                    tris = new Tri[] {
                        new Tri { p1 = new Vector3(0, 0, 0), p2 = new Vector3(1, 0, 0), p3 = new Vector3(1, 1, 0), fill = true, col = new Color(250, 50, 50) }, //FRONT
                        new Tri { p1 = new Vector3(0, 0, 0), p2 = new Vector3(0, 1, 0), p3 = new Vector3(1, 1, 0), fill = true, col = new Color(250, 50, 50) },

                        new Tri { p1 = new Vector3(0, 1, 0), p2 = new Vector3(0, 1, 1), p3 = new Vector3(1, 1, 1), fill = true, col = new Color(200, 100, 50) }, //BOTTOM
                        new Tri { p1 = new Vector3(0, 1, 0), p2 = new Vector3(1, 1, 0), p3 = new Vector3(1, 1, 1), fill = true, col = new Color(200, 100, 50) },

                        new Tri { p1 = new Vector3(0, 0, 1), p2 = new Vector3(1, 0, 1), p3 = new Vector3(1, 1, 1), fill = true, col = new Color(150, 150, 50) }, //BACK
                        new Tri { p1 = new Vector3(0, 0, 1), p2 = new Vector3(0, 1, 1), p3 = new Vector3(1, 1, 1), fill = true, col = new Color(150, 150, 50) },

                        new Tri { p1 = new Vector3(0, 0, 0), p2 = new Vector3(0, 0, 1), p3 = new Vector3(1, 0, 1), fill = true, col = new Color(100, 200, 50) }, //TOP
                        new Tri { p1 = new Vector3(0, 0, 0), p2 = new Vector3(1, 0, 0), p3 = new Vector3(1, 0, 1), fill = true, col = new Color(100, 200, 50) },

                        new Tri { p1 = new Vector3(1, 0, 0), p2 = new Vector3(1, 0, 1), p3 = new Vector3(1, 1, 1), fill = true, col = new Color(50, 250, 50) }, //RIGHT
                        new Tri { p1 = new Vector3(1, 0, 0), p2 = new Vector3(1, 1, 0), p3 = new Vector3(1, 1, 1), fill = true, col = new Color(50, 250, 50) },

                        new Tri { p1 = new Vector3(0, 0, 0), p2 = new Vector3(0, 0, 1), p3 = new Vector3(0, 1, 1), fill = true, col = new Color(50, 200, 100) }, //LEFT
                        new Tri { p1 = new Vector3(0, 0, 0), p2 = new Vector3(0, 1, 0), p3 = new Vector3(0, 1, 1), fill = true, col = new Color(50, 200, 100) },
                    }
                };

                meshes = new Mesh[] { cube };

                started = true;
            }
            else
            {
                ICanvas canv = Graphics.GetOutputCanvas();

                canv.Clear(Color.Black);

                //TODO: fix sorting algorithm

                sortedTris = meshes[0].tris;

                bool trisSorted = false;

                int idx = 0;

                while (!trisSorted)
                {
                    if (idx == sortedTris.Length - 1)
                    { idx = 0; }
                    else
                    {
                        if (
                            (sortedTris[idx].p1.Z + sortedTris[idx].p2.Z + sortedTris[idx].p3.Z) / 3f <
                            (sortedTris[idx + 1].p1.Z + sortedTris[idx + 1].p2.Z + sortedTris[idx + 1].p3.Z) / 3f
                        )
                        {
                            Tri idxp1 = sortedTris[idx+1];

                            sortedTris[idx + 1] = sortedTris[idx];
                            sortedTris[idx] = idxp1;
                        }

                        int sortAmt = 0;

                        for (int i = 0; i < sortedTris.Length; i++)
                        {
                            if (i != sortedTris.Length - 1)
                            {
                                if (
                                    (sortedTris[i].p1.Z + sortedTris[i].p2.Z + sortedTris[i].p3.Z) / 3f >
                                    (sortedTris[i + 1].p1.Z + sortedTris[i + 1].p2.Z + sortedTris[i + 1].p3.Z) / 3f
                                )
                                { sortAmt++; }
                            }
                            else
                            { if (sortedTris[i] == sortedTris.Max()) { sortAmt++; } }
                        }

                        idx++;

                        if (sortAmt == sortedTris.Length - 1)
                        { trisSorted = true; }
                    }
                }

                for (int i = 0; i < sortedTris.Length; i++) //draw
                {
                    drawpolyVec3(
                        sortedTris[i].p1 * 5,
                        sortedTris[i].p2 * 5,
                        sortedTris[i].p3 * 5,

                        sortedTris[i].col,

                        sortedTris[i].fill,

                        canv
                    );
                }
            }
        }

        public class Mesh
        {
            public Tri[] tris { get; set; }
        }

        public class Tri
        {
            public Vector3 p1 { get; set; }
            public Vector3 p2 { get; set; }
            public Vector3 p3 { get; set; }

            public bool fill { get; set; }

            public Color col { get; set; }
        }

        public void drawpolyVec3(Vector3 one, Vector3 two, Vector3 three, Color col, bool fill, ICanvas canv)
        {
            canv.Fill(col);

            canv.DrawPolygon(
                new Vector2[] {
                    new Vector2(
                        one.X / (one.Z + near) * fovRad,
                        one.Y / (one.Z + near) * fovRad
                    ),
                    new Vector2(
                        two.X / (two.Z + near) * fovRad,
                        two.Y / (two.Z + near) * fovRad
                    ),
                    new Vector2(
                        three.X / (three.Z + near) * fovRad,
                        three.Y / (three.Z + near) * fovRad
                    )
                },
                fill
            );
        }
    }
}
