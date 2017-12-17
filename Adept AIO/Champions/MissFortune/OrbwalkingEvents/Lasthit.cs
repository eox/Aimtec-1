using System.Linq;

namespace Adept_AIO.Champions.MissFortune.OrbwalkingEvents
{
    using Aimtec;
    using Aimtec.SDK.Damage;
    using Aimtec.SDK.Damage.JSON;
    using Aimtec.SDK.Extensions;
    using SDK.Unit_Extensions;
    using Core;
    using GameObjects = SDK.Unit_Extensions.GameObjects;

    class Lasthit
    {
        public static void OnUpdate()
        {
            var minion = GameObjects.EnemyMinions.OrderBy(x => x.Health)
                .FirstOrDefault(x => x.IsValidTarget(SpellManager.Q.Range) && x.Health < Global.Player.GetSpellDamage(x, SpellSlot.Q) && 
                GameObjects.EnemyMinions.Any(y => y.NetworkId != x.NetworkId &&
                SpellManager.Cone(x).IsInside(y.ServerPosition.To2D()) &&
                y.Health < Global.Player.GetSpellDamage(y, SpellSlot.Q, DamageStage.Empowered)));

            if (minion == null)
            {
                return;
            }

            SpellManager.Q.CastOnUnit(minion);
        }
    }
}
