namespace Adept_AIO.Champions.Jinx.Core
{
    using Aimtec;
    using Aimtec.SDK.Damage;
    using SDK.Unit_Extensions;

    class Dmg
    {
        public double Damage(Obj_AI_Base target)
        {
            if (target == null)
            {
                return 0;
            }

            var dmg = 0d;

            if (Global.Orbwalker.CanAttack())
            {
                dmg += Global.Player.GetAutoAttackDamage(target);
            }

            return dmg;
        }
    }
}