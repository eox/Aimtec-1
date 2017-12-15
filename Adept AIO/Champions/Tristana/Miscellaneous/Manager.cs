namespace Adept_AIO.Champions.Tristana.Miscellaneous
{
    using System.Linq;
    using Aimtec.SDK.Damage;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Orbwalking;
    using OrbwalkingEvents;
    using SDK.Unit_Extensions;
    using OrbwalkingMode = SDK.Orbwalking.OrbwalkingMode;
    using PostAttackEventArgs = SDK.Orbwalking.PostAttackEventArgs;
    using PreAttackEventArgs = SDK.Orbwalking.PreAttackEventArgs;

    class Manager
    {
        private readonly Combo combo;
        private readonly Harass harass;
        private readonly JungleClear jungleClear;
        private readonly LaneClear laneClear;

        public Manager(Combo combo, Harass harass, LaneClear laneClear, JungleClear jungleClear)
        {
            this.combo = combo;
            this.harass = harass;
            this.laneClear = laneClear;
            this.jungleClear = jungleClear;
        }

        public void OnPostAttack(object sender, PostAttackEventArgs args)
        {
            switch (Global.Orbwalker.Mode)
            {
                case OrbwalkingMode.Laneclear:
                    jungleClear.OnPostAttack(args.Target);
                    laneClear.OnPostAttack();
                    break;
            }
        }

        public void OnPreAttack(object sender, PreAttackEventArgs args)
        {
            switch (Global.Orbwalker.Mode)
            {
                case OrbwalkingMode.Combo:
                case OrbwalkingMode.Mixed:
                    var enemy = GameObjects.EnemyHeroes.FirstOrDefault(x => x.IsValidTarget() && x.HasBuff("TristanaECharge"));
                    if (enemy != null && enemy.IsValidAutoRange())
                    {
                        args.Target = enemy;
                    }
                    break;
                case OrbwalkingMode.Laneclear:
                case OrbwalkingMode.Lasthit:

                    if (GameObjects.EnemyMinions.Any(x => x.IsValidAutoRange() && x.Health < Global.Player.GetAutoAttackDamage(x)))
                    {
                        return;
                    }
                    var minion = GameObjects.EnemyMinions.FirstOrDefault(x => x.IsValidAutoRange() && x.HasBuff("TristanaECharge"));
                    if (minion != null)
                    {
                        args.Target = minion;
                    }
                    break;
            }
        }

        public void OnUpdate()
        {
            if (Global.Player.IsDead || Global.Orbwalker.IsWindingUp)
            {
                return;
            }

            switch (Global.Orbwalker.Mode)
            {
                case OrbwalkingMode.Combo:
                    combo.OnUpdate();
                    break;
                case OrbwalkingMode.Mixed:
                    harass.OnUpdate();
                    break;
                case OrbwalkingMode.Laneclear:
                    laneClear.OnUpdate();
                    jungleClear.OnUpdate();
                    break;
            }
        }
    }
}