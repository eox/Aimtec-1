namespace Adept_AIO.Champions.Yasuo.Miscellaneous
{
    using System.Threading;
    using Aimtec;
    using Aimtec.SDK.Events;
    using Aimtec.SDK.Orbwalking;
    using Aimtec.SDK.Util;
    using Core;
    using OrbwalkingEvents;
    using SDK.Unit_Extensions;
    using OrbwalkingMode = SDK.Orbwalking.OrbwalkingMode;
    using PostAttackEventArgs = SDK.Orbwalking.PostAttackEventArgs;

    class Manager
    {
        public static void PostAttack(object sender, PostAttackEventArgs args)
        {
            if (Extension.BeybladeMode.Active)
            {
                Beyblade.OnPostAttack();
            }

            switch (Global.Orbwalker.Mode)
            {
                case OrbwalkingMode.Combo:
                    Combo.OnPostAttack();
                    break;
                case OrbwalkingMode.Laneclear:
                    LaneClear.OnPostAttack();
                    JungleClear.OnPostAttack();
                    break;
            }
        }

        public static void OnUpdate()
        {
            if (Global.Player.IsDead || Global.Orbwalker.IsWindingUp && Global.Orbwalker.Mode != OrbwalkingMode.Lasthit)
            {
                return;
            }

            Stack.OnUpdate();

            switch (Global.Orbwalker.Mode)
            {
                case OrbwalkingMode.Combo:
                    Combo.OnUpdate();
                    break;
                case OrbwalkingMode.Mixed:
                    Harass.OnUpdate();
                    break;
                case OrbwalkingMode.Laneclear:
                    LaneClear.OnUpdate();
                    break;
                case OrbwalkingMode.Lasthit:
                    Lasthit.OnUpdate();
                    break;
                case OrbwalkingMode.None:
                    MinionHelper.ExtendedTarget = Vector3.Zero;
                    MinionHelper.ExtendedMinion = Vector3.Zero;
                    break;
            }
        }

        public static void BuffManagerOnOnAddBuff(Obj_AI_Base sender, Buff buff)
        {
            if (sender == null)
            {
                return;
            }

            if (sender.IsEnemy && (buff.Type == BuffType.Knockup || buff.Type == BuffType.Knockback))
            {
                KnockUpHelper.Sender = sender;
                KnockUpHelper.KnockedUpTick = Game.TickCount;
                KnockUpHelper.BuffStart = (int) buff.StartTime;
                KnockUpHelper.BuffEnd = (int) buff.EndTime;
            }

            if (sender.IsMe)
            {
                switch (buff.Name)
                {
                    case "YasuoQ3W":
                        SpellConfig.SetSkill(Global.Player.IsDashing() ? Mode.DashingTornado : Mode.Tornado);
                        break;
                }
            }
        }

        public static void BuffManagerOnOnRemoveBuff(Obj_AI_Base sender, Buff buff)
        {
            if (sender == null)
            {
                return;
            }

            if (sender.IsMe)
            {
                switch (buff.Name)
                {
                    case "YasuoQ3W":
                        SpellConfig.SetSkill(Mode.Normal);
                        break;
                }
            }
        }

        public static void OnPlayAnimation(Obj_AI_Base sender, Obj_AI_BasePlayAnimationEventArgs args)
        {
            if (sender == null || !sender.IsMe)
            {
                return;
            }

            switch (args.Animation)
            {
                case "Spell3":
                    if (Extension.CurrentMode == Mode.Tornado)
                    {
                        SpellConfig.SetSkill(Mode.DashingTornado);
                    }
                    else
                    {
                        SpellConfig.SetSkill(Mode.Dashing);

                        DelayAction.Queue(1000,
                            () =>
                            {
                                SpellConfig.SetSkill(Mode.Normal);
                            },
                            new CancellationToken(false));
                    }
                    break;
            }
        }
    }
}