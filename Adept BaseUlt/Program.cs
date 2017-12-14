namespace Adept_BaseUlt
{
    using Aimtec.SDK.Events;
    using Baseult_Reworked;
    using Local_SDK;
   
    class Program
    {
        private static void Main()
        {
            GameEvents.GameStart += delegate
            {
                new MenuConfig();

                switch (Global.Player.ChampionName)
                {
                    case "Ashe":
                        new Baseult(1600, 130, 250, 1);
                        break;
                    case "Draven":
                        new Baseult(2000, 160, 300);
                        break;
                    case "Ezreal":
                        new Baseult(2000, 160, 1000);
                        break;
                    case "Jinx":
                        new Baseult(2200, 140, 500, 1);
                        break;
                    case "Karthus":
                        new Baseult(int.MaxValue, int.MaxValue, 3000);
                        break;
                    case "Xerath":
                        new Baseult(float.MaxValue, 130f, 700, int.MaxValue, 2500f);
                        break;
                    case "Ziggs":
                        new Baseult(1750, 275, 250, int.MaxValue, 5250);
                        break;
                }
            };
        }
    }
}