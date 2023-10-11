using SimulationFramework;
using SimulationFramework.Drawing;
using System.Numerics;
using viylouuInc_Launcher;
using NAudio.Wave;
using SimulationFramework.Input;
using ImGuiNET;

Start<Program>();

partial class Program : Simulation
{
    Color BG = Color.Black;
    Color TEXT = Color.White;
    Color PRIMARY = Color.Gray;
    Color SECONDARY = Color.DarkGray;
    Color ACCENT = Color.Gray;

    IFont smallTxt = null;

    WaveStream[] ins = new WaveStream[] {
    new WaveFileReader(Directory.GetCurrentDirectory() + @"\Assets\Audio\buttonClick.wav"),
    new WaveFileReader(Directory.GetCurrentDirectory() + @"\Assets\Audio\switchTab.wav")
};

    WaveOutEvent[] outs = new WaveOutEvent[0];

    bool settingsOpen = false;

    float menuAnimTime = 1.4f;

    float setMenuY = 0;

    static double fps1 = 48;
    double fps = 1 / fps1;
    double accum = 0;
    DateTime lastItTime = DateTime.UtcNow;

    int gameSelected = 0;

    bool fullScreen = false;

    gameInfo cells = new gameInfo
    {
        name = "Cells",
        desc = "Cells is a simple cellulaur automata engine",
        updater = new CellsGame().Update,
        ver = 0.1f
    };

    gameInfo lisk = new gameInfo
    {
        name = "Lisk",
        desc = "Lisk is a 3d raycaster in the doom style",
        updater = new LiskGame().Update,
        ver = 0.0f
    };


    gameInfo[] games = null;

    bool gameStarted = false;

    int pallate = 0;

    bool lightMode = false;

    ColorManager cm = new ColorManager();

    bool gamMen = false;

    public override void OnInitialize()
    {
        Window.Title = "viylouu games";

        cm.ApplyColors(ref TEXT, ref BG, ref PRIMARY, ref SECONDARY, ref ACCENT, lightMode, pallate);

        smallTxt = Graphics.LoadFont(@"Assets\Fonts\Comfortaa-Bold.ttf");

        outs = new WaveOutEvent[ins.Length];
        for (int i = 0; i < outs.Length; i++)
        { outs[i] = new WaveOutEvent(); }

        games = new gameInfo[] {
            cells,
            lisk
        };
    }

