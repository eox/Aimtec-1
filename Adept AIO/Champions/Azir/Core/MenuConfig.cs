namespace Adept_AIO.Champions.Azir.Core
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
        public static Menu Combo, InsecMenu, Harass, Lane, Jungle, Killsteal, Drawings;

        public static void Attach()
        {
            var mainMenu = new Menu(string.Empty, $"Adept AIO - {Global.Player.ChampionName}", true);
            mainMenu.Attach();

            Global.Orbwalker.Attach(mainMenu);
            Gapcloser.Attach(mainMenu, "Anti Gapcloser");

            AzirHelper.JumpMode = new OrbwalkerMode("Jump (Cursor)", KeyCode.A, null, Flee.OnKeyPressed);
            AzirHelper.InsecMode = new OrbwalkerMode("Insec", KeyCode.T, () => null, null);

            Global.Orbwalker.AddMode(AzirHelper.JumpMode);
            Global.Orbwalker.AddMode(AzirHelper.InsecMode);

            InsecMenu = new Menu("AzirInsec", "Insec")
            {
                new MenuBool("Flash", "Flash"),
                new MenuSlider("Range", "Q | Distance To Soldier <=", 600, 450, 800),
                new MenuSliderBool("Auto", "Auto Insec If X Hit", true, 3, 2, 5)
            };

            Combo = new Menu("AzirCombo", "Combo")
            {
                new MenuBool("Q", "Use Q"),
                new MenuBool("Extend", "Extended Q"),
                new MenuSlider("QCount", "Q If X Soldiers Alive", 2, 1, 4),
                new MenuBool("W", "Use W"),
                new MenuBool("E", "Use E"),
                new MenuSlider("EDmg", "Force E When Health% <= ", 80),
                new MenuBool("R", "Use R")
            };

            Harass = new Menu("AzirHarass", "Harass")
            {
                new MenuSliderBool("Q", "Use Q (min. Mana%)", true, 40),
                new MenuSliderBool("W", "Use W (min. Mana%)", true, 25),
                new MenuSliderBool("E", "Use E (min. Mana%)", false, 35)
            };

            Lane = new Menu("AzirLane", "Lane")
            {
                new MenuBool("Check", "Safe Clear"),
                new MenuSliderBool("Q", "Use Q (min Mana%)", true, 40),
                new MenuSlider("QHit", "Min Hit By Q", 3, 1, 7),
                new MenuSliderBool("W", "Use W (min Mana%)", true, 25),
                new MenuSliderBool("E", "Use E (min Mana%)", false, 70),
                new MenuSlider("EHit", "Min Hit By E", 4, 1, 7)
            };

            Jungle = new Menu("AzirJungle", "Jungle")
            {
                new MenuSliderBool("Q", "Use Q (min. Mana%)", true, 40),
                new MenuSliderBool("W", "Use W (min. Mana%)", true, 25)
            };

            Killsteal = new Menu("AzirKillsteal", "Killsteal")
            {
                new MenuBool("Q", "Use Q"),
                new MenuBool("E", "Use E")
            };

            Drawings = new Menu("AzirDrawManager", "DrawManager")
            {
                new MenuSlider("Segments", "Segments", 100, 10, 150).SetToolTip("Smoothness of the circles"),
                new MenuBool("Dmg", "Damage"),
                new MenuBool("Soldiers", "Draw Soldiers"),
                new MenuBool("Q", "Draw Q Range"),
                new MenuBool("R", "Draw R Range")
            };

            foreach (var menu in new List<Menu>
            {
                InsecMenu,
                Combo,
                Harass,
                Lane,
                Jungle,
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