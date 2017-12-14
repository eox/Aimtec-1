namespace Adept_AIO.Champions.LeeSin.Ward_Manager
{
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using SDK.Generic;
    using SDK.Unit_Extensions;
    using SDK.Usables;

    class WardManager : IWardManager
    {
        private readonly IWardTracker wardTracker;

        public WardManager(IWardTracker wardTracker)
        {
            this.wardTracker = wardTracker;
        }

        public float LastTimeCasted { get; private set; }

        public void WardJump(Vector3 position, int range)
        {
            if (Game.TickCount - wardTracker.LastWardCreated < 500)
            {
                return;
            }

            var ward = wardTracker.Ward();

            if (ward == null)
            {
                DebugConsole.WriteLine("There are no wards. Failed to continue.", MessageState.Warn);
                return;
            }

            position = Global.Player.ServerPosition.Extend(position, range);

            this.LastTimeCasted = Game.TickCount;
            wardTracker.LastWardCreated = Game.TickCount;
            wardTracker.WardPosition = position;
        
            Items.CastItem(ward, position);

            if (NavMesh.WorldToCell(position).Flags.HasFlag(NavCellFlags.Wall))
            {
                wardTracker.IsAtWall = true;
            }
            else
            {
                wardTracker.IsAtWall = false;
                Global.Player.SpellBook.CastSpell(SpellSlot.W, position);
            }
        }
    }
}