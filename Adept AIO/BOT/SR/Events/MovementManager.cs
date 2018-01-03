namespace Adept_AIO.BOT.SR.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Util;
    using SDK.Generic;
    using SDK.Geometry_Related;
    using SDK.Unit_Extensions;
    using Utilities;

    class MovementManager
    {
       
        private float lastFoundGrassTick; // Hotfix.

        private Vector3 lastGrassVector;
        private float turretDestroyTick;

        public MovementManager()
        {
            Obj_AI_Base.OnProcessAutoAttack += OnProcessAutoAttack;
            GameObject.OnDestroy += OnDestroy;
        }

        private static Obj_AI_Base AllyTurret => GameObjects.AllyTurrets.Where(x => !x.IsDead && x.IsValid).OrderBy(x => Global.Player.Distance(x)).FirstOrDefault();
        private static Obj_AI_Base EnemyTurret => GameObjects.EnemyTurrets.Where(x => !x.IsDead && x.IsValid).OrderBy(x => Global.Player.Distance(x)).FirstOrDefault();

        private static Obj_AI_Hero AllyHero => GameObjects.AllyHeroes.
            OrderBy(x => x.Distance(Global.Player)).
            ThenByDescending(x => x.Health).
            FirstOrDefault(x => !x.IsMe && !x.IsRecalling() && x.HealthPercent() > 35 && x.Distance(Global.Player) <= 3000);

        private static Obj_AI_Base Allyminion => GameObjects.AllyMinions.
            Where(x => x.IsValidTarget(3000, true) && !x.UnitSkinName.ToLower().Contains("shroom") && !x.IsWard()).
            OrderBy(x => x.MaxHealth).
            ThenBy(x => Global.Player.Distance(x)).
            FirstOrDefault();

        private void OnDestroy(GameObject sender)
        {
            if (sender == null || !sender.IsValid || !sender.IsTurret || !sender.IsEnemy || Global.Player.Distance(sender) > 1500)
            {
                return;
            }

            turretDestroyTick = Environment.TickCount;
        }

        private void OnProcessAutoAttack(Obj_AI_Base sender, Obj_AI_BaseMissileClientDataEventArgs args)
        {
            if (sender == null || !args.Target.IsMe)
            {
                return;
            }

            if (args.Sender.IsTurret || args.Sender.IsHero && Global.Player.UnderEnemyTURRET())
            {
                BotData.SetDanger();
            }
        }

        public void Update()
        {
            if (BotData.CannotContinueMovement() || !Global.Orbwalker.CanMove() || Environment.TickCount - BotData.LastStepTick <= Global.Random.Next(700, 1700) || Environment.TickCount - lastFoundGrassTick <= 8000)
            {
                return;
            }

            if (Game.ClockTime >= 105 &&
                BotData.EnemyCount == 0 &&
                !GameObjects.Minions.Any(x => x.IsValidTarget(1800, true) && x.Distance(Global.Player) <= 1800) &&
                !(Global.Player.Distance(BotData.GetBotPushLane()) <= 800) &&
                GameObjects.AllyHeroes.Any(x => x.IsValidTarget(3000, true)))
            {
                DebugConsole.WriteLine($"Running to: BEST LANE", MessageState.Debug);

                BotData.SetNewMovePosition(BotData.GetBotPushLane());
                return;
            }

            if (Game.ClockTime <= 82)
            {
                DebugConsole.WriteLine($"Running to: PRE LANE-PHASE POSITION", MessageState.Debug);

                BotData.SetNewMovePosition(BotData.GetBotPreLanePhase(), false);
                return;
            }

            if (Game.ClockTime <= 87)
            {
                var allyJungler = GameObjects.AllyHeroes.FirstOrDefault(x => x.IsJungler());
                if (allyJungler != null && allyJungler.Distance(BotData.GetBotPushLane()) <= 4500)
                {
                    DebugConsole.WriteLine($"Running to: ALLY JUNGLER", MessageState.Debug);
                    var pos = allyJungler.ServerPosition.Extend(Global.Player.ServerPosition, BotData.MyRange / 2f);
                    BotData.SetNewMovePosition(pos);

                    return;
                }
            }

            if (Game.ClockTime <= 95)
            {
                DebugConsole.WriteLine($"Running to: STARTING POSITION", MessageState.Debug);

                BotData.SetNewMovePosition(BotData.GetBotPushLane().Extend(Global.Player.ServerPosition, Global.Player.AttackRange), false);
                return;
            }

            if (BotData.IsInDanger ||
                Global.Player.UnderEnemyTURRET() && GameObjects.AllyMinions.Count(x => x.IsValidTarget(1100, true) && x.UnderEnemyTURRET()) <= 3)
            {
                BotData.SetDanger();
                DebugConsole.WriteLine($"ATTACKED OR WILL BE ATTACKED BY TURRET", MessageState.Warn);
                GetToSafety(AllyTurret, false);
                return;
            }

            if (Environment.TickCount - turretDestroyTick <= 6500 &&
                Game.ClockTime < 650 &&
                GameObjects.EnemyTurrets.Count() >= 7 &&
                BotData.EnemyCount == 0 &&
                !GameObjects.EnemyMinions.Any(x => x.Distance(Global.Player) <= 1000))
            {
                DebugConsole.WriteLine($"RECALLING (Killed turret)", MessageState.Debug);
                Global.Player.SpellBook.CastSpell(SpellSlot.Recall);
                BotData.LastRecallAttempt = Environment.TickCount;
                return;
            }

            if (BotData.MinionAttacks.Count >= 4 && Game.ClockTime < 650 ||
                Global.Player.HealthPercent() <= 25 ||
                Global.Player.HealthPercent() <= 40 &&
                GameObjects.EnemyHeroes.Any(x => x.IsValidTarget(1300) &&
                                                 x.HealthPercent() >= 60) &&
                BotData.EnemyCount > BotData.AllyCount ||
                BotData.EnemyCount >= 2 && !BotData.IsAggressive() ||
                !GameObjects.AllyMinions.Any(x => x.UnderEnemyTURRET() && x.Distance(Global.Player) <= 950 * 2) && Global.Player.UnderEnemyTURRET())
            {
                var noAggro = BotData.EnemyCount == 0 &&
                              GameObjects.EnemyMinions.OrderBy(x => x.Distance(Global.Player)).Count(x => x.Distance(Global.Player) <= 500) <= 2;

                if (noAggro && BotData.MinionAttacks.Count <= 0 && !Global.Player.UnderFountain() && Global.Player.Distance(BotData.MovePosition) <= 100)
                {
                    DebugConsole.WriteLine($"RECALLING", MessageState.Debug);
                    Global.Player.SpellBook.CastSpell(SpellSlot.Recall);
                    BotData.LastRecallAttempt = Environment.TickCount;
                    return;
                }
                BotData.SetDanger();
                DebugConsole.WriteLine($"NEED TO GET TO SAFE POSITION", MessageState.Warn);
                GetToSafety(AllyTurret, noAggro);
                return;
            }

            if (GameObjects.AllyMinions.Count(x => x.IsValidTarget(950, true) && x.UnderEnemyTURRET(850)) >= 2 &&
                EnemyTurret != null &&
                !GameObjects.EnemyHeroes.Any(x => x.Distance(Global.Player) <= BotData.MyRange))
            {
                DebugConsole.WriteLine($"Running to: ENEMY TURRET | ATTACKING", MessageState.Debug);
                BotData.SetNewMovePosition(EnemyTurret.ServerPosition.Extend(Global.Player.ServerPosition, Global.Player.AttackRange), false);
                return;
            }

            var minion = GameObjects.EnemyMinions.OrderBy(x => x.Health).ThenBy(x => x.Distance(Global.Player)).FirstOrDefault(x => x.IsValidTarget(1500));
            if (minion != null &&
                AllyTurret != null &&
                !minion.UnderEnemyTURRET() && !(GameObjects.AllyMinions.Count(x => x.IsValidTarget(1000, true, true, minion.ServerPosition)) <= 1 && Game.ClockTime <= 600) &&
                Global.Player.CountEnemyHeroesInRange(2000) <= 3)
            {
                DebugConsole.WriteLine($"Running to: ENEMY MINION", MessageState.Debug);
                var pos = minion.ServerPosition.Extend(AllyTurret.ServerPosition, Global.Player.AttackRange * .85f);
                BotData.SetNewMovePosition(pos, true, 300);
                return;
            }

            var target = Global.TargetSelector.GetTarget(BotData.IsAggressive() ? 1500 : BotData.MyRange);

            if (target != null &&
                !target.UnderEnemyTURRET(970) &&
                !Global.Player.UnderEnemyTURRET(1000) &&
                BotData.EnemyCount <= 4 &&
                !(BotData.MinionAttacks.Count >= 4 && Game.ClockTime <= 640))
            {
                DebugConsole.WriteLine($"Running to: TARGET", MessageState.Debug);
                var pos = target.ServerPosition.Extend(Global.Player.ServerPosition,
                    Global.Player.IsRanged ? Global.Player.AttackRange * .8f : Global.Player.AttackRange * .25f);
                BotData.SetNewMovePosition(pos, true, 180);
                return;
            }

            if (AllyHero != null)
            {
                DebugConsole.WriteLine($"Running to: ALLY HEROES", MessageState.Debug);
                var pos = AllyHero.ServerPosition.Extend(Global.Player.ServerPosition, -BotData.MyRange * .25f);
                BotData.SetNewMovePosition(pos, true, 700);
                return;
            }

            if (Allyminion != null && AllyTurret != null)
            {
                DebugConsole.WriteLine($"Running to: ALLY MINIONS", MessageState.Debug);
                var path = Allyminion.Path.LastOrDefault();
                var minionPos = Allyminion.ServerPosition;
                var pathPos = path + (path - minionPos).Normalized() * (2.75f * Allyminion.MoveSpeed);
                BotData.SetNewMovePosition(Allyminion.HasPath ? pathPos : Allyminion.ServerPosition.Extend(AllyTurret.ServerPosition, 100));
                return;
            }

            DebugConsole.WriteLine($"BOT CONFUSED", MessageState.Error);
            BotData.SetNewMovePosition(BotData.GetBotPushLane(), false);
        }

        private void GetToSafety(GameObject allyTurret, bool runToGrass = true)
        {
            if (allyTurret == null)
            {
                Console.WriteLine("CANT GET OUT, ALLY TURRET IS NULL");
                return;
            }

            BotData.SetDanger();

            if (runToGrass)
            {
                var nearestGrass = WallExtension.NearestGrass(Global.Player, 800);

                if (!nearestGrass.IsZero)
                {
                    lastGrassVector = nearestGrass;
                    lastFoundGrassTick = Environment.TickCount;
                }

                if (!lastGrassVector.IsZero &&
                    Global.Player.Distance(lastGrassVector) < Global.Player.Distance(allyTurret.ServerPosition))
                {
                    DebugConsole.WriteLine($"Running to: GRASS (SAFETY)", MessageState.Debug);
                    BotData.SetNewMovePosition(Global.Player.ServerPosition.Extend(lastGrassVector, 600), false);
                    return;
                }
                return;
            }

            var allyHero2 = GameObjects.AllyHeroes.FirstOrDefault(x => !x.IsMe && x.IsValidTarget(1800, true) && !x.UnderEnemyTURRET());

            if (allyTurret.Distance(Global.Player) <= 3500 || allyHero2 == null)
            {
                DebugConsole.WriteLine($"Running to: ALLY TURRET (SAFETY)", MessageState.Debug);
                var pos = allyTurret.ServerPosition.Extend(Global.Player.GetFountainPos(), 800);
                BotData.SetNewMovePosition(pos);
            }

            if (allyHero2 != null)
            {
                DebugConsole.WriteLine($"Running to: ALLY HERO (SAFETY)", MessageState.Debug);
                var pos = Global.Player.ServerPosition.Extend(allyHero2.ServerPosition, 800);
                BotData.SetNewMovePosition(pos, true, 700);
            }
        }
    }
}