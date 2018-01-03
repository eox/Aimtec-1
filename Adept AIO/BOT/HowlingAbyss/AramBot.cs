namespace Adept_AIO.BOT.HowlingAbyss
{
    using Events;
    using Menu;
    using Utilities;

    class AramBot
    {
        public AramBot()
        {
            new MenuConfig();
            new UpdateManager();
            new RenderManager();
            new LevelUpManager();
        }
    }
}
