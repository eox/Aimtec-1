namespace Adept_AIO
{
    using System.Linq;
    using Aimtec.SDK.Events;
    using Aimtec.SDK.Prediction.Skillshots;
    using Champions.Azir;
    using Champions.Draven;
    using Champions.Ezreal;
    using Champions.Gnar;
    using Champions.Gragas;
    using Champions.Graves;
    using Champions.Irelia;
    using Champions.Jax;
    using Champions.Jhin;
    using Champions.Jinx;
    using Champions.Kalista;
    using Champions.Kayn;
    using Champions.LeeSin;
    using Champions.Lucian;
    using Champions.MissFortune;
    using Champions.Rengar;
    using Champions.Riven;
    using Champions.Tristana;
    using Champions.Twitch;
    using Champions.Vayne;
    using Champions.Xerath;
    using Champions.Yasuo;
    using Champions.Yorick;
    using Champions.Zed;
    using Champions.Zoe;
    using SDK.Generic;
    using SDK.Geometry_Related;
    using SDK.Unit_Extensions;
    using SDK.Usables;

    class Bootstrap
    {
        private static readonly string[] Valid =
        {
            "MissFortune",
            "Yorick",
            "Graves",
            "Draven",
            "Xerath",
            "Zoe",
            "Jhin",
            "Gnar",
            "Kalista",
            "Twitch",
            "Lucian",
            "Gragas",
            "Ezreal",
            "Azir",
            "Riven",
            "Irelia",
            "Jax",
            "Rengar",
            "Yasuo",
            "Kayn",
            "LeeSin",
            "Jinx",
            "Tristana",
            "Zed",
            "Vayne"
        };

        private static void Main()
        {
            GameEvents.GameStart += GameEvents_GameStart;
        }

        private static void GameEvents_GameStart()
        {
            Prediction.Instance.AddPredictionImplementation("Adept Prediction", new LocalPrediction());

            if (Valid.All(x => Global.Player.ChampionName != x))
            {
                return;
            }

            GameObjects.Initialize();
            new SummonerSpells();
            new Global();
            new GetRandom();

            switch (Global.Player.ChampionName)
            {
                case "MissFortune":
                    new MissFortune();
                    break;
                case "Yorick":
                    new Yorick();
                    break;
                case "Graves":
                    new Graves();
                    break;
                case "Draven":
                    new Draven();
                    break;
                case "Xerath":
                    new Xerath();
                    break;
                case "Zoe":
                    new Zoe();
                    break;
                case "Jhin":
                    new Jhin();
                    break;
                case "Gnar":
                    new Gnar();
                    break;
                case "Kalista":
                    new Kalista();
                    break;
                case "Twitch":
                    new Twitch();
                    break;
                case "Lucian":
                    new Lucian();
                    break;
                case "Gragas":
                    new Gragas();
                    break;
                case "Ezreal":
                    new Ezreal();
                    break;
                case "Azir":
                    new Azir();
                    break;
                case "Irelia":
                    new Irelia();
                    break;
                case "Jax":
                    new Jax();
                    break;
                case "Jinx":
                    new Jinx();
                    break;
                case "Kayn":
                    new Kayn();
                    break;
                case "LeeSin":
                    new LeeSin();
                    break;
                case "Rengar":
                    new Rengar();
                    break;
                case "Riven":
                    new Riven();
                    break;
                case "Tristana":
                    new Tristana();
                    break;
                case "Yasuo":
                    new Yasuo();
                    break;
                case "Zed":
                    new Zed();
                    break;
                case "Vayne":
                    new Vayne();
                    break;
            }
        }
    }
}