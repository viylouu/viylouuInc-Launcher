﻿using SimulationFramework;
using SimulationFramework.Drawing;
using System.Numerics;
using viylouuInc_Launcher;
using NAudio.Wave;
using SimulationFramework.Input;
using ImGuiNET;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
    string pallateName = "null";

    bool lightMode = false;

    ColorManager cm = new ColorManager();

    bool gamMen = false;

    ITexture settingsIcon = null;
    ITexture homeIcon = null;

    public override void OnInitialize()
    {
        Window.Title = "viylouu games";

        settingsIcon = Graphics.LoadTexture(@"Assets\Sprites\Menu-SettingsIcon.png");
        homeIcon = Graphics.LoadTexture(@"Assets\Sprites\Menu-HomeIcon.png");

        ApplyColors(ref TEXT, ref BG, ref PRIMARY, ref SECONDARY, ref ACCENT, lightMode, pallate, ref pallateName);

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

            DrawModernBox(canv, new Vector2(Window.Width / 2, 60), new Vector2(80, 80), 45, PRIMARY);
            DrawModernBox(canv, new Vector2(Window.Width / 2 + 100, 60), new Vector2(80, 80), 45, PRIMARY);
            DrawModernBox(canv, new Vector2(Window.Width / 2 - 100, 60), new Vector2(80, 80), 45, PRIMARY);

            canv.Translate(new Vector2(Window.Width / 2, 60));
            canv.Rotate(180 * (float.Pi / 180));
            canv.DrawTexture(settingsIcon, new Vector2(0, 0), new Vector2(60, 60), Alignment.Center);

            canv.ResetState();
            canv.Translate(new Vector2(Window.Width / 2 - 100, 60));
            canv.Rotate(180 * (float.Pi / 180));
            canv.DrawTexture(homeIcon, new Vector2(0, 0), new Vector2(60, 60), Alignment.Center);

            canv.ResetState();

            //opens settings
            if (rectPoint(new Vector2(Window.Width / 2, 60), new Vector2(80, 80), Mouse.Position))
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
            else if (rectPoint(new Vector2(Window.Width / 2 - 100, 60), new Vector2(80, 80), Mouse.Position))
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

            canv.FontSize(25);
            canv.DrawText(games[gameSelected].desc, new Vector2(Window.Width / 2, 250), Alignment.Center);

            canv.DrawText("Version: " + games[gameSelected].ver, new Vector2(Window.Width / 2, 300), Alignment.Center);

            DrawModernBox(canv, new Vector2(Window.Width / 2, Window.Height / 2 + 80), new Vector2(500, 150), 45, PRIMARY);

            canv.Fill(TEXT);
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

            canv.FontSize(15);
            canv.DrawText("Pallate: " + pallateName, new Vector2(140, setMenuY + 160), Alignment.CenterLeft); ;

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

                    ApplyColors(ref TEXT, ref BG, ref PRIMARY, ref SECONDARY, ref ACCENT, lightMode, pallate, ref pallateName);
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

                    ApplyColors(ref TEXT, ref BG, ref PRIMARY, ref SECONDARY, ref ACCENT, lightMode, pallate, ref pallateName);
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

    void ApplyColors(ref Color txt, ref Color bg, ref Color pri, ref Color sec, ref Color acc, bool light, int pallate, ref string pallatename)
    {
        cm.ApplyColors(ref txt, ref bg, ref pri, ref sec, ref acc, light, pallate, ref pallatename);

        ApplyColorToTex(settingsIcon, txt);
        ApplyColorToTex(homeIcon, txt);
    }

    void ApplyColorToTex(ITexture tex, Color col)
    {
        for (int x = 0; x < tex.Width; x++)
        {
            for (int y = 0; y < tex.Height; y++)
            {
                if (tex.GetPixel(x, y).A > 0)
                {
                    tex.GetPixel(x, y) = col;
                }
                else
                {
                    tex.GetPixel(x, y) = new Color(0, 0, 0, 0);
                }
            }
        }

        tex.ApplyChanges();
    }
}