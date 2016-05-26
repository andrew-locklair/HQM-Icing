using HQMEditorDedicated;

namespace IcingRef
{
    class Program
    {
        static void Main(string[] args)
        {
            MemoryEditor.Init();
            Linesman linesman = new Linesman();
            linesman.setIcingType(HQMIcing.Properties.Settings.Default.icingType);

            while(true)
            {
                if (GameInfo.Period > 0 && GameInfo.IntermissionTime == 0 &&
                    (GameInfo.AfterGoalFaceoffTime == 0 || GameInfo.AfterGoalFaceoffTime >= 649))
                    linesman.checkForIcing();
                else if (GameInfo.AfterGoalFaceoffTime != 0 && linesman.currIcingState != Linesman.icingState.None)
                    linesman.clearIcing();
            }
        }
    }
}
