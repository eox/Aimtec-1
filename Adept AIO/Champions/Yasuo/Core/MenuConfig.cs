namespace Adept_AIO.Champions.Yasuo.Core
{
    using System.Collections.Generic;
    using Aimtec.SDK.Menu;
    using Aimtec.SDK.Menu.Components;
    using Aimtec.SDK.Util;
    using OrbwalkingEvents;
    using SDK.Delegates;
    using SDK.Menu_Extension;
    using SDK.Unit_Extensions;
    using GameObjects = Aimtec.SDK.Util.Cache.GameObjects;
    using OrbwalkerMode = SDK.Orbwalking.OrbwalkerMode;

    class MenuConfig
    {
        public static Menu Combo, Whitelist, Harass, LaneClear, JungleClear, Killsteal, Misc, Drawings;

        public static void Attach()
        {
            var mainMenu = new Menu(string.Empty, $"Adept AIO - {Global.Player.ChampionName}", true);
            mainMenu.Attach();

            Extension.FleeMode = new OrbwalkerMode("Flee", KeyCode.A, null, Flee.OnKeyPressed);
            Extension.BeybladeMode = new OrbwalkerMode("Keyblade", KeyCode.T, () => Global.TargetSelector.GetSelectedTarget(), Beyblade.OnKeyPressed);

            Extension.BeybladeMode.ModeBehaviour.Invoke();
            Extension.BeybladeMode.GetTargetImplementation.Invoke();

            Global.Orbwalker.AddMode(Extension.FleeMode);
            Global.Orbwalker.AddMode(Extension.BeybladeMode);

            Global.Orbwalker.Attach(mainMenu);

            Whitelist = new Menu("Whitelist", "Whitelist");
            foreach (var hero in GameObjects.EnemyHeroes)
            {
                Whitelist.Add(new MenuBool(hero.ChampionName, "Use R Against: " + hero.ChampionName));
            }

            Combo = new Menu("YCombo", "Combo")
            {
                new MenuBool("Walk", "Walk Behind Minion To Dash"),
                new MenuBool("Dodge", "Windwall Targetted Spells"),
                new MenuSlider("Count", "Use R If X Airbourne", 2, 0, 5),
                new MenuBool("Flash", "Use Flash (Beyblade)").SetToolTip("Will try to E-Q -> Flash. AKA Beyblade"),
                new MenuBool("Turret", "Avoid Using E Under Turret"),
                new MenuList("Dash",
                    "Dash Mode: ",
                    new[]
                    {
                        "Cursor",
                        "From Player"
                    },
                    0),
                new MenuSlider("Range", "Mouse Dash Range: ", 650, 1, 1000),
                new MenuSlider("MRange", "Search Range For Behind Minions", 180, 100, 240)
            };

            // Todo: Add Check and go: EQ AA -> E Out 
            Harass = new Menu("Harass", "Harass")
            {
                new MenuBool("Q", "Use Q3"),
                new MenuBool("E", "Use E")
            };

            LaneClear = new Menu("YLane", "LaneClear")
            {
                new MenuBool("Check", "Don't Clear When Enemies Nearby"),
                new MenuBool("Turret", "Don't Clear Under Turret"),
                new MenuBool("Q3", "Use Q3"),
                new MenuBool("EAA", "Only E After AA"),
                new MenuList("Mode",
                    "E Mode: ",
                    new[]
                    {
                        "Disabled",
                        "Lasthit",
                        "Fast Clear"
                    },
                    1)
            };

            JungleClear = new Menu("YJungle", "JungleClear")
            {
                new MenuBool("Q3", "Allow Q3 Usage"),
                new MenuBool("Q", "Allow Q1 Usage"),
                new MenuBool("E", "Allow E  Usage")
            };

            Killsteal = new Menu("YKillsteal", "Killsteal")
            {
                new MenuBool("Ignite", "Ignite"),
                new MenuBool("Q", "Use Q"),
                new MenuBool("Q3", "Use Q3"),
                new MenuBool("E", "Use E")
            };

            Misc = new Menu("YMisc", "Miscellaneous")
            {
                new MenuBool("Stack", "Stack Q").SetToolTip("Wont Stack when enemy is within 900 units."),
                new MenuBool("LasthitE", "Lasthit With E"),
                new MenuBool("LasthitQ", "Lasthit With Q"),
                new MenuBool("LasthitQ3", "Lasthit With Tornado (Q3)")
            };

            Drawings = new Menu("YDrawManager", "Drawings")
            {
                new MenuSlider("Segments", "Segments", 100, 10, 150).SetToolTip("Smoothness of the circles"),
                new MenuBool("Dmg", "Damage"),
                new MenuBool("R", "Draw R Range"),
                new MenuBool("Path", "Draw Minion Path"),
                new MenuBool("Range", "Draw Minion Search Range"),
                new MenuBool("Debug", "Debug")
            };

            Gapcloser.Attach(mainMenu, "Anti Gapcloser");

            foreach (var menu in new List<Menu>
            {
                Whitelist,
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