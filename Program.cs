using SimulationFramework;
using SimulationFramework.Drawing;
using System.Numerics;
using viylouuInc_Launcher;
using NAudio.Wave;
using SimulationFramework.Input;
using SimulationFramework.Components;
using SimulationFramework.Desktop;
using System.ComponentModel;

Simulation sim = Simulation.Create(Init, Rend);
sim.Run(new DesktopPlatform());

partial class Program
{
    //color

    static Color BG = new Color(209, 111, 19);
    static Color BG2 = new Color(252, 176, 35);

    static Color TEXT = new Color(240, 223, 201);

    static Color PRIMARY = new Color(122, 80, 62);
    static Color SECONDARY = new Color(94, 50, 32);

    //other

    static IFont smallTxt = null;

    static WaveStream[] ins = new WaveStream[] {
        new WaveFileReader(Directory.GetCurrentDirectory() + @"\Assets\Audio\buttonClick.wav"),
        new WaveFileReader(Directory.GetCurrentDirectory() + @"\Assets\Audio\switchTab.wav"),
        new WaveFileReader(Directory.GetCurrentDirectory() + @"\Assets\Audio\startup.wav")
    };

    static WaveOutEvent[] outs = new WaveOutEvent[0];

    static bool settingsOpen = false;

    static bool infoOpen = false;

    static float setMenuY = 0;

    static float infoMenuY = 0;

    static double fps1 = 48;
    static double fps = 1 / fps1;
    static double accum = 0;
    static DateTime lastItTime = DateTime.UtcNow;

    static int gameSelected = 0;

    static bool fullScreen = false;

    static gameInfo cells = new gameInfo
    {
        name = "Cells",
        desc = "Cells is a simple cellulaur automata engine",
        updater = new CellsGame().Update,
        ver = "0.1.5"
    };

    static gameInfo lisk = new gameInfo
    {
        name = "Lisk",
        desc = "Lisk is a 3d raycaster in the doom style",
        updater = new LiskGame().Update,
        ver = "0.0 BETA"
    };

    static gameInfo tink = new gameInfo
    {
        name = "Tink",
        desc = "Tink is a 2d RPG game",
        updater = new TinkGame().Update,
        ver = "0.0 BETA"
    };

    static gameInfo bask = new gameInfo
    {
        name = "Bask",
        desc = "Bask is a 2d topdown racing game using Spritestacking",
        updater = new BaskGame().Update,
        ver = "0.0 BETA"
    };

    static gameInfo farmlight = new gameInfo
    {
        name = "Farmlight",
        desc = "Farmlight is a 2d farming platformer",
        updater = new FarmlightGame().Update,
        ver = "0.0 BETA"
    };

    static gameInfo linki = new gameInfo
    {
        name = "Linki",
        desc = "Linki is a 3d rendering test",
        updater = new LinkiGame().Update,
        ver = "0.0 BETA"
    };

    static gameInfo life = new gameInfo
    {
        name = "Life",
        desc = "Life is a simulation of life originally in scratch",
        updater = new LifeGame().Update,
        ver = "0.0 BETA"
    };

    static gameInfo veloc = new gameInfo
    {
        name = "Veloc",
        desc = "Veloc is a 2d platformer game based on momentum",
        updater = new VelocGame().Update,
        ver = "0.0 BETA"
    };

    static gameInfo[] games = null;

    public static bool gameStarted = false;
    public static int framesUntilGameStarted = 0;

    static int pallate = 0;
    static string pallateName = "null";

    static bool lightMode = false;

    static ITexture settingsIcon = null;
    static ITexture homeIcon = null;
    static ITexture infoIcon = null;
    static ITexture closeIcon = null;

    static bool vsync = false;

