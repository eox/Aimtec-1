namespace Adept_AIO.BOT.HowlingAbyss.Events
{
    using System;
    using System.Drawing;
    using System.Linq;
    using Aimtec;
    using Menu;
    using SDK.Unit_Extensions;
    using Utilities;

    class RenderManager
    {
   
        public RenderManager()
        {
            Render.OnRender += OnRender;
        }

        private void OnRender()
        {
            try
            {
                if (!MenuConfig.Mainmenu["Debug"].Enabled)
                {
                    return;
                }

                foreach (var shield in RelicShield.Shields)
                {
                    Render.Circle(shield, 100, 100, Color.Green);
                }

                Render.WorldToScreen(Global.Player.Position, out var playerV2);

                if (!Global.Player.IsRanged)
                    Render.Circle(Global.Player.Position, BotData.MyRange, 100, Color.Gray);

                foreach (var activeTurret in GameObjects.Turrets)
                {
                    Render.Circle(activeTurret.Position, 100, 100, activeTurret.Team == Global.Player.Team ? Color.Green : Color.Crimson);
                }

                var mode = MenuConfig.Mainmenu["Mode"].Enabled;
                Render.Text($"Adept AI: {(mode ? "ACTIVE" : "DISABLED")}", new Vector2(playerV2.X - 20, playerV2.Y + 30), RenderTextFlags.Bottom, mode ? Color.White : Color.Gray);

                foreach (var path in Global.Player.Path.Skip(1))
                {
                    if (Render.WorldToScreen(path, out var pathV2))
                    {
                        Render.Line(playerV2, pathV2, Color.Crimson);
                    }
                }

                if (Global.Player.HasPath)
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
