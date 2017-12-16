namespace Adept_AIO.SDK.Orbwalking
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Damage;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Menu;
    using Aimtec.SDK.Menu.Components;
    using Aimtec.SDK.Menu.Config;
    using Aimtec.SDK.Prediction.Health;
    using Aimtec.SDK.TargetSelector;
    using Aimtec.SDK.Util;
    using Aimtec.SDK.Util.Cache;

    internal class OrbwalkingImpl : AOrbwalker
    {
        #region Fields

        /// <summary>
        ///     The time the last attack command was sent (determined locally)
        /// </summary>
        protected float LastAttackCommandSentTime;

        #endregion

        #region Constructors and Destructors

        internal OrbwalkingImpl()
        {
            this.Initialize();
        }

        #endregion

        #region Public Properties

        public float AnimationTime => Player.AttackCastDelay * 1000;

        public float AttackCoolDownTime
        {
            get
            {
                var champion = Player.ChampionName;

                var attackDelay = Player.AttackDelay * 1000;

                if (champion.Equals("Graves"))
                {
                    attackDelay = (1.0740296828f * Player.AttackDelay - 0.7162381256175f) * 1000;
                }

                if (champion.Equals("Kalista") && !this.Config["Misc"]["KalistaFly"].Enabled)
                {
                    return attackDelay;
                }

                return attackDelay - this.AttackDelayReduction;
            }
        }

        public override bool IsWindingUp
        {
            get
            {
                var detectionTime = Math.Max(this.ServerAttackDetectionTick, this.LastAttackCommandSentTime);
                return Game.TickCount + Game.Ping / 2 - detectionTime <= this.WindUpTime;
            }
        }

        public override float WindUpTime => this.AnimationTime + this.ExtraWindUp;

        #endregion

        #region Properties

        protected bool AttackReady => Game.TickCount + Game.Ping / 2 - this.ServerAttackDetectionTick
                                      >= this.AttackCoolDownTime;

        private bool Attached { get; set; }

        private int AttackDelayReduction => this.Config["Advanced"]["AttackDelayReduction"].Value;

        private int ExtraWindUp => this.Config["Attacking"]["ExtraWindup"].Value;

        private int HoldPositionRadius => this.Config["Misc"]["HoldPositionRadius"].Value;

        private bool DrawAttackRange => this.Config["Drawings"]["DrawAttackRange"].Enabled;

        private bool DrawHoldPosition => this.Config["Drawings"]["DrawHoldRadius"].Enabled;

        private bool DrawKillable => this.Config["Drawings"]["DrawKillableMinion"].Enabled;


        /// <summary>
        ///     Special auto attack names that do not trigger OnProcessAutoAttack
        /// </summary>
        private readonly string[] specialAttacks =
        {
            "caitlynheadshotmissile",
            "goldcardpreattack",
            "redcardpreattack",
            "bluecardpreattack",
            "viktorqbuff",
            "quinnwenhanced",
            "renektonexecute",
            "renektonsuperexecute",
            "trundleq",
            "xenzhaothrust",
            "xenzhaothrust2",
            "xenzhaothrust3",
            "frostarrow",
            "garenqattack",
            "kennenmegaproc",
            "masteryidoublestrike",
            "mordekaiserqattack",
            "reksaiq",
            "warwickq",
            "vaynecondemnmissile",
            "masochismattack"
        };

        /// <summary>
        ///     Gets or sets the Forced Target
        /// </summary>
        private AttackableUnit ForcedTarget { get; set; }

        private AttackableUnit LastTarget { get; set; }

        //Members
        private float ServerAttackDetectionTick { get; set; }

        /// <summary>
        ///     Champions whos attack is not wasted on invulnerable targets or when blinded
        /// </summary>
        private readonly string[] noWasteAttackChamps = { "Kalista", "Twitch" };

        private Obj_AI_Hero GangPlank { get; set; }
        private Obj_AI_Hero Jax { get; set; }

        #endregion

        #region Public Methods and Operators

        public override void Attach(IMenu menu)
        {
            if (!this.Attached)
            {
                this.Attached = true;
                menu.Add(this.Config);
                Obj_AI_Base.OnProcessAutoAttack += this.ObjAiHeroOnProcessAutoAttack;
                Obj_AI_Base.OnProcessSpellCast += this.Obj_AI_Base_OnProcessSpellCast;
                Game.OnUpdate += this.Game_OnUpdate;
                SpellBook.OnStopCast += this.SpellBook_OnStopCast;
                Render.OnRender += this.RenderManager_OnRender;
            }
            else
            {
                this.Logger.Info("This Orbwalker instance is already attached to a Menu.");
            }
        }

        public override bool Attack(AttackableUnit target)
        {
            var preAttackargs = this.FirePreAttack(target);
            if (!preAttackargs.Cancel)
            {
                var targetToAttack = preAttackargs.Target;
                if (this.ForcedTarget != null &&
                    this.ForcedTarget.IsValidAutoRange())
                {
                    targetToAttack = this.ForcedTarget;
                }

                if (Player.IssueOrder(OrderType.AttackUnit, targetToAttack))
                {
                    this.LastAttackCommandSentTime = Game.TickCount;
                    return true;
                }
            }

            return false;
        }

        public bool IsValidAttackableObject(AttackableUnit unit)
        {
            //Valid check
            if (!unit.IsValidAutoRange())
            {
                return false;
            }

            if (unit is Obj_AI_Hero || unit is Obj_AI_Turret || unit.Type == GameObjectType.obj_BarracksDampener || unit.Type == GameObjectType.obj_HQ)
            {
                return true;
            }

            //J4 flag
            if (unit.Name.Contains("Beacon"))
            {
                return false;
            }

            var mBase = unit as Obj_AI_Base;

            if (mBase == null || !mBase.IsFloatingHealthBarActive)
            {
                return false;
            }
        

            var minion = unit as Obj_AI_Minion;

            if (minion == null)
            {
                return false;
            }


            var name = minion.UnitSkinName.ToLower();
            
            if (name.Contains("zyraseed")) 
            {
                return false;
            }
            
            if (!this.Config["Farming"]["AttackPlants"].Enabled && name.Contains("sru_plant_"))
            {
                return false;
            }

            if (!this.Config["Farming"]["AttackWards"].Enabled && name.Contains("ward"))
            {
                return false;
            }
            
            if (this.GangPlank != null)
            {
                if (name.Contains("gangplankbarrel"))
                {
                    if (!this.Config["Farming"]["AttackBarrels"].Enabled)
                    {
                        return false;
                    }

                    //dont attack ally barrels
                    if (this.GangPlank.IsAlly)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override bool CanAttack()
        {
            return this.CanAttack(this.GetActiveMode());
        }

        public bool CanAttack(OrbwalkerMode mode)
        {
            if (mode == null)
            {
                return false;
            }

            if (!this.AttackingEnabled || !mode.AttackingEnabled)
            {
                return false;
            }

            if (Player.HasBuffOfType(BuffType.Polymorph))
            {
                return false;
            }

            if (Player.HasBuffOfType(BuffType.Blind) &&
                !this.noWasteAttackChamps.Contains(Player.ChampionName))
            {
                return false;
            }

            if (Player.ChampionName.Equals("Jhin") && Player.HasBuff("JhinPassiveReload"))
            {
                return false;
            }

            if (Player.ChampionName.Equals("Graves") && !Player.HasBuff("GravesBasicAttackAmmo1"))
            {
                return false;
            }

            if (this.NoCancelChamps.Contains(Player.ChampionName))
            {
                if (Player.ChampionName.Equals("Kalista") && this.Config["Misc"]["KalistaFly"].Enabled)
                {
                    return true;
                }
            }

            if (this.IsWindingUp)
            {
                return false;
            }

            return this.AttackReady;
        }

        public override bool CanMove()
        {
            return this.CanMove(this.GetActiveMode());
        }

        public bool CanMove(OrbwalkerMode mode)
        {
            if (mode == null)
            {
                return false;
            }

            if (!this.MovingEnabled || !mode.MovingEnabled)
            {
                return false;
            }

            if (Player.Distance(Game.CursorPos) < this.HoldPositionRadius)
            {
                return false;
            }

            if (this.NoCancelChamps.Contains(Player.ChampionName))
            {
                return true;
            }

            if (this.IsWindingUp)
            {
                return false;
            }

            return true;
        }

        public override void Dispose()
        {
            this.Config.Dispose();
            Obj_AI_Base.OnProcessAutoAttack -= this.ObjAiHeroOnProcessAutoAttack;
            Obj_AI_Base.OnProcessSpellCast -= this.Obj_AI_Base_OnProcessSpellCast;
            Game.OnUpdate -= this.Game_OnUpdate;
            SpellBook.OnStopCast -= this.SpellBook_OnStopCast;
            Render.OnRender -= this.RenderManager_OnRender;
            this.Attached = false;
        }

        public override void ForceTarget(AttackableUnit unit)
        {
            this.ForcedTarget = unit;
        }

        public override AttackableUnit GetOrbwalkingTarget()
        {
            return this.LastTarget;
        }

        public override AttackableUnit FindTarget(OrbwalkerMode mode)
        {
            if (this.ForcedTarget != null &&
                this.ForcedTarget.IsValidAutoRange())
            {
                return this.ForcedTarget;
            }

            return mode?.GetTarget();
        }

        public override bool Move(Vector3 movePosition)
        {
            var preMoveArgs = this.FirePreMove(movePosition);
            if (!preMoveArgs.Cancel)
            {
                if (Player.IssueOrder(OrderType.MoveTo, preMoveArgs.MovePosition))
                {
                    return true;
                }
            }

            return false;
        }

        public override void Orbwalk()
        {
            var mode = this.GetActiveMode();
            if (mode == null)
            {
                return;
            }

            if (this.ForcedTarget != null &&
                !this.ForcedTarget.IsValidAutoRange())
            {
                this.ForcedTarget = null;
            }

#pragma warning disable 1587
            /// <summary>
            ///     Execute the specific logic for this mode if any
            /// </summary>
#pragma warning restore 1587
            mode.Execute();

            if (!mode.BaseOrbwalkingEnabled)
            {
                return;
            }

            if (this.CanAttack(mode))
            {
                var target = this.LastTarget = this.FindTarget(mode);
                if (target != null)
                {
                    this.Attack(target);
                }
            }

            if (this.CanMove(mode))
            {
                this.Move(Game.CursorPos);
            }
        }

        public override void ResetAutoAttackTimer()
        {
            this.ServerAttackDetectionTick = 0;
            this.LastAttackCommandSentTime = 0;
        }

        #endregion

        #region Methods

        protected void Game_OnUpdate()
        {
            if (Player.IsDead)
            {
                return;
            }

            this.Orbwalk();
        }

        protected void ObjAiHeroOnProcessAutoAttack(Obj_AI_Base sender, Obj_AI_BaseMissileClientDataEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (args.Target is AttackableUnit targ)
            {
                this.ServerAttackDetectionTick = Game.TickCount - Game.Ping / 2;
                this.LastTarget = targ;
                this.ForcedTarget = null;
                DelayAction.Queue((int)this.WindUpTime, () => this.FirePostAttack(targ));
            }
        }

        private bool CanKillMinion(Obj_AI_Base minion, int time = 0)
        {
            var rtime = time == 0 ? this.TimeForAutoToReachTarget(minion) : time;
            var pred = this.GetPredictedHealth(minion, rtime);
            if (pred > 0)
            {
                return pred <= this.GetRealAutoAttackDamage(minion);
            }

            this.FireNonKillableMinion(minion);
            return false;
        }

        private AttackableUnit GetHeroTarget()
        {
            var targets = TargetSelector.Implementation.GetOrderedTargets(0, true);
            return targets.FirstOrDefault(target => this.Jax == null || target.NetworkId != this.Jax.NetworkId || !target.HasBuff("JaxCounterStrike"));
        }

        private AttackableUnit GetLaneClearTarget()
        {
            if (UnderTurretMode())
            {
                return this.GetUnderTurret();
            }

            var attackable = ObjectManager.Get<AttackableUnit>().Where(this.IsValidAttackableObject);
            var attackableUnits = attackable as AttackableUnit[] ?? attackable.ToArray();
            IEnumerable<Obj_AI_Base> minions = attackableUnits.Where(x => x is Obj_AI_Base).Cast<Obj_AI_Base>().OrderByDescending(x => x.MaxHealth);

            var actualKillable = GetLastHitTarget(minions.ToArray());
            if (actualKillable != null)
            {
                return actualKillable;
            }

            //Killable
            var enumerable = minions.ToArray();
            //AttackableUnit killableMinion = enumerable.FirstOrDefault(x => this.CanKillMinion(x));
            //if (killableMinion != null)
            //{
            //    return killableMinion;
            //}

            var waitableMinion = enumerable.Any(this.ShouldWaitMinion);
            if (waitableMinion)
            {
                Player.IssueOrder(OrderType.MoveTo, Game.CursorPos);
                return null;
            }

            var structure = GetStructureTarget(attackableUnits);
            if (structure != null)
            {
                return structure;
            }
            
            if (this.LastTarget != null &&
                this.LastTarget.IsValidAutoRange())
            {
                if (this.LastTarget is Obj_AI_Base b)
                {
                    var predHealth = this.GetPredictedHealth(b);

                    //taking damage
                    if (Math.Abs(this.LastTarget.Health - predHealth) < 0)
                    {
                        return this.LastTarget;
                    }
                }
            }
            
            foreach (var minion in enumerable)
            {
                var predHealth = this.GetPredictedHealth(minion);

                //taking damage
                if (minion.Health * 1.15f - predHealth > 0)
                {
                    continue;
                }
                
                return minion;
            }

            var first = enumerable.MaxBy(x => x.Health);
            if (first != null)
            {
                return first;
            }

            //Heros
            var hero = this.GetHeroTarget();
            if (hero != null)
            {
                return hero;
            }

            return null;
        }

        public static bool UnderTurretMode()
        {
            var nearestTurret = TurretAttackManager.GetNearestTurretData(Player, TurretAttackManager.TurretTeam.Ally);
            return nearestTurret != null && nearestTurret.Turret.IsValid && nearestTurret.Turret.Distance(Player) + Player.AttackRange * 1.1 <= 950;
        }

        public AttackableUnit GetUnderTurret()
        {
            var attackable = ObjectManager.Get<AttackableUnit>().Where(this.IsValidAttackableObject);

            var nearestTurret = TurretAttackManager.GetNearestTurretData(Player, TurretAttackManager.TurretTeam.Ally);

            if (nearestTurret == null)
            {
                return null;
            }

            var attackableUnits = attackable as AttackableUnit[] ?? attackable.ToArray();
            var underTurret = attackableUnits.Where(x => x.ServerPosition.Distance(nearestTurret.Turret.ServerPosition) < 900 && x.IsValidAutoRange());

            if (underTurret.Any())
            {
                var tData = TurretAttackManager.GetTurretData(nearestTurret.Turret.NetworkId);
                if (tData == null || !tData.TurretActive)
                {
                    return null;
                }

                var tTarget = tData.LastTarget;
                if (!tTarget.IsValidAutoRange())
                {
                    return null;
                }

                var attacks = tData.Attacks.Where(x => !x.Inactive);

                foreach (var attack in attacks)
                {
                    //turret related
                    var arrival = attack.PredictedLandTime;
                    var eta = arrival - Game.TickCount;
                    var tDmg = tData.Turret.GetAutoAttackDamage(tTarget);

                    //var tWillKill = tDmg > tTarget.Health;
                    var numTurretAutosToKill = (int)Math.Ceiling(tTarget.Health / tDmg);
                    var turretDistance = tData.Turret.Distance(tTarget) - Player.BoundingRadius - tTarget.BoundingRadius;
                    var tCastDelay = tData.Turret.AttackCastDelay * 1000;
                    var tTravTime = turretDistance / tData.Turret.BasicAttack.MissileSpeed * 1000;
                    var tTotalTime = tCastDelay + tTravTime + Game.Ping / 2f;

                    //myattack related
                    var castDelay = Player.AttackCastDelay * 1000;
                    //var minDelay = castDelay;
                    var dist = Player.Distance(tTarget) - Player.BoundingRadius - tTarget.BoundingRadius;
                    var travTime = dist / Player.BasicAttack.MissileSpeed * 1000;
                    var totalTime = (int)(castDelay + travTime + Game.Ping / 2f);

                    //minion hpred
                    var tMinionDmgPredHealth = HealthPrediction.Instance.GetPrediction(tTarget, totalTime);

                    //myattack
                    const int ExtraBuffer = 50;
                    //if total time > turret attack arrival time by buffer (can be early/late)
                    var canReachSooner = totalTime - eta > ExtraBuffer;

                    var myAutoDmg = this.GetRealAutoAttackDamage(tTarget);

                    //if my attk reach sooner than turret & my auto can kill it
                    if (canReachSooner && myAutoDmg >= tMinionDmgPredHealth)
                    {
                        return tTarget;
                    }

                    var remHealth = tMinionDmgPredHealth - tDmg;

                    //Minion wont die
                    if (remHealth > 0)
                    {
                        if (remHealth <= myAutoDmg)
                        {
                            return null;
                        }

                        if (totalTime - tTotalTime < 50)
                        {
                            return null;
                        }

                        for (var i = 1; i <= numTurretAutosToKill; i++)
                        {
                            var dmg = i * tDmg;
                            var health = tTarget.Health - dmg;
                            if (health > 0 && health < myAutoDmg)
                            {
                                break;
                            }

                            if (i == numTurretAutosToKill)
                            {
                                return tTarget;
                            }
                        }
                    }

                    //Turret will kill min and nothing i can do about it
                    else
                    {
                        foreach (var min in attackableUnits)
                        {
                            if (min.NetworkId == tTarget.NetworkId)
                            {
                                continue;
                            }

                            var minBase = min as Obj_AI_Base;
                            if (minBase == null)
                            {
                                continue;
                            }

                            //myattack related
                            var castDelay1 = Player.AttackCastDelay * 1000;
                            var dist1 = Player.Distance(min) - Player.BoundingRadius - min.BoundingRadius;
                            var travTime1 = dist1 / Player.BasicAttack.MissileSpeed * 1000;
                            var totalTime1 = (int)(castDelay1 + travTime1 + Game.Ping / 2f);

                            var dmg1 = this.GetRealAutoAttackDamage(minBase);
                            var pred1 = HealthPrediction.Instance.GetPrediction(minBase, totalTime1);
                            if (dmg1 > pred1)
                            {
                                return min;
                            }
                        }
                    }
                }
            }

            return null;
        }

        private AttackableUnit GetLastHitTarget()
        {
            return this.GetLastHitTarget(null);
        }

        private AttackableUnit GetLastHitTarget(IEnumerable<AttackableUnit> attackable)
        {
            
            if (attackable == null)
            {
                attackable = ObjectManager.Get<AttackableUnit>().Where(this.IsValidAttackableObject);
            }

            var availableMinionTargets = attackable
                .OfType<Obj_AI_Base>().Where(x => this.CanKillMinion(x));

            var bestMinionTarget = availableMinionTargets
                .OrderByDescending(x => x.MaxHealth)
                .ThenBy(x => x.Health)
                .FirstOrDefault();

            return bestMinionTarget;
        }

        //In mixed mode we prioritize killable units, then structures, then heros. If none are found, then we don't attack anything.
        private AttackableUnit GetMixedModeTarget()
        {
            var attackable = ObjectManager.Get<AttackableUnit>().Where(this.IsValidAttackableObject);

            var attackableUnits = attackable as AttackableUnit[] ?? attackable.ToArray();

            var killable = this.GetLastHitTarget(attackableUnits);

            //Killable unit 
            if (killable != null)
            {
                return killable;
            }

            //Structures
            var structure = GetStructureTarget(attackableUnits);
            if (structure != null)
            {
                return structure;
            }

            //Heros
            var hero = this.GetHeroTarget();
            return hero;
        }

        private int GetPredictedHealth(Obj_AI_Base minion, int time = 0)
        {
            var rtime = time == 0 ? this.TimeForAutoToReachTarget(minion) : time;
            return (int)Math.Ceiling(HealthPrediction.Instance.GetPrediction(minion, rtime));
        }

        //Gets a structure target based on the following order (Nexus, Turret, Inihibitor)
        private static AttackableUnit GetStructureTarget(IEnumerable<AttackableUnit> attackable)
        {
            //Nexus
            var attackableUnits = attackable as AttackableUnit[] ?? attackable.ToArray();
            var nexus = attackableUnits.Where(x => x.Type == GameObjectType.obj_HQ).MinBy(x => x.Distance(Player));
            if (nexus != null && nexus.IsValidAutoRange())
            {
                return nexus;
            }

            //Turret
            var turret = attackableUnits.Where(x => x is Obj_AI_Turret).MinBy(x => x.Distance(Player));
            if (turret != null && turret.IsValidAutoRange())
            {
                return turret;
            }

            //Inhib
            var inhib = attackableUnits.Where(x => x.Type == GameObjectType.obj_BarracksDampener)
                                       .MinBy(x => x.Distance(Player));
            if (inhib != null && inhib.IsValidAutoRange())
            {
                return inhib;
            }

            return null;
        }

        private void Initialize()
        {
            var advanced = new Menu("Advanced", "Advanced")
                               {
                                   new MenuSlider("AttackDelayReduction", "Attack Delay Reduction", 90, 0, 180, true)
                               };

            var attacking = new Menu("Attacking", "Attacking")
                                { new MenuSlider("ExtraWindup", "Additional Windup", Game.Ping / 2, 0, 200, true) };

            var farming = new Menu("Farming", "Farming")
                              {
                                  new MenuBool("AttackPlants", "Attack Plants", false, true),
                                  new MenuBool("AttackWards", "Attack Wards", true, true),
                                  new MenuBool("AttackBarrels", "Attack Barrels", true, true)
                              };

            var misc = new Menu("Misc", "Misc")
                           {
                               new MenuSlider("HoldPositionRadius", "Hold Radius", 50, 0, 400, true),
                               Player.ChampionName.Equals("Kalista") ? new MenuBool("KalistaFly", "Kalista Fly", true, true) : null
                           };

            var drawings = new Menu("Drawings", "Drawings")
                               {
                                   new MenuBool("DrawAttackRange", "Draw Attack Range"),
                                   new MenuBool("DrawHoldRadius", "Draw Hold Radius", false),
                                   new MenuBool("DrawKillableMinion", "Indicate Killable", false)
                               };

            this.Config.Add(advanced);
            this.Config.Add(attacking);
            this.Config.Add(farming);
            this.Config.Add(misc);
            this.Config.Add(drawings);


            this.AddMode(this.Combo = new OrbwalkerMode("Combo", GlobalKeys.ComboKey, this.GetHeroTarget, null));
            this.AddMode(this.LaneClear = new OrbwalkerMode("Laneclear", GlobalKeys.WaveClearKey, this.GetLaneClearTarget, null));
            this.AddMode(this.LastHit = new OrbwalkerMode("Lasthit", GlobalKeys.LastHitKey, this.GetLastHitTarget, null));
            this.AddMode(this.Mixed = new OrbwalkerMode("Mixed", GlobalKeys.MixedKey, this.GetMixedModeTarget, null));

            this.CheckSpecialHeroes();
        }

        private void CheckSpecialHeroes()
        {
            foreach (var hero in GameObjects.Heroes)
            {
                var name = hero.ChampionName.ToLower();
                switch (name)
                {
                    case "gangplank":
                        this.GangPlank = hero;
                        break;

                    case "jax":
                        this.Jax = hero;
                        break;
                }
            }
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, Obj_AI_BaseMissileClientDataEventArgs e)
        {
            if (!sender.IsMe)
            {
                return;
            }

            var name = e.SpellData.Name.ToLower();
            if (this.specialAttacks.Any(x => name.StartsWith(x)))
            {
                this.ObjAiHeroOnProcessAutoAttack(sender, e);
            }

            if (this.IsReset(name))
            {
                this.ResetAutoAttackTimer();
            }
        }

        private void RenderManager_OnRender()
        {
            if (this.DrawAttackRange)
            {
                Render.Circle(Player.Position, Player.AttackRange + Player.BoundingRadius, 30, Color.DeepSkyBlue);
            }

            if (this.DrawHoldPosition)
            {
                Render.Circle(Player.Position, this.HoldPositionRadius, 30, Color.White);
            }

            if (!this.DrawKillable)
            {
                return;
            }

            foreach (var m in ObjectManager.Get<Obj_AI_Minion>().Where(x => this.IsValidAttackableObject(x) && x.Health <= this.GetRealAutoAttackDamage(x)))
            {
                m.SetGlow(Color.LimeGreen, 4);
            }

            foreach (var m in ObjectManager.Get<Obj_AI_Minion>().OrderBy(x => x.Distance(Player)).Where(x => x.HasGlow() && x.Health <= 0))
            {
                m.RemoveGlow();
            }
        }

        private double GetRealAutoAttackDamage(Obj_AI_Base minion)
        {
            return minion.IsWard() ? 1 : Player.GetAutoAttackDamage(minion) * 0.97f; 
        }

        private bool ShouldWaitMinion(Obj_AI_Base minion)
        {
            var time = this.TimeForAutoToReachTarget(minion) + (int)Player.AttackDelay * 1000 * 1.2f;
            var pred = HealthPrediction.Instance.GetLaneClearHealthPrediction(minion, (int)time);

            return pred < this.GetRealAutoAttackDamage(minion);
        }

        private void SpellBook_OnStopCast(Obj_AI_Base sender, SpellBookStopCastEventArgs e)
        {
            if (sender.IsMe && (e.DestroyMissile || e.ForceStop)) // || e.StopAnimation
            {
                this.ResetAutoAttackTimer();
            }
        }

        private static float GetBasicAttackMissileSpeed(Obj_AI_Hero hero)
        {
            if (hero.IsMelee)
            {
                return float.MaxValue;
            }

            switch (hero.ChampionName)
            {
                case "Azir":
                case "Velkoz":
                case "Thresh":
                case "Rakan":
                    return float.MaxValue;

                case "Kayle":
                    if (hero.HasBuff("JudicatorRighteousFury"))
                    {
                        return float.MaxValue;
                    }
                    break;

                case "Viktor":
                    if (hero.HasBuff("ViktorPowerTransferReturn"))
                    {
                        return float.MaxValue;
                    }
                    break;
            }

            return hero.BasicAttack.MissileSpeed;
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private int TimeForAutoToReachTarget(AttackableUnit minion)
        {
            var dist = Player.ServerPosition.Distance(minion.ServerPosition);
            var attackTravelTime = dist / (int)GetBasicAttackMissileSpeed(ObjectManager.GetLocalPlayer()) * 1000f * 1.2f;
            var totalTime = (int)(this.AnimationTime + attackTravelTime + Game.Ping / 2f);
            return totalTime;
        }

        #endregion
    }
}
