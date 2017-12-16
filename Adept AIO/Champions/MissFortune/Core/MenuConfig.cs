namespace Adept_AIO.Champions.MissFortune.Core
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
        public static Menu Combo, Harass, LaneClear, JungleClear, Automatic, Drawings;

        public MenuConfig()
        {
            var mainMenu = new Menu(string.Empty, $"Adept AIO - {Global.Player.ChampionName}", true);
            mainMenu.Attach();

            Orbwalker.Implementation.Attach(mainMenu);

            Combo = new Menu("MissFortuneCombo", "Combo")
            {
                new MenuBool("Q", "Use Q"),
                new MenuBool("W", "Use W"),
                new MenuBool("E", "Use E")
            };

            Harass = new Menu("MissFortuneHarass", "Harass")
            {
                new MenuSliderBool("Q", "Use Q | If Mana % >=", true, 50),
                new MenuSliderBool("W", "Use W | If Mana % >=", false, 50),
                new MenuSliderBool("E", "Use E | If Mana % >=", false, 50),
            };

            LaneClear = new Menu("MissFortuneLane", "LaneClear")
            {
                new MenuBool("Check", "Don't Clear When Enemies Nearby"),
                new MenuSliderBool("Q", "Use Q | If Mana % >=", true, 50),
                new MenuSliderBool("W", "Use W | If Mana % >=", true, 50),
                new MenuSliderBool("E", "Use E | If Mana % >=", true, 50)
            };

            JungleClear = new Menu("MissFortuneJungle", "JungleClear")
            {
                new MenuSliderBool("Q", "Use Q | If Mana % >=", true, 35),
                new MenuSliderBool("W", "Use W | If Mana % >=", true, 35),
                new MenuSliderBool("E", "Use E | If Mana % >=", true, 50),
            };

            Automatic = new Menu("MissFortuneAuto", "Automatic")
            {
                new MenuBool("Path", "Auto Path To Q Extend"),
                new MenuBool("Q", "Use Q (Killable"),
                new MenuBool("QAuto", "Use Q (When Enemy Can Be Hit)"),
                new MenuBool("RCC", "Use R (Hard CC")
            };

            Drawings = new Menu("MissFortuneDrawManager", "DrawManager")
            {
                new MenuBool("Dmg", "Damage"),
                new MenuBool("Cone", "Q Cone")
            };

            Gapcloser.Attach(mainMenu, "Anti Gapcloser");

            foreach (var menu in new List<Menu>
            {
                Combo,
                Harass,
                LaneClear,
                JungleClear,
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