    static void Init()
    {
        Window.Title = "viylouu games";

        settingsIcon = Graphics.LoadTexture(@"Assets\Sprites\Menu-SettingsIcon.png");
        homeIcon = Graphics.LoadTexture(@"Assets\Sprites\Menu-HomeIcon.png");
        infoIcon = Graphics.LoadTexture(@"Assets\Sprites\Menu-InfoIcon.png");
        closeIcon = Graphics.LoadTexture(@"Assets\Sprites\Menu-CloseIcon.png");

        ApplyColors(ref TEXT, ref BG, ref PRIMARY, ref SECONDARY, ref BG2, lightMode, pallate, ref pallateName);

        smallTxt = Graphics.LoadFont(@"Assets\Fonts\Comfortaa-Bold.ttf");

        outs = new WaveOutEvent[ins.Length];
        for (int i = 0; i < outs.Length; i++)
        { outs[i] = new WaveOutEvent(); }

        outs[2].Init(ins[2]);
        outs[2].Play();

        games = new gameInfo[] {
            cells,
            lisk,
            tink,
            bask,
            linki,
            farmlight,
            life,
            veloc
        };
    }

    static void Rend(ICanvas canv)
    {
        if (!gameStarted)
        {
            canv.Clear(BG);

            Simulation.SetFixedResolution(1920, 1080, Color.Black, false, false, false);

            UpdSep((DateTime.UtcNow - lastItTime).TotalSeconds, Fix);
            lastItTime = DateTime.UtcNow;

            Gradient g = new LinearGradient(0, 0, canv.Width, canv.Height, new Color[] { BG, BG2 });

            canv.Fill(g);
            canv.DrawRect(Vector2.Zero, new Vector2(canv.Width, canv.Height));

            canv.Fill(new Color(0, 0, 0, 150));
            canv.DrawRect(Vector2.Zero, new Vector2(canv.Width, 120));

            canv.Fill(PRIMARY);
            canv.DrawRoundedRect(new Vector2(canv.Width / 2 - 140, 60), new Vector2(80, 80), 45, Alignment.Center);
            canv.DrawRoundedRect(new Vector2(canv.Width / 2 - 50, 60), new Vector2(80, 80), 45, Alignment.Center);
            canv.DrawRoundedRect(new Vector2(canv.Width / 2 + 50, 60), new Vector2(80, 80), 45, Alignment.Center);
            canv.DrawRoundedRect(new Vector2(canv.Width / 2 + 140, 60), new Vector2(80, 80), 45, Alignment.Center);

            canv.DrawTexture(settingsIcon, new Vector2(canv.Width / 2 - 140, 60), new Vector2(60, 60), Alignment.Center);

            canv.DrawTexture(homeIcon, new Vector2(canv.Width / 2 - 50, 60), new Vector2(60, 60), Alignment.Center);

            canv.DrawTexture(infoIcon, new Vector2(canv.Width / 2 + 50, 60), new Vector2(60, 60), Alignment.Center);

            canv.DrawTexture(closeIcon, new Vector2(canv.Width / 2 + 140, 60), new Vector2(60, 60), Alignment.Center);

            //opens settings
            if (rectPoint(new Vector2(canv.Width / 2 - 140, 60), new Vector2(80, 80), Mouse.Position))
            {
                if (Mouse.IsButtonPressed(MouseButton.Left))
                {
                    if (outs[0].PlaybackState is PlaybackState.Playing) { outs[0].Stop(); }
                    ins[0].CurrentTime = new TimeSpan(0L);
                    outs[0].Init(ins[0]);
                    outs[0].Play();

                    settingsOpen = true;
                    infoOpen = false;
                }
            }
            //opens home area again
            else if (rectPoint(new Vector2(canv.Width / 2 - 50, 60), new Vector2(80, 80), Mouse.Position))
            {
                if (Mouse.IsButtonPressed(MouseButton.Left))
                {
                    if (outs[0].PlaybackState is PlaybackState.Playing) { outs[0].Stop(); }
                    ins[0].CurrentTime = new TimeSpan(0L);
                    outs[0].Init(ins[0]);
                    outs[0].Play();

                    settingsOpen = false;
                    infoOpen = false;
                }
            }
            //closes everything
            else if (rectPoint(new Vector2(canv.Width / 2 + 140, 60), new Vector2(80, 80), Mouse.Position))
            { if (Mouse.IsButtonPressed(MouseButton.Left)) { Environment.Exit(0); } }
            //info menu
            else if (rectPoint(new Vector2(canv.Width / 2 + 50, 60), new Vector2(80, 80), Mouse.Position))
            {
                if (Mouse.IsButtonPressed(MouseButton.Left))
                {
                    if (outs[0].PlaybackState is PlaybackState.Playing) { outs[0].Stop(); }
                    ins[0].CurrentTime = new TimeSpan(0L);
                    outs[0].Init(ins[0]);
                    outs[0].Play();

                    settingsOpen = false;
                    infoOpen = true;
                }
            }

            //gameinfo
            canv.Fill(TEXT);
            //canv.Font(smallTxt);
            canv.FontSize(50);
            canv.DrawText(games[gameSelected].name, new Vector2(canv.Width / 2, 175), Alignment.Center);

            canv.FontSize(25);
            canv.DrawText(games[gameSelected].desc, new Vector2(canv.Width / 2, 250), Alignment.Center);

            canv.DrawText("Version: " + games[gameSelected].ver, new Vector2(canv.Width / 2, 300), Alignment.Center);

            canv.Fill(PRIMARY);
            canv.DrawRoundedRect(new Vector2(canv.Width / 2, canv.Height / 2 + 80), new Vector2(500, 150), 45, Alignment.Center);

            canv.Fill(TEXT);
            canv.FontSize(75);
            canv.DrawText("Start", new Vector2(canv.Width / 2, canv.Height / 2 + 80), Alignment.Center);

            if (rectPoint(new Vector2(canv.Width / 2, canv.Height / 2 + 80), new Vector2(500, 150), Mouse.Position) && !settingsOpen && !infoOpen)
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
            canv.DrawRect(new Vector2(0, 120 + setMenuY), new Vector2(canv.Width, canv.Height), Alignment.TopLeft);

            canv.FontSize(15);
            canv.DrawText("Pallate: " + pallateName, new Vector2(140, setMenuY + 160), Alignment.CenterLeft);

            canv.Fill(PRIMARY);
            canv.DrawRoundedRect(new Vector2(40, setMenuY + 160), new Vector2(50, 50), 45, Alignment.Center);

            canv.Fill(TEXT);
            //canv.Font(smallTxt);
            canv.FontSize(35);
            canv.DrawText(fullScreen ? "*" : "-", new Vector2(40, setMenuY + 160), Alignment.Center);

            canv.FontSize(15);
            canv.DrawText("Fullscreen", new Vector2(80, setMenuY + 160), Alignment.CenterLeft);

            canv.Fill(PRIMARY);
            canv.DrawRoundedRect(new Vector2(40, setMenuY + 220), new Vector2(50, 50), 45, Alignment.Center);

            canv.Fill(TEXT);
            //canv.Font(smallTxt);
            canv.FontSize(35);
            canv.DrawText(vsync ? "*" : "-", new Vector2(40, setMenuY + 220), Alignment.Center);

            canv.FontSize(15);
            canv.DrawText("Vsync", new Vector2(80, setMenuY + 220), Alignment.CenterLeft);

            if (rectPoint(new Vector2(40, setMenuY + 220), new Vector2(50, 50), Mouse.Position) && settingsOpen)
            {
                if (Mouse.IsButtonPressed(MouseButton.Left))
                {
                    if (outs[0].PlaybackState is PlaybackState.Playing) { outs[0].Stop(); }
                    ins[0].CurrentTime = new TimeSpan(0L);
                    outs[0].Init(ins[0]);
                    outs[0].Play();

                    fullScreen = !fullScreen;

                    if (fullScreen)
                    { Window.EnterFullscreen(); }
                    else
                    { Window.ExitFullscreen(); }
                }
            }

            if (rectPoint(new Vector2(40, setMenuY + 280), new Vector2(50, 50), Mouse.Position) && settingsOpen)
            {
                if (Mouse.IsButtonPressed(MouseButton.Left))
                {
                    if (outs[0].PlaybackState is PlaybackState.Playing) { outs[0].Stop(); }
                    ins[0].CurrentTime = new TimeSpan(0L);
                    outs[0].Init(ins[0]);
                    outs[0].Play();

                    vsync = !vsync;

                    Graphics.SwapInterval = vsync ? 1 : 0;
                }
            }

            //info menu

            canv.Fill(SECONDARY);
            canv.DrawRect(new Vector2(0, 120 + infoMenuY), new Vector2(canv.Width, canv.Height), Alignment.TopLeft);

            canv.Fill(TEXT);
            //canv.Font(smallTxt);
            canv.FontSize(35);
            canv.DrawText("This launcher is a game launcher made by viylouu in 2023.", new Vector2(20, infoMenuY + 160), Alignment.CenterLeft);
            canv.DrawText("It includes multiple games hand coded by viylouu himself.", new Vector2(20, infoMenuY + 200), Alignment.CenterLeft);
            canv.DrawText("The launcher is open sourced and all code can be found", new Vector2(20, infoMenuY + 250), Alignment.CenterLeft);
            canv.DrawText("in the github listed on the itch.io page.", new Vector2(20, infoMenuY + 290), Alignment.CenterLeft);
            canv.DrawText("All assets can be found on github or in the files for the", new Vector2(20, infoMenuY + 340), Alignment.CenterLeft);
            canv.DrawText("game itself, which includes all fonts, audio, sprites, and more.", new Vector2(20, infoMenuY + 380), Alignment.CenterLeft);
            canv.DrawText("The libraries used by viylouu are Simulation Framework, ", new Vector2(20, infoMenuY + 430), Alignment.CenterLeft);
            canv.DrawText("NAudio, and Newtonsoft.", new Vector2(20, infoMenuY + 470), Alignment.CenterLeft);
            canv.DrawText("Links:", new Vector2(20, infoMenuY + 570), Alignment.CenterLeft);
            canv.DrawText(@"github.com/Redninja106/simulationframework", new Vector2(20, infoMenuY + 610), Alignment.CenterLeft);
            canv.DrawText(@"github.com/viylouu/viylouuInc-Launcher", new Vector2(20, infoMenuY + 650), Alignment.CenterLeft);
            canv.DrawText(@"viylow.itch.io/viylouuinc-game-launcher", new Vector2(20, infoMenuY + 690), Alignment.CenterLeft);

            if (Keyboard.IsKeyPressed(Key.LeftArrow) && !settingsOpen && !infoOpen)
            {
                if (outs[1].PlaybackState is PlaybackState.Playing) { outs[1].Stop(); }
                ins[1].CurrentTime = new TimeSpan(0L);
                outs[1].Init(ins[1]);
                outs[1].Play();

                gameSelected -= 1;
                if (gameSelected < 0)
                { gameSelected = games.Length - 1; }
            }

            if (Keyboard.IsKeyPressed(Key.RightArrow) && !settingsOpen && !infoOpen)
            {
                if (outs[1].PlaybackState is PlaybackState.Playing) { outs[1].Stop(); }
                ins[1].CurrentTime = new TimeSpan(0L);
                outs[1].Init(ins[1]);
                outs[1].Play();

                gameSelected += 1;
                if (gameSelected > games.Length - 1)
                { gameSelected = 0; }
            }
        }

        if (gameStarted)
        {
            games[gameSelected].updater.Invoke();
        }
        else
        { framesUntilGameStarted++; }
    }

    static void UpdSep(double delta, Action act)
    {
        //pov: most simple code in entire project

        accum += delta;

        while (accum >= fps)
        {
            act.Invoke();
            accum -= fps;
        }
    }

    static void Fix()
    {
        ICanvas canv = Graphics.GetOutputCanvas();

        if (settingsOpen)
        {
            setMenuY += -setMenuY / 5;
        }
        else
        {
            setMenuY += (canv.Height - setMenuY) / 5;
        }

        if (infoOpen)
        {
            infoMenuY += -infoMenuY / 5;
        }
        else
        {
            infoMenuY += (canv.Height - infoMenuY) / 5;
        }
    }

    static bool rectPoint(Vector2 rp, Vector2 rs, Vector2 p)
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

        public string ver { get; set; }
    }

    static void ApplyColors(ref Color txt, ref Color bg, ref Color pri, ref Color sec, ref Color acc, bool light, int pallate, ref string pallatename)
    {
        ApplyColorToTex(settingsIcon, txt);
        ApplyColorToTex(homeIcon, txt);
        ApplyColorToTex(infoIcon, txt);
        ApplyColorToTex(closeIcon, txt);
    }

    static void ApplyColorToTex(ITexture tex, Color col)
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
