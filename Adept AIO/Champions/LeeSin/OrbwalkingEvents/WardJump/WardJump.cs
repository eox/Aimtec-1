namespace Adept_AIO.Champions.LeeSin.OrbwalkingEvents.WardJump
{
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Core.Spells;
    using SDK.Unit_Extensions;
    using Ward_Manager;

    class WardJump : IWardJump
    {
        private readonly ISpellConfig spellConfig;

        private readonly IWardManager wardManager;

        private readonly IWardTracker wardTracker;

        public WardJump(IWardTracker wardTracker, IWardManager wardManager, ISpellConfig spellConfig)
        {
            this.wardTracker = wardTracker;
            this.wardManager = wardManager;
            this.spellConfig = spellConfig;
        }

        public bool Enabled { get; set; }

        public void OnKeyPressed()
        {
            if (!this.Enabled)
            {
                return;
            }

            if (spellConfig.W.Ready && spellConfig.IsFirst(spellConfig.W) && wardTracker.IsWardReady())
            {
                var cursorDist = (int) Global.Player.Distance(Game.CursorPos);
                var dist = cursorDist <= spellConfig.WardRange ? cursorDist : spellConfig.WardRange;
                wardManager.WardJump(Game.CursorPos, dist);
            }
        }
    }
}