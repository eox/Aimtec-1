namespace Adept_AIO.Champions.Rengar.Core
{
    using System.Collections.Generic;
    using Aimtec.SDK.Menu;
    using Aimtec.SDK.Menu.Components;
    using SDK.Menu_Extension;
    using SDK.Unit_Extensions;
    using GameObjects = Aimtec.SDK.Util.Cache.GameObjects;

    class MenuConfig
    {
        private static Menu mainMenu;

        public static Menu Combo, AssassinManager, LaneClear, JungleClear, Killsteal, Drawings;

        public static void Attach()
        {
            mainMenu = new Menu(string.Empty, $"Adept AIO - {Global.Player.ChampionName}", true);
            mainMenu.Attach();

            Global.Orbwalker.Attach(mainMenu);

            AssassinManager = new Menu("AssassinManager", "Assassin Manager");
            foreach (var hero in GameObjects.EnemyHeroes)
            {
                AssassinManager.Add(new MenuBool(hero.ChampionName, "Assassinate: " + hero.ChampionName, false));
            }

            Combo = new Menu("Combo", "Combo")
            {
                new MenuBool("Q", "Allow Empowered Q"),
                new MenuBool("W", "Use W To Stack"),
                new MenuBool("E", "Allow Empowered E"),
                new MenuList("Mode",
                    "Empowered W: ",
                    new[]
                    {
                        "Deal Damage",
                        "Against Hard CC"
                    },
                    1),
                new MenuSlider("Health", "Force W When Below (% HP)", 10)
            };

            LaneClear = new Menu("LaneClear", "LaneClear")
            {
                new MenuBool("Check", "Don't Clear When Enemies Nearby"),
                new MenuBool("Q", "Allow Empowered Q"),
                new MenuBool("W", "Allow Empowered W"),
                new MenuSlider("Health", "Force W When Below (% HP)", 10)
            };

            JungleClear = new Menu("JungleClear", "JungleClear")
            {
                new MenuBool("Q", "Allow Empowered Q"),
                new MenuBool("W", "Allow Empowered W"),
                new MenuBool("W2", "Use W To Stack"),
                new MenuSlider("Health", "Force W When Below (% HP)", 10)
            };

            Killsteal = new Menu("Killsteal", "Killsteal")
            {
                new MenuBool("Q", "Use Q"),
                new MenuBool("W", "Use W"),
                new MenuBool("E", "Use E")
            };

            Drawings = new Menu("DrawManager", "DrawManager")
            {
                new MenuSlider("Segments", "Segments", 100, 10, 150).SetToolTip("Smoothness of the circles"),
                new MenuBool("Dmg", "Damage"),
                new MenuBool("Q", "Draw Q Range", false),
                new MenuBool("W", "Draw W Range", false),
                new MenuBool("E", "Draw E Range")
            };

            foreach (var menu in new List<Menu>
            {
                AssassinManager,
                Combo,
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