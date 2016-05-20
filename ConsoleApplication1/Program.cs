using HQMEditorDedicated;

namespace IcingRef
{
    class Program
    {
        static void Main(string[] args)
        {
            MemoryEditor.Init();
            Linesman linesman = new Linesman();

            while(true)
            {
                if (GameInfo.Period > 0 && GameInfo.IntermissionTime == 0 && GameInfo.AfterGoalFaceoffTime == 0)
                    linesman.checkForIcing();
                System.Threading.Thread.Sleep(50);       // approximates ping
            }
        }
    }
}
