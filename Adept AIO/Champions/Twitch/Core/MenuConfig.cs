namespace Adept_AIO.Champions.Twitch.Core
{
    using System.Collections.Generic;
    using Aimtec.SDK.Menu;
    using Aimtec.SDK.Menu.Components;
    using SDK.Menu_Extension;
    using SDK.Orbwalking;
    using SDK.Unit_Extensions;

    class MenuConfig
    {
        public static Menu MainMenu, Combo, Harass, LaneClear, JungleClear, Killsteal, Drawings;

        public MenuConfig()
        {
            MainMenu = new Menu(string.Empty, $"Adept AIO - {Global.Player.ChampionName}", true);
            MainMenu.Attach();
            MainMenu.Add(new MenuBool("Stealth", "Stealth Recall"));
            Orbwalker.Implementation.Attach(MainMenu);

            Combo = new Menu("TwitchCombo", "Combo")
            {
                new MenuBool("Q", "Use Q (For Attackspeed)"),
                new MenuBool("W", "Use W"),
                new MenuBool("W2", "Don't Use W When Ult Active"),
                new MenuBool("R", "Use R"),
                new MenuSlider("R2", "Use R If X Enemies Nearby", 3, 0, 5)
            };

            Harass = new Menu("TwitchHarass", "Harass")
            {
                new MenuBool("Q", "Use Q", false),
                new MenuBool("W", "Use W")
            };

            LaneClear = new Menu("TwitchLaneClear", "Lane")
            {
                new MenuBool("Check", "Dont' Clear When Enemies Nearby"),
                new MenuSliderBool("W", "Min. W Hit", true, 4, 1, 7),
                new MenuSliderBool("E", "Min. E Killed", true, 4, 1, 7)
            };

            JungleClear = new Menu("TwitchJungle", "Jungle")
            {
                new MenuBool("E", "Use E"),
                new MenuBool("W", "Use W")
            };

            Killsteal = new Menu("TwitchKillsteal", "Killsteal")
            {
                new MenuBool("E", "Use E (AA)")
            };

            Drawings = new Menu("TwitchDrawings", "Drawings")
            {
                new MenuSlider("Segments", "Segments", 100, 10, 150).SetToolTip("Smoothness of the circles"),
                new MenuBool("Dmg", "Damage"),
                new MenuBool("World", "Draw Q Time (WORLD)"),
                new MenuBool("Map", "Draw Q Time (MAP)"),
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
                MainMenu.Add(menu);
            }
        }
    }
}