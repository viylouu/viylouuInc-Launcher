using SimulationFramework.Drawing;
using SimulationFramework;
using SimulationFramework.Input;
using System.Numerics;
using ImGuiNET;
using Silk.NET.OpenGL;
using NAudio.Wave;

namespace viylouuInc_Launcher
{
    internal class CellsGame
    {
        public int stPixSize = 6;
        public int pixSize = 6;

        public int drawRad = 1;

        public bool recentTexUpd = false;

        public bool set = false;

        public bool debugDrawRad = false;

        public bool debugMode = false;

        public bool fixedUpdate = true;

        public Cell sand = new Cell {
            name = "Sand",
            cols = new Color[] 
            { 
                new Color(250, 197, 105), 
                new Color(209, 115, 44)
            },
            simpCol = new Color(255, 196, 117),
            erase = false,
            move = true,
            moveChance = 90.00f,
            stack = false,
            liquid = false
        };

        public Cell water = new Cell
        {
            name = "Water",
            cols = new Color[]
            {
                new Color(70, 169, 240),
                new Color(38, 103, 209)
            },
            simpCol = new Color(102, 141, 189),
            erase = false,
            move = true,
            moveChance = 98.55f,
            stack = false,
            liquid = true
        };

        public Cell dirt = new Cell
        {
            name = "Dirt",
            cols = new Color[] 
            { 
                new Color(87, 63, 38), 
                new Color(48, 28, 16)
            },
            simpCol = new Color(82, 54, 38),
            erase = false,
            move = true,
            moveChance = 95.85f,
            stack = true,
            liquid = false
        };

        public Cell stone = new Cell
        {
            name = "Stone",
            cols = new Color[] 
            { 
                new Color(86, 86, 87), 
                new Color(43, 43, 43)
            },
            simpCol = new Color(99, 99, 99),
            erase = false,
            move = false,
            moveChance = 0f,
            stack = false,
            liquid = false
        };

        public Cell eraser = new Cell
        {
            name = "Eraser",
            cols = new Color[] 
            { new Color(0, 0, 0) },
            simpCol = new Color(0, 0, 0),
            erase = true,
            move = false,
            moveChance = 0,
            stack = false,
            liquid = false
        };

        public Cell[] cells = null;

        public Cell[,] matrix = null;
        public bool[,] updMatrix = null;

        public int mSX = 0;
        public int mSY = 0;

        public int cellSel = 0;

        public bool started = true;

        public double accum = 0;
        public double fps = 1f / 60;
        public int changeFPS = 60;
        public int stChangeFPS = 60;
        public DateTime lastItTime = DateTime.UtcNow;

        public Random r = new Random();

        public bool strted = false;

        public ITexture tex = null;

        Vector2 drawPos = new Vector2(0, 0);

        public bool simpleGraphics = false;

