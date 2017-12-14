﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Adept_AIO.SDK.Geometry_Related
//{
//    using Aimtec;
//    using Aimtec.SDK.Extensions;
//    using Unit_Extensions;
//    using Spell = Aimtec.SDK.Spell;

//    public class LocalPrediction
//    {
//        static readonly Dictionary<int, float> Timers = new Dictionary<int, float>();
//        private static float lastQ;

//        public LocalPrediction()
//        {
//            foreach (var hero in GameObjects.EnemyHeroes)
//            {
//                Timers.Add(hero.NetworkId, 0);
//            }

//            Obj_AI_Base.OnNewPath += delegate(Obj_AI_Base sender, Obj_AI_BaseNewPathEventArgs args)
//            {
//                var id = sender.NetworkId;
//                if (!Timers.ContainsKey(id))
//                {
//                    return;
//                }

//                Timers[id] = Game.ClockTime;
//            };

//            Obj_AI_Base.OnProcessSpellCast += delegate(Obj_AI_Base sender, Obj_AI_BaseMissileClientDataEventArgs args)
//            {
//                if (!sender.IsMe || args.SpellSlot == SpellSlot.Q || Game.ClockTime <= lastQ + 0.2f)
//                {
//                    return;
//                }

//                lastQ = Game.ClockTime;
//            };
//        }

//        public static bool CastQ(Obj_AI_Base target)
//        {
//            int range = 750 + (int)((Game.ClockTime - lastQ) * 433);
//            if (range > 1500)
//            {
//                range = 1500;
//            }
//            var enemy = target;
//            if (!enemy.IsValidTarget())
//                return false;
//            int enemyid = enemy.NetworkId;
//            Vector2 enemypos = enemy.Position.To2D();
//            float enemyspeed = enemy.MoveSpeed;
//            Vector3[] path = enemy.Path;
//            int lenght = path.Length;
//            Vector3 predpos = Vector3.Zero;
//            if (lenght > 1)
//            {
//                float s_in_time = enemyspeed * (Game.ClockTime - Timers[enemyid] + (Game.Ping * 0.001f));
//                float d = 0f;
//                for (int i = 0; i < lenght - 1; i++)
//                {
//                    Vector2 vi = path[i].To2D();
//                    Vector2 vi1 = path[i + 1].To2D();
//                    d += vi.Distance(vi1);
//                    if (d >= s_in_time)
//                    {
//                        float dd = enemypos.Distance(vi1);
//                        float ss = enemyspeed * 0.5f;
//                        if (dd >= ss)
//                        {
//                            predpos = (enemypos + ((vi1 - enemypos).Normalized() * ss)).To3D();
//                            break;
//                        }
//                        if (i + 1 == lenght - 1)
//                        {
//                            predpos = (enemypos + ((vi1 - enemypos).Normalized() * enemypos.Distance(vi1))).To3D();
//                            break;
//                        }
//                        for (int j = i + 1; j < lenght - 1; j++)
//                        {
//                            Vector2 vj = path[j].To2D();
//                            Vector2 vj1 = path[j + 1].To2D();
//                            ss -= dd;
//                            dd = vj.Distance(vj1);
//                            if (dd >= ss)
//                            {
//                                predpos = (vj + ((vj1 - vj).Normalized() * ss)).To3D();
//                                break;
//                            }
//                            if (j + 1 == lenght - 1)
//                            {
//                                predpos = (vj + ((vj1 - vj).Normalized() * dd)).To3D();
//                                break;
//                            }
//                        }
//                        break;
//                    }
//                    if (i + 1 == lenght - 1)
//                    {
//                        predpos = (vi + ((vi1 - vi).Normalized() * vi.Distance(vi1))).To3D();
//                        break;
//                    }
//                }
//            }
//            else
//            {
//                predpos = enemy.Position;
//            }
//            float dist = predpos.Distance(Global.Player);
//            if (dist > 1300)
//                range += 150;
//            if (predpos.IsZero || dist > range - 150 || (int)path.LastOrDefault().X != (int)enemy.Path.LastOrDefault().X)
//                return false;
      
//            return true;
//        }

//        public static Vector3 GetPrediction(Obj_AI_Base target, Spell spell)
//        {
//            var range = spell.Range;

//            if (target == null || !target.IsValidTarget())
//            {
//                return Vector3.Zero;
//            }

//            var enemyid = target.NetworkId;
//            var enemypos = target.Position.To2D();
//            var enemyspeed = target.MoveSpeed;
//            var path = target.Path;
//            var lenght = path.Length;
//            var predpos = Vector3.Zero;

//            if (lenght > 1)
//            {
//                var sInTime = enemyspeed * (Game.ClockTime - Timers[enemyid] + Game.Ping * 0.001f);
//                var d = 0f;

//                for (var i = 0; i < lenght - 1; i++)
//                {
//                    var vi = path[i].To2D();
//                    var vi1 = path[i + 1].To2D();
//                    d += vi.Distance(vi1);
//                    if (d >= sInTime)
//                    {
//                        var dd = enemypos.Distance(vi1);
//                        var ss = enemyspeed * 0.5f;
//                        if (dd >= ss)
//                        {
//                            predpos = (enemypos + (vi1 - enemypos).Normalized() * ss).To3D();
//                            break;
//                        }
//                        if (i + 1 == lenght - 1)
//                        {
//                            predpos = (enemypos + (vi1 - enemypos).Normalized() * enemypos.Distance(vi1)).To3D();
//                            break;
//                        }
//                        for (var j = i + 1; j < lenght - 1; j++)
//                        {
//                            var vj = path[j].To2D();
//                            var vj1 = path[j + 1].To2D();

//                            ss -= dd;
//                            dd = vj.Distance(vj1);

//                            if (dd >= ss)
//                            {
//                                predpos = (vj + (vj1 - vj).Normalized() * ss).To3D();
//                                break;
//                            }

//                            if (j + 1 != lenght - 1)
//                            {
//                                continue;
//                            }

//                            predpos = (vj + (vj1 - vj).Normalized() * dd).To3D();
//                            break;
//                        }
//                        break;
//                    }
//                    if (i + 1 != lenght - 1)
//                    {
//                        continue;
//                    }
//                    predpos = (vi + (vi1 - vi).Normalized() * vi.Distance(vi1)).To3D();
//                    break;
//                }
//            }
//            else
//            {
//                predpos = target.Position;
//            }

//            var dist = predpos.Distance(Global.Player);
//            if (dist > 1300)
//                range += 150;

//            if (predpos.IsZero || dist > range - 150 || (int) path.LastOrDefault().X != (int) target.Path.LastOrDefault().X)
//                return Vector3.Zero;

//            return predpos;
//        }
//    }
//}
