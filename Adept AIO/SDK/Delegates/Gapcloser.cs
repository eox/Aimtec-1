﻿using System;
using System.Collections.Generic;
using System.Linq;
using Adept_AIO.SDK.Extensions;
using Aimtec;
using Aimtec.SDK.Extensions;
using Aimtec.SDK.Menu;
using Aimtec.SDK.Menu.Components;

namespace Adept_AIO.SDK.Delegates
{
    #region

    #endregion

    public delegate void OnGapcloserEvent(Obj_AI_Hero sender, GapcloserArgs args);

    public enum SpellType
    {
        Melee     = 0,
        Dash      = 1,
        SkillShot = 2,
        Targeted  = 3
    }

    internal struct SpellData
    {
        public string ChampionName { get; set; }
        public string SpellName    { get; set; }
        public SpellSlot Slot      { get; set; }
        public SpellType SpellType { get; set; }
    }

    public class GapcloserArgs
    {
        internal Obj_AI_Hero Unit    { get; set; }
        public SpellSlot Slot        { get; set; }
        public string SpellName      { get; set; }
        public SpellType Type        { get; set; }
        public Vector3 StartPosition { get; set; }
        public Vector3 EndPosition   { get; set; }
        public int StartTick         { get; set; }
        public int EndTick           { get; set; }
        public int DurationTick      { get; set; }
        public bool HaveShield       { get; set; }
    }

    public static class Gapcloser
    {
        public static event OnGapcloserEvent OnGapcloser;

        public static Dictionary<int, GapcloserArgs> Gapclosers = new Dictionary<int, GapcloserArgs>();
        internal static List<SpellData> Spells = new List<SpellData>();

        public static Menu Menu;

        static Gapcloser()
        {
            Initialize();
        }

        public static void Attach(Menu mainMenu, string MenuName)
        {
            if (ObjectManager.Get<Obj_AI_Hero>().All(x => !x.IsEnemy))
            {
                return;
            }

            Menu = new Menu("", MenuName)
            {
                new MenuBool("Enabled", "Enabled"),
            };

            mainMenu.Add(Menu);

            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy))
            {
                var heroMenu = new Menu(enemy.ChampionName, enemy.ChampionName)
                {
                    new MenuBool  (enemy.ChampionName + ".Enabled", "Enabled"),
                    new MenuSlider(enemy.ChampionName + ".Distance", "If Target Distance To Player <", 550, 1, 700),
                    new MenuSlider(enemy.ChampionName + ".HPercent", "When Player (HP %) < ", 100, 1)
                };

                Menu.Add(heroMenu);

                if (enemy.IsMelee)
                {
                    heroMenu.Add(new MenuSliderBool(enemy.ChampionName + ".Melee", "Anti Melee Attack | Player (HP %) <", true, 40, 1, 99));
                }

                foreach (var spell in Spells.Where(x => x.ChampionName == enemy.ChampionName))
                {
                    heroMenu.Add(new MenuBool(spell.SpellName, "Anti Slot: " + spell.Slot + "(" + spell.SpellName + ")"));
                }
            }

            Game.OnUpdate += OnUpdate;
         
