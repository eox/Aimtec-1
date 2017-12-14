namespace Adept_AIO.Champions.Irelia.Core
{
    using System.Collections.Generic;
    using Aimtec.SDK.Menu;
    using Aimtec.SDK.Menu.Components;
    using SDK.Menu_Extension;
    using SDK.Unit_Extensions;

    class MenuConfig
    {
        private static Menu mainMenu;

        public static Menu Combo, Harass, Clear, Killsteal, Drawings;

        public static void Attach()
        {
            mainMenu = new Menu(string.Empty, $"Adept AIO - {Global.Player.ChampionName}", true);
            mainMenu.Attach();

            Global.Orbwalker.Attach(mainMenu);

            Combo = new Menu("Combo", "Combo")
            {
                new MenuBool("R", "R Minion To Gapclose Q"),
                new MenuBool("Q", "Use Q To Gapclose"),
                new MenuBool("Killable", "Q Target If Killable"),
                new MenuBool("Force", "Force Q To Stun (When Getting Ganked)"),
                new MenuSlider("Range", "Min. Range For Q", 450, 0, 650),
                new MenuList("Mode",
                    "Dash Mode: ",
                    new[]
                    {
                        "Cursor",
                        "Player Position"
                    },
                    0),
                new MenuBool("Turret", "Dash Turret When Killable")
            };

            Clear = new Menu("Clear", "Clear")
            {
                new MenuBool("Turret", "Check Turret"),
                new MenuBool("Check", "Avoid Farming When Nearby Enemies"),
                new MenuSliderBool("Q", "Use Q On Killable Minions (Min. Mana %)", true, 50),
                new MenuSliderBool("Lasthit", "Lasthit Q", true, 65),
                new MenuBool("W", "Use W (Jungle)"),
                new MenuSliderBool("E", "Use E On Big Mobs (Jungle) (Min. Mana %)", true, 50)
            };

            Harass = new Menu("Harass", "Harass")
            {
                new MenuBool("Away", "Safe Harass (Q Away)"),
                new MenuSliderBool("Q", "(Q) Min. Mana %", true, 50),
                new MenuBool("W", "Use W"),
                new MenuSliderBool("E", "(E) Min. Mana %", true, 50)
            };

            Killsteal = new Menu("Killsteal", "Killsteal")
            {
                new MenuBool("Q", "Q"),
                new MenuBool("E", "E"),
                new MenuBool("R", "R")
            };

            Drawings = new Menu("DrawManager", "DrawManager")
            {
                new MenuSlider("Segments", "Segments", 100, 10, 150).SetToolTip("Smoothness of the circles"),
                new MenuBool("Dmg", "Damage"),
                new MenuBool("Engage", "Draw Q Search Range"),
                new MenuBool("Q", "Q Range"),
                new MenuBool("R", "R Range")
            };

            foreach (var menu in new List<Menu>
            {
                Combo,
                Harass,
                Clear,
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