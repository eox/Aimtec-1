namespace Adept_Prediction
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Events;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Prediction.Collision;
    using Aimtec.SDK.Prediction.Skillshots;

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

        private static readonly Dictionary<int, float> Timers = new Dictionary<int, float>();

        public LocalPrediction()
        {
            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy))
            {
                Timers.Add(hero.NetworkId, 0);
            }

            Game.OnUpdate += delegate
            {
                foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy))
                {
                    var id = hero.NetworkId;
                    if (!Timers.ContainsKey(id))
                    {
                        return;
                    }

                    Timers[id] = Game.ClockTime;
                }
            };
        }

        public PredictionOutput GetPrediction(PredictionInput input)
        {
            return this.GetPrediction(input, false, true);
        }

        public PredictionOutput GetPrediction(PredictionInput input, bool ft, bool collision)
        {
            if (!input.Unit.IsValidTarget() || !input.Unit.IsValid)
            {
                return new PredictionOutput { Input = input };
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
            if (Environment.TickCount - lastTickChecked <= 50)
            {
                return new PredictionOutput()
                {
                    HitChance = HitChance.Impossible,
                    Input = input
                };
            }

            lastTickChecked = Environment.TickCount;

            var result = new PredictionOutput { Input = input, HitChance = HitChance.VeryHigh };

            var unit = input.Unit;

            if (!Timers.ContainsKey(input.Unit.NetworkId))
            {
                Timers.Add(input.Unit.NetworkId, Game.ClockTime);
            }

            var unitPosition = unit.Position.To2D();
            var unitSpeed = unit.MoveSpeed;

            var path = unit.Path;
            var lenght = path.Length;
            var predpos = Vector3.Zero;

            if (input.AoE || input.Type == SkillshotType.Circle)
            {
                input.Radius /= 2f;
            }

            if (lenght > 1)
            {
                var time = unitSpeed * (Game.ClockTime - Timers[input.Unit.NetworkId] + Game.Ping * 0.001f);
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
                            predpos = (unitPosition + (vi1 - unitPosition).Normalized() * ss).To3D();
                            break;
                        }

                        if (i + 1 == lenght - 1)
                        {
                            predpos = (unitPosition + (vi1 - unitPosition).Normalized() * unitPosition.Distance(vi1)).To3D();
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

                            if (j + 1 != lenght - 1)
                            {
                                continue;
                            }

                            predpos = (vj + (vj1 - vj).Normalized() * dd).To3D();
                            break;
                        }
                        break;
                    }

                    if (i + 1 != lenght - 1)
                    {
                        continue;
                    }

                    predpos = (vi + (vi1 - vi).Normalized() * vi.Distance(vi1)).To3D();
                    break;
                }
            }
            else
            {
                predpos = unit.Position;
            }

            if (predpos.IsZero || (int)path.LastOrDefault().X != (int)unit.Path.LastOrDefault().X)
            {
                result.HitChance = HitChance.Impossible;
            }

            if (input.RangeCheckFrom.Distance(predpos) > input.Range)
            {
                result.HitChance = HitChance.OutOfRange;
            }

            result.UnitPosition = input.Unit.ServerPosition;

            result.CastPosition = predpos;

            if (!checkCollision || !input.Collision)
            {
                return result;
            }

            var collisionObjects = Collision.GetCollision(new List<Vector3> { input.Unit.ServerPosition, result.UnitPosition, result.CastPosition }, input);

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
