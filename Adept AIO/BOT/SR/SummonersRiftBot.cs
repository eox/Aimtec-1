namespace Adept_AIO.BOT.SR
{
    using Events;
    using Menu;
    using Utilities;

    class SummonersRiftBot
    {
        public SummonersRiftBot()
        {
            new MenuConfig();
            new UpdateManager();
            new RenderManager();
            new LevelUpManager();
        }
    }
}
