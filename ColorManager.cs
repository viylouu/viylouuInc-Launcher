using SimulationFramework;
using static viylouuInc_Launcher.ColorManager;

namespace viylouuInc_Launcher
{
    internal class ColorManager
    {
        public static colPallate darkGreen = new colPallate
        { txtCol = new Color(246, 249, 240), bgCol = new Color(15, 19, 7), priCol = new Color(38, 82, 30),
            secCol = new Color(17, 44, 26), accCol = new Color(76, 189, 116) };
        public static colPallate lightGreen = new colPallate
        { txtCol = new Color(12, 15, 6), bgCol = new Color(244, 248, 236), priCol = new Color(181, 225, 173),
            secCol = new Color(211, 238, 220), accCol = new Color(66, 179, 106) };

        public static colPallate darkRed = new colPallate
        { txtCol = new Color(232, 250, 253), bgCol = new Color(1, 12, 14), priCol = new Color(102, 22, 10),
            secCol = new Color(55, 37, 6), accCol = new Color(233, 32, 106) };
        public static colPallate lightRed = new colPallate
        { txtCol = new Color(2, 20, 23), bgCol = new Color(241, 252, 254), priCol = new Color(245, 165, 153),
            secCol = new Color(249, 231, 200), accCol = new Color(223, 22, 96) };

        public static colPallate darkBlue = new colPallate
        { txtCol = new Color(251, 253, 254), bgCol = new Color(7, 26, 44), priCol = new Color(49, 138, 221),
            secCol = new Color(55, 37, 6), accCol = new Color(233, 32, 106) };
        public static colPallate lightBlue = new colPallate
        { txtCol = new Color(1, 3, 4), bgCol = new Color(211, 230, 248), priCol = new Color(34, 123, 206),
            secCol = new Color(203, 225, 246), accCol = new Color(186, 94, 110) };

        public static dlPallate green = new dlPallate { DarkMode = darkGreen, LightMode = lightGreen, name = "Mint" };
        public static dlPallate red = new dlPallate { DarkMode = darkRed, LightMode = lightRed, name = "Rose" };
        public static dlPallate blue = new dlPallate { DarkMode = darkBlue, LightMode = lightBlue, name = "Sapphire" };

        public static dlPallate[] pallates = new dlPallate[] {
            green,
            red,
            blue
        };

        public void ApplyColors(ref Color txt, ref Color bg, ref Color pri, ref Color sec, ref Color acc, bool light, int pallate)
        { 
            txt = light? pallates[pallate].LightMode.txtCol : pallates[pallate].DarkMode.txtCol;
            bg = light? pallates[pallate].LightMode.bgCol : pallates[pallate].DarkMode.bgCol;
            pri = light? pallates[pallate].LightMode.priCol : pallates[pallate].DarkMode.priCol;
            sec = light? pallates[pallate].LightMode.secCol : pallates[pallate].DarkMode.secCol;
            acc = light? pallates[pallate].LightMode.accCol : pallates[pallate].DarkMode.accCol;
        }

        public class colPallate
        { 
            public Color txtCol { get; set; }
            public Color bgCol { get; set; }

            public Color priCol { get; set; }
            public Color secCol { get; set; }
            public Color accCol { get; set; }
        }

        public class dlPallate
        {
            public colPallate DarkMode { get; set; }
            public colPallate LightMode { get; set; }

            public string name { get; set; }
        }
    }
}
