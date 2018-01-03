namespace Adept_AIO.Champions.Draven.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Damage;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Prediction.Skillshots;
    using SDK.Generic;
    using SDK.Unit_Extensions;
    using Geometry = SDK.Geometry_Related.Geometry;
    using OrbwalkingMode = SDK.Orbwalking.OrbwalkingMode;
    using PreMoveEventArgs = SDK.Orbwalking.PreMoveEventArgs;
    using Spell = Aimtec.SDK.Spell;

    class SpellManager
    {
        public static Dictionary<GameObject, int> AxeList { get; } = new Dictionary<GameObject, int>();
        public static int AxeCount => (Global.Player.HasBuff("dravenspinning") ? 1 : 0) + (Global.Player.HasBuff("dravenspinningleft") ? 1 : 0) + AxeList.Count;
  
        public static Spell Q, W, E, R;

        public SpellManager()
        {
            Q = new Spell(SpellSlot.Q);
          
            W = new Spell(SpellSlot.W);
          
            E = new Spell(SpellSlot.E, 950);
            E.SetSkillshot(0.25f, 100f, 1400f, false, SkillshotType.Line);

            R = new Spell(SpellSlot.R, 5000);
            R.SetSkillshot(0.4f, 160f, 2000, false, SkillshotType.Line);

            GameObject.OnCreate += OnCreate;
            GameObject.OnDestroy += OnDestroy;
            Global.Orbwalker.PreMove += PreMove;
           
        }

        private static KeyValuePair<GameObject, int> AxeObject()
        {
            return AxeList.Where(x => x.Key.Position.Distance(Game.CursorPos) <= MenuConfig.Misc["Range"].Value).
                OrderBy(x => x.Key.Distance(Global.Player)).
                ThenBy(x => x.Key.Distance(Game.CursorPos)).
                FirstOrDefault();
        }

        private static void OnCreate(GameObject sender)
        {
        
            if (!sender.Name.ToLower().Contains("draven") || !sender.Name.ToLower().Contains("reticle_self"))
            {
                return;
            }

            DebugConsole.WriteLine($"GOT AXE!", MessageState.Debug);
            AxeList.Add(sender, Game.TickCount + 1800);
        }

        private static void OnDestroy(GameObject sender)
        {
            if (AxeList.All(o => o.Key.NetworkId != sender.NetworkId))
            {
                return;
            }

            AxeList.Remove(sender);
        }

        private void PreMove(object sender, PreMoveEventArgs args)
        {
            if (Global.Orbwalker.IsWindingUp || Global.Player.IsDead || Global.Player.IsRecalling())
            {
                return;
            }
       
            if (MenuConfig.Misc["Catch"].Value == 1 && Global.Orbwalker.Mode != OrbwalkingMode.Combo)
            {
                return;
            }

            var axe = AxeObject().Key;

            if (axe == null || Global.Player.Distance(axe) < 100)
            {
                return;
            }
         
            if (axe.IsUnderEnemyTurret() && !Global.Player.IsUnderEnemyTurret())
            {
                return;
            }

            if (Global.Orbwalker.Mode == OrbwalkingMode.Combo || Global.Orbwalker.Mode == OrbwalkingMode.Mixed)
            {
                var target = Global.TargetSelector.GetTarget(Global.Player.AttackRange + 100);
                if (target != null && target.Health < Global.Player.GetAutoAttackDamage(target) * 3)
                {
                    return;
                }
            }

            if (W.Ready && MenuConfig.Misc["W"].Enabled && axe.Distance(Global.Player) / (Global.Player.MoveSpeed * 1000) >= AxeObject().Value - Game.TickCount)
            {
                Console.WriteLine("USING W");
                W.Cast();
            }

            args.MovePosition = axe.ServerPosition;
        }

        public static void CastQ()
        {
            Q.Cast();
        }

        public static void CastW()
        {
            W.Cast();
        }

        public static void CastE(Obj_AI_Base target)
        {
            if (!target.IsValidTarget(E.Range))
            {
                return;
            }

            var rect = ERect(target);
            if (rect == null)
            {
                return;
            }

          
            E.Cast(target);
        }

        public static void CastR(Obj_AI_Base target)
        {
            if(target.IsValidTarget(R.Range))
            R.Cast(target);
        }

        public static Geometry.Rectangle ERect(Obj_AI_Base target)
        {
            return new Geometry.Rectangle(Global.Player.ServerPosition.To2D(),
                (Global.Player.ServerPosition + target.ServerPosition).To2D().Normalized() * E.Range,
                E.Width);
        }
    }
}