        public void Update()
        {
            if (!strted)
            {
                mSX = (int)Math.Round((double)Window.Width / pixSize);
                mSY = (int)Math.Round((double)Window.Height / pixSize);

                updMatrix = new bool[mSX, mSY];
                matrix = new Cell[mSX, mSY];

                lastItTime = DateTime.UtcNow;

                tex = Graphics.CreateTexture(mSX, mSY, TextureOptions.None);

                recentTexUpd = true;

                cells = new Cell[] {
                    sand,
                    water,
                    dirt,
                    stone,
                    eraser
                };

                strted = true;
            }
            else
            {
                ICanvas canv = Graphics.GetOutputCanvas();

                canv.Clear(new Color(32, 32, 33));

                if (fixedUpdate)
                {
                    FixUpdStarter(FixUpd, (DateTime.UtcNow - lastItTime).TotalSeconds);
                }
                else { processCells(); }
                lastItTime = DateTime.UtcNow;

                if (Mouse.IsButtonReleased(MouseButton.Left) && started)
                { started = false; }

                if (pixSize == stPixSize? (mSX != (int)Math.Round((double)Window.Width / pixSize) || mSY != (int)Math.Round((double)Window.Height / pixSize)) : true)
                {
                    if (stPixSize != pixSize)
                    {
                        stPixSize = pixSize;

                        mSX = (int)Math.Round((double)Window.Width / pixSize);
                        mSY = (int)Math.Round((double)Window.Height / pixSize);

                        matrix = new Cell[mSX, mSY];
                        updMatrix = new bool[mSX, mSY];

                        if (tex != null) { tex.Dispose(); }
                        tex = Graphics.CreateTexture(mSX, mSY, TextureOptions.None);

                        recentTexUpd = true;
                    }
                    else
                    {
                        int oldGridSX = mSX;
                        int oldGridSY = mSY;
                        Cell[,] oldGrid = matrix;

                        mSX = (int)Math.Round((double)Window.Width / pixSize);
                        mSY = (int)Math.Round((double)Window.Height / pixSize);

                        matrix = new Cell[mSX, mSY];
                        updMatrix = new bool[mSX, mSY];

                        if (tex != null) { tex.Dispose(); }
                        tex = Graphics.CreateTexture(mSX, mSY, TextureOptions.None);

                        recentTexUpd = true;

                        for (int x = 0; x < mSX; x++)
                        {
                            for (int y = 0; y < mSY; y++)
                            {
                                if (x < oldGridSX && y < oldGridSY)
                                {
                                    matrix[x, y] = oldGrid[x, y];
                                }
                            }
                        }
                    }
                }

                if (Mouse.IsButtonDown(MouseButton.Left) && !started)
                {
                    int dx = (int)Math.Round(drawPos.X / pixSize);
                    int dy = (int)Math.Round((Window.Height - drawPos.Y) / pixSize);

                    if (
                        dx >= 0 &&
                        dx < mSX &&
                        dy / pixSize >= 0 &&
                        dy < mSY
                    )
                    {
                        if (cells[cellSel].erase)
                        {
                            if (drawRad > 1)
                            {
                                for (int x = -drawRad / 2; x < drawRad / 2; x++)
                                {
                                    for (int y = -drawRad / 2; y < drawRad / 2; y++)
                                    {
                                        if (
                                            dx + x >= 0 &&
                                            dx + x < mSX &&
                                            dy + y >= 0 &&
                                            dy + y < mSY
                                        )
                                        { 
                                            if (matrix[dx + x, dy + y] != null) 
                                            { 
                                                matrix[dx + x, dy + y] = null; 
                                                tex.GetPixel(dx + x, dy + y) = new Color(0, 0, 0, 0); 
                                            } 
                                        }
                                    }
                                }
                            }
                            else
                            { 
                                if (matrix[dx, dy] != null) 
                                { 
                                    matrix[dx, dy] = null;
                                    tex.GetPixel(dx, dy) = new Color(0, 0, 0, 0); 
                                } 
                            }
                        }
                        else
                        {
                            if (drawRad > 1)
                            {
                                for (int x = -drawRad / 2; x < drawRad / 2; x++)
                                {
                                    for (int y = -drawRad / 2; y < drawRad / 2; y++)
                                    {
                                        if (
                                            dx + x >= 0 &&
                                            dx + x < mSX &&
                                            dy + y >= 0 &&
                                            dy + y < mSY
                                        )
                                        { 
                                            if (matrix[dx + x, dy + y] == null) 
                                            { 
                                                Cell c = cloneProps(cells[cellSel]); 
                                                c.color = Color.Lerp(c.cols[0], c.cols[1], r.Next(0, 1000) / 1000f);

                                                matrix[dx + x, dy + y] = c; 
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            { 
                                if (matrix[dx, dy] == null) 
                                { 
                                    Cell c = cloneProps(cells[cellSel]); 
                                    c.color = Color.Lerp(c.cols[0], c.cols[1], r.Next(0, 1000) / 1000f);
                                    
                                    matrix[dx, dy] = c; 
                                } 
                            }
                        }
                    }

                    recentTexUpd = true;
                }

                canv.Scale(1, -1);
                canv.DrawTexture(tex, Vector2.Zero, new Vector2(mSX * pixSize, mSY * pixSize), Alignment.BottomLeft);
                canv.ResetState();

                if (Keyboard.IsKeyDown(Key.LeftControl))
                {
                    if (simpleGraphics)
                    { canv.Fill(new Color(110, 255, 110, 255)); }
                    else
                    { canv.Fill(new Color(110, 255, 110, 100)); }

                    canv.DrawRect(
                        new Vector2(
                            (int)Math.Round(drawPos.X / pixSize) * pixSize - (float)Math.Round((float)drawRad * pixSize / 2), 
                            (int)Math.Round(drawPos.Y / pixSize) * pixSize - (float)Math.Round((float)drawRad * pixSize / 2)
                        ), 
                        new Vector2((float)Math.Round((float)drawRad / 2 - .1f) * 2 * pixSize, (float)Math.Round((float)drawRad / 2 - .1f) * 2 * pixSize)
                    );

                    drawRad += (int)Mouse.ScrollWheelDelta;
                    if (drawRad <= 0)
                    { drawRad = 1; }
                }
                else
                {
                    cellSel += (int)Mouse.ScrollWheelDelta;
                    cellSel = cellSel % cells.Length;
                    if (cellSel < 0)
                    { cellSel = cells.Length - 1; }
                }

                if(Mouse.IsButtonDown(MouseButton.Left) && simpleGraphics)
                { 
                    canv.Fill(new Color(110, 255, 110, 255));

                    canv.DrawRect(
                        new Vector2(
                            (int)Math.Round(drawPos.X / pixSize) * pixSize - (float)Math.Round((float)drawRad * pixSize / 2),
                            (int)Math.Round(drawPos.Y / pixSize) * pixSize - (float)Math.Round((float)drawRad * pixSize / 2)
                        ),
                        new Vector2((float)Math.Round((float)drawRad / 2 - .1f) * 2 * pixSize, (float)Math.Round((float)drawRad / 2 - .1f) * 2 * pixSize)
                    );
                }

                canv.Fill(Color.White);
                canv.DrawText(Math.Round(1 / Time.DeltaTime) + " FPS", new Vector2(5, 5), Alignment.TopLeft);
                canv.DrawText(cellSel < cells.Length ? cells[cellSel].name : "null error", new Vector2(Window.Width - 5, 5), Alignment.TopRight);

                if (debugMode && debugDrawRad)
                {
                    canv.DrawText("<" + drawRad + "> drawSize", new Vector2(5, 30), Alignment.TopLeft);
                }

                if (Keyboard.IsKeyPressed(Key.Esc))
                {
                    set = !set;
                }

                if (set)
                {
                    ImGui.Begin("Settings");

                    ImGui.SliderInt("Pixel Size", ref pixSize, 1, 10);

                    ImGui.Checkbox("Debug Mode", ref debugMode);

                    bool storedSimple = simpleGraphics;

                    ImGui.Checkbox("Simple Mode", ref simpleGraphics);

                    if (storedSimple != simpleGraphics)
                    {
                        recentTexUpd = true;
                    }

                    ImGui.SliderInt("Tick Speed", ref changeFPS, 30, 360);

                    if (stChangeFPS != changeFPS)
                    { 
                        stChangeFPS = changeFPS;
                        fps = 1f / changeFPS;
                    }

                    ImGui.Checkbox("Fixed Update", ref fixedUpdate);

                    if (ImGui.Button("Clear"))
                    {
                        mSX = (int)Math.Round((double)Window.Width / pixSize);
                        mSY = (int)Math.Round((double)Window.Height / pixSize);

                        matrix = new Cell[mSX, mSY];
                        updMatrix = new bool[mSX, mSY];

                        if (tex != null) { tex.Dispose(); }
                        tex = Graphics.CreateTexture(mSX, mSY, TextureOptions.None);

                        recentTexUpd = true;
                    }

                    ImGui.End();

                    if (debugMode)
                    {
                        ImGui.Begin("Debug Menu");

                        ImGui.Checkbox("Show Draw Size", ref debugDrawRad);

                        ImGui.End();
                    }
                }
            }
        }

        public void FixUpdStarter(Action fix, double delta)
        {
            accum += delta;

            while (accum >= fps)
            {
                fix.Invoke();
                accum -= fps;
            }
        }

        public void FixUpd()
        { processCells(); }

        public void processCells()
        {
            updMatrix = new bool[mSX, mSY];

            bool texUpdated = false;

            if (Mouse.IsButtonDown(MouseButton.Left))
            { drawPos += (Mouse.Position - drawPos) / 5; }
            else
            { drawPos = Mouse.Position; }

            for (int x = 0; x < mSX; x++)
            {
                for (int y = 0; y < mSY; y++)
                {
                    if (matrix[x, y] != null && !updMatrix[x, y])
                    {
                        if (matrix[x, y].move)
                        {
                            if (!matrix[x, y].nFreeFall)
                            {
                                bool moved = false;

                                bool move = r.Next(0, 10000) < matrix[x, y].moveChance * 100;

                                Vector2 moveDir = new Vector2(0, 0);
                                bool swapped = false;

                                if (move)
                                {
                                    if (y > 0)
                                    {
                                        if (matrix[x, y - 1] == null)
                                        {
                                            //make these into a function or something
                                            Cell c = matrix[x, y];
                                            matrix[x, y - 1] = c;
                                            matrix[x, y] = null;
                                            updMatrix[x, y - 1] = true;
                                            moved = true;
                                            moveDir = new Vector2(0, -1);
                                        }
                                    }

                                    if (!moved)
                                    {
                                        if (!matrix[x, y].stack)
                                        {
                                            if (y > 0 && x > 0)
                                            {
                                                if (matrix[x - 1, y - 1] == null && matrix[x - 1, y] == null)
                                                {
                                                    Cell c = matrix[x, y];
                                                    matrix[x - 1, y - 1] = c;
                                                    matrix[x, y] = null;
                                                    updMatrix[x - 1, y - 1] = true;
                                                    moved = true;
                                                    moveDir = new Vector2(-1, -1);
                                                }
                                            }

                                            if (!moved)
                                            {
                                                if (y > 0 && x < mSX - 1)
                                                {
                                                    if (matrix[x + 1, y - 1] == null && matrix[x + 1, y] == null)
                                                    {
                                                        Cell c = matrix[x, y];
                                                        matrix[x + 1, y - 1] = c;
                                                        matrix[x, y] = null;
                                                        updMatrix[x + 1, y - 1] = true;
                                                        moved = true;
                                                        moveDir = new Vector2(1, -1);
                                                    }
                                                }
                                            }
                                        }

                                        if (!moved)
                                        {
                                            if (matrix[x, y].liquid)
                                            {
                                                if (x > 0)
                                                {
                                                    if (matrix[x - 1, y] == null)
                                                    {
                                                        Cell c = matrix[x, y];
                                                        matrix[x - 1, y] = c;
                                                        matrix[x, y] = null;
                                                        updMatrix[x - 1, y] = true;
                                                        moved = true;
                                                        moveDir = new Vector2(-1, 0);
                                                    }
                                                }

                                                if (!moved)
                                                {
                                                    if (x < mSX - 1)
                                                    {
                                                        if (matrix[x + 1, y] == null)
                                                        {
                                                            Cell c = matrix[x, y];
                                                            matrix[x + 1, y] = c;
                                                            matrix[x, y] = null;
                                                            updMatrix[x + 1, y] = true;
                                                            moved = true;
                                                            moveDir = new Vector2(1, 0);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (y > 0)
                                                {
                                                    if (matrix[x, y - 1] != null)
                                                    {
                                                        if (matrix[x, y - 1].liquid)
                                                        {
                                                            Cell c = matrix[x, y];
                                                            Cell l = matrix[x, y - 1];
                                                            matrix[x, y - 1] = c;
                                                            matrix[x, y] = l;
                                                            updMatrix[x, y - 1] = true;
                                                            moved = true;
                                                            moveDir = new Vector2(0, -1);
                                                            swapped = true;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (moved)
                                    {
                                        if (!swapped)
                                        { tex.GetPixel(x, y) = new Color(0, 0, 0, 0); }
                                        else
                                        {
                                            if (simpleGraphics)
                                            { tex.GetPixel(x, y) = matrix[x, y].simpCol; }
                                            else
                                            { tex.GetPixel(x, y) = matrix[x, y].color; }
                                        }

                                        if (simpleGraphics)
                                        { tex.GetPixel(x + (int)moveDir.X, y + (int)moveDir.Y) = matrix[x + (int)moveDir.X, y + (int)moveDir.Y].simpCol; }
                                        else
                                        { tex.GetPixel(x + (int)moveDir.X, y + (int)moveDir.Y) = matrix[x + (int)moveDir.X, y + (int)moveDir.Y].color; }

                                        texUpdated = true;
                                    }
                                    else if (!moved && !matrix[x, y].firstFrame)
                                    {
                                        matrix[x, y].firstFrame = true;

                                        if (simpleGraphics)
                                        { tex.GetPixel(x, y) = matrix[x, y].simpCol; }
                                        else
                                        { tex.GetPixel(x, y) = matrix[x, y].color; }

                                        texUpdated = true;
                                    }

                                    if (!moved)
                                    {
                                        matrix[x, y].nFreeFall = true;
                                    }
                                    else
                                    {
                                        //seperate this to somewhere else or like a function or something

                                        //update nearby cells
                                        if (y > 0)
                                        {
                                            if (matrix[x, y - 1] != null)
                                            {
                                                if (matrix[x, y - 1].nFreeFall)
                                                { matrix[x, y - 1].nFreeFall = false; }
                                            }

                                            if (x > 0)
                                            {
                                                if (matrix[x - 1, y - 1] != null)
                                                {
                                                    if (matrix[x - 1, y - 1].nFreeFall)
                                                    { matrix[x - 1, y - 1].nFreeFall = false; }
                                                }
                                            }

                                            if (x < mSX - 1)
                                            {
                                                if (matrix[x + 1, y - 1] != null)
                                                {
                                                    if (matrix[x + 1, y - 1].nFreeFall)
                                                    { matrix[x + 1, y - 1].nFreeFall = false; }
                                                }
                                            }
                                        }

                                        if (x > 0)
                                        {
                                            if (matrix[x - 1, y] != null)
                                            {
                                                if (matrix[x - 1, y].nFreeFall)
                                                { matrix[x - 1, y].nFreeFall = false; }
                                            }
                                        }

                                        if (x < mSX - 1)
                                        {
                                            if (matrix[x + 1, y] != null)
                                            {
                                                if (matrix[x + 1, y].nFreeFall)
                                                { matrix[x + 1, y].nFreeFall = false; }
                                            }
                                        }

                                        if (y < mSY - 1)
                                        {
                                            if (matrix[x, y + 1] != null)
                                            {
                                                if (matrix[x, y + 1].nFreeFall)
                                                { matrix[x, y + 1].nFreeFall = false; }
                                            }

                                            if (x > 0)
                                            {
                                                if (matrix[x - 1, y + 1] != null)
                                                {
                                                    if (matrix[x - 1, y + 1].nFreeFall)
                                                    { matrix[x - 1, y + 1].nFreeFall = false; }
                                                }
                                            }

                                            if (x < mSX - 1)
                                            {
                                                if (matrix[x + 1, y + 1] != null)
                                                {
                                                    if (matrix[x + 1, y + 1].nFreeFall)
                                                    { matrix[x + 1, y + 1].nFreeFall = false; }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (y > 0)
                                {
                                    if (matrix[x, y - 1] == null)
                                    {
                                        matrix[x, y].nFreeFall = false;
                                    }
                                }

                                if (recentTexUpd)
                                {
                                    if (simpleGraphics)
                                    { tex.GetPixel(x, y) = matrix[x, y].simpCol; }
                                    else
                                    { tex.GetPixel(x, y) = matrix[x, y].color; }
                                }
                            }
                        }
                        else if (!matrix[x, y].firstFrame || recentTexUpd)
                        {
                            matrix[x, y].firstFrame = true;

                            if (simpleGraphics)
                            { tex.GetPixel(x, y) = matrix[x, y].simpCol; }
                            else
                            { tex.GetPixel(x, y) = matrix[x, y].color; }

                            texUpdated = true;
                        }
                    }
                }
            }

            if (texUpdated || recentTexUpd)
            { tex.ApplyChanges(); recentTexUpd = recentTexUpd ? false : recentTexUpd; }
        }

        public Cell cloneProps(Cell cellToClone)
        { 
            Cell c = new Cell();

            c.name = "cloned cell is not named";
            c.cols = cellToClone.cols;
            c.erase = cellToClone.erase;
            c.move = cellToClone.move;
            c.moveChance = cellToClone.moveChance;
            c.stack = cellToClone.stack;
            c.liquid = cellToClone.liquid;
            c.simpCol = cellToClone.simpCol;

            return c;

            //why do i always forget to update this when i add new variable :P
        }

        public class Cell
        { 
            public string name { get; set; }

            public Color[] cols { get; set; }
            public Color color { get; set; }
            public Color simpCol { get; set; }

            public bool erase { get; set; }

            public bool move { get; set; }

            public float moveChance { get; set; }

            public bool stack { get; set; }

            public bool firstFrame { get; set; }

            public bool nFreeFall { get; set; }

            public bool liquid { get; set; }
        }
    }
}
