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

        icingType currIcingType = icingType.Touch;
        HQMTeam lastTouchedPuck = HQMTeam.NoTeam;
        float previousPuckZ = Puck.Position.Z;
        bool hasWarned = false;
        icingState currIcingState = icingState.None;

        public void checkForIcing()
        {
            float currentPuckZ = Puck.Position.Z;
            float currentPuckY = Puck.Position.Y;
            float currentPuckX = Puck.Position.X;
            bool puckTouched = isPuckTouched();
            Player[] players = PlayerManager.Players;
            if (puckTouched)
                lastTouchedPuck = teamTouchedPuck();

            // detect red icing
            if (lastTouchedPuck == HQMTeam.Red &&
                !puckTouched &&                             // has puck been shot by red and no longer touched?
                previousPuckZ > CENTER_ICE &&
                currentPuckZ <= CENTER_ICE)
            {
                currIcingState = icingState.Red;
            }

            // detect blue icing
            else if (lastTouchedPuck == HQMTeam.Blue &&
                !puckTouched &&                             // has puck been shot by blue and no longer touched?
                previousPuckZ < CENTER_ICE &&
                currentPuckZ >= CENTER_ICE)
            {
                currIcingState = icingState.Blue;
            }
            // call icing method
            else if (currIcingState == icingState.Red)
            {
                if (currentPuckZ > BLUE_GOALLINE_Z && puckTouched)
                {
                    currIcingState = icingState.None;
                    if (hasWarned)
                        Chat.SendMessage("ICING CLEAR");
                }

                else if (currentPuckZ < BLUE_WARNINGLINE && !hasWarned)
                {
                    Chat.SendMessage("ICING WARNING - RED");
                    hasWarned = true;
                }

                else if (currentPuckZ <= BLUE_GOALLINE_Z)
                {
                    if (currIcingType == icingType.Touch)
                    {
                        if (currentPuckX < LEFT_GOALPOST || currentPuckX > RIGHT_GOALPOST || 
                            currentPuckY > TOP_GOALPOST)
                            currIcingState = icingState.RedCross;
                    }

                    else if ((currentPuckX < LEFT_GOALPOST || currentPuckX > RIGHT_GOALPOST) || 
                              currentPuckY > TOP_GOALPOST)
                    {
                        Chat.SendMessage("ICING - RED");
                        Tools.PauseGame();
                        System.Threading.Thread.Sleep(5000);
                        Tools.ForceFaceoff();
                        Tools.ResumeGame();
                        currIcingState = icingState.None;
                    }

                    else
                    {
                        currIcingState = icingState.None;
                        Chat.SendMessage("GOOD GOAL - NO ICING");
                    }
                }
            }

            else if (currIcingState == icingState.RedCross)
            {
                if (puckTouched)
                {
                    if (teamTouchedPuck() == HQMTeam.Red)
                    {
                        currIcingState = icingState.None;
                        Chat.SendMessage("ICING CLEAR");
                    }

                    else if (teamTouchedPuck() == HQMTeam.Blue)
                    {
                        Chat.SendMessage("ICING - RED");
                        Tools.PauseGame();
                        System.Threading.Thread.Sleep(5000);
                        Tools.ForceFaceoff();
                        Tools.ResumeGame();
                        currIcingState = icingState.None;
                    }
                }
            }

            else if (currIcingState == icingState.Blue)
            {
                if (currentPuckZ < RED_GOALLINE_Z && puckTouched)
                {
                    currIcingState = icingState.None;
                    if (hasWarned)
                        Chat.SendMessage("ICING CLEAR");
                }

                else if (currentPuckZ > RED_WARNINGLINE && !hasWarned)
                {
                    Chat.SendMessage("ICING WARNING - BLUE");
                    hasWarned = true;
                }

                else if (currentPuckZ >= RED_GOALLINE_Z)
                {
                    if (currIcingType == icingType.Touch)
                    {
                        if (currentPuckX < LEFT_GOALPOST || currentPuckX > RIGHT_GOALPOST ||
                            currentPuckY > TOP_GOALPOST)
                            currIcingState = icingState.BlueCross;
                    }

                    else if ((currentPuckX < LEFT_GOALPOST || currentPuckX > RIGHT_GOALPOST) || currentPuckY > TOP_GOALPOST)
                    {
                        Chat.SendMessage("ICING - BLUE");
                        Tools.PauseGame();
                        System.Threading.Thread.Sleep(5000);
                        Tools.ForceFaceoff();
                        Tools.ResumeGame();
                        currIcingState = icingState.None;
                    }
                    else
                    {
                        currIcingState = icingState.None;
                        Chat.SendMessage("GOOD GOAL - NO ICING");
                    }
                }
            }

            else if (currIcingState == icingState.BlueCross)
            {
                if (puckTouched)
                {
                    if (teamTouchedPuck() == HQMTeam.Blue)
                    {
                        currIcingState = icingState.None;
                        Chat.SendMessage("ICING CLEAR");
                    }

                    else if (teamTouchedPuck() == HQMTeam.Red)
                    {
                        Chat.SendMessage("ICING - BLUE");
                        Tools.PauseGame();
                        System.Threading.Thread.Sleep(5000);
                        Tools.ForceFaceoff();
                        Tools.ResumeGame();
                        currIcingState = icingState.None;
                    }
                }
            }

            if (currIcingState == icingState.None)
                hasWarned = false;

            previousPuckZ = Puck.Position.Z;
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

        bool isPuckTouched()
        {
            foreach (Player p in PlayerManager.Players)
            {
                if ((p.StickPosition - Puck.Position).Magnitude < 0.25f)
                {
                    return true;
                }
            }
            return false;
        }

        enum icingState
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