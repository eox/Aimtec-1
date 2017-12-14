namespace Adept_AIO.Champions.LeeSin.Miscellaneous
{
    using System;
    using Aimtec.SDK.Orbwalking;
    using OrbwalkingEvents.Combo;
    using OrbwalkingEvents.Harass;
    using OrbwalkingEvents.JungleClear;
    using OrbwalkingEvents.LaneClear;
    using OrbwalkingEvents.LastHit;
    using SDK.Unit_Extensions;

    class Manager
    {
        private readonly ICombo combo;
        private readonly IHarass harass;
        private readonly IJungleClear jungleClear;
        private readonly ILaneClear laneClear;
        private readonly ILasthit lasthit;

        public Manager(ICombo combo, IHarass harass, IJungleClear jungleClear, ILaneClear laneClear, ILasthit lasthit)
        {
            this.combo = combo;
            this.harass = harass;
            this.jungleClear = jungleClear;
            this.laneClear = laneClear;
            this.lasthit = lasthit;
        }

        public void PostAttack(object sender, PostAttackEventArgs args)
        {
            switch (Global.Orbwalker.Mode)
            {
                case OrbwalkingMode.Combo:
                    combo.OnPostAttack(args.Target);
                    break;
                case OrbwalkingMode.Mixed:
                    harass.OnPostAttack(args.Target);
                    break;
                case OrbwalkingMode.Laneclear:
                    laneClear.OnPostAttack();
                    jungleClear.OnPostAttack(args.Target);
                    break;
            }
        }

        public void OnUpdate()
        {
            try
            {
                if (Global.Player.IsDead || Global.Orbwalker.IsWindingUp)
                {
                    return;
                }

                switch (Global.Orbwalker.Mode)
                {
                    case OrbwalkingMode.Combo:
                        combo.OnUpdate();
                        break;
                    case OrbwalkingMode.Mixed:
                        harass.OnUpdate();
                        break;
                    case OrbwalkingMode.Laneclear:
                        laneClear.OnUpdate();
                        jungleClear.OnUpdate();
                        break;
                    case OrbwalkingMode.Lasthit:
                        lasthit.OnUpdate();
                        break;
                }

                jungleClear.SmiteMob();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}