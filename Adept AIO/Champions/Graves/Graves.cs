namespace Adept_AIO.Champions.Graves
{
    using Core;
    using Drawings;
    using Miscellaneous;
    using SDK.Delegates;

    class Graves
    {
        public Graves()
        {
            new MenuConfig();
            new SpellManager();

            new Killsteal();
            new Manager();

            new DrawManager();

            Gapcloser.OnGapcloser += AntiGapcloser.OnGapcloser;
        }
    }
}