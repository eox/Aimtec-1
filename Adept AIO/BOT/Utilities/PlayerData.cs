namespace Adept_AIO.BOT.Utilities
{
    class PlayerData
    {
        public static ChampionType CHAMPION_TYPE;
        public static LaneType LANE_TYPE;
    }

    public class TypeNames
    {
        public static string[] MAGES =
        {
            "Annie", "Ahri", "Anivia", "Annie", "Brand", "Cassiopeia", "Diana", "FiddleSticks",
            "Gragas", "Heimerdinger", "Karthus",
            "Kassadin", "Leblanc", "Lissandra", "Lux", "Malzahar",
            "Morgana", "Orianna", "Swain", "Syndra", "TwistedFate", "Veigar", "Viktor",
            "Xerath", "Ziggs", "Zyra", "Vel'Koz",
            "Azir", "Kayle", "Teemo", "Gragas", "Galio", "Singed", "Nunu", "Evelynn", "Elise", "Ryze", "Ekko", "Fizz", "Nidalee",
            "Akali", "Katarina", "Vladimir", "Rumble", "Mordekaiser", "Kennen", "Zoe", "Taliyah", "AurelianSol"
        };

        public static string[] SUPPORTS =
        {
            "Alistar", "Blitzcrank", "Janna", "Karma", "Nami", "Sona", "Soraka", "Taric",
            "Thresh", "Zilean", "Lulu", "Bard"
        };

        public static string[] TANKS =
        {
            "Amumu", "DrMundo", "Sion", "Galio", "Hecarim", "Rammus", "Sejuani",
            "Shen", "Singed", "Skarner", "Volibear", "Leona",
            "Yorick", "Zac", "Udyr", "Nasus", "Trundle", "Irelia", "Braum", "Vi",
            "Warwick", "Ornn", "Ivern", "Malphite", "Maokai", "Chogath"
        };

        public static string[] ADC =
        {
            "Ashe", "Caitlyn", "Corki", "Draven", "Ezreal", "Graves", "KogMaw", "MissFortune",
            "Sivir", "Jinx", "Xayah", "Rakkan",
            "Tristana", "Twitch", "Varus", "Lucian", "Quinn", "Kalista", "Vayne", " Jhin"
        };

        public static string[] BRUISERS =
        {
            "Jayce", "Talon", "Urgot", "Gangplank", "Zed", "Aatrox", "Darius", "Fiora", "Garen", "JarvanIV", "Jax", "Khazix", "LeeSin",
            "Nautilus", "Nocturne", "Olaf", "Poppy", "Kindred",
            "Renekton", "Rengar", "Riven", "Shyvana", "Tryndamere", "MonkeyKing", "XinZhao", "Pantheon",
            "Rek'Sai", "Gnar", "Wukong", "RekSai", "Kayn", "Camille", "Kled", "Yasuo", "MasterYi", "Shaco"
        };
    }

    enum LaneType
    {
        Botlane = 0,
        Midlane = 1,
        Toplane = 2,
        //Jungle
    }

    enum ChampionType
    {
        MAGE,
        TANK,
        SUPPORT,
        ADC,
        BRUISER
    }
}