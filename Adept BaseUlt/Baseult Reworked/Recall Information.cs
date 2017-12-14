namespace Adept_BaseUlt.Baseult_Reworked
{
    using Aimtec;

    class RecallInformation
    {
        public int NetworkId;
        public int Duration;
        public int Start;
        public Obj_AI_Base Sender;
     
        public RecallInformation(int netid, int duration, Obj_AI_Base sender, int start = 0)
        {
            NetworkId = netid;
            Duration = duration;
            Sender = sender;
            Start = start;
        }
    }
}
