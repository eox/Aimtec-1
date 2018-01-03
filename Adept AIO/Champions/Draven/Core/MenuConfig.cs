namespace Adept_AIO.Champions.Draven.Core
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

            Combo = new Menu("DravenCombo", "Combo")
            {
                new MenuBool("Q", "Use Q"),
                new MenuBool("W", "Use W").SetToolTip("To Catch Axe OR Kite"),
                new MenuBool("E", "Use E"),
                new MenuSliderBool("R", "Use R | If Target Health % <=", true, 30)
            };

            Harass = new Menu("DravenHarass", "Harass")
            {
                new MenuSliderBool("Q", "Use Q | If Mana % >=", true, 15),
                new MenuSliderBool("W", "Use W | If Mana % >=", true, 30),
                new MenuSliderBool("E", "Use E | If Mana % >=", true, 50),
            };

            LaneClear = new Menu("DravenLane", "LaneClear")
            {
                new MenuBool("Check", "Don't Clear When Enemies Nearby"),
                new MenuSliderBool("Q", "Use Q | If Mana % >=", true, 20),
                new MenuSliderBool("W", "Use W | If Mana % >=", true, 25)
            };

            JungleClear = new Menu("DravenJungle", "JungleClear")
            {
                new MenuSliderBool("Q", "Use Q | If Mana % >=", true, 20),
                new MenuSliderBool("W", "Use W | If Mana % >=", true, 25),
                new MenuSliderBool("E", "Use E | If Mana % >=", true, 45),
            };

            Killsteal = new Menu("DravenKillsteal", "Killsteal")
            {
                new MenuBool("E", "Use E"),
                new MenuBool("R", "Use R"),
            };

            Misc = new Menu("DravenMisc", "Miscellaneous")
            {
                new MenuList("Catch", "Catch Mode:", new []{"Always", "Combo Only", "Disabled"}, 0),
                new MenuSlider("Range", "Catch Range (From Cursor)", 600, 300, 1000),
                new MenuBool("W", "Use W If Axe Too Far Away")
            };

            Drawings = new Menu("DravenDrawManager", "DrawManager")
            {
                new MenuBool("Dmg", "Damage"),
                new MenuBool("Catch", "Draw Axe Catch Range"),
                new MenuBool("Axe", "Draw Axe Position")
            };

            Gapcloser.Attach(mainMenu, "Anti Gapcloser");

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