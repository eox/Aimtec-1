namespace Adept_AIO.Champions.Vayne.OrbwalkingEvents
{
    using System.Linq;
    using System.Threading;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Orbwalking;
    using Aimtec.SDK.Util;
    using Core;
    using SDK.Unit_Extensions;
    using SDK.Usables;
    using PostAttackEventArgs = SDK.Orbwalking.PostAttackEventArgs;

    class Combo
    {
        public static void PostAttack(object sender, PostAttackEventArgs args)
        {
            if (!SpellManager.Q.Ready || MenuConfig.Combo["Q"].Value == 1)
            {
                return;
            }

            var t = Global.TargetSelector.GetTarget(SpellManager.Q.Range + Global.Player.AttackRange);
            if (t == null)
            {
                return;
            }

            SpellManager.CastQ(t, MenuConfig.Combo["Mode"].Value, MenuConfig.Combo["ToE"].Enabled);
        }

        public static void OnUpdate()
        {
            var target = Global.TargetSelector.GetTarget(SpellManager.Q.Range + Global.Player.AttackRange);
            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            if (SpellManager.Q.Ready && MenuConfig.Combo["Q"].Value == 1)
            {
                SpellManager.CastQ(target, MenuConfig.Combo["Mode"].Value, MenuConfig.Combo["ToE"].Enabled);
            }

            else if (SpellManager.E.Ready && MenuConfig.Combo["E"].Enabled && MenuConfig.Combo[target.ChampionName].Enabled)
            {
                SpellManager.CastE(target);
            }

            else if (SpellManager.R.Ready && MenuConfig.Combo["R"].Enabled)
            {
                if (target.Health > Dmg.Damage(target) && MenuConfig.Combo["Killable"].Enabled)
                {
                    return;
                }

                if (Global.Player.CountEnemyHeroesInRange(1500) >= MenuConfig.Combo["Count"].Value && target.HealthPercent() >= 25)
                {
                    SpellManager.R.Cast();
                }
            }

            if (!MenuConfig.Combo["Flash"].Enabled || !SpellManager.E.Ready || !SummonerSpells.IsValid(SummonerSpells.Flash))
            {
                return;
            }

            var allyT = GameObjects.AllyTurrets.FirstOrDefault(x => x.Distance(target) <= 900);
            if (allyT == null)
            {
                return;
            }

            var pos = target.ServerPosition + (target.ServerPosition - allyT.ServerPosition).Normalized() * 200;
            if (pos.Distance(Global.Player) > 425)
            {
                return;
            }

            SpellManager.E.CastOnUnit(target);
            DelayAction.Queue(100, () => SummonerSpells.Flash.Cast(pos), new CancellationToken(false));
        }
    }
}