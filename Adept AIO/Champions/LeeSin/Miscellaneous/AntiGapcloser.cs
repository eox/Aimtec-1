namespace Adept_AIO.Champions.LeeSin.Miscellaneous
{
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Core.Spells;
    using SDK.Delegates;
    using SDK.Unit_Extensions;
    using Ward_Manager;

    class AntiGapcloser
    {
        private readonly ISpellConfig spellConfig;
        private readonly IWardManager wardManager;
        private readonly IWardTracker wardTracker;

        public AntiGapcloser(ISpellConfig spellConfig, IWardManager wardManager, IWardTracker wardTracker)
        {
            this.spellConfig = spellConfig;
            this.wardManager = wardManager;
            this.wardTracker = wardTracker;
        }

        public void OnGapcloser(Obj_AI_Hero sender, GapcloserArgs args)
        {
            if (sender.IsMe || !sender.IsEnemy || !spellConfig.W.Ready || !spellConfig.IsFirst(spellConfig.W) || !wardTracker.IsWardReady() ||
                args.EndPosition.Distance(Global.Player) > 425)
            {
                return;
            }

            wardManager.WardJump(Game.CursorPos, spellConfig.WardRange);
        }
    }
}