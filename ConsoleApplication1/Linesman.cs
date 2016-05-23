using HQMEditorDedicated;

namespace IcingRef
{
    public class Linesman
    {
        const float BLUE_GOALLINE_Z = 4.15f;
        const float RED_GOALLINE_Z = 56.85f;
        const float CENTER_ICE = 30.5f;
        const float BLUE_WARNINGLINE = 17f;
        const float RED_WARNINGLINE = 47f;
        const float LEFT_GOALPOST = 13.75f;
        const float RIGHT_GOALPOST = 16.25f;
        const float TOP_GOALPOST = 0.83f;

        icingType currIcingType;
        public icingState currIcingState = icingState.None;
        HQMTeam lastTouchedPuck = HQMTeam.NoTeam;
        float previousPuckZ = Puck.Position.Z;
        bool hasWarned = false;

        float currentPuckX, currentPuckY, currentPuckZ;
        bool puckTouched;


        public void checkForIcing()
        {
            currentPuckZ = Puck.Position.Z;
            currentPuckY = Puck.Position.Y;
            currentPuckX = Puck.Position.X;
            puckTouched = true;
            if (teamTouchedPuck() == HQMTeam.NoTeam)
                puckTouched = false;

            Player[] players = PlayerManager.Players;
            if (puckTouched)
                lastTouchedPuck = teamTouchedPuck();

            setIcingState();
            detectIcingState();
            detectTouchIcingState();

            if (currIcingState == icingState.None)
                hasWarned = false;

            previousPuckZ = Puck.Position.Z;
        }

        void setIcingState()
        {
            // detect red icing
            if (lastTouchedPuck == HQMTeam.Red &&
                !puckTouched &&                             // has puck been shot by red and no longer touched?
                previousPuckZ > CENTER_ICE &&
                currentPuckZ <= CENTER_ICE)
                currIcingState = icingState.Red;

            // detect blue icing
            else if (lastTouchedPuck == HQMTeam.Blue &&
                !puckTouched &&                             // has puck been shot by blue and no longer touched?
                previousPuckZ < CENTER_ICE &&
                currentPuckZ >= CENTER_ICE)
                currIcingState = icingState.Blue;
        }

        void detectIcingState()
        {
            if (currIcingState == icingState.Red)
            {
                if (currentPuckZ > BLUE_GOALLINE_Z && puckTouched)
                    clearIcing();

                else if (currentPuckZ < BLUE_WARNINGLINE && !hasWarned)
                    warnIcing("RED");

                else if (currentPuckZ <= BLUE_GOALLINE_Z)
                {
                    if (currIcingType == icingType.Touch && !puckOnNet(currentPuckX, currentPuckY))
                        currIcingState = icingState.RedCross;

                    else if (!puckOnNet(currentPuckX, currentPuckY))
                        callIcing("RED");

                    else
                        clearIcing();
                }
            }

            else if (currIcingState == icingState.Blue)
            {
                if (currentPuckZ < RED_GOALLINE_Z && puckTouched)
                    clearIcing();

                else if (currentPuckZ > RED_WARNINGLINE && !hasWarned)
                    warnIcing("BLUE");

                else if (currentPuckZ >= RED_GOALLINE_Z)
                {
                    if (currIcingType == icingType.Touch && !puckOnNet(currentPuckX, currentPuckY))
                        currIcingState = icingState.BlueCross;

                    else if (!puckOnNet(currentPuckX, currentPuckY))
                        callIcing("BLUE");

                    else
                        clearIcing();
                }
            }
        }

        void detectTouchIcingState()
        {
            if (currIcingState == icingState.RedCross)
            {
                if (puckTouched)
                {
                    if (teamTouchedPuck() == HQMTeam.Red)
                        clearIcing();

                    else if (teamTouchedPuck() == HQMTeam.Blue)
                        callIcing("RED");
                }
            }



            else if (currIcingState == icingState.BlueCross)
            {
                if (puckTouched)
                {
                    if (teamTouchedPuck() == HQMTeam.Blue)
                        clearIcing();

                    else if (teamTouchedPuck() == HQMTeam.Red)
                        callIcing("BLUE");
                }
            }
        }

        HQMTeam teamTouchedPuck()
        {
            foreach (Player p in PlayerManager.Players)
            {
                if ((p.StickPosition - Puck.Position).Magnitude < 0.25f)
                {
                    return p.Team;
                }
            }
            return HQMTeam.NoTeam;
        }

        public void clearIcing()
        {
            currIcingState = icingState.None;
            if (hasWarned)
                Chat.SendMessage("ICING CLEAR");
        }

        void callIcing(string team = "")
        {
            Chat.SendMessage("ICING - " + team);
            Tools.PauseGame();
            System.Threading.Thread.Sleep(5000);
            Tools.ForceFaceoff();
            Tools.ResumeGame();
            currIcingState = icingState.None;
        }

        void warnIcing(string team = "")
        {
            Chat.SendMessage("ICING WARNING - " + team);
            hasWarned = true;
        }

        bool puckOnNet(float x, float y)
        {
            return !(x < LEFT_GOALPOST || x > RIGHT_GOALPOST || y > TOP_GOALPOST);
        }

        public void setIcingType(string type)
        {
            switch (type)
            {
                case "Touch":
                    {
                        currIcingType = icingType.Touch;
                        break;
                    }
                case "NoTouch":
                    {
                        currIcingType = icingType.NoTouch;
                        break;
                    }
                case "Hybrid":
                    { 
                    currIcingType = icingType.Hybrid;
                    break;
                    }
            }
        }

        public enum icingState
        {
            None,
            Red,
            Blue,
            RedCross,
            BlueCross
        }

        enum icingType
        {
            NoTouch,
            Touch,
            Hybrid
        }
    }
}