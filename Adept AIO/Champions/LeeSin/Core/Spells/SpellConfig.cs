namespace Adept_AIO.Champions.LeeSin.Core.Spells
{
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Orbwalking;
    using Aimtec.SDK.Prediction.Skillshots;
    using SDK.Unit_Extensions;
    using SDK.Usables;
    using OrbwalkerMode = SDK.Orbwalking.OrbwalkerMode;
    using Spell = Aimtec.SDK.Spell;

    class SpellConfig : ISpellConfig
    {
        private const string PassiveName = "blindmonkpassive_cosmetic";
        public float LastQ1CastAttempt { get; set; }

        public bool QAboutToEnd => Game.TickCount - this.LastQ1CastAttempt >= 3100 - Game.Ping / 2f;

        public bool IsQ2()
        {
            return !IsFirst(this.Q) && this.Q.Ready;
        }

        public bool IsFirst(Spell spell)
        {
            return Global.Player.SpellBook.GetSpell(spell.Slot).SpellData.Name.ToLower().Contains("one");
        }

        public bool HasQ2(Obj_AI_Base target)
        {
            return target.HasBuff("BlindMonkSonicWave");
        }

        public void QSmite(Obj_AI_Base target)
        {
            var pred = this.Q.GetPrediction(target);
            var objects = pred.CollisionObjects;

            if (pred.HitChance != HitChance.Collision || !objects.Any())
            {
                return;
            }

            if (SummonerSpells.Smite == null || !SummonerSpells.Smite.Ready || SummonerSpells.Ammo("Smite") < 2)
            {
                return;
            }

            var current = objects.FirstOrDefault();

            if (current == null || current.NetworkId == target.NetworkId || current.Health > SummonerSpells.SmiteMonsters() ||
                current.ServerPosition.Distance(Global.Player) > SummonerSpells.Smite.Range)
            {
                return;
            }

            SummonerSpells.Smite.CastOnUnit(current);
            Global.Player.SpellBook.CastSpell(SpellSlot.Q, target.ServerPosition);
        }

        public Spell W { get; private set; }
        public Spell Q { get; private set; }
        public Spell E { get; private set; }
        public Spell R { get; private set; }
        public Spell R2 { get; private set; }

        public OrbwalkerMode InsecMode { get; set; }
        public OrbwalkerMode WardjumpMode { get; set; }
        public OrbwalkerMode KickFlashMode { get; set; }

        public int WardRange { get; } = 600;

        public int PassiveStack()
        {
            return Global.Player.GetBuffCount(PassiveName);
        }

        public void Load()
        {
            this.Q = new Spell(SpellSlot.Q, 1000);
            this.Q.SetSkillshot(0.25f, 60, 1800, true, SkillshotType.Line, false, HitChance.None);

            this.W = new Spell(SpellSlot.W, 700);

            this.E = new Spell(SpellSlot.E, 425);

            this.R = new Spell(SpellSlot.R, 375);
            this.R2 = new Spell(SpellSlot.R, 900);
            this.R2.SetSkillshot(0.25f, 80, 1500, false, SkillshotType.Line);
        }

        public void OnProcessSpellCast(Obj_AI_Base sender, Obj_AI_BaseMissileClientDataEventArgs args)
        {
            if (sender == null || !sender.IsMe)
            {
                return;
            }

            if (args.SpellSlot == SpellSlot.Q && args.SpellData.Name.ToLower().Contains("one"))
            {
                this.LastQ1CastAttempt = Game.TickCount;
            }
        }
    }
}