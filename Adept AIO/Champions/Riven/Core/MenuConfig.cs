namespace Adept_AIO.Champions.Riven.Core
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
        public static Menu Combo, BurstMenu, Harass, Lane, Jungle, Killsteal, Miscellaneous, Drawings;

        public static OrbwalkerMode BurstMode, FleeMode;

        public MenuConfig()
        {
            var mainMenu = new Menu(string.Empty, $"Adept AIO - {Global.Player.ChampionName}", true);
            mainMenu.Attach();

            FleeMode = new OrbwalkerMode("Flee", KeyCode.A, null, Flee.OnKeyPressed);
            BurstMode = new OrbwalkerMode("Burst", KeyCode.T, () => Global.TargetSelector.GetSelectedTarget(), Burst.OnUpdate);

            BurstMode.ModeBehaviour.Invoke();
            BurstMode.GetTargetImplementation.Invoke();

            Global.Orbwalker.AddMode(BurstMode);
            Global.Orbwalker.AddMode(FleeMode);

            Global.Orbwalker.Attach(mainMenu);
            Gapcloser.Attach(mainMenu, "Anti Gapcloser");

            Combo = new Menu("RivenCombo", "Combo")
            {
                //new MenuList("Mode", "Combo Mode: ", new[] {"Automatic", "Max Damage", "Fast"}, 0),
                new MenuSlider("Change", "Fast Combo When DMG% (target) >= ", 70),
                new MenuList("Chase",
                    "Chase Mode",
                    new[]
                    {
                        "Disabled",
                        "Q",
                        "Q & E"
                    },
                    0),
                new MenuBool("Flash", "Flash").SetToolTip("Will flash when an target is safely killable."),
                new MenuSliderBool("Check", "Don't Use R1 If X (% HP) <=", true, 20),
                new MenuList("R",
                    "R1 Mode: ",
                    new[]
                    {
                        "Never",
                        "Always",
                        "Killable"
                    },
                    2),
                new MenuBool("R2", "Use R2")
            };

            BurstMenu = new Menu("RivenBurst", "Burst")
            {
                new MenuSeperator("Note", "Select Target To Burst"),
                new MenuList("Mode",
                    "Burst Mode:",
                    new[]
                    {
                        "Automatic",
                        "The Shy",
                        "Execution"
                    },
                    0),
                new MenuSeperator("endmylife")
            };
            foreach (var hero in GameObjects.EnemyHeroes)
            {
                BurstMenu.Add(new MenuBool(hero.ChampionName, "Burst: " + hero.ChampionName));
            }

            Harass = new Menu("RivenHarass", "Harass")
            {
                new MenuList("Mode",
                    "Mode: ",
                    new[]
                    {
                        "Automatic",
                        "Semi Combo",
                        "Q3 To Safety",
                        "Q3 To Target"
                    },
                    0),
                new MenuList("Dodge",
                    "Dodge: ",
                    new[]
                    {
                        "Turret",
                        "Cursor",
                        "Away From Target"
                    },
                    0),
                new MenuSeperator("Whitelist", "Whitelist")
            };
            foreach (var hero in GameObjects.EnemyHeroes)
            {
                Harass.Add(new MenuBool(hero.ChampionName, "Harass: " + hero.ChampionName));
            }

            Lane = new Menu("RivenLane", "Lane")
            {
                new MenuBool("Check", "Safe Clear").SetToolTip("Wont clear when enemies are nearby"),
                new MenuBool("Q", "Q"),
                new MenuBool("W", "W"),
                new MenuBool("E", "E")
            };

            Jungle = new Menu("RivenJungle", "Jungle")
            {
                new MenuBool("Check", "Safe Clear").SetToolTip("Wont clear when enemies are nearby"),
                new MenuBool("Q", "Q"),
                new MenuBool("W", "W"),
                new MenuBool("E", "E")
            };

            Killsteal = new Menu("RivenKillsteal", "Killsteal")
            {
                new MenuBool("Ignite", "Ignite"),
                new MenuBool("Q", "Q"),
                new MenuBool("W", "W"),
                new MenuBool("R2", "R2")
            };

            Miscellaneous = new Menu("RivenMiscellaneous", "Miscellaneous")
            {
                new MenuBool("Walljump", "Walljump During Flee"),
                new MenuBool("Force", "Spam Q1, Q2 During Flee"),
                new MenuBool("Active", "Keep Q Active"),
                new MenuBool("Interrupt", "Dodge Certain Spells")
            };

            Drawings = new Menu("RivenDrawManager", "DrawManager")
            {
                new MenuSlider("Segments", "Segments", 100, 10, 150).SetToolTip("Smoothness of the circles"),
                new MenuBool("Dmg", "Damage"),
                new MenuBool("Mouse", "Mouse Helper").SetToolTip("Shows where to put mouse to properly Q AA chase the target"),
                new MenuBool("Target", "Draw Line At Target"),
                new MenuBool("Engage", "Engage Range"),
                new MenuBool("R2", "R2 Range", false),
                new MenuBool("Pattern", "Current Pattern")
            };

            foreach (var menu in new List<Menu>
            {
                Combo,
                BurstMenu,
                Harass,
                Lane,
                Jungle,
                Killsteal,
                Drawings,
                Miscellaneous,
                MenuShortcut.Credits
            })
            {
                mainMenu.Add(menu);
            }
        }
    }
}