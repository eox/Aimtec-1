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

    class LaneClear
    {
        public static void PostAttack(object sender, PostAttackEventArgs args)
        {
            if (!SpellManager.Q.Ready || Global.Player.ManaPercent() <= 35 || MenuConfig.LaneClear["Q"].Value == 2)
            {
                return;
            }

            if (Global.Player.CountEnemyHeroesInRange(900) == 0 && MenuConfig.LaneClear["Turret"].Enabled)
            {
                var t = args.Target as Obj_AI_Base;

                if (t != null && (t.Type == GameObjectType.obj_AI_Turret || t.Type == GameObjectType.obj_AI_Base))
                {
                    SpellManager.CastQ(t);
                }
            }

            if (MenuConfig.LaneClear["Q"].Value == 1)
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

        public static void OnUpdate()
        {
            var minion = GameObjects.EnemyMinions.FirstOrDefault(x => x.IsValidTarget(SpellManager.Q.Range + Global.Player.AttackRange) && x.MaxHealth > 30);
            if (minion == null || !SpellManager.Q.Ready || Global.Player.ManaPercent() <= 35 || MenuConfig.LaneClear["Q"].Value == 2)
            {
                return;
            }

            if (MenuConfig.LaneClear["Q"].Value == 1 && minion.Health < Global.Player.GetAutoAttackDamage(minion))
            {
                SpellManager.CastQ(minion, MenuConfig.LaneClear["QMode"].Value);
            }
        }
    }
}