﻿namespace Adept_Prediction
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Events;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Prediction.Collision;
    using Aimtec.SDK.Prediction.Skillshots;
    using Aimtec.SDK.Prediction.Skillshots.AoE;

    interface IPrediction
    {
        PredictionOutput GetDashPrediction(PredictionInput input, bool checkCollision);

        PredictionOutput GetIdlePrediction(PredictionInput input, bool checkCollision);

        PredictionOutput GetMovementPrediction(PredictionInput input, bool checkCollision);

        PredictionOutput GetImmobilePrediction(PredictionInput input);
    }

    class LocalPrediction : ISkillshotPrediction, IPrediction
    {
        private static float lastTickChecked;

       // private static readonly Dictionary<int, float> Timers = new Dictionary<int, float>();

        public LocalPrediction()
        {
            //foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy))
            //{
            //    Timers.Add(hero.NetworkId, 0);
            //}

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
        }

        public PredictionOutput GetPrediction(PredictionInput input)
        {
            return this.GetPrediction(input, false, true);
        }

        public PredictionOutput GetPrediction(PredictionInput input, bool ft, bool collision)
        {
            if (!input.Unit.IsValidTarget() || !input.Unit.IsValid)
            {
                return new PredictionOutput();
            }

            if (ft)
            {
                input.Delay += Game.Ping / 2000f + 0.06f;
                input.From = input.From - (input.Unit.ServerPosition - input.From).Normalized()
                             * ObjectManager.GetLocalPlayer().BoundingRadius;

                if (input.AoE)
                {
                    return AoePrediction.GetAoEPrediction(input);
                }
            }
            
            if (input.Unit.IsDashing())
            {
                return this.GetDashPrediction(input, collision);
            }

            return input.Unit.IsMoving
                ? this.GetMovementPrediction(input, collision)
                : this.GetIdlePrediction(input, collision);
        }

        public PredictionOutput GetDashPrediction(PredictionInput input, bool checkCollision)
        {
            var dashInfo = input.Unit.GetDashInfo();

            var result = new PredictionOutput
            {
                Input = input,
                UnitPosition = input.Unit.ServerPosition,
                CastPosition = dashInfo.EndPos.To3D(),
                HitChance = HitChance.Dashing
            };

            if (input.Unit.Distance(input.RangeCheckFrom) > input.Range)
            {
                result.HitChance = HitChance.OutOfRange;
            }

            if (!checkCollision || !input.Collision)
            {
                return result;
            }

            var collisionObjects = Collision.GetCollision(new List<Vector3> { input.Unit.ServerPosition }, input);

            result.CollisionObjects = collisionObjects;

            if (collisionObjects.Count > 0)
            {
                result.HitChance = HitChance.Collision;
            }

            return result;
        }

        public PredictionOutput GetIdlePrediction(PredictionInput input, bool checkCollision)
        {
            var result = new PredictionOutput
            {
                Input = input,
                UnitPosition = input.Unit.ServerPosition,
                CastPosition = input.Unit.ServerPosition,
                HitChance = HitChance.High
            };

            if (input.Unit.Distance(input.RangeCheckFrom) > input.Range)
            {
                result.HitChance = HitChance.OutOfRange;
            }

            if (!checkCollision || !input.Collision)
            {
                return result;
            }

            var collisionObjects = Collision.GetCollision(new List<Vector3> { input.Unit.ServerPosition }, input);

            result.CollisionObjects = collisionObjects;

            if (collisionObjects.Count > 0) result.HitChance = HitChance.Collision;

            return result;
        }

        public PredictionOutput GetMovementPrediction(PredictionInput input, bool checkCollision)
        {
            if (Environment.TickCount - lastTickChecked <= 10)
            {
                return new PredictionOutput();
            }

            lastTickChecked = Environment.TickCount;

            var result = new PredictionOutput
            {
                Input = input, HitChance = HitChance.VeryHigh,
                UnitPosition = input.Unit.ServerPosition
            };

            var unit = input.Unit;

            var unitPosition = unit.Position.To2D();
            var unitSpeed = unit.MoveSpeed;

            var path = unit.Path;
            var lenght = path.Length;
          
            if (input.AoE || input.Type == SkillshotType.Circle)
            {
                input.Radius /= 2f;
            }

            if (lenght > 1)
            {
                var time = unitSpeed * (/*Game.ClockTime - Timers[input.Unit.NetworkId] +*/ Game.Ping / 2f / 1000f);
                var d = 0f;

                for (var i = 0; i < lenght - 1; i++)
                {
                    var vi = path[i].To2D();
                    var vi1 = path[i + 1].To2D();
                    d += vi.Distance(vi1);

                    if (d >= time)
                    {
                        var dd = unitPosition.Distance(vi1);
                        var ss = unitSpeed * 0.5f;

                        if (dd >= ss)
                        {
                            result.CastPosition = (unitPosition + (vi1 - unitPosition).Normalized() * ss).To3D();
                            break;
                        }

                        if (i + 1 == lenght - 1)
                        {
                            result.CastPosition = (unitPosition + (vi1 - unitPosition).Normalized() * unitPosition.Distance(vi1)).To3D();
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
                                result.CastPosition = (vj + (vj1 - vj).Normalized() * ss).To3D();
                                break;
                            }

                            if (j + 1 != lenght - 1)
                            {
                                continue;
                            }

                            result.CastPosition = (vj + (vj1 - vj).Normalized() * dd).To3D();
                            break;
                        }
                        break;
                    }

                    if (i + 1 != lenght - 1)
                    {
                        continue;
                    }

                    result.CastPosition = (vi + (vi1 - vi).Normalized() * vi.Distance(vi1)).To3D();
                    break;
                }
            }
            else
            {
                result.CastPosition = unit.Position;
            }

            if (result.CastPosition.IsZero || (int)path.LastOrDefault().X != (int)unit.Path.LastOrDefault().X)
            {
                result.HitChance = HitChance.Impossible;
            }

            if (input.RangeCheckFrom.Distance(result.CastPosition) > input.Range)
            {
                result.HitChance = HitChance.OutOfRange;
            }

            if (!checkCollision || !input.Collision)
            {
                return result;
            }

            var collisionObjects = Collision.GetCollision(new List<Vector3> { unit.ServerPosition, result.UnitPosition, result.CastPosition }, input);

            if (collisionObjects.Count > 0)
            {
                result.HitChance = HitChance.Collision;
            }

            return result;
        }

        public PredictionOutput GetImmobilePrediction(PredictionInput input)
        {
            throw new NotImplementedException();
        }
    }
}