namespace Adept_AIO.Champions.Graves.Core
{
    using System.Collections.Generic;
    using Aimtec.SDK.Menu;
    using Aimtec.SDK.Menu.Components;
    using SDK.Delegates;
    using SDK.Menu_Extension;
    using SDK.Unit_Extensions;

    class MenuConfig
    {
        public static Menu Combo, Harass, LaneClear, JungleClear, Killsteal, Drawings;

        public MenuConfig()
        {
            var mainMenu = new Menu(string.Empty, $"Adept AIO - {Global.Player.ChampionName}", true);
            mainMenu.Attach();

            Global.Orbwalker.Attach(mainMenu);

            Combo = new Menu("GravesCombo", "Combo")
            {
                new MenuBool("Q", "Use Q"),
                new MenuBool("W", "Use W"),
                new MenuBool("WCC", "Only W On Hard CC"),
                new MenuBool("E", "Use E"),
                new MenuSliderBool("RHealth", "Use R | If Target Health % <=", true, 30),
                new MenuSliderBool("RCount", "Use R | If Enemies Count >=", true, 2, 1, 5)
            };

            Harass = new Menu("GravesHarass", "Harass")
            {
                new MenuSliderBool("Q", "Use Q | If Mana % >=", true, 15),
                new MenuSliderBool("W", "Use W | If Mana % >=", true, 30),
                new MenuSliderBool("E", "Use E | If Mana % >=", true, 50),
            };

            LaneClear = new Menu("GravesLane", "LaneClear")
            {
                new MenuBool("Check", "Don't Clear When Enemies Nearby"),
                new MenuSliderBool("Q", "Use Q | If Mana % >=", true, 25),
                new MenuSliderBool("E", "Use E | If Mana % >=", true, 20)
            };

            JungleClear = new Menu("GravesJungle", "JungleClear")
            {
                new MenuSliderBool("Q", "Use Q | If Mana % >=", true, 25),
                new MenuSliderBool("W", "Use W | If Mana % >=", true, 35),
                new MenuSliderBool("E", "Use E | If Mana % >=", true, 20),
            };

            Killsteal = new Menu("GravesKillsteal", "Killsteal")
            {
                new MenuBool("Q", "Use Q"),
                new MenuBool("R", "Use R"),
            };

            Drawings = new Menu("GravesDrawManager", "DrawManager")
            {
                new MenuBool("Dmg", "Damage"),
            };

            Gapcloser.Attach(mainMenu, "Anti Gapcloser");

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