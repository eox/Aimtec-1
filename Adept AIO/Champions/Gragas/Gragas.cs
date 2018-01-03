namespace Adept_AIO.Champions.Gragas
{
    using Aimtec;
    using Core;
    using Drawings;
    using Miscellaneous;
    using OrbwalkerMode = SDK.Orbwalking.OrbwalkerMode;

    class Gragas
    {
        public static OrbwalkerMode InsecOrbwalkerMode;

        public Gragas()
        {
            new SpellManager();
            new MenuConfig();

            Obj_AI_Base.OnProcessSpellCast += SpellManager.OnProcessSpellCast;
            GameObject.OnDestroy += SpellManager.OnDestroy;

            Game.OnUpdate += Manager.OnUpdate;
            Game.OnUpdate += Automatic.OnUpdate;

            Render.OnPresent += DrawManager.OnPresent;
            Render.OnPresent += DrawManager.OnRender;
        }
    }
}