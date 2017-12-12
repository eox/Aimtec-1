namespace Adept_BaseUlt.Manager
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Threading;
    using Aimtec;
    using Aimtec.SDK.Damage;
    using Aimtec.SDK.Damage.JSON;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Menu;
    using Aimtec.SDK.Menu.Components;
    using Aimtec.SDK.Util;
    using Aimtec.SDK.Util.Cache;
    using Local_SDK;
    using Geometry = Local_SDK.Geometry;
    using Spell = Aimtec.SDK.Spell;

    class BaseUlt
    {
        private static Menu _menu;
        private readonly float _delay;
        private readonly int _maxCollisionObjects;
        private readonly float _range;

        private readonly float _speed;
        private readonly Spell _ultimate;
        private readonly float _width;

        private readonly List<Obj_AI_Hero> _lastEnemyChecked;

        private Vector3 _lastSeenPosition;

        private int _lastSeenTick;
        private Vector3 _predictedPosition;

        private int _recallStartTick;
        private float _recallTime;

        private Obj_AI_Hero _target;

        private int _timeUntilCastingUlt = -1;

        public BaseUlt(float speed, float width, float delay, int maxCollisionObjects = int.MaxValue, float range = float.MaxValue)
        {
            _ultimate = new Spell(SpellSlot.R, _range);

            _range = range;
            _speed = speed;
            _width = width;
            _delay = delay;
            _maxCollisionObjects = maxCollisionObjects;

            AttatchMenu();
            Global.Init();

            _lastEnemyChecked = new List<Obj_AI_Hero>();
            foreach (var enemies in GameObjects.EnemyHeroes)
            {
                _lastEnemyChecked.Add(enemies);
            }

            Teleport.OnTeleport += OnTeleport;
            Game.OnUpdate += OnUpdate;
            Render.OnRender += OnRender;
        }

        private static void AttatchMenu()
        {
            _menu = new Menu("hello", $"BaseUlt | {Global.Player.ChampionName}", true);
            _menu.Attach();

            _menu.Add(new MenuBool("RandomUlt", "Use RandomUlt").SetToolTip("Will GUESS the enemy position and ult there"));

            if (Global.Player.ChampionName == "Draven")
            {
                _menu.Add(new MenuBool("Draven", "Include R Back (Draven)"));
            }

            _menu.Add(new MenuBool("Collision", "Check Collision"));

            _menu.Add(new MenuSeperator("yes", "Whitelist"));

            foreach (var hero in GameObjects.EnemyHeroes)
            {
                _menu.Add(new MenuBool(hero.ChampionName, "ULT: " + hero.ChampionName));
            }

            _menu.Add(new MenuSeperator("no"));
            _menu.Add(new MenuSlider("Distance", "Max Distance | RandomUlt", 5000, 1000, 10000));
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
                    if (_target != null && _target.NetworkId == sender.NetworkId)
                    {
                        Reset();
                    }
                    break;
                case TeleportStatus.Start:

                    if (_target == null)
                    {
                        Set(args.Duration, Environment.TickCount, (Obj_AI_Hero)sender);
                    }

                    break;
            }
        }

        private void OnUpdate()
        {
            if (_menu["RandomUlt"].Enabled)
            {
                foreach (var enemy in _lastEnemyChecked.Where(x => x.IsValidTarget()))
                {
                    if (Environment.TickCount - _lastSeenTick <= Game.Ping / 2f)
                    {
                        return;
                    }

                    _lastSeenTick = Environment.TickCount;
                    _lastSeenPosition = enemy.ServerPosition;
                }
            }

            if (_target == null || !_menu[_target.ChampionName].Enabled || !_ultimate.Ready || PlayerDamage() < TargetHealth())
            {
                return;
            }

            if (_menu["RandomUlt"].Enabled)
            {
                var enemy = _lastEnemyChecked.FirstOrDefault(x => x.NetworkId == _target.NetworkId);
                if (enemy == null)
                {
                    return;
                }

                var direction = _target.ServerPosition + (_target.ServerPosition - enemy.ServerPosition).Normalized();

                var distance = (_recallStartTick - _lastSeenTick) / 1000f * _target.MoveSpeed;

                _predictedPosition = _lastSeenPosition.Extend(enemy.ServerPosition.Extend(direction, distance), distance);

                if (distance > _menu["Distance"].Value)
                {
                    return;
                }

                CastUlt(_predictedPosition);
            }

            _timeUntilCastingUlt = GetCastTime(GetFountainPos(_target));

            if (_timeUntilCastingUlt <= Game.Ping)
            {
                CastUlt(GetFountainPos(_target));
            }
        }

        private void CastUlt(Vector3 pos)
        {
            if (pos.IsZero)
            {
                DebugConsole.WriteLine("POSITION IS ZERO, CORE IS LIKELY BROKEN", MessageState.Warn);
                return;
            }

            var rectangle = new Geometry.Rectangle(Global.Player.ServerPosition.To2D(), pos.To2D(), _width);

            if (_menu["Collision"].Enabled &&
                GameObjects.EnemyHeroes.Count(x => x.NetworkId != _target.NetworkId && rectangle.IsInside(x.ServerPosition.To2D())) > _maxCollisionObjects ||
                pos.Distance(Global.Player) > _range ||
                pos.Distance(Global.Player) < 1200)
            {
                return;
            }

            DebugConsole.WriteLine($"Successfully Fired At: {_target.ChampionName} With The Distance: {Global.Player.Distance(pos)}", MessageState.Debug);

            _ultimate.Cast(pos);
            Reset();
        }

        private int GetCastTime(Vector3 pos)
        {
            return (int)(-(Environment.TickCount - (_recallStartTick + _recallTime)) - TravelTime(pos));
        }

        private void OnRender()
        {
            if (_lastSeenPosition != Vector3.Zero && _predictedPosition != Vector3.Zero)
            {
                Render.Circle(_lastSeenPosition, 50, 100, Color.White);
                Render.Circle(_predictedPosition, _width, 100, Color.Red);

                Render.WorldToScreen(_predictedPosition, out var castVector2);
                Render.Text("Random Ult", castVector2, RenderTextFlags.Center, Color.White);

                Render.WorldToScreen(_lastSeenPosition, out var lsV2);
                Render.WorldToScreen(_predictedPosition, out var ppV2);
                Render.Line(lsV2, ppV2, Color.Orange);
            }

            if (_timeUntilCastingUlt == -1 || _target == null)
            {
                return;
            }

            var barY = Render.Height * 0.8f;
            const int barHeight = 6;

            var barX = Render.Width * 0.425f;
            var barWidth = Render.Width - 2 * barX;

            const int i = 5;
            var scale = barWidth / 8000;

            Render.Rectangle(barX + scale * _timeUntilCastingUlt, barY + i + barHeight - 3, 1, 10, Color.Orange);
            Render.Rectangle(barX, barY, (int)(scale * _recallTime), barHeight, Color.Crimson);
            Render.Rectangle(barX + scale * _recallTime - 1, barY + i + barHeight - 3, 1, barHeight, Color.IndianRed);
        }

        private float TravelTime(Vector3 pos)
        {
            return Global.Player.Distance(pos) / _speed * 1000 + _delay + Game.Ping / 2f;
        }

        private float PlayerDamage()
        {
            switch (Global.Player.ChampionName)
            {
                case "Draven":
                    if (_menu["Draven"].Enabled)
                    {
                        return (float)(Global.Player.GetSpellDamage(_target, SpellSlot.R, DamageStage.SecondForm) + Global.Player.GetSpellDamage(_target, SpellSlot.R));
                    }
                    return (float)Global.Player.GetSpellDamage(_target, SpellSlot.R, DamageStage.SecondForm);
                case "Jinx": return (float)Global.Player.GetSpellDamage(_target, SpellSlot.R, DamageStage.Empowered);
            }
            return (float)Global.Player.GetSpellDamage(_target, SpellSlot.R);
        }

        private float TargetHealth()
        {
            if (_target == null)
            {
                return 0f;
            }

            var hpReg = _target.HPRegenRate;
            var invisible = _lastEnemyChecked.FirstOrDefault(x => x.NetworkId == _target.NetworkId);

            if (invisible == null)
            {
                return 0f;
            }

            var final = _target.Health + (hpReg * (invisible.LifetimeTicks / 10000f) + TravelTime(GetFountainPos(_target)) / 1000);

            DebugConsole.WriteLine($"Target Health: {(int)final} | Damage: {PlayerDamage()}", MessageState.Debug);

            return final;
        }

        private static Vector3 GetFountainPos(GameObject target)
        {
            switch (Game.Type)
            {
                case GameType.Normal:
                    switch (target.Team)
                    {
                        case GameObjectTeam.Order: return new Vector3(396, 185.1325f, 462);
                        case GameObjectTeam.Chaos: return new Vector3(14340, 171.9777f, 14390);
                    }
                    break;
            }
            return Vector3.Zero;
        }

        private void Set(float recall, int tickCount, Obj_AI_Hero target, int timeUntilCastingUlt = -1)
        {
            _recallTime = recall;
            _recallStartTick = tickCount;
            _target = target;
            _timeUntilCastingUlt = timeUntilCastingUlt;
        }

        private void Reset()
        {
            DebugConsole.WriteLine($"Reset Baseult", MessageState.Debug);

            _lastSeenPosition = Vector3.Zero;
            _predictedPosition = Vector3.Zero;

            _recallTime = 0;
            _recallStartTick = 0;
            _target = null;
            _timeUntilCastingUlt = -1;
        }
    }
}