namespace Adept_AIO.Champions.Azir.OrbwalkingEvents
{
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Events;
    using Aimtec.SDK.Extensions;
    using Core;
    using SDK.Geometry_Related;
    using SDK.Unit_Extensions;
    using SDK.Usables;

    class Insec
    {
        public static void OnKeyPressed()
        {
            var target = Global.TargetSelector.GetSelectedTarget();

            if (target != null && (AzirHelper.InsecMode.Active || MenuConfig.InsecMenu["Auto"].Enabled && MenuConfig.InsecMenu["Auto"].Value <= target.CountEnemyHeroesInRange(500)))
            {
                var dist = Global.Player.Distance(target);
                var allyT = GameObjects.AllyTurrets.OrderBy(x => x.Distance(Global.Player)).FirstOrDefault(x => x.IsValid && !x.IsDead).ServerPosition;

                var targetPos = target.ServerPosition;
                var soldierPos = SoldierManager.GetSoldierNearestTo(target.ServerPosition);

                var targetExtend = Global.Player.ServerPosition.Extend(allyT, SpellConfig.R.Range - target.BoundingRadius - 30);

                AzirHelper.Rect = new Geometry.Rectangle(targetExtend.To2D(), Global.Player.ServerPosition.Extend(allyT, -SpellConfig.R.Width / 2f).To2D(), SpellConfig.R.Width / 2f);

                if (SpellConfig.Q.Ready)
                {
                    if (soldierPos.Distance(target) <= 200)
                    {
                        if (dist <= MenuConfig.InsecMenu["Range"].Value)
                        {
                            SpellConfig.Q.Cast(allyT.Extend(Game.CursorPos, -600));
                        }
                    }
                    else if (soldierPos.Distance(Global.Player) <= MenuConfig.InsecMenu["Range"].Value)
                    {
                        SpellConfig.Q.Cast(targetPos);
                    }
                }

                if (dist > InsecRange())
                {
                    return;
                }

                if (soldierPos != Vector3.Zero)
                {
                    SpellConfig.E.Cast(soldierPos);
                }
                else
                {
                    SpellConfig.W.Cast(Global.Player.ServerPosition.Extend(targetPos, SpellConfig.W.Range));
                }

                if (AzirHelper.Rect.IsInside(target.ServerPosition.To2D()))
                {
                    if (SpellConfig.E.Ready && soldierPos.Distance(Global.Player) > 600)
                    {
                        SpellConfig.E.Cast(allyT);
                    }

                    if (SpellConfig.R.Ready)
                    {
                        SpellConfig.R.Cast(allyT);
                    }
                }

                if (SummonerSpells.IsValid(SummonerSpells.Flash) && MenuConfig.InsecMenu["Flash"].Enabled && SpellConfig.R.Ready && !SpellConfig.Q.Ready && !SpellConfig.E.Ready &&
                    dist > 450 &&
                    soldierPos.Distance(target) > 450 && !Global.Player.IsDashing())
                {
                    if (Game.TickCount - AzirHelper.LastE <= 900 || Game.TickCount - AzirHelper.LastQ <= 900)
                    {
                        return;
                    }
                    SummonerSpells.Flash.Cast(target.ServerPosition);
                }
            }
        }

        public static float InsecRange()
        {
            var range = 250f;
            if (SpellConfig.E.Ready)
            {
                range += SpellConfig.E.Range - 65;
            }

            if (MenuConfig.InsecMenu["Flash"].Enabled && SummonerSpells.IsValid(SummonerSpells.Flash))
            {
                range += SummonerSpells.Flash.Range;
            }

            return range;
        }
    }
}