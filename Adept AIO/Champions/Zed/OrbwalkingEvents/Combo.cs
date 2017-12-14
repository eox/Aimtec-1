namespace Adept_AIO.Champions.Zed.OrbwalkingEvents
{
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Events;
    using Aimtec.SDK.Extensions;
    using Core;
    using SDK.Unit_Extensions;

    class Combo
    {
        public static void OnUpdate()
        {
            var target = Global.TargetSelector.GetTarget(SpellManager.WCastRange + SpellManager.Q.Range);
            if (target == null)
            {
                return;
            }

            if (SpellManager.R.Ready &&
                target.IsValidTarget(SpellManager.R.Range) &&
                !(MenuConfig.Combo["Killable"].Enabled && Dmg.Damage(target) < target.Health))
            {
                if (target.HealthPercent() <= 25 ||
                    !MenuConfig.Combo[target.ChampionName].Enabled)
                {
                    return;
                }

                SpellManager.CastR(target);
            }

            if (SpellManager.W.Ready &&
                MenuConfig.Combo["W"].Enabled &&
                target.IsValidTarget(SpellManager.WCastRange + SpellManager.R.Range))
            {
                if (ShadowManager.CanCastFirst(SpellSlot.W))
                {
                    if (Game.TickCount - SpellManager.LastR < 1500)
                    {
                        foreach (var shadow in ShadowManager.Shadows)
                        {
                            if (shadow == null)
                            {
                                continue;
                            }

                            if (target.IsDashing())
                            {
                                SpellManager.W.Cast(target.GetDashInfo().EndPos);
                            }

                            switch (MenuConfig.Combo["Style"].Value)
                            {
                                case 0:
                                    var trianglePos = target.ServerPosition + (target.ServerPosition - shadow.ServerPosition).To2D().Perpendicular().To3D().Normalized() * 350;
                                    if (trianglePos.Distance(target) > SpellManager.WCastRange)
                                    {
                                        goto case 1;
                                    }

                                    SpellManager.W.Cast(trianglePos);
                                    break;
                                case 1:
                                    SpellManager.W.Cast(target.ServerPosition.Extend(shadow.ServerPosition, -SpellManager.WCastRange));
                                    break;
                                case 2:
                                    SpellManager.W.Cast(Game.CursorPos);
                                    break;
                            }
                        }
                    }

                    else if (target.IsValidTarget(SpellManager.WCastRange + 200) && ShadowManager.CanCastFirst(SpellSlot.W) && !(SpellManager.R.Ready && target.HealthPercent() > 25))
                    {
                        SpellManager.W.Cast(target.ServerPosition);
                    }
                }
                else if (ShadowManager.CanSwitchToShadow(SpellSlot.W) &&
                         ShadowManager.Shadows.FirstOrDefault().Distance(target) <= Global.Player.Distance(target) &&
                         target.Distance(Global.Player) > Global.Player.AttackRange + 65)
                {
                    SpellManager.W.Cast();
                }
            }

            else if (SpellManager.Q.Ready &&
                     MenuConfig.Combo["Q"].Enabled)
            {
                SpellManager.CastQ(target);
            }

            if (SpellManager.E.Ready &&
                MenuConfig.Combo["E"].Enabled)
            {
                SpellManager.CastE(target);
            }
        }
    }
}