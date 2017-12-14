namespace Adept_BaseUlt.Baseult_Reworked
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Prediction.Skillshots;
    using Aimtec.SDK.Util.Cache;
    using Local_SDK;
    using Geometry = Local_SDK.Geometry;
    using Spell = Aimtec.SDK.Spell;

    class Baseult
    {
        private readonly int maxCollisionObjects;
        private readonly Spell spell;
        private static Recall_Information recallInformation;

        private Dictionary<int, Vector3> positionsWithId;
        private Dictionary<int, int> lastSeenTickWithId;
      
        private int lastCheckTick;

        private static float TimeUntilCasting => Helper.GetCastTime(Helper.GetFountainPos(), recallInformation);

        private Vector3 recallPosition;

        public Baseult(float speed, float width, float delay, int maxCollisionObjects = int.MaxValue, float range = int.MaxValue)
        {
            this.maxCollisionObjects = maxCollisionObjects;

            spell = new Spell(SpellSlot.R, range);
            spell.SetSkillshot(delay / 100, width, speed, false, SkillshotType.Line);

            positionsWithId = new Dictionary<int, Vector3>();
            lastSeenTickWithId = new Dictionary<int, int>();
            
            Game.OnUpdate += OnRandomUlt;
            Game.OnUpdate += OnBaseUlt;
            Render.OnRender += OnRender;
            Teleport.OnTeleport += OnTeleport;
        }

        private void OnTeleport(Obj_AI_Base sender, Teleport.TeleportEventArgs args)
        {
            if (sender == null || !sender.IsEnemy || args.Type != TeleportType.Recall)
            {
                return;
            }

            switch (args.Status)
            {
                case TeleportStatus.Abort:
                case TeleportStatus.Finish:
                case TeleportStatus.Unknown:

                    if (recallInformation != null && recallInformation.NetworkID == sender.NetworkId)
                    {
                        recallInformation = null;
                    }

                    break;
                case TeleportStatus.Start:

                    if(recallInformation == null)
                    recallInformation = new Recall_Information(sender.NetworkId, args.Duration, sender, args.Start);

                    break;
            }
        }

        private void OnBaseUlt()
        {
            if (Environment.TickCount - lastCheckTick > Game.Ping / 2)
            {
                lastSeenTickWithId = new Dictionary<int, int>();
                positionsWithId = new Dictionary<int, Vector3>();

                foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy && x.IsVisible))
                {
                    lastSeenTickWithId.Add(hero.NetworkId, Game.TickCount);

                    var pos = hero.ServerPosition.Extend(hero.Path.FirstOrDefault(), 50);
                    positionsWithId.Add(hero.NetworkId, pos);
                }
                lastCheckTick = Environment.TickCount;
            }

            if (recallInformation == null || !Helper.IsValid(recallInformation) || !spell.Ready)
            {
                return;
            }

            Helper.Delay = spell.Delay * 100;
            Helper.Speed = spell.Speed;

            if (TimeUntilCasting <= Game.Ping)
            {
                spell.Cast(Helper.GetFountainPos());
            }
        }

        private void OnRandomUlt()
        {
            if (!MenuConfig.Menu["RandomUlt"].Enabled || !spell.Ready)
            {
                return;
            }

            if (recallInformation == null|| !Helper.IsValid(recallInformation))
            {
                return;
            }

            var lastSeenPosition = positionsWithId.FirstOrDefault(x => x.Key == recallInformation.Sender.NetworkId).Value;
            if (lastSeenPosition.IsZero)
            {
                DebugConsole.WriteLine($"LAST SEEN POS IS ZERO, WAIT FOR NEW UPDATE.", MessageState.Error);
                return;
            }

            var lastSeenTick = lastSeenTickWithId.FirstOrDefault(x => x.Key == recallInformation.NetworkID).Value;

            var dist = (recallInformation.Start - lastSeenTick) / 1000f * recallInformation.Sender.MoveSpeed;

            var recallPos = lastSeenPosition.Extend(recallInformation.Sender.ServerPosition, dist);
            this.recallPosition = recallPos;

            if (dist > MenuConfig.Menu["Distance"].Value)
            {
                return;
            }

            Cast(recallPos);
        }

        private void Cast(Vector3 position)
        {
            if (position.IsZero)
            {
                DebugConsole.WriteLine("POSITION IS ZERO, CORE IS LIKELY BROKEN", MessageState.Warn);
                return;
            }

            if (MenuConfig.Menu["Collision"].Enabled)
            {
                var rectangle = new Geometry.Rectangle(Global.Player.ServerPosition.To2D(), position.To2D(), spell.Width);

                if (GameObjects.EnemyHeroes.Count(x => x.NetworkId != recallInformation.NetworkID && rectangle.IsInside(x.ServerPosition.To2D())) >
                    maxCollisionObjects ||
                    position.Distance(Global.Player) > spell.Range ||
                    position.Distance(Global.Player) < 1200)
                {
                    return;
                }
            }

            DebugConsole.WriteLine("Successfully Fired", MessageState.Debug);
            spell.Cast(position);
        }

        private void OnRender()
        {
            if (recallInformation == null)
            {
                return;
            }

            if ( !recallPosition.IsZero)
            {
                Render.Circle(recallPosition, spell.Width, 100, Color.Red);

                Render.WorldToScreen(recallPosition, out var castVector2);
                Render.Text("Random Ult", castVector2, RenderTextFlags.Center, Color.White);

                Render.WorldToScreen(recallPosition, out var ppV2);
            }

            var barY = Render.Height * 0.8f;
            const int barHeight = 6;

            var barX = Render.Width * 0.425f;
            var barWidth = Render.Width - 2 * barX;

            const int i = 5;
            var scale = barWidth / 8000;

            var timeUntilCastingUlt = TimeUntilCasting;
            if (timeUntilCastingUlt < 0)
            {
                return;
            }

            Render.Rectangle(barX + scale * timeUntilCastingUlt, barY + i + barHeight - 3, 1, 10, Color.Orange);
            Render.Rectangle(barX, barY, (int)(scale * recallInformation.Duration), barHeight, Color.Crimson);
            Render.Rectangle(barX + scale * recallInformation.Duration - 1, barY + i + barHeight - 3, 1, barHeight, Color.IndianRed);
        }
    }
}
