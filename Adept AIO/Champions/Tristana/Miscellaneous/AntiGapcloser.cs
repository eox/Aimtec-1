namespace Adept_AIO.Champions.Tristana.Miscellaneous
{
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Core;
    using SDK.Delegates;
    using SDK.Spell_DB;
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
            if (sender.IsMe || sender.IsAlly || args.EndPosition.Distance(Global.Player) > spellConfig.FullRange)
            {
                return;
            }

            var missile = SpellDatabase.GetByName(args.SpellName);
            if (missile == null || !missile.IsDangerous)
            {
                return;
            }

            if (spellConfig.W.Ready)
            {
                spellConfig.W.Cast(GameObjects.AllySpawnPoints.FirstOrDefault().ServerPosition);
            }
            else if (spellConfig.R.Ready)
            {
                spellConfig.R.CastOnUnit(sender);
            }
        }
    }
}