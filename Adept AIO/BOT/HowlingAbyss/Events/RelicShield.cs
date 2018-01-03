using System.Collections.Generic;
using System.Linq;

namespace Adept_AIO.BOT.HowlingAbyss.Events
{
    using System;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using SDK.Unit_Extensions;

    class RelicShield
    {
        public RelicShield()
        {
            GameObject.OnCreate += delegate(GameObject sender)
            {
                if (sender.Name.Contains("HA_AP_Health"))
                {
                    Console.WriteLine($"CREATED SHIELD {sender.Name}");
                    Shields.Add(sender.ServerPosition);
                }
            };

            GameObject.OnDestroy += delegate (GameObject sender)
            {
                if (sender.Name.Contains("HA_AP_Health"))
                {
                    Console.WriteLine($"DELETED SHIELD {sender.Name}");
                    Shields.Remove(sender.ServerPosition);
                }
            };
        }

        public static Vector3 GetClosestShield()
        {
            var shield = Shields.FirstOrDefault(x => x.Distance(Global.Player) <= 1500 && x.CountEnemyHeroesInRange(1500) == 0);
            return !shield.IsZero ? shield : Vector3.Zero;
        }

        public static List<Vector3> Shields = new List<Vector3>();
    }
}