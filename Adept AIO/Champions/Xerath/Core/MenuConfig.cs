namespace Adept_AIO.Champions.Xerath.Core
{
    using System.Collections.Generic;
    using Aimtec.SDK.Menu;
    using Aimtec.SDK.Menu.Components;
    using Aimtec.SDK.Util;
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

            Combo = new Menu("XerathCombo", "Combo")
            {
                new MenuBool("Q", "Use Q"),
                new MenuBool("W", "Use W"),
                new MenuBool("E", "Use E")
            };

            Harass = new Menu("XerathHarass", "Harass")
            {
                new MenuSliderBool("Q", "Use Q | If Mana % >=", true, 50),
                new MenuSliderBool("W", "Use W | If Mana % >=", true, 50),
                new MenuSliderBool("E", "Use E | If Mana % >=", true, 50),
            };

            LaneClear = new Menu("XerathLane", "LaneClear")
            {
                new MenuBool("Check", "Don't Clear When Enemies Nearby"),
                new MenuSliderBool("Q", "Use Q | If Mana % >=", true, 50),
                new MenuSliderBool("W", "Use W | If Mana % >=", true, 50),
                new MenuSliderBool("E", "Use E | If Mana % >=", false, 80)
            };

            JungleClear = new Menu("XerathJungle", "JungleClear")
            {
                new MenuBool("Q", "Use Q"),
                new MenuBool("W", "Use W"),
                new MenuBool("E", "Use E"),
            };

            Killsteal = new Menu("XerathKillsteal", "Killsteal")
            {
                new MenuBool("Q", "Use Q"),
                new MenuBool("W", "Use W"),
                new MenuBool("E", "Use E"),
                new MenuBool("R", "Use R"),
            };

            Misc = new Menu("XerathMisc", "Miscellaneous")
            {
                new MenuList("Mode", "R Mode", new []{"Tap", "Auto"}, 1),
                new MenuKeyBind("Key", "R Tap Key", KeyCode.T, KeybindType.Press)
            };

            Misc["Key"].OnValueChanged += OnTap;

            Drawings = new Menu("XerathDrawManager", "DrawManager")
            {
                new MenuList("Dmg", "Damage", new []{"Disabled", "Ultimate Only", "All", "All EXCEPT ultimate"}, 2),
                
                new MenuBool("Minimap", "Draw R (Minimap)"),
                new MenuBool("Q", "Draw Q Range"),
                new MenuBool("Pred", "Draw Q Prediction (Debug)", false)
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

        private void OnTap(MenuComponent sender, ValueChangedArgs args)
        {
            if (Misc["Mode"].Value != 0 || !SpellManager.CastingUltimate)
            {
                return;
            }

            var target = Global.TargetSelector.GetTarget(SpellManager.R.Range);
            if(target != null)
            SpellManager.CastR(target);
        }
    }
}