    public override void OnRender(ICanvas canv)
    {
        if (!gameStarted)
        {
            canv.Clear(BG);

            UpdSep((DateTime.UtcNow - lastItTime).TotalSeconds, Fix);
            lastItTime = DateTime.UtcNow;

            Gradient g = new LinearGradient(0, 0, Window.Width, Window.Height, new Color[] { BG, SECONDARY});

            canv.Fill(g);
            canv.DrawRect(Vector2.Zero, new Vector2(Window.Width, Window.Height));

            DrawModernBox(canv, new Vector2((Window.Width - 160) / 2 / 2 + 160 + 8, 60), new Vector2((Window.Width - 180) / 2 - 15, 80), 25, PRIMARY);
            DrawModernBox(canv, new Vector2((Window.Width - 160) / 2 / 2 + 160 + (Window.Width - 160) / 2 - 5, 60), new Vector2((Window.Width - 190) / 2 - 15, 80), 25, PRIMARY);

            canv.Fill(TEXT);
            canv.Font(smallTxt);
            canv.FontSize(50);
            canv.DrawText("settings", new Vector2((Window.Width - 110) / 2 / 2 + 110, 60), Alignment.Center);
            canv.DrawText("home", new Vector2((Window.Width - 110) / 2 / 2 + 110 + (Window.Width - 110) / 2, 60), Alignment.Center);

            if (rectPoint(new Vector2((Window.Width - 160) / 2 / 2 + 160, 60), new Vector2((Window.Width - 190) / 2 - 15, 80), Mouse.Position))
            {
                if (Mouse.IsButtonPressed(MouseButton.Left))
                {
                    if (outs[0].PlaybackState is PlaybackState.Playing) { outs[0].Stop(); }
                    ins[0].CurrentTime = new TimeSpan(0L);
                    outs[0].Init(ins[0]);
                    outs[0].Play();

                    settingsOpen = true;
                }
            }
            else if (rectPoint(new Vector2((Window.Width - 160) / 2 / 2 + 160 + (Window.Width - 160) / 2, 60), new Vector2((Window.Width - 190) / 2 - 15, 80), Mouse.Position))
            {
                if (Mouse.IsButtonPressed(MouseButton.Left))
                {
                    if (outs[0].PlaybackState is PlaybackState.Playing) { outs[0].Stop(); }
                    ins[0].CurrentTime = new TimeSpan(0L);
                    outs[0].Init(ins[0]);
                    outs[0].Play();

                    settingsOpen = false;
                }
            }

            canv.Fill(TEXT);
            canv.Font(smallTxt);
            canv.FontSize(50);
            canv.DrawText(games[gameSelected].name, new Vector2(Window.Width / 2 + 80, 175), Alignment.Center);

            canv.Fill(TEXT);
            canv.Font(smallTxt);
            canv.FontSize(25);
            canv.DrawText(games[gameSelected].desc, new Vector2(Window.Width / 2 + 80, 250), Alignment.Center);

            canv.Fill(TEXT);
            canv.Font(smallTxt);
            canv.FontSize(25);
            canv.DrawText("Version: " + games[gameSelected].ver, new Vector2(Window.Width / 2 + 80, 300), Alignment.Center);

            //soooooo messy

            DrawModernBox(canv, new Vector2(Window.Width / 2 + 80, Window.Height / 2 + 80), new Vector2(500, 150), 25, PRIMARY);

            canv.Fill(TEXT);
            canv.Font(smallTxt);
            canv.FontSize(75);
            canv.DrawText("Start", new Vector2(Window.Width / 2 + 80, Window.Height / 2 + 80), Alignment.Center);

            if (rectPoint(new Vector2(Window.Width / 2 + 80, Window.Height / 2 + 80), new Vector2(500, 150), Mouse.Position) && !settingsOpen)
            {
                if (Mouse.IsButtonPressed(MouseButton.Left))
                {
                    if (outs[0].PlaybackState is PlaybackState.Playing) { outs[0].Stop(); }
                    ins[0].CurrentTime = new TimeSpan(0L);
                    outs[0].Init(ins[0]);
                    outs[0].Play();

                    gameStarted = true;
                }
            }

            DrawModernBox(canv, new Vector2(Window.Width / 2 + 80, setMenuY + Window.Height / 2 + 55), new Vector2(Window.Width - 180, Window.Height - 130), 25, SECONDARY);

            DrawModernBox(canv, new Vector2(210, setMenuY + 160), new Vector2(50, 50), 25, PRIMARY);
            DrawModernBox(canv, new Vector2(270, setMenuY + 160), new Vector2(50, 50), 25, PRIMARY);
            DrawModernBox(canv, new Vector2(210, setMenuY + 220), new Vector2(50, 50), 25, PRIMARY);

            canv.Fill(TEXT);
            canv.Font(smallTxt);
            canv.FontSize(35);
            canv.DrawText("+", new Vector2(210, setMenuY + 160), Alignment.Center);

            canv.Fill(TEXT);
            canv.Font(smallTxt);
            canv.FontSize(35);
            canv.DrawText("-", new Vector2(270, setMenuY + 160), Alignment.Center);

            canv.Fill(TEXT);
            canv.Font(smallTxt);
            canv.FontSize(35);
            canv.DrawText(lightMode ? "*" : "x", new Vector2(210, setMenuY + 220), Alignment.Center);

            canv.Fill(TEXT);
            canv.Font(smallTxt);
            canv.FontSize(35);
            canv.DrawText(lightMode ? "Light Mode" : "Dark Mode", new Vector2(250, setMenuY + 220), Alignment.CenterLeft);

            //i literally have no idea what this does

            if (rectPoint(new Vector2(210, setMenuY + 160), new Vector2(50, 50), Mouse.Position))
            {
                if (Mouse.IsButtonPressed(MouseButton.Left))
                {
                    if (outs[0].PlaybackState is PlaybackState.Playing) { outs[0].Stop(); }
                    ins[0].CurrentTime = new TimeSpan(0L);
                    outs[0].Init(ins[0]);
                    outs[0].Play();

                    pallate += 1;
                    pallate = pallate % ColorManager.pallates.Length;
                    if (pallate < 0)
                    { pallate = ColorManager.pallates.Length - 1; }

                    cm.ApplyColors(ref TEXT, ref BG, ref PRIMARY, ref SECONDARY, ref ACCENT, lightMode, pallate);
                }
            }

            if (rectPoint(new Vector2(270, setMenuY + 160), new Vector2(50, 50), Mouse.Position))
            {
                if (Mouse.IsButtonPressed(MouseButton.Left))
                {
                    if (outs[0].PlaybackState is PlaybackState.Playing) { outs[0].Stop(); }
                    ins[0].CurrentTime = new TimeSpan(0L);
                    outs[0].Init(ins[0]);
                    outs[0].Play();

                    pallate -= 1;
                    pallate = pallate % ColorManager.pallates.Length;
                    if (pallate < 0)
                    { pallate = ColorManager.pallates.Length - 1; }

                    cm.ApplyColors(ref TEXT, ref BG, ref PRIMARY, ref SECONDARY, ref ACCENT, lightMode, pallate);
                }
            }

            if (rectPoint(new Vector2(210, setMenuY + 220), new Vector2(50, 50), Mouse.Position))
            {
                if (Mouse.IsButtonPressed(MouseButton.Left))
                {
                    if (outs[0].PlaybackState is PlaybackState.Playing) { outs[0].Stop(); }
                    ins[0].CurrentTime = new TimeSpan(0L);
                    outs[0].Init(ins[0]);
                    outs[0].Play();

                    lightMode = !lightMode;

                    cm.ApplyColors(ref TEXT, ref BG, ref PRIMARY, ref SECONDARY, ref ACCENT, lightMode, pallate);
                }
            }

            canv.Fill(TEXT);
            canv.Font(smallTxt);
            canv.FontSize(15);
            canv.DrawText("Pallate: " + ColorManager.pallates[pallate].name, new Vector2(310, setMenuY + 160), Alignment.CenterLeft);

            DrawModernBox(canv, new Vector2(150 / 2 + 10, Window.Height / 2 + ((Window.Height - 40) / 2 - 10) / 2 + 10), new Vector2(130, (Window.Height - 40) / 2 - 5), 25, PRIMARY);

            canv.Fill(TEXT);
            canv.Font(smallTxt);
            canv.FontSize(35);
            canv.DrawText(games[0].name, new Vector2(150 / 2 + 10, Window.Height / 2 + ((Window.Height - 40) / 2 - 10) / 2 + 10), Alignment.Center);

            DrawModernBox(canv, new Vector2(150 / 2 + 10, Window.Height / 2 - ((Window.Height - 40) / 2 - 10) / 2 - 10), new Vector2(130, (Window.Height - 40) / 2 - 5), 25, PRIMARY);

            // y no work :(((

            canv.Fill(TEXT);
            canv.Font(smallTxt);
            canv.FontSize(35);
            canv.DrawText(games[1].name, new Vector2(150 / 2 + 10, Window.Height / 2 - ((Window.Height - 40) / 2 - 10) / 2 - 10), Alignment.Center);

            if (rectPoint(new Vector2(150 / 2 + 10, Window.Height / 2 + ((Window.Height - 40) / 2 - 10) / 2 + 10), new Vector2(130, (Window.Height - 40) / 2 - 5), Mouse.Position))
            {
                if (Mouse.IsButtonPressed(MouseButton.Left))
                {
                    if (outs[0].PlaybackState is PlaybackState.Playing) { outs[0].Stop(); }
                    ins[0].CurrentTime = new TimeSpan(0L);
                    outs[0].Init(ins[0]);
                    outs[0].Play();

                    gameSelected = 0;
                }
            }

            if (rectPoint(new Vector2(150 / 2 + 10, Window.Height / 2 - ((Window.Height - 40) / 2 - 10) / 2 - 10), new Vector2(130, (Window.Height - 40) / 2 - 5), Mouse.Position))
            {
                if (Mouse.IsButtonPressed(MouseButton.Left))
                {
                    if (outs[0].PlaybackState is PlaybackState.Playing) { outs[0].Stop(); }
                    ins[0].CurrentTime = new TimeSpan(0L);
                    outs[0].Init(ins[0]);
                    outs[0].Play();

                    gameSelected = 1;
                }
            }
        }

        if (gameStarted)
        {
            games[gameSelected].updater.Invoke();
        }
    }

