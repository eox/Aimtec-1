﻿namespace Adept_AIO.SDK.Unit_Extensions
{
    using System;
    using Aimtec;
    using Aimtec.SDK.Prediction.Health;
    using Aimtec.SDK.TargetSelector;
    using Draw_Extension;

    using IOrbwalker = Orbwalking.IOrbwalker;
    using Orbwalker = Orbwalking.Orbwalker;

    class Global
    {
        public static Random Random;
        public static IOrbwalker Orbwalker;
        public static ITargetSelector TargetSelector;
        public static IHealthPrediction HealthPrediction;

        public static Obj_AI_Hero Player = ObjectManager.GetLocalPlayer();
        public static DamageIndicator DamageIndicator;

        public Global()
        {
            Random = new Random();
            Orbwalker = new Orbwalker();
            TargetSelector = Aimtec.SDK.TargetSelector.TargetSelector.Implementation;
            HealthPrediction = new HealthPrediction();
            DamageIndicator = new DamageIndicator();
        }
    }
}