using System;
using System.Collections.Generic;
using System.Linq;

namespace Adept_AIO.SDK.Geometry_Related
{
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Unit_Extensions;
    using Spell = Aimtec.SDK.Spell;

    public class XerathPrediction
    {
        static readonly Dictionary<int, float> Timers = new Dictionary<int, float>();
      
        public static void Load()
        {
            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy))
            {
                Timers.Add(hero.NetworkId, 0);
            }

            //Game.OnUpdate += delegate
            //{
            //    foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy))
            //    {
            //        var id = hero.NetworkId;
            //        if (!Timers.ContainsKey(id))
            //        {
            //            return;
            //        }

            //        Timers[id] = Game.ClockTime;
            //    }
            //};

            Obj_AI_Base.OnNewPath += delegate (Obj_AI_Base sender, Obj_AI_BaseNewPathEventArgs args)
            {
                if (!sender.IsEnemy)
                {
                    return;
                }
                Console.WriteLine("???????????");
                var id = sender.NetworkId;
                if (!Timers.ContainsKey(id))
                {
                    return;
                }

                Timers[id] = Game.ClockTime;
            };
        }

        public static bool CastQ(Obj_AI_Base target, Spell xerathQ)
        {
            var range = xerathQ.Range;
         
            var enemy = target;
            if (!enemy.IsValidTarget())
                return false;
            var enemyid = enemy.NetworkId;
            var enemypos = enemy.Position.To2D();
            var enemyspeed = enemy.MoveSpeed;
            var path = enemy.Path;
            var lenght = path.Length;
            var predpos = Vector3.Zero;
            if (lenght > 1)
            {
                var sInTime = enemyspeed * (Game.ClockTime - Timers[enemyid] + Game.Ping * 0.001f);
                var d = 0f;

                for (var i = 0; i < lenght - 1; i++)
                {
                    var vi = path[i].To2D();
                    var vi1 = path[i + 1].To2D();
                    d += vi.Distance(vi1);

                    if (d >= sInTime)
                    {
                        var dd = enemypos.Distance(vi1);
                        var ss = enemyspeed * 0.5f;
                        if (dd >= ss)
                        {
                            predpos = (enemypos + (vi1 - enemypos).Normalized() * ss).To3D();
                            break;
                        }
                        if (i + 1 == lenght - 1)
                        {
                            predpos = (enemypos + (vi1 - enemypos).Normalized() * enemypos.Distance(vi1)).To3D();
                            break;
                        }
                        for (var j = i + 1; j < lenght - 1; j++)
                        {
                            var vj = path[j].To2D();
                            var vj1 = path[j + 1].To2D();
                            ss -= dd;
                            dd = vj.Distance(vj1);
                            if (dd >= ss)
                            {
                                predpos = (vj + (vj1 - vj).Normalized() * ss).To3D();
                                break;
                            }
                            if (j + 1 == lenght - 1)
                            {
                                predpos = (vj + (vj1 - vj).Normalized() * dd).To3D();
                                break;
                            }
                        }
                        break;
                    }
                    if (i + 1 == lenght - 1)
                    {
                        predpos = (vi + (vi1 - vi).Normalized() * vi.Distance(vi1)).To3D();
                        break;
                    }
                }
            }
            else
            {
                predpos = enemy.Position;
            }
            var dist = predpos.Distance(Global.Player);
            if (dist > 1300)
                range += 150;
            if (predpos.IsZero || dist > range - 150 || (int)path.LastOrDefault().X != (int)enemy.Path.LastOrDefault().X)
                return false;

            Global.Player.SpellBook.UpdateChargedSpell(SpellSlot.Q, predpos, true);

            return true;
        }
    }
}
