namespace Adept_AIO.Champions.Zoe.Core
{
    using System;
    using System.Linq;
    using System.Threading;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Prediction.Skillshots;
    using Aimtec.SDK.Util;
    using SDK.Generic;
    using SDK.Unit_Extensions;
    using Geometry = SDK.Geometry_Related.Geometry;
    using Spell = Aimtec.SDK.Spell;

    class SpellManager
    {
        public static Spell Q, W, E, R;
        public static Vector3 PaddleStarPosition;
        private static float lastCastTime;

        public SpellManager()
        {
            Q = new Spell(SpellSlot.Q, 900);
            Q.SetSkillshot(0.25f, 50, 1700, true, SkillshotType.Line);

            W = new Spell(SpellSlot.W, 600);

            E = new Spell(SpellSlot.E, 800);
            E.SetSkillshot(0.25f, 40, 1700, true, SkillshotType.Line);

            R = new Spell(SpellSlot.R, 575);
        }

        public static void OnProcessSpellCast(Obj_AI_Base sender, Obj_AI_BaseMissileClientDataEventArgs args)
        {
            if (sender == null ||
                !sender.IsMe ||
                args.SpellData.Name != "ZoeQ")
            {
                return;
            }

            PaddleStarPosition = args.End;
            lastCastTime = Game.TickCount;
            DelayAction.Queue(1200, () => PaddleStarPosition = Vector3.Zero, new CancellationToken(false));
        }

        public static Vector3 GeneratePaddleStarPrediction(Obj_AI_Base target, Spell spell)
        {
            if (PaddleStarPosition != Vector3.Zero)
            {
                return Vector3.Zero;
            }

            var dir = target.Orientation.To2D(); 
            var pos = target.ServerPosition + (target.ServerPosition - Global.Player.ServerPosition).Normalized();

            for (var i = 0; i < 360; i += 10)
            {
                var angleRad = Maths.DegreeToRadian(i);
                var rotated = (pos.To2D() + spell.Range * dir.Rotated((float) angleRad)).To3D();

                var rectBefore = new Geometry.Rectangle(Global.Player.ServerPosition.To2D(), rotated.To2D(), Q.Width + target.BoundingRadius);
                var rectAfter = new Geometry.Rectangle(rotated.To2D(), target.ServerPosition.To2D(), Q.Width + target.BoundingRadius + 100);

                if (GameObjects.Enemy.OrderBy(x => x.Distance(Global.Player)).
                    Where(x => x.NetworkId != target.NetworkId).
                    Any(x => x.MaxHealth > 20 && (rectAfter.IsInside(x.ServerPosition.To2D()) || rectBefore.IsInside(x.ServerPosition.To2D()))))
                {
                    continue;
                }

                if (GameObjects.Jungle.OrderBy(x => x.Distance(Global.Player)).
                    Where(x => x.NetworkId != target.NetworkId).
                    Any(x => x.MaxHealth > 20 && (rectAfter.IsInside(x.ServerPosition.To2D()) || rectBefore.IsInside(x.ServerPosition.To2D()))))
                {
                    continue;
                }

                if (rotated.Distance(target) < Global.Player.Distance(target) || rotated.Distance(Global.Player) > Q.Range + 200)
                {
                    continue;
                }

                return rotated;
            }

            return Vector3.Zero;
        }

        public static Spell GetAggressiveWSpell()
        {
            var spellName = Global.Player.SpellBook.GetSpell(SpellSlot.W).Name.ToLower();
            Spell spell = null;
           
            switch (spellName)
            {
                case "summonerflash":
                    spell = new Spell(SpellSlot.W, 425);
                    break;
                case "summonerdot":
                    spell = new Spell(SpellSlot.W, 600);
                    break;
                case "summonerexhaust":
                    spell = new Spell(SpellSlot.W, 650);
                    break;
                case "s5_summonersmiteplayerganker":
                case "hextechgunblade":
                case "itemwillboltspellbase":
                case "itemredemption":
                case "itemsofboltspellbase":
                case "zoew":
                    spell = new Spell(SpellSlot.W, 700);
                    break;
            }

            if (spell != null)
            {
                return spell;
            }

            DebugConsole.WriteLine($"Zoe: This (W) spell is NOT supported yet! (AGGRESIVE) | {spellName}", MessageState.Warn);
            return null;
        }

        public static Spell GetDefensiveWSpell()
        {
            var spellName = Global.Player.SpellBook.GetSpell(SpellSlot.W).Name.ToLower();
            Spell spell = null;

            switch (spellName)
            {
                case "summonerbarrier":
                case "summonerheal":
                case "summonerhaste":
                case "itemredemption":
                    spell = new Spell(SpellSlot.W, 700);
                    break;
            }

            if (spell == null)
            {
                DebugConsole.WriteLine($"Zoe: This (W) spell is NOT supported yet! (DEFENSIVE) | {spellName}", MessageState.Warn);
            }
            return spell;
        }

        public static void CastW(Vector3 pos)
        {
            var spellName = Global.Player.SpellBook.GetSpell(SpellSlot.W).Name.ToLower();
            Spell spell = null;

            if (spellName == "summonerflash")
            {
                spell = new Spell(SpellSlot.W, 425);
            }

            if (spell == null)
            {
                return;
            }

            W.Cast(pos);
        }

        public static void CastW(Obj_AI_Base target)
        {
            var spell = target.IsMe ? GetDefensiveWSpell() : GetAggressiveWSpell();
       
            if (spell == null)
            {
                return;
            }

            if (target.IsValidTarget(spell.Range))
            {
                W.Cast(target);
            }
        }

        public static void CastQ(Obj_AI_Base target)
        {
            if (Game.TickCount - lastCastTime < 800 - Game.Ping / 2)
            {
                return;
            }

            if (PaddleStarPosition == Vector3.Zero)
            {
                var paddleStarPrediction = GeneratePaddleStarPrediction(target, Q);
                if (paddleStarPrediction.IsZero)
                {
                    return;
                }

                Q.Cast(paddleStarPrediction);
            }
            else if (target.IsValidTarget(Q.Range + 150))
            {
                var pred = Q.GetPrediction(target, PaddleStarPosition, Global.Player.ServerPosition);
                if (pred.CastPosition.IsZero)
                {
                    return;
                }
                Q.Cast(pred.CastPosition);
            }
        }

        public static void CastE(Obj_AI_Base target)
        {
            var rect = new Geometry.Rectangle(Global.Player.ServerPosition.To2D(), target.ServerPosition.To2D(), 80 + E.Width);

            if (target.IsValidTarget(E.Range) && !GameObjects.Enemy.OrderBy(x => x.Distance(Global.Player)).Any(x => x.NetworkId != target.NetworkId && rect.IsInside(x.ServerPosition.To2D()) && Global.Player.Distance(x) < Global.Player.Distance(target)))
            {
                E.Cast(target);
            }
        }

        public static void CastR(Obj_AI_Base target, bool flash = false)
        {
            var paddleStar = GeneratePaddleStarPrediction(target, R);
            if (paddleStar.IsZero)
            {
                return;
            }

            R.Cast(paddleStar);

            if (!flash)
            {
                return;
            }

            DelayAction.Queue(250, delegate
            {
                CastW(paddleStar);
            }, new CancellationToken(false));
        }
    }
}