using System;
using System.Collections.Generic;
using System.Linq;

namespace Adept_AIO.BOT.SR.Data
{
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using SDK.Unit_Extensions;

    class JungleData
    {
        public static Vector3 GetClosestNormalCamp()
        {
            var closest = Camps.AllCamps.OrderBy(x => x.Position.Distance(Global.Player)).FirstOrDefault(x => !x.IsObjective && x.IsCampValid());
            if (closest != null)
            {
                return closest.Position;
            }
            return Vector3.Zero;
        }
    }

    public class JungleCamp
    {
        public Vector3 Position { get; set; }
        public float RespawnTime { get; set; }
        public float SpawnTime { get; set; }
        public bool IsObjective { get; set; }
     
        public bool IsCampValid(int time = 0)
        {
            return (Environment.TickCount - this.SpawnTime) / 1000 > this.RespawnTime - time && Game.ClockTime >= SpawnTime;
        }
    }

    public class Camps
    {
        public Camps()
        {
            #region Neutral

            Herald = new JungleCamp
            {
                Position = new Vector3(4980, -71, 10404),
                RespawnTime = int.MaxValue,
                SpawnTime = 570,
                IsObjective = true
            };

            Baron = new JungleCamp
            {
                Position = new Vector3(4980, -71, 10404),
                RespawnTime = 420,
                SpawnTime = 1200,
                IsObjective = true
            };

            Dragon = new JungleCamp
            {
                Position = new Vector3(9838, -71, 4374),
                RespawnTime = 360,
                SpawnTime = 1200,
                IsObjective = true
            };

            CrabTopSide = new JungleCamp
            {
                Position = new Vector3(4345, -66, 9511),
                RespawnTime = 180,
                SpawnTime = 135,
                IsObjective = false
            };

            CrabBotSide = new JungleCamp
            {
                Position = new Vector3(10535, -63, 4945),
                RespawnTime = 180,
                SpawnTime = 135,
                IsObjective = false
            };

            ObjectiveCamps = new List<JungleCamp>()
            {
                Dragon,
                Baron
            };

            #endregion


            #region Order

            WolvesOrder = new JungleCamp
            {
                Position = new Vector3(3828, -71, 4374),
                RespawnTime = 150,
                SpawnTime = 90,
                IsObjective = false,
            };

            GrompOrder = new JungleCamp
            {
                Position = new Vector3(2194, -52, 8382),
                RespawnTime = 150,
                SpawnTime = 107,
                IsObjective = false,
            };

            BlueBuffOrder = new JungleCamp
            {
                Position = new Vector3(3800, 52, 7900),
                RespawnTime = 300,
                SpawnTime = 90,
                IsObjective = false
            };

            RedBuffOrder = new JungleCamp
            {
                Position = new Vector3(7754, 54, 3959),
                RespawnTime = 300,
                SpawnTime = 90,
                IsObjective = false
            };

            RaptorOrder = new JungleCamp
            {
                Position = new Vector3(6993, 51, 5363),
                RespawnTime = 150,
                SpawnTime = 90,
                IsObjective = false,
            };

            GolemOrder = new JungleCamp
            {
                Position = new Vector3(8423, 51, 2646),
                RespawnTime = 150,
                SpawnTime = 90,
                IsObjective = false,
            };

            OrderCamps = new List<JungleCamp>
            {
                GolemOrder,
                RaptorOrder,
                RedBuffOrder,
                BlueBuffOrder,
                GrompOrder,
                WolvesOrder,
                CrabTopSide
            };

            #endregion

            #region Chaos

            WolvesChaos = new JungleCamp
            {
                Position = new Vector3(10976, 62, 8308),
                RespawnTime = 150,
                SpawnTime = 90,
                IsObjective = false,
            };

            GrompChaos = new JungleCamp
            {
                Position = new Vector3(12663, 52, 6361),
                RespawnTime = 150,
                SpawnTime = 107,
                IsObjective = false,
            };

            BlueBuffChaos = new JungleCamp
            {
                Position = new Vector3(10994, 52, 6951),
                RespawnTime = 300,
                SpawnTime = 90,
                IsObjective = false
            };

            RedBuffChaos = new JungleCamp
            {
                Position = new Vector3(7128, 56, 10833),
                RespawnTime = 300,
                SpawnTime = 90,
                IsObjective = false
            };

            RaptorChaos = new JungleCamp
            {
                Position = new Vector3(7846, 52, 9569),
                RespawnTime = 150,
                SpawnTime = 90,
                IsObjective = false,
            };

            GolemChaos = new JungleCamp
            {
                Position = new Vector3(6458, 56, 12110),
                RespawnTime = 150,
                SpawnTime = 90,
                IsObjective = false,
            };

            ChaosCamps = new List<JungleCamp>
            {
                GolemChaos,
                RaptorChaos,
                RedBuffChaos,
                BlueBuffChaos,
                GrompChaos,
                WolvesChaos,
                CrabBotSide
            };

            AllCamps = new List<JungleCamp>();
            foreach (var chaosCamp in ChaosCamps)
            {
                AllCamps.Add(chaosCamp);
            }

            foreach (var orderCamp in OrderCamps)
            {
                AllCamps.Add(orderCamp);
            }

            foreach (var objectiveCamp in ObjectiveCamps)
            {
                AllCamps.Add(objectiveCamp);
            }

            #endregion
        }

        public static JungleCamp Baron, Herald, Dragon;
        public static JungleCamp CrabTopSide, CrabBotSide;

        public static JungleCamp BlueBuffOrder, RedBuffOrder, WolvesOrder, GrompOrder, RaptorOrder, GolemOrder;
        public static JungleCamp BlueBuffChaos, RedBuffChaos, WolvesChaos, GrompChaos, RaptorChaos, GolemChaos;

        public static List<JungleCamp> OrderCamps, ChaosCamps, ObjectiveCamps, AllCamps;
    }
}
