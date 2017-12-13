namespace Adept_BaseUlt.Baseult_Reworked
{
    using Aimtec.SDK.Menu;
    using Aimtec.SDK.Menu.Components;
    using Aimtec.SDK.Util.Cache;
    using Local_SDK;

    class MenuConfig
    {
        public static Menu Menu;

        public MenuConfig()
        {
            Menu = new Menu("hello", $"BaseUlt | {Global.Player.ChampionName}", true);
            Menu.Attach();

            Menu.Add(new MenuBool("RandomUlt", "Use RandomUlt").SetToolTip("Will GUESS the enemy position and ult there"));

            if (Global.Player.ChampionName == "Draven")
            {
                Menu.Add(new MenuBool("Draven", "Include R Back (Draven)"));
            }

            Menu.Add(new MenuBool("Collision", "Check Collision"));

            Menu.Add(new MenuSeperator("yes", "Whitelist"));

            foreach (var hero in GameObjects.EnemyHeroes)
            {
                Menu.Add(new MenuBool(hero.ChampionName, "ULT: " + hero.ChampionName));
            }

            Menu.Add(new MenuSeperator("no"));
            Menu.Add(new MenuSlider("Distance", "Max Distance | RandomUlt", 5000, 1000, 10000));
        }
    }
}
