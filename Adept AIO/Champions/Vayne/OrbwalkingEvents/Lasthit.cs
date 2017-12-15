namespace Adept_AIO.Champions.Vayne.OrbwalkingEvents
{
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Damage;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Orbwalking;
    using Core;
    using SDK.Unit_Extensions;
    using PostAttackEventArgs = SDK.Orbwalking.PostAttackEventArgs;

    class Lasthit
    {
        public static void PostAttack(object sender, PostAttackEventArgs args)
        {
            if (!SpellManager.Q.Ready || !MenuConfig.Lasthit["Q"].Enabled || Global.Player.ManaPercent() <= 35)
            {
                return;
            }

            var minion = GameObjects.EnemyMinions.FirstOrDefault(x => x != Global.Orbwalker.GetOrbwalkingTarget() &&
                                                                      x.Health < Global.Player.GetAutoAttackDamage(x) + Global.Player.GetSpellDamage(x, SpellSlot.Q) &&
                                                                      x.Health > Global.Player.GetAutoAttackDamage(x) && x.IsValidAutoRange());

            if (minion == null)
            {
                return;
            }

            SpellManager.Q.Cast(Game.CursorPos);
        }
    }
}