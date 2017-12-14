namespace Adept_AIO.Champions.MissFortune
{
    using Core;
    using Drawings;
    using Miscellaneous;

    class MissFortune
    {
        public MissFortune()
        {
            new MenuConfig();
            new SpellManager();

            new Automatic();
            new Manager();

            new DrawManager();

            new AntiGapcloser();
        }
    }
}