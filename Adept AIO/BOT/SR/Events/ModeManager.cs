namespace Adept_AIO.BOT.SR.Events
{
    using System.Linq;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Menu.Config;
    using SDK.Unit_Extensions;
    using Utilities;

    class ModeManager
    {
        public void Update()
        {
            if (BotData.CannotContinueMovement())
            {
                return;
            }

            if (BotData.IsInDanger)
            {
                KeyManager.ChangeKey(GlobalKeys.LastHitKey);
                return;
            }

            var safeToHit = !Global.Player.UnderEnemyTURRET(1000) && GameObjects.EnemyHeroes.Count(x => x.UnderEnemyTURRET(900) && x.IsValidTarget(1400, true)) == 0 && !BotData.IsInDanger;

            if (Global.Player.CountEnemyHeroesInRange(BotData.IsAggressive() ? 1100 : BotData.MyRange * 1.1f) >= 1 && safeToHit)
            {
                KeyManager.ChangeKey(GlobalKeys.ComboKey);
                return;
            }

            var minion = GameObjects.EnemyMinions.FirstOrDefault(x => x.IsValidTarget(BotData.MyRange));
            var jungleMob = GameObjects.Jungle.FirstOrDefault(x => !x.IsAlly && x.IsValidTarget(BotData.MyRange));
            var enemyTurret = GameObjects.EnemyTurrets.FirstOrDefault(x => x.IsValidTarget(BotData.MyRange + 100));

            if (enemyTurret != null || (minion != null || jungleMob != null) && Global.Player.CountEnemyHeroesInRange(BotData.MyRange) <= 0)
            {
                KeyManager.ChangeKey(GlobalKeys.WaveClearKey);
            }

            else if(safeToHit)
            {
                KeyManager.ChangeKey(GlobalKeys.MixedKey);
            }
        }
    }
}