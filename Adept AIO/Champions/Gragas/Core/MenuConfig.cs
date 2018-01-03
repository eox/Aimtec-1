namespace Adept_AIO.Champions.Gragas.Core
{
    using System.Collections.Generic;
    using Aimtec.SDK.Menu;
    using Aimtec.SDK.Menu.Components;
    using Aimtec.SDK.Util;
    using OrbwalkingEvents;
    using SDK.Delegates;
    using SDK.Menu_Extension;
    using SDK.Unit_Extensions;
    using OrbwalkerMode = SDK.Orbwalking.OrbwalkerMode;

    class MenuConfig
    {
        public static Menu InsecMenu, Combo, Harass, Lane, Jungle, Automatic, Drawings;

        public MenuConfig()
        {
            var mainMenu = new Menu(string.Empty, $"Adept AIO - {Global.Player.ChampionName}", true);
            mainMenu.Attach();

            Global.Orbwalker.Attach(mainMenu);
            Gragas.InsecOrbwalkerMode = new OrbwalkerMode("Insec", KeyCode.T, () => Global.TargetSelector.GetSelectedTarget(), Insec.OnKeyPressed);
            Global.Orbwalker.AddMode(Gragas.InsecOrbwalkerMode);

            Gapcloser.Attach(mainMenu, "Anti Gapcloser");

            InsecMenu = new Menu("GragasInsec", "Insec")
            {
                new MenuBool("Flash", "Flash | E Flash"),
                //  new MenuSliderBool("Auto", "Auto Insec If X Hit", true, 3, 2, 5),
                new MenuBool("Q", "Use Q Before R")
            };

            Combo = new Menu("GragasCombo", "Combo")
            {
                new MenuBool("Q", "Use Q"),
                new MenuBool("W", "Use W"),
                new MenuBool("Flash", "Flash | E Flash"),
                new MenuBool("E", "Use E"),
                new MenuBool("R", "Use R"),
                new MenuBool("Killable", "R | ONLY If Killable")
            };

            Harass = new Menu("GragasHarass", "Harass")
            {
                new MenuBool("Q", "Use Q"),
                new MenuBool("W", "Use W"),
                new MenuBool("E", "Use E")
            };

            Lane = new Menu("GragasLane", "LaneClear")
            {
                new MenuBool("Check", "Safe Clear"),
                new MenuSliderBool("Q", "Use Q If minimum hit", true, 4, 1, 7),
                new MenuSlider("QMana", "Q | Minimum Mana %", 35),
                new MenuBool("W", "Use W"),
                new MenuSliderBool("E", "Use E If minimum hit", true, 3, 1, 7),
                new MenuSlider("EMana", "E | Minimum Mana %", 40)
            };

            Jungle = new Menu("GragasJungle", "Jungle")
            {
                new MenuBool("Q", "Use Q"),
                new MenuBool("W", "Use W"),
                new MenuBool("E", "Use E")
            };

            Automatic = new Menu("GragasAuto", "Automatic")
            {
                new MenuBool("Q", "Auto Q (Smart)"),
                new MenuBool("E", "Killsteal E"),
                new MenuBool("Disengage", "R | Disengage", false)
            };

            Drawings = new Menu("GragasDrawManager", "DrawManager")
            {
                new MenuSlider("Segments", "Segments", 100, 10, 150).SetToolTip("Smoothness of the circles"),
                new MenuBool("Dmg", "Damage"),
                new MenuBool("Q", "Draw Q Range"),
                new MenuBool("R", "Draw R Range"),
                new MenuBool("Debug", "Debug DrawManager")
            };

            foreach (var menu in new List<Menu>
            {
                InsecMenu,
                Combo,
                Harass,
                Lane,
                Jungle,
                Automatic,
                Drawings,
                MenuShortcut.Credits
            })
            {
                mainMenu.Add(menu);
            }
        }
    }
}