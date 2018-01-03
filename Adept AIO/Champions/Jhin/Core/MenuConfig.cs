namespace Adept_AIO.Champions.Jhin.Core
{
    using System.Collections.Generic;
    using Aimtec.SDK.Menu;
    using Aimtec.SDK.Menu.Components;
    using SDK.Delegates;
    using SDK.Menu_Extension;
    using SDK.Unit_Extensions;

    class MenuConfig
    {
        public static Menu Combo, Harass, LaneClear, JungleClear, Killsteal, Misc, Drawings;

        public MenuConfig()
        {
            var mainMenu = new Menu(string.Empty, $"Adept AIO - {Global.Player.ChampionName}", true);
            mainMenu.Attach();
            Global.Orbwalker.Attach(mainMenu);

            Gapcloser.Attach(mainMenu, "Anti Gapcloser");

            Combo = new Menu("JhinCombo", "Combo")
            {
               new MenuBool("Q", "Use Q"),
               new MenuBool("E", "Use E"),
               new MenuBool("R", "Auto R (Smart)", false)
            };

            Harass = new Menu("JhinHarass", "Harass")
            {
                new MenuBool("Q", "Use Q"),
                new MenuBool("E", "Use E"),
            };

            LaneClear = new Menu("JhinLaneClear", "Lane")
            {
                new MenuBool("Check", "Dont' Clear When Enemies Nearby"),
                new MenuSliderBool("Q", "Min. Q Hit", true,  4, 1, 4),
                new MenuSliderBool("E", "Min. E Hit", false, 4, 1, 7),
            };

            JungleClear = new Menu("JhinJungle", "Jungle")
            {
                new MenuBool("Q", "Use Q"),
                new MenuBool("W", "Use W", false),
                new MenuBool("E", "Use E")
            };

            Killsteal = new Menu("JhinKillsteal", "Killsteal")
            {
                new MenuBool("Q", "Use Q"),
                new MenuBool("W", "Use W"),
                new MenuBool("E", "Use E"),
            };

            Misc = new Menu("JhinMisc", "Miscellaneous")
            {
                new MenuBool("W", "Use W Automatically").SetToolTip("Will use W when enemy can be stunned by W"),
                new MenuBool("E", "Anti-Gapcloser (E)")
            };

            Drawings = new Menu("JhinDrawings", "Drawings")
            {
                new MenuSlider("Segments", "Segments", 100, 10, 150).SetToolTip("Smoothness of the circles"),
                new MenuBool("Dmg", "Damage"),
                new MenuBool("R", "R Range"),
                new MenuBool("Debug", "Debug")
            };

            foreach (var menu in new List<Menu>
            {
                Combo,
                Harass,
                LaneClear,
                JungleClear,
                Killsteal,
                Misc,
                Drawings,
                MenuShortcut.Credits
            })
            {
                mainMenu.Add(menu);
            }
        }
    }
}