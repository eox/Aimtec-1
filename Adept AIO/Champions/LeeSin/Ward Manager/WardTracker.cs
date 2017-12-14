namespace Adept_AIO.Champions.LeeSin.Ward_Manager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aimtec;
    using Core.Spells;
    using SDK.Generic;
    using SDK.Unit_Extensions;
    using SDK.Usables;

    public class WardTracker : IWardTracker
    {
        private readonly ISpellConfig spellConfig;

        private readonly IEnumerable<string> wardNames = new List<string>
        {
            "TrinketTotemLvl1",
            "ItemGhostWard",
            "JammerDevice"
        };

        public WardTracker(ISpellConfig spellConfig)
        {
            this.spellConfig = spellConfig;
        }

        public bool DidJustWard => Game.TickCount - this.LastWardCreated <= 800 + Game.Ping / 2f;

        public bool IsWardReady()
        {
            return wardNames.Any(Items.CanUseItem);
        }

        public string Ward()
        {
            return wardNames.FirstOrDefault(Items.CanUseItem);
        }

        public bool IsAtWall { get; set; }

        public float LastWardCreated { get; set; }

        public string WardName { get; private set; }

        public Vector3 WardPosition { get; set; }

        public void OnCreate(GameObject sender)
        {
            if (this.DidJustWard || !spellConfig.IsFirst(spellConfig.W) || !this.IsAtWall)
            {
                return;
            }

            var ward = sender as Obj_AI_Minion;

            if (ward == null || !ward.Name.ToLower().Contains("ward"))
            {
                return;
            }

            this.LastWardCreated = Game.TickCount;
            this.WardName = ward.Name;
            this.WardPosition = ward.ServerPosition;

            DebugConsole.WriteLine("Located Ally Ward.", MessageState.Debug);
            Global.Player.SpellBook.CastSpell(SpellSlot.W, this.WardPosition); // Bug: This position is unrealistic and does not work.
        }
    }
}