namespace Adept_AIO.BOT.Utilities
{
    using System.Collections.Generic;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Util;
    using SDK.Unit_Extensions;

    class LevelUpManager
    {
        public LevelUpManager()
        {
            Obj_AI_Base.OnLevelUp += OnLevelUp;
        }

        private void OnLevelUp(Obj_AI_Base sender, Obj_AI_BaseLevelUpEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            foreach (var i in new Dictionary<SpellSlot, int>())
            {
                var spell = Global.Player.SpellBook.GetSpell(i.Key);

                if (/*args.NewLevel <= 3 && spell.Level == 0 && */CanLevelUpSpell(i.Key, args.NewLevel))
                {
                    DelayAction.Queue(600, () => Global.Player.SpellBook.LevelSpell(i.Key));
                }
            }
        }

        private bool CanLevelUpSpell(SpellSlot spell, int newLevel)
        {
            var currentLevel = Global.Player.GetSpell(spell).Level;

            if (spell == SpellSlot.R && Global.Player.ChampionName != "Udyr")
            {
                if (newLevel >= 6  && newLevel < 11     && currentLevel == 0 ||
                    newLevel >= 11 && currentLevel < 16 && newLevel == 1     ||
                    newLevel >= 16 && currentLevel == 2)
                {
                    return true;
                }

                if (newLevel >= 16 && newLevel <= 2)
                {
                    return Global.Player.ChampionName != "Ryze" || currentLevel != 2;
                }
            }
            else
            {
                if (newLevel >= 1 && newLevel < 3 && currentLevel == 0 ||
                    newLevel >= 3 && newLevel < 5 && currentLevel <= 1 ||
                    newLevel >= 5 && newLevel < 7 && currentLevel <= 2 ||
                    newLevel >= 7 && newLevel < 9 && currentLevel <= 3 ||
                    newLevel >= 9 &&                 currentLevel <= 4)
                {
                    return true;
                }

                if (Global.Player.ChampionName == "Ryze" && spell == SpellSlot.Q && newLevel >= 11 && currentLevel <= 5)
                {
                    return true;
                }
            }
            return false;
        }
    }
}