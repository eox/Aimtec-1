namespace Adept_AIO.Champions.Zed.Core
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
        public static OrbwalkerMode FleeOrbwalkerMode;

        private static Menu mainMenu;

        public static Menu Combo, Harass, LaneClear, JungleClear, Lasthit, Killsteal, Misc, Drawings;

        public static void Attach()
        {
            mainMenu = new Menu(string.Empty, $"Adept AIO - {Global.Player.ChampionName}", true);
            mainMenu.Attach();

            FleeOrbwalkerMode = new OrbwalkerMode("Flee", KeyCode.A, null, Flee.OnKeyPressed);
            Global.Orbwalker.AddMode(FleeOrbwalkerMode);

            Global.Orbwalker.Attach(mainMenu);

            Combo = new Menu("Combo", "Combo")
            {
                new MenuBool("Q", "Use Q"),
                new MenuBool("W", "Use W"),
                new MenuBool("E", "Use E"),
                new MenuBool("R", "Use R"),
                new MenuList("Style", "Shadow Style", new []{"Triangle", "Line", "Cursor"}, 0),
                new MenuBool("Killable", "Only Use R When Killable"),
                new MenuSeperator("Whitelist")
            };

            foreach (var enemy in GameObjects.EnemyHeroes)
            {
                Combo.Add(new MenuBool(enemy.ChampionName, $"Use R On: {enemy.ChampionName}"));
            }

            Harass = new Menu("Harass", "Harass")
            {
                new MenuBool("Q", "Use Q"),
                new MenuBool("W", "Use W"),
                new MenuSliderBool("W2", "Use W Twice | Not If >= X Enemies", true, 2, 1, 5).SetToolTip("Max Range W, will try to W AA "),
                new MenuSlider("Health", "Don't Use W2 if % HP is Below", 30),
                new MenuBool("E", "Use E"),
                new MenuSlider("Energy", "Min. Energy %", 40)
            };

            LaneClear = new Menu("Lane", "LaneClear")
            {
                new MenuBool("Check", "Don't Clear When Enemies Nearby"),
                new MenuBool("TurretFarm", "Smart Under Turret Farm"),
                new MenuSliderBool("Q", "Use Q | Min. Minions Hit", true, 3, 1, 6),
                new MenuBool("W", "Use W"),
                new MenuSliderBool("E", "Use E | Min. Minions Hit", true, 3, 1, 6),
                new MenuSlider("Energy", "Min. Energy %", 40)
            };

            JungleClear = new Menu("Jungle", "JungleClear")
            {
                new MenuBool("Q", "Use Q"),
                new MenuBool("E", "Use E"),
                new MenuSlider("Energy", "Min. Energy %", 40)
            };

            Lasthit = new Menu("Lasthit", "Lasthit")
            {
                new MenuBool("Q", "Use Q"),
                new MenuBool("E", "Use E"),
                new MenuSlider("Energy", "Min. Energy %", 40)
            };

            Killsteal = new Menu("Killsteal", "Killsteal")
            {
                new MenuBool("Q", "Use Q"),
                new MenuBool("E", "Use E")
            };

            Misc = new Menu("Misc", "Miscellaneous")
            {
                new MenuBool("Q", "Always Use Q On Enemies", false),
                new MenuBool("E", "Use E To Slow Enemies"),
                new MenuBool("R", "Use R To Dodge Enemy Spells"),
                new MenuSlider("Health", "Dodge if % Health is below", 20),
                new MenuBool("W", "Anti-Gapclose with W")
            };

            Drawings = new Menu("DrawManager", "DrawManager")
            {
                new MenuSlider("Segments", "Segments", 100, 10, 150).SetToolTip("Smoothness of the circles"),
                new MenuBool("Dmg", "Damage"),
                new MenuBool("Range", "Draw Engage Range"),
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
                Lasthit,
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