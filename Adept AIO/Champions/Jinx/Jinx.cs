namespace Adept_AIO.Champions.Jinx
{
    using Aimtec;
    using Core;
    using Drawings;
    using Miscellaneous;
    using OrbwalkingEvents;
    using SDK.Delegates;

    class Jinx
    {
        public Jinx()
        {
            var spellConfig = new SpellConfig();
            spellConfig.Load();

            var menuConfig = new MenuConfig();
            menuConfig.Attach();

            var combo = new Combo(spellConfig, menuConfig);
            var harass = new Harass(spellConfig, menuConfig);
            var laneclear = new LaneClear(menuConfig, spellConfig);
            var jungleclear = new JungleClear(menuConfig, spellConfig);

            var misc = new Misc(spellConfig, menuConfig);
            var gapcloser = new AntiGapcloser(spellConfig);

            var manager = new Manager(combo, harass, laneclear, jungleclear, spellConfig);

            var drawManager = new DrawManager(menuConfig, new Dmg(), spellConfig);

            Game.OnUpdate += manager.OnUpdate;
            Game.OnUpdate += misc.OnUpdate;

            Render.OnPresent += drawManager.OnPresent;
            Render.OnRender += drawManager.OnRender;

            Gapcloser.OnGapcloser += gapcloser.OnGapcloser;
        }
    }
}