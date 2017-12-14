namespace Adept_AIO.Champions.Azir.OrbwalkingEvents
{
    using System.Threading;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Util;
    using Core;
    using SDK.Unit_Extensions;

    class Flee
    {
        public static void OnKeyPressed()
        {
            Jump(Game.CursorPos);
        }

        public static void Jump(Vector3 pos)
        {
            if (SpellConfig.W.Ready || Game.TickCount - AzirHelper.LastW <= 1000)
            {
                var extend = Global.Player.ServerPosition.Extend(pos, SpellConfig.W.Range);
                if (SoldierManager.Soldiers.Count == 0)
                {
                    SpellConfig.W.Cast(extend);
                }

                DelayAction.Queue(350, () => SpellConfig.Q.Cast(pos), new CancellationToken(false));
                DelayAction.Queue(150, () => SpellConfig.E.Cast(extend), new CancellationToken(false));
            }
            else if (SoldierManager.GetSoldierNearestTo(pos).Distance(pos) <= 300)
            {
                var extend = SoldierManager.GetSoldierNearestTo(pos).Extend(pos, SpellConfig.Q.Range);
                if (SoldierManager.Soldiers.Count == 0)
                {
                    SpellConfig.W.Cast(extend);
                }
                DelayAction.Queue(350, () => SpellConfig.Q.Cast(pos), new CancellationToken(false));
                DelayAction.Queue(150, () => SpellConfig.E.Cast(extend), new CancellationToken(false));
            }
        }
    }
}