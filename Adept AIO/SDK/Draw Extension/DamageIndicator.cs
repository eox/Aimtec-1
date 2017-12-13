namespace Adept_AIO.SDK.Draw_Extension
{
    using System.Drawing;
    using Aimtec;

    class DamageIndicator
    {
        internal static int Height => 9;
        internal static int Width => 104;

        public Obj_AI_Base Unit { get; set; }

        private Vector2 Offset
        {
            get
            {
                if (this.Unit != null)
                {
                    return this.Unit.IsAlly ? new Vector2(34, 9) : new Vector2(30, 7);
                }
                return new Vector2();
            }
        }

        public Vector2 StartPosition()
        {
            return new Vector2(this.Unit.FloatingHealthBarPosition.X + this.Offset.X, this.Unit.FloatingHealthBarPosition.Y + this.Offset.Y);
        }

        private Vector2 EndPosition(float dmg)
        {
            var w = GetHpProc(dmg) * Width;
            return new Vector2(StartPosition().X + w, StartPosition().Y);
        }

        private float GetHpProc(float dmg)
        {
            return (this.Unit.Health - dmg > 0 ? this.Unit.Health - dmg : 0) / this.Unit.MaxHealth;
        }

        public void DrawDmg(float dmg, Color color)
        {
            var from = EndPosition(0);
            var to = EndPosition(dmg);

            if (from.IsZero || to.IsZero || this.Unit == null || from.X <= 200)
            {
                return;
            }

            Render.Line(new Vector2(from.X, from.Y - 5), new Vector2(to.X, to.Y - 5), Height, false, color);
        }
    }
}