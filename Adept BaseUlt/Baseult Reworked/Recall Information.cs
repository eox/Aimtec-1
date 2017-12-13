namespace Adept_BaseUlt.Baseult_Reworked
{
    using Aimtec;

    class Recall_Information
    {
        public int NetworkID;
        public int Duration;
        public int Start;
        public Obj_AI_Base Sender;
     
        public Recall_Information(int netid, int duration, Obj_AI_Base sender, int start = 0)
        {
            NetworkID = netid;
            Duration = duration;
            Sender = sender;
            Start = start;
        }
    }
}