    void UpdSep(double delta, Action act)
    {
        //pov: most simple code in entire project

        accum += delta;

        while (accum >= fps)
        {
            act.Invoke();
            accum -= fps;
        }
    }

    void Fix()
    {
        if (settingsOpen)
        {
            setMenuY += -setMenuY / 5;
        }
        else
        {
            setMenuY += (Window.Height - setMenuY) / 5;
        }
    }


    void DrawModernBox(ICanvas canv, Vector2 pos, Vector2 scale, int roundness, Color col)
    {
        canv.Fill(col);

        canv.DrawRect(pos, scale - new Vector2(0, roundness), Alignment.Center);
        canv.DrawRect(pos, scale - new Vector2(roundness, 0), Alignment.Center);

        canv.DrawEllipse(pos + new Vector2(scale.X / 2 - roundness / 2, scale.Y / 2 - roundness / 2), new Vector2(roundness / 2, roundness / 2), Alignment.Center);
        canv.DrawEllipse(pos + new Vector2(-scale.X / 2 + roundness / 2, scale.Y / 2 - roundness / 2), new Vector2(roundness / 2, roundness / 2), Alignment.Center);
        canv.DrawEllipse(pos + new Vector2(scale.X / 2 - roundness / 2, -scale.Y / 2 + roundness / 2), new Vector2(roundness / 2, roundness / 2), Alignment.Center);
        canv.DrawEllipse(pos + new Vector2(-scale.X / 2 + roundness / 2, -scale.Y / 2 + roundness / 2), new Vector2(roundness / 2, roundness / 2), Alignment.Center);
    }

    bool rectPoint(Vector2 rp, Vector2 rs, Vector2 p)
    {
        //wat :|

        if (p.X >= rp.X - rs.X / 2 &&
          p.X <= rp.X + rs.X / 2 &&
          p.Y >= rp.Y - rs.Y &&
          p.Y <= rp.Y + rs.Y / 2)
        {
            return true;
        }
        return false;
    }

    class gameInfo
    {
        public Action updater { get; set; }

        public string desc { get; set; }
        public string name { get; set; }

        public float ver { get; set; }
    }
}