namespace Adept_Prediction
{
    using Adept_AIO.SDK.Geometry_Related;
    using Aimtec.SDK.Events;
    using Aimtec.SDK.Prediction.Skillshots;

    class Program
    {
        private static void Main()
        {
            GameEvents.GameStart += GameEvents_GameStart;
        }

        private static void GameEvents_GameStart()
        {
            Prediction.Instance.AddPredictionImplementation("Adept Prediction", new LocalPrediction());
        }
    }
}
