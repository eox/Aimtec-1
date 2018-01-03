namespace Adept_AIO.BOT.Menu
{
    using Aimtec.SDK.Menu;
    using Aimtec.SDK.Menu.Components;
    using SDK.Unit_Extensions;
    using Utilities;

    class MenuConfig
    {
        public static Menu Mainmenu;

        public MenuConfig()
        {
            BotData.GetChampionType();

            Mainmenu = new Menu("ADEPT AI", "Adept AI", true)
            {
                new MenuBool("Debug", "Debug", false),
                new MenuBool("Mode", "BOT MODE", false),
                new MenuList("Type", "Bot Lane", new []{"Botlane", "Mid", "Top"}, (int) PlayerData.LANE_TYPE)
            };

            if (!Global.Player.IsRanged)
            {
                Mainmenu.Add(new MenuSlider("Range", "Range", 500, 300, 1000));
            }

            Mainmenu["Mode"].OnValueChanged += delegate(MenuComponent sender, ValueChangedArgs args)
            {
                if (!args.GetNewValue<MenuBool>().Enabled)
                {
                    KeyManager.ClearKeys();
                }
            };

            Mainmenu["Type"].OnValueChanged += delegate (MenuComponent sender, ValueChangedArgs args)
            {
                PlayerData.LANE_TYPE = (LaneType)args.GetNewValue<MenuList>().Value;
            };

            Mainmenu.Attach();
        }
    }
}
