namespace Adept_AIO.Champions.Zoe.Core
{
    using System.Collections.Generic;
    using Aimtec.SDK.Menu;
    using Aimtec.SDK.Menu.Components;
    using SDK.Delegates;
    using SDK.Menu_Extension;
    using SDK.Unit_Extensions;

    class MenuConfig
    {
        private static Menu mainMenu;

        public static Menu Combo, Harass, LaneClear, JungleClear, Killsteal, Drawings;

        public MenuConfig()
        {
            mainMenu = new Menu(string.Empty, $"Adept AIO - {Global.Player.ChampionName}", true);
            mainMenu.Attach();

            Global.Orbwalker.Attach(mainMenu);

            Combo = new Menu("ZoeCombo", "Combo")
            {
                new MenuBool("Q", "Use Q"),
                new MenuBool("QHit", "Q ONLY if target has CC", false),
                new MenuSliderBool("W", "Use Aggressive W | When target HP % <= ", true, 70),
                new MenuSliderBool("WP", "Use Defensive W | When Player HP % <= ", true, 70),
                new MenuBool("Flash", "Flash (W) After Ult"),
                new MenuBool("E", "Use E"),
                new MenuBool("R", "Use R"),
            };

            Harass = new Menu("ZoeHarass", "Harass")
            {
                new MenuBool("Q", "Use Q"),
                new MenuBool("E", "Use E", false)
            };

            LaneClear = new Menu("ZoeLane", "LaneClear")
            {
                new MenuBool("Check", "Don't Clear When Enemies Nearby"),
                new MenuBool("Q", "Use Q")
            };

            JungleClear = new Menu("ZoeJungle", "JungleClear")
            {
                new MenuBool("Q", "Use Q"),
                new MenuBool("E", "Use E"),
                new MenuBool("R", "Use R"),
            };

            Killsteal = new Menu("ZoeKillsteal", "Killsteal")
            {
                new MenuBool("Q", "Use Q"),
                new MenuBool("E", "Use E")
            };

            Drawings = new Menu("ZoeDrawManager", "DrawManager")
            {
                new MenuSlider("Segments", "Segments", 100, 10, 150).SetToolTip("Smoothness of the circles"),
                new MenuBool("Dmg", "Damage"),
                new MenuBool("Q", "Draw Q Range"),
                new MenuBool("Pred", "Draw Q Prediction")
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