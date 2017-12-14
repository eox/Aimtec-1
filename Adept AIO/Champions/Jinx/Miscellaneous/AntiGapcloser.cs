namespace Adept_AIO.Champions.Jinx.Miscellaneous
{
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Core;
    using SDK.Delegates;
    using SDK.Unit_Extensions;

    class AntiGapcloser
    {
        private readonly SpellConfig spellConfig;

        public AntiGapcloser(SpellConfig spellConfig)
        {
            this.spellConfig = spellConfig;
        }

        public void OnGapcloser(Obj_AI_Hero sender, GapcloserArgs args)
        {
            if (!sender.IsEnemy || !spellConfig.E.Ready || args.EndPosition.Distance(Global.Player) > spellConfig.E.Range)
            {
                return;
            }

            spellConfig.E.Cast(args.EndPosition);
        }
    }
}