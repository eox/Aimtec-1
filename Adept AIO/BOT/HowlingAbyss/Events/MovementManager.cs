namespace Adept_AIO.BOT.HowlingAbyss.Events
{
    using System;
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using SDK.Generic;
    using SDK.Unit_Extensions;
    using Utilities;

    class MovementManager
    {
        public MovementManager()
        {
            Obj_AI_Base.OnProcessAutoAttack += OnProcessAutoAttack;
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
          
            if (BotData.CannotContinueMovement() || !Global.Orbwalker.CanMove() || Environment.TickCount - BotData.LastStepTick <= Global.Random.Next(700, 1700))
            {
                return;
            }

            if (Game.ClockTime <= 55)
            {
                DebugConsole.WriteLine($"Running to: LANE", MessageState.Debug);

                BotData.SetNewMovePosition(BotData.GetBotPushLaneAram());
                return;
            }

            var allyTurret = GameObjects.AllyTurrets.Where(x => !x.IsDead).OrderBy(x => Global.Player.Distance(x)).FirstOrDefault();

            var enemyTurret = GameObjects.EnemyTurrets.Where(x => !x.IsDead).OrderBy(x => Global.Player.Distance(x)).FirstOrDefault();

            var allyHero = GameObjects.AllyHeroes.
                OrderBy(x => x.Distance(Global.Player)).
                ThenByDescending(x => x.Health).
                FirstOrDefault(x => x.NetworkId != Global.Player.NetworkId && !x.IsRecalling() && x.HealthPercent() > 40 && x.Distance(Global.Player) <= 2000);

            var allyminion = GameObjects.AllyMinions.
                Where(x => x.IsValidTarget(3000, true) && x.IsMinion).
                OrderBy(x => x.MaxHealth).
                ThenBy(x => Global.Player.Distance(x)).
                FirstOrDefault(x => x.Team == Global.Player.Team);


            if (BotData.IsInDanger || Global.Player.UnderEnemyTURRET() && GameObjects.AllyMinions.Count(x => x.IsValidTarget(1100, true) && x.UnderEnemyTURRET()) <= 3)
            {
                GetToSafety(allyTurret);
                return;
            }

            var shield = RelicShield.GetClosestShield();
            if (shield.IsZero && Global.Player.HealthPercent() <= 50)
            {
                BotData.SetNewMovePosition(shield, false);
            }

            if (Global.Player.HealthPercent() <= 30 ||
                Global.Player.HealthPercent() <= 40 &&
                 GameObjects.EnemyHeroes.Any(x => x.IsValidTarget(1300) &&
                                                 x.HealthPercent() >= 60) &&
                BotData.EnemyCount > BotData.AllyCount ||
                !GameObjects.AllyMinions.Any(x => x.IsUnderEnemyTurret() && x.Distance(Global.Player) <= 950 * 2) && Global.Player.IsUnderEnemyTurret())
            {
                DebugConsole.WriteLine($"ATTACKED OR WILL BE ATTACKED BY TURRET", MessageState.Warn);
                GetToSafety(allyTurret);
                return;
            }

            if (allyminion != null && allyminion.IsUnderEnemyTurret() && enemyTurret != null && !GameObjects.EnemyHeroes.Any(x => x.Distance(Global.Player) <= 1000))
            {
                DebugConsole.WriteLine($"Running to: ENEMY TURRET | ATTACKING", MessageState.Debug);
                BotData.SetNewMovePosition(enemyTurret.ServerPosition.Extend(Global.Player.ServerPosition, Global.Player.AttackRange), false);
                return;
            }

            var minion = GameObjects.EnemyMinions.OrderBy(x => x.Health).ThenBy(x => x.Distance(Global.Player)).FirstOrDefault(x => x.IsValidTarget(1500));
            if (minion != null &&
                allyTurret != null &&
                !minion.UnderEnemyTURRET() && !(GameObjects.AllyMinions.Count(x => x.IsValidTarget(1000, true, true, minion.ServerPosition)) <= 1 && Game.ClockTime <= 600) &&
                Global.Player.CountEnemyHeroesInRange(2000) <= 3)
            {
                DebugConsole.WriteLine($"Running to: ENEMY MINION", MessageState.Debug);
                var pos = minion.ServerPosition.Extend(allyTurret.ServerPosition, Global.Player.AttackRange * .85f);
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

            if (allyHero != null)
            {
                DebugConsole.WriteLine($"Running to: ALLY HEROES", MessageState.Debug);
                var pos = allyHero.ServerPosition.Extend(Global.Player.ServerPosition, -BotData.MyRange * .25f);
                BotData.SetNewMovePosition(pos, true, 700);
                return;
            }

            if (allyminion != null && allyTurret != null)
            {
                DebugConsole.WriteLine($"Running to: ALLY MINIONS", MessageState.Debug);
                var path = allyminion.Path.LastOrDefault();
                var minionPos = allyminion.ServerPosition;
                var pathPos = path + (path - minionPos).Normalized() * (2.75f * allyminion.MoveSpeed);
                BotData.SetNewMovePosition(allyminion.HasPath ? pathPos : allyminion.ServerPosition);
                return;
            }

            DebugConsole.WriteLine($"BOT CONFUSED", MessageState.Error);
            BotData.SetNewMovePosition(BotData.GetBotPushLaneAram(), false);
        }

    
        private void GetToSafety(GameObject allyTurret)
        {
            if (allyTurret == null)
            {
                return;
            }

            DebugConsole.WriteLine($"Running to: ALLY TURRET", MessageState.Debug);
            var pos = Global.Player.ServerPosition.Extend(allyTurret.ServerPosition, 800);
            BotData.SetNewMovePosition(pos);
        }
    }
}