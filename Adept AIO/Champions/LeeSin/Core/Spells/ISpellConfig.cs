namespace Adept_AIO.Champions.LeeSin.Core.Spells
{
    using Aimtec;
    using OrbwalkerMode = SDK.Orbwalking.OrbwalkerMode;
    using Spell = Aimtec.SDK.Spell;

    public interface ISpellConfig
    {
        float LastQ1CastAttempt { get; set; }
        bool QAboutToEnd { get; }
        int WardRange { get; }

        OrbwalkerMode InsecMode { get; }
        OrbwalkerMode WardjumpMode { get; }
        OrbwalkerMode KickFlashMode { get; }

        Spell Q { get; }
        Spell W { get; }
        Spell E { get; }
        Spell R { get; }
        Spell R2 { get; }
        bool IsQ2();

        bool HasQ2(Obj_AI_Base target);
        int PassiveStack();
        bool IsFirst(Spell spell);

        void QSmite(Obj_AI_Base target);
        void Load();
        void OnProcessSpellCast(Obj_AI_Base sender, Obj_AI_BaseMissileClientDataEventArgs args);
    }
}