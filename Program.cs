﻿using SimulationFramework;
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

            Gradient g = new LinearGradient(0, 0, Window.Width, Window.Height, new Color[] { BG, SECONDARY });

            canv.Fill(g);
            canv.DrawRect(Vector2.Zero, new Vector2(Window.Width, Window.Height));

            DrawModernBox(canv, new Vector2(Window.Width / 2 - 60, 60), new Vector2(80, 80), 45, PRIMARY);
            DrawModernBox(canv, new Vector2(Window.Width / 2 + 60, 60), new Vector2(80, 80), 45, PRIMARY);

            //opens settings
            if (rectPoint(new Vector2(Window.Width / 2 - 60, 60), new Vector2(80, 80), Mouse.Position))
            {
                if (Mouse.IsButtonPressed(MouseButton.Left))
                {
                    if (outs[0].PlaybackState is PlaybackState.Playing) { outs[0].Stop(); }
                    ins[0].CurrentTime = new TimeSpan(0L);
                    outs[0].Init(ins[0]);
                    outs[0].Play();

                    settingsOpen = true;
                }
            } //opens home area again
            else if (rectPoint(new Vector2(Window.Width / 2 + 60, 60), new Vector2(80, 80), Mouse.Position))
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

            //gameinfo
            canv.Fill(TEXT);
            canv.Font(smallTxt);
            canv.FontSize(50);
            canv.DrawText(games[gameSelected].name, new Vector2(Window.Width / 2, 175), Alignment.Center);

            canv.Fill(TEXT);
            canv.Font(smallTxt);
            canv.FontSize(25);
            canv.DrawText(games[gameSelected].desc, new Vector2(Window.Width / 2, 250), Alignment.Center);

            canv.Fill(TEXT);
            canv.Font(smallTxt);
            canv.FontSize(25);
            canv.DrawText("Version: " + games[gameSelected].ver, new Vector2(Window.Width / 2, 300), Alignment.Center);

            DrawModernBox(canv, new Vector2(Window.Width / 2, Window.Height / 2 + 80), new Vector2(500, 150), 45, PRIMARY);

            canv.Fill(TEXT);
            canv.Font(smallTxt);
            canv.FontSize(75);
            canv.DrawText("Start", new Vector2(Window.Width / 2, Window.Height / 2 + 80), Alignment.Center);

            if (rectPoint(new Vector2(Window.Width / 2, Window.Height / 2 + 80), new Vector2(500, 150), Mouse.Position) && !settingsOpen)
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

            //settings menu

            canv.Fill(SECONDARY);
            canv.DrawRect(new Vector2(0, 120 + setMenuY), new Vector2(Window.Width, Window.Height), Alignment.TopLeft);

            DrawModernBox(canv, new Vector2(40, setMenuY + 160), new Vector2(50, 50), 45, PRIMARY);
            DrawModernBox(canv, new Vector2(100, setMenuY + 160), new Vector2(50, 50), 45, PRIMARY);

            canv.Fill(TEXT);
            canv.Font(smallTxt);
            canv.FontSize(35);
            canv.DrawText("+", new Vector2(40, setMenuY + 160), Alignment.Center);

            canv.DrawText("-", new Vector2(100, setMenuY + 160), Alignment.Center);

            canv.DrawText("Pallate: " + pallate, new Vector2(100, setMenuY + 160), Alignment.Center); ;

            if (rectPoint(new Vector2(40, setMenuY + 160), new Vector2(50, 50), Mouse.Position))
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

            if (rectPoint(new Vector2(100, setMenuY + 160), new Vector2(50, 50), Mouse.Position))
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

            if (Keyboard.IsKeyPressed(Key.LeftArrow))
            {
                gameSelected -= 1;
                if (gameSelected < 0)
                { gameSelected = games.Length - 1; }
            }

            if (Keyboard.IsKeyPressed(Key.RightArrow))
            {
                gameSelected += 1;
                if (gameSelected > games.Length - 1)
                { gameSelected = 0; }
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