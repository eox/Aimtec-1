namespace Adept_AIO.BOT.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Prediction.Health;
    using Aimtec.SDK.Util;
    using Menu;
    using SDK.Generic;
    using SDK.Unit_Extensions;
    using SR.Data;

    class BotData
    {
        public static void Load()
        {
            Obj_AI_Base.OnProcessAutoAttack += OnProcessAutoAttack;
            Game.OnUpdate += delegate
            {
                foreach (var minionAttack in MinionAttacks.ToList())
                {
                    if (minionAttack.Key == null || Environment.TickCount - minionAttack.Value < 2500 + Game.Ping)
                    {
                        continue;
                    }
                    DebugConsole.WriteLine($"DELETED ATTACK", MessageState.Warn);
                    MinionAttacks.Remove(minionAttack.Key);
                }
            };
        }

        private static void OnProcessAutoAttack(Obj_AI_Base sender, Obj_AI_BaseMissileClientDataEventArgs args)
        {
            if (sender == null || !args.Target.IsMe)
            {
                return;
            }

            if (args.Sender.IsMinion && !MinionAttacks.ContainsKey(args.Sender))
            {
                MinionAttacks.Add(args.Sender, Environment.TickCount);
                DebugConsole.WriteLine($"INCOMING ATTACKS | {MinionAttacks.Count}", MessageState.Warn);
            }
        }

        public static void SetNewMovePosition(Vector3 position, bool randomizeMovement = true, int range = 450)
        {
            if (randomizeMovement && Global.Player.Distance(position) <= range)
            {
                position += new Vector3(Global.Random.Next(-range, range));
            }

            if (GameObjects.AllyMinions.Count(x => x.IsValidTarget(1500, true) && x.UnderEnemyTURRET(850)) <= 2 &&
                Global.Player.Path.LastOrDefault().UnderEnemyTURRET(950) &&
                Global.Player.Distance(position) <= 800)
            {
                DebugConsole.WriteLine($"PATH IS UNDER TURRET, OVERRIDING PATH TO SAFETY", MessageState.Error);

                position = Global.Player.ServerPosition.Extend(Global.Player.GetFountainPos(), Global.Player.Distance(position));
            }

            if (GameObjects.EnemyHeroes.Any(x => x.IsValidTarget() &&
                                                 (x.HasBuffOfType(BuffType.Poison) || Global.Player.ChampionName == "Twitch" && x.HasBuff("twitchdeadlyvenom")) &&
                                                 x.Distance(Global.Player) <= 1300) &&
                position.UnderEnemyTURRET())
            {
                DebugConsole.WriteLine($"ENEMY HAS POSIION DEBUFF, CANT BE IN ENEMY TURRET RIGHT NOW", MessageState.Error);
                return;
            }

            if (Global.Player.Path.LastOrDefault().Distance(position) <= 200 ||
                NavMesh.WorldToCell(position).Flags == NavCellFlags.Wall ||
                NavMesh.WorldToCell(position).Flags == NavCellFlags.Building)
            {
                return;
            }

            LastStepTick = Environment.TickCount;
            MovePosition = position;
        }

        public static void SetDanger()
        {
            IsInDanger = true;

            DelayAction.Queue(1000,
                () =>
                {
                    IsInDanger = false;
                },
                new CancellationToken(false));
        }

        public static Dictionary<Obj_AI_Base, float> MinionAttacks;

        public static Vector3 MovePosition;
        public static float LastStepTick;
        public static float LastRecallAttempt;
        public static bool IsInDanger;

        public static bool IsTurretValid(TurretAttackManager.TurretData turretData)
        {
            return turretData != null && turretData.TurretActive && turretData.Turret != null;
        }

        public static bool CannotContinueMovement()
        {
            return Global.Player.HealthPercent() < 80 && Global.Player.UnderFountain() || Game.ClockTime < 14.5;
        }

        public static int AllyCount => Global.Player.CountAllyHeroesInRange(1500);
        public static int EnemyCount => Global.Player.CountEnemyHeroesInRange(1800);
        public static int MyRange => (int) (Global.Player.IsRanged ? Global.Player.AttackRange : MenuConfig.Mainmenu["Range"].Value);

        public static bool IsAggressive()
        {
            if (EnemyCount >= 3 && AllyCount + 1 <= 2 || EnemyCount == 2 && AllyCount + 1 < 2)
            {
                return false;
            }

            var enemy = GameObjects.EnemyHeroes.FirstOrDefault(x => Global.Player.HealthPercent() > 50 && (x.HealthPercent() <= 70 || x.CountAllyHeroesInRange(500) >= 1 || x.IsHardCc()) && x.IsValidTarget(1500));
            return enemy != null;
        }

        public static Vector3 GetBotPreLanePhase()
        {
            switch (Global.Player.Team)
            {
                case GameObjectTeam.Order:

                    switch (PlayerData.LANE_TYPE)
                    {
                        case LaneType.Botlane: return new Vector3(7090, 53, 3177);
                        case LaneType.Midlane: return new Vector3(5090, -32, 8515.5f);
                        case LaneType.Toplane: return new Vector3(3923.4f, -68.4f, 9447f);
                    }
                    break;

                case GameObjectTeam.Chaos:
                    switch (PlayerData.LANE_TYPE)
                    {
                        case LaneType.Botlane: return new Vector3(10836, -64, 5553);
                        case LaneType.Midlane: return new Vector3(9705, -39, 6326);
                        case LaneType.Toplane: return new Vector3(4542.8f, 56f, 11583f);
                    }
                    break;
            }


            return Vector3.Zero;
        }

        public static Vector3 GetBotPushLaneAram()
        {
            switch (Global.Player.Team)
            {
                case GameObjectTeam.Order: return new Vector3(5307, -178, 4876);
                case GameObjectTeam.Chaos: return new Vector3(7871, -177, 7445);
            }

            return Vector3.Zero;
        }

        public static Vector3 GetBotPushLane()
        {
            if (Game.ClockTime >= 700)
            {
                var fountain = Global.Player.GetFountainPos();
                var enemyNearFountain = GameObjects.EnemyHeroes.FirstOrDefault(x => x.IsValidTarget() && x.Distance(fountain) <= 3000);
                if (enemyNearFountain != null)
                {
                    return enemyNearFountain.ServerPosition;
                }

                foreach (var allyTurret in GameObjects.AllyTurrets.Where(x => x.IsValid))
                {
                    if (GameObjects.EnemyHeroes.Count(x => x.Distance(allyTurret) <= 1500) >= 2 || GameObjects.EnemyMinions.Count(x => x.Distance(allyTurret) <= 2250) >= 6 && GameObjects.AllyHeroes.Count(x => x.Distance(allyTurret) <= 2000) == 0)
                    {
                        return allyTurret.ServerPosition; 
                    }
                }

                foreach (var enemyTurret in GameObjects.EnemyTurrets.Where(x => x.IsValid))
                {
                    if (GameObjects.AllyHeroes.Count(x => x.Distance(enemyTurret) <= 1500) >= 2)
                    {
                        return enemyTurret.ServerPosition;
                    }
                }

                foreach (var hero in GameObjects.AllyHeroes.Where(x => x.IsValidTarget(Single.MaxValue, true) && (x.CountEnemyHeroesInRange(2000) >= 3 || x.CountAllyHeroesInRange(3000) >= 3)))
                {
                    return hero.ServerPosition;
                }

                var closestJungleCamp = JungleData.GetClosestNormalCamp();
                if (closestJungleCamp.Distance(Global.Player) <= 1000)
                {
                    return closestJungleCamp;
                }

                // Checks if there is any viable mob for player. Currently only works if the mobs are visible
                foreach (var mob in GameObjects.Jungle.OrderBy(x => x.MaxHealth).ThenBy(x => x.Distance(Global.Player)).Where(x => x.CountAllyHeroesInRange(700) >= 1))
                {
                    if (mob.GetJungleType() == GameObjects.JungleType.Legendary)
                    {
                        return mob.ServerPosition;
                    }

                    if (mob.Distance(Global.Player) > 4000)
                    {
                        continue;
                    }

                    if (mob.UnitSkinName.ToLower().Contains("red") && (PlayerData.CHAMPION_TYPE == ChampionType.ADC || PlayerData.CHAMPION_TYPE == ChampionType.BRUISER))
                    {
                        return mob.ServerPosition;
                    }

                    if (mob.UnitSkinName.ToLower().Contains("blue") &&
                        PlayerData.CHAMPION_TYPE == ChampionType.MAGE &&
                        GameObjects.AllyHeroes.Any(x => x.IsJungler() && x.Distance(mob) <= 700))
                    {
                        return mob.ServerPosition;
                    }
                }
            }

            switch (Global.Player.Team)
            {
                case GameObjectTeam.Order:

                    switch (PlayerData.LANE_TYPE)
                    {
                        case LaneType.Botlane: return new Vector3(10459, 50, 1485);
                        case LaneType.Midlane: return new Vector3(6301, 52, 6335);
                        case LaneType.Toplane: return new Vector3(1399, 53, 10427);
                    }
                    break;

                case GameObjectTeam.Chaos:

                    switch (PlayerData.LANE_TYPE)
                    {
                        case LaneType.Botlane: return new Vector3(13482, 52, 4406);
                        case LaneType.Midlane: return new Vector3(8656, 54, 8721);
                        case LaneType.Toplane: return new Vector3(4254, 53, 13560);
                    }

                    break;
            }


            return Vector3.Zero;
        }

        public static void GetChampionType()
        {
            var playerChampionName = Global.Player.ChampionName;

            if (TypeNames.ADC.Contains(playerChampionName))
            {
                PlayerData.CHAMPION_TYPE = ChampionType.ADC;
            }

            if (TypeNames.MAGES.Contains(playerChampionName))
            {
                PlayerData.CHAMPION_TYPE = ChampionType.MAGE;
            }

            if (TypeNames.TANKS.Contains(playerChampionName))
            {
                PlayerData.CHAMPION_TYPE = ChampionType.TANK;
            }

            if (TypeNames.SUPPORTS.Contains(playerChampionName))
            {
                PlayerData.CHAMPION_TYPE = ChampionType.SUPPORT;
            }

            if (TypeNames.BRUISERS.Contains(playerChampionName))
            {
                PlayerData.CHAMPION_TYPE = ChampionType.BRUISER;
            }

            switch (PlayerData.CHAMPION_TYPE)
            {
                case ChampionType.ADC:
                case ChampionType.SUPPORT:
                    PlayerData.LANE_TYPE = LaneType.Botlane;
                    break;
                case ChampionType.MAGE:
                    PlayerData.LANE_TYPE = LaneType.Midlane;
                    break;
                case ChampionType.BRUISER:
                case ChampionType.TANK:
                    PlayerData.LANE_TYPE = LaneType.Toplane;
                    break;
            }
        }
    }
}