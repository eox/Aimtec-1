namespace Adept_AIO.BOT.SR.Events
{
    using System;
    using System.Drawing;
    using System.Linq;
    using Aimtec;
    using Data;
    using Menu;
    using SDK.Unit_Extensions;
    using Utilities;

    class RenderManager
    {
        public RenderManager()
        {
            Render.OnRender += OnRender;
        }

        private static void OnRender()
        {
            try
            {
                if (!MenuConfig.Mainmenu["Debug"].Enabled)
                {
                    return;
                }

                foreach (var jungleCamp in Camps.AllCamps)
                {
                    Render.Circle(jungleCamp.Position, 50, 100, jungleCamp.IsCampValid() ? Color.Green : Color.Gray);
                }

                Render.WorldToScreen(Global.Player.Position, out var playerV2);

                if(!Global.Player.IsRanged)
                Render.Circle(Global.Player.ServerPosition, BotData.MyRange, 100, Color.Gray);

                foreach (var spawnPoint in GameObjects.SpawnPoints.Where(x => x.IsOnScreen))
                {
                    Render.Circle(spawnPoint.Position, 100, 100, spawnPoint.Team == Global.Player.Team ? Color.Green : Color.Crimson);

                    Render.WorldToScreen(spawnPoint.Position, out var spawnVector2);
                    Render.Text($"Spawnspoint", spawnVector2, RenderTextFlags.Center, Color.White);
                }

                foreach (var activeTurret in GameObjects.Turrets.Where(x => x.IsOnScreen))
                {
                    Render.Circle(activeTurret.Position, 880, 200, activeTurret.Team == Global.Player.Team ? Color.Green : Color.Crimson);
                }

                var mode = MenuConfig.Mainmenu["Mode"].Enabled;
                Render.Text($"Adept AI: {(mode ? "ACTIVE" : "DISABLED" )}", new Vector2(playerV2.X - 20, playerV2.Y + 30), RenderTextFlags.Bottom, mode ? Color.White : Color.Gray);

                foreach (var path in Global.Player.Path.Skip(1))
                {
                    if (Render.WorldToScreen(path, out var pathV2))
                    {
                        Render.Line(playerV2, pathV2, Color.Crimson);
                    }
                }

                if(Global.Player.HasPath)
                Render.Circle(Global.Player.Path.LastOrDefault(), Global.Player.BoundingRadius, 100, Color.AliceBlue);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
