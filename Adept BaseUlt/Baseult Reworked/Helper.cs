namespace Adept_BaseUlt.Baseult_Reworked
{
    using Aimtec;
    using Aimtec.SDK.Damage;
    using Aimtec.SDK.Damage.JSON;
    using Aimtec.SDK.Extensions;
    using Local_SDK;

    class Helper
    {
        public static float Speed, Delay;

        public static bool IsValid(Recall_Information information)
        {
            return MenuConfig.Menu[((Obj_AI_Hero)information.Sender).ChampionName].Enabled && PlayerDamage(information.Sender) > TargetHealth(information.Sender);
        }

        public static float TravelTime(Vector3 position)
        {
            return Global.Player.Distance(position) / Speed * 1000 + Delay + Game.Ping / 2f;
        }

        public static int GetCastTime(Vector3 position, Recall_Information information)
        {
            return (int)(-(Game.TickCount - (information.Start + information.Duration)) - TravelTime(position));
        }

        public static float TargetHealth(Obj_AI_Base target)
        {
            if (target == null)
            {
                return 0f;
            }

            //var invisible = Baseult.Enemies.FirstOrDefault(x => x.NetworkId == _target.NetworkId);

            //if (invisible == null)
            //{
            //    Console.WriteLine("ENEMY IS NULL, CANT GET HP");
            //    return 0f;
            //}

            //var hpReg = invisible.HPRegenRate;
            //var final = invisible.Health + (hpReg * (_target.LifetimeTicks / 10000f) + TravelTime(GetFountainPos()) / 1000);

            //DebugConsole.WriteLine($"Target Health: {(int)final} | Damage: {PlayerDamage(_target)}", MessageState.Debug);

            return target.Health * 1.1f;
        }

        public static Vector3 GetFountainPos()
        {
            switch (Game.Type)
            {
                case GameType.Normal:
                    switch (Global.Player.Team)
                    {
                        case GameObjectTeam.Order: return new Vector3(14340, 171.9777f, 14390);
                        case GameObjectTeam.Chaos: return new Vector3(396,   185.1325f, 462);
                    }
                    break;
            }
            return Vector3.Zero;
        }

        public static float PlayerDamage(Obj_AI_Base target)
        {
            switch (Global.Player.ChampionName)
            {
                case "Draven":
                    if (MenuConfig.Menu["Draven"].Enabled)
                    {
                        return (float)(Global.Player.GetSpellDamage(target, SpellSlot.R, DamageStage.SecondForm) + Global.Player.GetSpellDamage(target, SpellSlot.R));
                    }
                    return (float)Global.Player.GetSpellDamage(target, SpellSlot.R, DamageStage.SecondForm);
                case "Jinx": return (float)Global.Player.GetSpellDamage(target, SpellSlot.R, DamageStage.Empowered);
            }
            return (float)Global.Player.GetSpellDamage(target, SpellSlot.R);
        }
    }
}
