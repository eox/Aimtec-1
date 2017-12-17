namespace Adept_AIO.Champions.Lucian.Core
{
    using System.Collections.Generic;
    using Aimtec.SDK.Menu;
    using Aimtec.SDK.Menu.Components;
    using SDK.Delegates;
    using SDK.Menu_Extension;
    using SDK.Orbwalking;
    using SDK.Unit_Extensions;

    class MenuConfig
    {
        public static Menu Combo, Harass, LaneClear, JungleClear, Killsteal, Drawings;

        public MenuConfig()
        {
            var mainMenu = new Menu(string.Empty, $"Adept AIO - {Global.Player.ChampionName}", true);
            mainMenu.Attach();
            Orbwalker.Implementation.Attach(mainMenu);

            Gapcloser.Attach(mainMenu, "Anti Gapcloser");

            Combo = new Menu("Combo", "Combo")
            {
                new MenuList("E1",
                    "E Mode",
                    new[]
                    {
                        "After Auto",
                        "Engage"
                    },
                    0),
                new MenuList("Mode1",
                    "E To:",
                    new[]
                    {
                        "Cursor",
                        "Side"
                    },
                    1),
                new MenuBool("Q", "Use Q"),
                new MenuBool("W", "Use W"),
                new MenuBool("Last", "Use R When No Other Spells Avaible")
            };

            Harass = new Menu("Harass", "Harass")
            {
                new MenuList("E2",
                    "E Mode",
                    new[]
                    {
                        "After Auto",
                        "Engage"
                    },
                    0),
                new MenuList("Mode2",
                    "E To:",
                    new[]
                    {
                        "Cursor",
                        "Side"
                    },
                    1),
                new MenuBool("Q", "Use Q"),
                new MenuBool("W", "Use W")
            };

            LaneClear = new Menu("LucianLaneClear", "Lane")
            {
                new MenuBool("Check", "Dont' Clear When Enemies Nearby"),
                new MenuList("E3",
                    "E Mode",
                    new[]
                    {
                        "After Auto",
                        "Engage"
                    },
                    0),
                new MenuList("Mode3",
                    "E To:",
                    new[]
                    {
                        "Cursor",
                        "Side"
                    },
                    1),
                new MenuSliderBool("Q", "Min. Q Hit", true, 3, 1, 7),
                new MenuSliderBool("W", "Min. Minions Nearby To W", true, 3, 1, 7),
                new MenuSlider("Mana", "Min. Mana %", 40)
            };

            JungleClear = new Menu("Jungle", "Jungle")
            {
                new MenuList("E4",
                    "E Mode",
                    new[]
                    {
                        "After Auto",
                        "Engage"
                    },
                    0),
                new MenuList("Mode4",
                    "E To:",
                    new[]
                    {
                        "Cursor",
                        "Side"
                    },
                    1),
                new MenuBool("Q", "Use Q"),
                new MenuBool("W", "Use W")
            };

            Killsteal = new Menu("Killsteal", "Killsteal")
            {
                new MenuBool("Q", "Use Q"),
                new MenuBool("W", "Use W"),
                new MenuBool("E", "Use E (AA)"),
                new MenuBool("R", "Use R")
            };

            Drawings = new Menu("Drawings", "Drawings")
            {
                new MenuSlider("Segments", "Segments", 100, 10, 150).SetToolTip("Smoothness of the circles"),
                new MenuBool("Dmg", "Damage"),
                new MenuBool("Q", "Draw Q Range"),
                new MenuBool("Extended", "Draw Q Extended"),
                new MenuBool("Debug", "Debug")
            };

            foreach (var menu in new List<Menu>
            {
                Combo,
                Harass,
                LaneClear,
                JungleClear,
                Killsteal,
                Drawings,
                MenuShortcut.Credits
            })
            {
                mainMenu.Add(menu);
            }
        }
    }
}