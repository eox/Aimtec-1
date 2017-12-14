namespace Adept_AIO.Champions.LeeSin.OrbwalkingEvents.KickFlash
{
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Core.Insec_Manager;
    using Core.Spells;
    using SDK.Unit_Extensions;
    using SDK.Usables;

    class KickFlash : IKickFlash
    {
        private readonly IInsecManager insecManager;
        private readonly ISpellConfig spellConfig;

        public KickFlash(ISpellConfig spellConfig, IInsecManager insecManager)
        {
            this.spellConfig = spellConfig;
            this.insecManager = insecManager;
        }

        private Obj_AI_Hero Target => Global.TargetSelector.GetSelectedTarget();

        public void OnKeyPressed()
        {
            if (!this.Enabled || this.Target == null || !spellConfig.R.Ready || !this.Target.IsValidTarget(spellConfig.R.Range) || SummonerSpells.Flash == null ||
                !SummonerSpells.Flash.Ready)
            {
                return;
            }

            spellConfig.R.CastOnUnit(this.Target);
        }

        public void OnProcessSpellCast(Obj_AI_Base sender, Obj_AI_BaseMissileClientDataEventArgs args)
        {
            if (sender == null || !sender.IsMe || args.SpellSlot != SpellSlot.R || !this.Enabled)
            {
                return;
            }

            SummonerSpells.Flash.Cast(insecManager.InsecPosition(this.Target));
        }

        public bool Enabled { get; set; }
    }
}