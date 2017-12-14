namespace Adept_AIO.Champions.LeeSin.Core.Insec_Manager
{
    using System;
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using SDK.Unit_Extensions;
    using Spells;

    class InsecManager : IInsecManager
    {
        private readonly ISpellConfig spellConfig;

        public InsecManager(ISpellConfig spellConfig)
        {
            this.spellConfig = spellConfig;
        }

        public int InsecKickValue { get; set; }
        public int InsecPositionValue { get; set; }

        public float DistanceBehindTarget(Obj_AI_Base target = null)
        {
            return Math.Min((Global.Player.BoundingRadius + (target == null ? 65 : target.BoundingRadius) + 50) * 1.25f, spellConfig.R.Range);
        }

        public Vector3 InsecPosition(Obj_AI_Base target)
        {
            var pos = target.ServerPosition + (target.ServerPosition - GetTargetEndPosition()).Normalized() * DistanceBehindTarget(target);

            return NavMesh.WorldToCell(pos).Flags.HasFlag(NavCellFlags.Wall) ? Vector3.Zero : pos;
        }

        public Vector3 BkPosition(Obj_AI_Base target)
        {
            if (target == null)
            {
                return Vector3.Zero;
            }

            var secondEnemy = GameObjects.EnemyHeroes.FirstOrDefault(x => x.NetworkId != target.NetworkId && x.Distance(target) <= spellConfig.R2.Range + 100);

            if (secondEnemy == null)
            {
                return Vector3.Zero;
            }

            return secondEnemy.ServerPosition.Extend(secondEnemy.ServerPosition + (secondEnemy.ServerPosition - target.ServerPosition).Normalized(), DistanceBehindTarget());
        }

        public Vector3 GetTargetEndPosition()
        {
            var ally = GameObjects.AllyHeroes.FirstOrDefault(x => x.Distance(Global.Player) <= 2000);
            var turret = GameObjects.AllyTurrets.OrderBy(x => x.Distance(Global.Player)).FirstOrDefault();

            switch (this.InsecPositionValue)
            {
                case 0:
                    if (turret != null)
                    {
                        Temp.IsAlly = false;
                        return turret.ServerPosition;
                    }
                    else if (ally != null)
                    {
                        Temp.IsAlly = true;
                        return ally.ServerPosition;
                    }
                    break;
                case 1:
                    if (ally != null)
                    {
                        Temp.IsAlly = true;
                        return ally.ServerPosition;
                    }
                    else if (turret != null)
                    {
                        Temp.IsAlly = false;
                        return turret.ServerPosition;
                    }
                    break;
            }
            return Vector3.Zero;
        }
    }
}