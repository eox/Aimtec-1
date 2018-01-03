namespace Adept_AIO.Champions.Twitch.Core
{
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Damage;
    using Aimtec.SDK.Damage.JSON;
    using Aimtec.SDK.Extensions;
    using SDK.Unit_Extensions;

    class Dmg
    {
        private static readonly string[] Buffs =
        {
            "JaxCounterStrike",
            "FioraW",
            "BansheesVeil",
            "BlackShield",
            "KindredrNoDeathBuff",
            "malzaharpassiveshield",
            "NocturneShroudofDarkness",
            "OlafRagnarock",
            "UndyingRage",
            "SivirE"
        };

        private static readonly string[] ReviveBuffs =
        {
            "chronorevive",
            "zhonyasringshield",
            "AatroxPassiveDeath",
            "rebirth"
        };

        public static double EDmg(Obj_AI_Base target)
        {
            if (!target.HasBuff("twitchdeadlyvenom") || target.ValidActiveBuffs().Any(x => Buffs.Contains(x.Name)) || target.ValidActiveBuffs().Any(x => ReviveBuffs.Contains(x.Name)))
            {
                return 0;
            }

            return Global.Player.GetSpellDamage(target, SpellSlot.E) * 1.125f + Global.Player.GetSpellDamage(target, SpellSlot.E, DamageStage.Buff) * 1.38f;
        }
    }
}