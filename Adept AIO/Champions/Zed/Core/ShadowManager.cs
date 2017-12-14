namespace Adept_AIO.Champions.Zed.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Util;
    using SDK.Unit_Extensions;

    class ShadowManager
    {
        public static List<Obj_AI_Minion> Shadows;

        public static void SwitchToShadows()
        {
            if (CanSwitchToShadow(SpellSlot.W))
            {
                SpellManager.W.Cast();
            }
            else if (CanSwitchToShadow(SpellSlot.R))
            {
                SpellManager.R.Cast();
            }
        }

        public static bool CanSwitchToShadow(SpellSlot spellSlot)
        {
            return Global.Player.GetSpell(spellSlot).ToggleState != 0 && Shadows.Any(x => x.Distance(Global.Player) <= 1300);
        }

        public static bool CanCastFirst(SpellSlot spellSlot)
        {
            return Global.Player.GetSpell(spellSlot).ToggleState == 0;
        }

        public static bool IsShadow(Obj_AI_Minion shadow)
        {
            return shadow != null && shadow.IsAlly && shadow.UnitSkinName.ToLower().Contains("shadow");
        }

        public static void OnDelete(GameObject sender)
        {
            if (Shadows.Any(x => x.NetworkId == sender.NetworkId))
            {
                Shadows.RemoveAll(x => x.NetworkId == sender.NetworkId);
            }
        }

        public static void OnCreate(GameObject sender)
        {
            var shadow = sender as Obj_AI_Minion;
            if (shadow == null || !IsShadow(shadow) || Game.TickCount - SpellManager.LastR > 200 && Game.TickCount - SpellManager.LastR <= 1000)
            {
                return;
            }

            Shadows.Add(shadow);
            DelayAction.Queue(5000, () => Shadows.RemoveAll(x => x.NetworkId == shadow.NetworkId), new CancellationToken(false));
        }

        public static Vector3 GetShadowNearestTo(Vector3 pos)
        {
            foreach (var minion in GameObjects.AllyMinions.Where(IsShadow))
            {
                var shadow = Shadows.OrderBy(x => x.Distance(pos)).FirstOrDefault(x => x.NetworkId == minion.NetworkId);
                return shadow != null ? shadow.ServerPosition : Vector3.Zero;
            }
            return Vector3.Zero;
        }
    }
}