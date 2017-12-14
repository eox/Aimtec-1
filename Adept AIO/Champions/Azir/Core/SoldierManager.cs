namespace Adept_AIO.Champions.Azir.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Util;
    using SDK.Unit_Extensions;

    class SoldierManager
    {
        public static List<Obj_AI_Minion> Soldiers;

        private static bool IsSoldier(Obj_AI_Minion soldier)
        {
            if (soldier == null)
            {
                return false;
            }
            return soldier.IsAlly && !soldier.IsDead && soldier.IsValid && soldier.UnitSkinName.ToLower().Contains("soldier");
        }

        public static void OnDelete(GameObject sender)
        {
            if (Soldiers.Any(x => x.NetworkId == sender.NetworkId))
            {
                Soldiers.RemoveAll(x => x.NetworkId == sender.NetworkId);
            }
        }

        public static void OnCreate(GameObject sender)
        {
            var soldier = sender as Obj_AI_Minion;
            if (soldier != null && IsSoldier(soldier) && Game.TickCount - AzirHelper.LastR >= 100)
            {
                Soldiers.Add(soldier);
                DelayAction.Queue(9000 - Game.Ping / 2, () => Soldiers.RemoveAll(x => x.NetworkId == soldier.NetworkId), new CancellationToken(false));
            }
        }

        public static Vector3 GetSoldierNearestTo(Vector3 pos)
        {
            foreach (var minion in GameObjects.AllyMinions.Where(IsSoldier))
            {
                var soldier = Soldiers.OrderBy(x => x.Distance(pos)).FirstOrDefault(x => x.NetworkId == minion.NetworkId);
                return soldier != null ? soldier.ServerPosition : Vector3.Zero;
            }
            return Vector3.Zero;
        }
    }
}