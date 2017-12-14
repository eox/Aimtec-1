namespace Adept_AIO.Champions.Jax.Miscellaneous
{
    using System;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Core;
    using SDK.Unit_Extensions;

    class SpellManager
    {
        private static bool canUseE;
        private static Obj_AI_Base unit;

        public static void OnProcessSpellCast(Obj_AI_Base sender, Obj_AI_BaseMissileClientDataEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            switch (args.SpellData.Name)
            {
                case "JaxCounterStrike":
                    canUseE = false;
                    break;
            }
        }

        public static void OnUpdate()
        {
            if (unit == null || !canUseE || !unit.IsValid || SpellConfig.SecondE)
            {
                return;
            }

            if (Game.TickCount - SpellConfig.E.LastCastAttemptT > 1700 || unit.Distance(Global.Player) <= SpellConfig.E.Range + unit.BoundingRadius)
            {
                SpellConfig.E.Cast(unit);
            }
        }

        public static void CastE(Obj_AI_Base target)
        {
            canUseE = true;
            unit = target;
        }
    }
}