            Obj_AI_Base.OnProcessAutoAttack += OnProcessAutoAttack;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Obj_AI_Base.OnNewPath += OnNewPath;
        }

        private static void OnProcessAutoAttack(Obj_AI_Base sender, Obj_AI_BaseMissileClientDataEventArgs args)
        {
            if (sender == null || sender.Type != GameObjectType.obj_AI_Hero || !sender.IsEnemy || !sender.IsMelee)
            {
                return;
            }

            if (string.IsNullOrEmpty(args.SpellData.Name) 
            ||  args.Target == null 
            || !args.Target.IsMe 
            || !Menu[sender.UnitSkinName][sender.UnitSkinName + ".Melee"].Enabled)
            {
                return;
            }

            if (!Gapclosers.ContainsKey(sender.NetworkId))
            {
                Gapclosers.Add(sender.NetworkId, new GapcloserArgs());
            }

            var gapclosers = Gapclosers[sender.NetworkId];

            gapclosers.Unit = (Obj_AI_Hero) sender;
            gapclosers.Slot = SpellSlot.Unknown;
            gapclosers.Type = SpellType.Melee;

            gapclosers.SpellName = args.SpellData.Name;

            gapclosers.StartPosition = args.Start;
            gapclosers.EndPosition   = args.End;
            gapclosers.StartTick     = Game.TickCount;
        }

        private static void OnNewPath(Obj_AI_Base sender, Obj_AI_BaseNewPathEventArgs args)
        {
            if (sender == null || sender.Type != GameObjectType.obj_AI_Hero || !sender.IsEnemy || Gapclosers == null)
            {
                return;
            }

            if (sender.UnitSkinName == "Vi"    // Vi R
             || sender.UnitSkinName == "Sion"  // Sion R
             || sender.UnitSkinName == "Kayn"  // Kayn R
             || sender.UnitSkinName == "Fizz") // Fizz E
            {
                return;
            }

            if (!Gapclosers.ContainsKey(sender.NetworkId))
            {
                Gapclosers.Add(sender.NetworkId, new GapcloserArgs());
            }

            if (!args.IsDash)
            {
                return;
            }

            var gapclosers = Gapclosers[sender.NetworkId];
           
            gapclosers.Unit = (Obj_AI_Hero) sender;
            gapclosers.Slot = SpellSlot.Unknown;
            gapclosers.Type = SpellType.Dash;
            gapclosers.SpellName = sender.UnitSkinName + "_Dash";
            gapclosers.StartPosition = sender.ServerPosition;
            gapclosers.EndPosition   = args.Path.Last();
            gapclosers.StartTick     = Game.TickCount;
            gapclosers.EndTick       = (int) (gapclosers.EndPosition.DistanceSqr(gapclosers.StartPosition) / args.Speed * args.Speed * 1000) + gapclosers.StartTick;
            gapclosers.DurationTick  = gapclosers.EndTick - gapclosers.StartTick;
            gapclosers.HaveShield    = HaveShiledBuff(sender);
        }

        private static void OnUpdate()
        {
            if (Gapclosers.Values.Any(x => Game.TickCount - x.StartTick > 700 + Game.Ping))
            {
                Gapclosers.Clear();
            }

            if (OnGapcloser == null || !Menu["Enabled"].Enabled)
            {
                return;
            }

            foreach (var args in Gapclosers.Where(x => x.Value.Unit.IsValidTarget() && !x.Value.Unit.IsMe && Menu[x.Value.Unit.ChampionName][x.Value.Unit.ChampionName + ".Enabled"].Enabled))
            {
                switch (args.Value.Type)
                {
                    case SpellType.SkillShot:
                        if (args.Value.Unit.ServerPosition.DistanceSqr(ObjectManager.GetLocalPlayer().ServerPosition) <=
                            Menu[args.Value.Unit.ChampionName][args.Value.Unit.ChampionName + ".Distance"].Value * Menu[args.Value.Unit.ChampionName][args.Value.Unit.ChampionName + ".Distance"].Value &&
                            ObjectManager.GetLocalPlayer().HealthPercent() <= Menu[args.Value.Unit.ChampionName][args.Value.Unit.ChampionName + ".HPercent"].Value)
                        {
                            OnGapcloser(args.Value.Unit, args.Value);
                        }
                        break;
                    case SpellType.Dash:
                        if (args.Value.Type == SpellType.Dash &&
                            args.Value.EndPosition.DistanceSqr(ObjectManager.GetLocalPlayer().ServerPosition) <=
                            Menu[args.Value.Unit.ChampionName][args.Value.Unit.ChampionName + ".Distance"].Value *
                            Menu[args.Value.Unit.ChampionName][args.Value.Unit.ChampionName + ".Distance"].Value &&
                            ObjectManager.GetLocalPlayer().HealthPercent() <= Menu[args.Value.Unit.ChampionName][args.Value.Unit.ChampionName + ".HPercent"].Value)
                        {
                            OnGapcloser(args.Value.Unit, args.Value);
                        }
                        break;
                    case SpellType.Targeted:
                        if (ObjectManager.GetLocalPlayer().HealthPercent() <= Menu[args.Value.Unit.ChampionName][args.Value.Unit.ChampionName + ".HPercent"].Value)
                        {
                            OnGapcloser(args.Value.Unit, args.Value);
                        }
                        break;
                    case SpellType.Melee:
                        if (ObjectManager.GetLocalPlayer().HealthPercent() <= Menu[args.Value.Unit.ChampionName][args.Value.Unit.ChampionName + ".Melee"].Value)
                        {
                            OnGapcloser(args.Value.Unit, args.Value);
                        }
                        break;
                }
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, Obj_AI_BaseMissileClientDataEventArgs args)
        {
            if (sender == null
            || !sender.IsValid 
            ||  sender.Type != GameObjectType.obj_AI_Hero 
            || !sender.IsEnemy
            ||  string.IsNullOrEmpty(args.SpellData.Name) 
            ||  args.SpellData.Name.ToLower().Contains("attack") || args.SpellData.Name.ToLower().Contains("crit"))
            {
                return;
            }

            if (Spells.All(x => !string.Equals(x.SpellName.ToLower(), args.SpellData.Name, StringComparison.CurrentCultureIgnoreCase)) ||
                !Menu[sender.UnitSkinName][args.SpellData.Name.ToLower()].Enabled)
            {
                return;
            }

            if (!Gapclosers.ContainsKey(sender.NetworkId))
            {
                Gapclosers.Add(sender.NetworkId, new GapcloserArgs());
            }

            var gapclosers = Gapclosers[sender.NetworkId];

            gapclosers.Unit = (Obj_AI_Hero) sender;
            gapclosers.Slot = args.SpellSlot;
            gapclosers.Type = args.Target != null ? SpellType.Targeted : SpellType.SkillShot;

            gapclosers.SpellName = args.SpellData.Name;
            gapclosers.StartTick = Game.TickCount;
            gapclosers.HaveShield = HaveShiledBuff(sender);

            gapclosers.StartPosition = args.Start;
            gapclosers.EndPosition = args.End;
        }

        private static bool HaveShiledBuff(Obj_AI_Base target)
        {
            if (target == null || target.IsDead || target.Health <= 0 || !target.IsValidTarget())
            {
                return false;
            }

            return target.HasBuff("BlackShield") 
                || target.HasBuff("bansheesveil")
                || target.HasBuff("SivirE")
                || target.HasBuff("NocturneShroudofDarkness")
                || target.HasBuff("itemmagekillerveil")
                || target.HasBuffOfType(BuffType.SpellShield);
        }

        private static void Initialize()
        {
            #region Aatrox

            Spells.Add(new SpellData
            {
                ChampionName = "Aatrox",
                Slot = SpellSlot.Q,
                SpellName = "aatroxq",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Ahri

            Spells.Add(new SpellData
            {
                ChampionName = "Ahri",
                Slot = SpellSlot.R,
                SpellName = "ahritumble",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Akali

            Spells.Add(new SpellData
            {
                ChampionName = "Akali",
                Slot = SpellSlot.R,
                SpellName = "akalishadowdance",
                SpellType = SpellType.Targeted
            });

            #endregion

            #region Alistar

            Spells.Add(new SpellData
            {
                ChampionName = "Alistar",
                Slot = SpellSlot.W,
                SpellName = "headbutt",
                SpellType = SpellType.Targeted
            });

            #endregion

            #region Azir

            Spells.Add(new SpellData
            {
                ChampionName = "Azir",
                Slot = SpellSlot.E,
                SpellName = "azire",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Caitlyn

            Spells.Add(new SpellData
            {
                ChampionName = "Caitlyn",
                Slot = SpellSlot.E,
                SpellName = "caitlynentrapment",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Camille

            Spells.Add(new SpellData
            {
                ChampionName = "Camille",
                Slot = SpellSlot.E,
                SpellName = "camillee",
                SpellType = SpellType.SkillShot
            });

            Spells.Add(new SpellData
            {
                ChampionName = "Camille",
                Slot = SpellSlot.E,
                SpellName = "camilleedash2",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Corki

            Spells.Add(new SpellData
            {
                ChampionName = "Corki",
                Slot = SpellSlot.W,
                SpellName = "carpetbomb",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Diana

            Spells.Add(new SpellData
            {
                ChampionName = "Diana",
                Slot = SpellSlot.R,
                SpellName = "dianateleport",
                SpellType = SpellType.Targeted
            });

            #endregion

            #region Ekko

            Spells.Add(new SpellData
            {
                ChampionName = "Ekko",
                Slot = SpellSlot.E,
                SpellName = "ekkoeattack",
                SpellType = SpellType.Targeted
            });

            #endregion

            #region Elise

            Spells.Add(new SpellData
            {
                ChampionName = "Elise",
                Slot = SpellSlot.Q,
                SpellName = "elisespiderqcast",
                SpellType = SpellType.SkillShot
            });

            Spells.Add(new SpellData
            {
                ChampionName = "Elise",
                Slot = SpellSlot.E,
                SpellName = "elisespideredescent",
                SpellType = SpellType.Targeted
            });

            #endregion

            #region Ezreal

            Spells.Add(new SpellData
            {
                ChampionName = "Ezreal",
                Slot = SpellSlot.E,
                SpellName = "ezrealarcaneshift",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Fiora

            Spells.Add(new SpellData
            {
                ChampionName = "Fiora",
                Slot = SpellSlot.Q,
                SpellName = "fioraq",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Fizz

            Spells.Add(new SpellData
            {
                ChampionName = "Fizz",
                Slot = SpellSlot.Q,
                SpellName = "fizzpiercingstrike",
                SpellType = SpellType.Targeted
            });

            #endregion

            #region Galio

            Spells.Add(new SpellData
            {
                ChampionName = "Galio",
                Slot = SpellSlot.E,
                SpellName = "galioe",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Gnar

            Spells.Add(new SpellData
            {
                ChampionName = "Gnar",
                Slot = SpellSlot.E,
                SpellName = "gnarbige",
                SpellType = SpellType.SkillShot
            });

            Spells.Add(new SpellData
            {
                ChampionName = "Gnar",
                Slot = SpellSlot.E,
                SpellName = "gnare",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Gragas

            Spells.Add(new SpellData
            {
                ChampionName = "Gragas",
                Slot = SpellSlot.E,
                SpellName = "gragase",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Graves

            Spells.Add(new SpellData
            {
                ChampionName = "Graves",
                Slot = SpellSlot.E,
                SpellName = "gravesmove",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Hecarim

            Spells.Add(new SpellData
            {
                ChampionName = "Hecarim",
                Slot = SpellSlot.R,
                SpellName = "hecarimult",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Illaoi

            Spells.Add(new SpellData
            {
                ChampionName = "Illaoi",
                Slot = SpellSlot.W,
                SpellName = "illaoiwattack",
                SpellType = SpellType.Targeted
            });

            #endregion

            #region Irelia

            Spells.Add(new SpellData
            {
                ChampionName = "Irelia",
                Slot = SpellSlot.Q,
                SpellName = "ireliagatotsu",
                SpellType = SpellType.Targeted
            });

            #endregion

            #region JarvanIV

            Spells.Add(new SpellData
            {
                ChampionName = "JarvanIV",
                Slot = SpellSlot.Q,
                SpellName = "jarvanivdragonstrike",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Jax

            Spells.Add(new SpellData
            {
                ChampionName = "Jax",
                Slot = SpellSlot.Q,
                SpellName = "jaxleapstrike",
                SpellType = SpellType.Targeted
            });

            #endregion

            #region Jayce

            Spells.Add(new SpellData
            {
                ChampionName = "Jayce",
                Slot = SpellSlot.Q,
                SpellName = "jaycetotheskies",
                SpellType = SpellType.Targeted
            });

            #endregion

            #region Kassadin

            Spells.Add(new SpellData
            {
                ChampionName = "Kassadin",
                Slot = SpellSlot.R,
                SpellName = "riftwalk",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Katarina

            Spells.Add(new SpellData
            {
                ChampionName = "Katarina",
                Slot = SpellSlot.E,
                SpellName = "katarinae",
                SpellType = SpellType.Targeted
            });

            #endregion

            #region Kayn

            Spells.Add(new SpellData
            {
                ChampionName = "Kayn",
                Slot = SpellSlot.Q,
                SpellName = "kaynq",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Khazix

            Spells.Add(new SpellData
            {
                ChampionName = "Khazix",
                Slot = SpellSlot.E,
                SpellName = "khazixe",
                SpellType = SpellType.SkillShot
            });

            Spells.Add(new SpellData
            {
                ChampionName = "Khazix",
                Slot = SpellSlot.E,
                SpellName = "khazixelong",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Kindred

            Spells.Add(new SpellData
            {
                ChampionName = "Kindred",
                Slot = SpellSlot.Q,
                SpellName = "kindredq",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Leblanc

            Spells.Add(new SpellData
            {
                ChampionName = "Leblanc",
                Slot = SpellSlot.W,
                SpellName = "leblancslide",
                SpellType = SpellType.SkillShot
            });

            Spells.Add(new SpellData
            {
                ChampionName = "Leblanc",
                Slot = SpellSlot.W,
                SpellName = "leblancslidem",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region LeeSin

            Spells.Add(new SpellData
            {
                ChampionName = "LeeSin",
                Slot = SpellSlot.Q,
                SpellName = "blindmonkqtwo",
                SpellType = SpellType.Targeted
            });

            #endregion

            #region Leona

            Spells.Add(new SpellData
            {
                ChampionName = "Leona",
                Slot = SpellSlot.E,
                SpellName = "leonazenithblade",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Lucian

            Spells.Add(new SpellData
            {
                ChampionName = "Lucian",
                Slot = SpellSlot.E,
                SpellName = "luciane",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Malphite

            Spells.Add(new SpellData
            {
                ChampionName = "Malphite",
                Slot = SpellSlot.R,
                SpellName = "ufslash",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region MasterYi

            Spells.Add(new SpellData
            {
                ChampionName = "MasterYi",
                Slot = SpellSlot.Q,
                SpellName = "alphastrike",
                SpellType = SpellType.Targeted
            });

            #endregion

            #region MonkeyKing

            Spells.Add(new SpellData
            {
                ChampionName = "MonkeyKing",
                Slot = SpellSlot.E,
                SpellName = "monkeykingnimbus",
                SpellType = SpellType.Targeted
            });

            #endregion

            #region Nautilus

            Spells.Add(new SpellData
            {
                ChampionName = "Nautilus",
                Slot = SpellSlot.Q,
                SpellName = "nautilusq",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Nidalee

            Spells.Add(new SpellData
            {
                ChampionName = "Nidalee",
                Slot = SpellSlot.W,
                SpellName = "pounce",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Pantheon

            Spells.Add(new SpellData
            {
                ChampionName = "Pantheon",
                Slot = SpellSlot.W,
                SpellName = "pantheon_leapbash",
                SpellType = SpellType.Targeted
            });

            #endregion

            #region Poppy

            Spells.Add(new SpellData
            {
                ChampionName = "Poppy",
                Slot = SpellSlot.E,
                SpellName = "poppyheroiccharge",
                SpellType = SpellType.Targeted
            });

            #endregion

            #region Quinn

            Spells.Add(new SpellData
            {
                ChampionName = "Quinn",
                Slot = SpellSlot.E,
                SpellName = "quinne",
                SpellType = SpellType.Targeted
            });

            #endregion

            #region Rakan

            Spells.Add(new SpellData
            {
                ChampionName = "Rakan",
                Slot = SpellSlot.W,
                SpellName = "rakanw",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region RekSai

            Spells.Add(new SpellData
            {
                ChampionName = "RekSai",
                Slot = SpellSlot.E,
                SpellName = "reksaieburrowed",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Renekton

            Spells.Add(new SpellData
            {
                ChampionName = "Renekton",
                Slot = SpellSlot.E,
                SpellName = "renektonsliceanddice",
                SpellType = SpellType.SkillShot
            });

            Spells.Add(new SpellData
            {
                ChampionName = "Renekton",
                Slot = SpellSlot.E,
                SpellName = "renektonpreexecute",
                SpellType = SpellType.SkillShot
            });

            Spells.Add(new SpellData
            {
                ChampionName = "Renekton",
                Slot = SpellSlot.E,
                SpellName = "renektonsuperexecute",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Rengar

            Spells.Add(new SpellData
            {
                ChampionName = "Rengar",
                Slot = SpellSlot.Unknown,
                SpellName = "rengarpassivebuffdash",
                SpellType = SpellType.SkillShot
            });

            Spells.Add(new SpellData
            {
                ChampionName = "Rengar",
                Slot = SpellSlot.Unknown,
                SpellName = "rengarpassivebuffdashaadummy",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Riven

            Spells.Add(new SpellData
            {
                ChampionName = "Riven",
                Slot = SpellSlot.Q,
                SpellName = "riventricleave",
                SpellType = SpellType.SkillShot
            });

            Spells.Add(new SpellData
            {
                ChampionName = "Riven",
                Slot = SpellSlot.E,
                SpellName = "rivenfeint",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Sejuani

            Spells.Add(new SpellData
            {
                ChampionName = "Sejuani",
                Slot = SpellSlot.Q,
                SpellName = "sejuaniarcticassault",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Shen

            Spells.Add(new SpellData
            {
                ChampionName = "Shen",
                Slot = SpellSlot.E,
                SpellName = "shene",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Shyvana

            Spells.Add(new SpellData
            {
                ChampionName = "Shyvana",
                Slot = SpellSlot.R,
                SpellName = "shyvanatransformcast",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Talon

            Spells.Add(new SpellData
            {
                ChampionName = "Talon",
                Slot = SpellSlot.Q,
                SpellName = "talonq",
                SpellType = SpellType.Targeted
            });

            Spells.Add(new SpellData
            {
                ChampionName = "Talon",
                Slot = SpellSlot.E,
                SpellName = "taloncutthroat",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Tristana

            Spells.Add(new SpellData
            {
                ChampionName = "Tristana",
                Slot = SpellSlot.W,
                SpellName = "rocketjump",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Tryndamere

            Spells.Add(new SpellData
            {
                ChampionName = "Tryndamere",
                Slot = SpellSlot.E,
                SpellName = "slashcast",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Vi

            Spells.Add(new SpellData
            {
                ChampionName = "Vi",
                Slot = SpellSlot.Q,
                SpellName = "viq",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Vayne

            Spells.Add(new SpellData
            {
                ChampionName = "Vayne",
                Slot = SpellSlot.Q,
                SpellName = "vaynetumble",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Warwick

            Spells.Add(new SpellData
            {
                ChampionName = "Warwick",
                Slot = SpellSlot.R,
                SpellName = "warwickr",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region XinZhao

            Spells.Add(new SpellData
            {
                ChampionName = "XinZhao",
                Slot = SpellSlot.E,
                SpellName = "xenzhaosweep",
                SpellType = SpellType.Targeted
            });

            #endregion

            #region Yasuo

            Spells.Add(new SpellData
            {
                ChampionName = "Yasuo",
                Slot = SpellSlot.E,
                SpellName = "yasuodashwrapper",
                SpellType = SpellType.Targeted
            });

            #endregion

            #region Zac

            Spells.Add(new SpellData
            {
                ChampionName = "Zac",
                Slot = SpellSlot.E,
                SpellName = "zace",
                SpellType = SpellType.SkillShot
            });

            #endregion

            #region Zed

            Spells.Add(new SpellData
            {
                ChampionName = "Zed",
                Slot = SpellSlot.R,
                SpellName = "zedr",
                SpellType = SpellType.Targeted
            });

            #endregion

            #region Ziggs

            Spells.Add(new SpellData
            {
                ChampionName = "Ziggs",
                Slot = SpellSlot.W,
                SpellName = "ziggswtoggle",
                SpellType = SpellType.SkillShot
            });

            #endregion
        }
    }
}