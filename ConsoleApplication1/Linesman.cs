using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HockeyEditor;

namespace IcingRef
{
    public class Linesman
    {
        const float BLUE_GOALLINE_Z = 4.15f;
        const float RED_GOALLINE_Z = 56.85f;
        const float CENTER_ICE = 30.5f;

        HQMTeam lastTouchedPuck = HQMTeam.NoTeam;
        float previousPuckZ = Puck.Position.Z;
        bool hasWarned = false;
        icingState currIcingState = icingState.None;

        public void checkForIcing()
        {
            float currentPuckZ = Puck.Position.Z;
            float currentPuckY = Puck.Position.Y;
            float currentPuckX = Puck.Position.X;
            Player[] players = PlayerManager.Players;

            // detect red icing
            if (lastTouchedPuck == HQMTeam.Red &&
                !isPuckTouched() &&
                previousPuckZ > CENTER_ICE &&
                currentPuckZ <= CENTER_ICE)
            {
                currIcingState = icingState.Red;
            }

            // detect blue icing
            if (lastTouchedPuck == HQMTeam.Blue &&
                !isPuckTouched() &&
                previousPuckZ < CENTER_ICE &&
                currentPuckZ >= CENTER_ICE)
            {
                currIcingState = icingState.Blue;
            }

            if (currIcingState == icingState.Red)
            {
                if (currentPuckZ > BLUE_GOALLINE_Z && isPuckTouched())
                {
                    currIcingState = icingState.None;
                    if (hasWarned)
                        Chat.SendChatMessage("ICING CLEAR");
                }

                else if (currentPuckZ < 17f && !hasWarned)
                {
                    Chat.SendChatMessage("ICING WARNING - RED");
                    hasWarned = true;
                }

                else if (currentPuckZ <= BLUE_GOALLINE_Z)
                {
                    if ((currentPuckX < 13.75 || currentPuckX > 16.25) || currentPuckY > 0.83)
                    {
                        Chat.SendChatMessage("ICING - RED");
                        currIcingState = icingState.None;
                    }
                    else
                    {
                        currIcingState = icingState.None;
                        Chat.SendChatMessage("GOOD GOAL - NO ICING");
                    }
                }
            }

            else if (currIcingState == icingState.Blue)
            {
                if (currentPuckZ < RED_GOALLINE_Z && isPuckTouched())
                {
                    currIcingState = icingState.None;
                    if (hasWarned)
                        Chat.SendChatMessage("ICING CLEAR");
                }

                else if (currentPuckZ > 47f && !hasWarned)
                {
                    Chat.SendChatMessage("ICING WARNING - BLUE");
                    hasWarned = true;
                }

                else if (currentPuckZ >= RED_GOALLINE_Z)
                {
                    if ((currentPuckX < 13.75 || currentPuckX > 16.25) || currentPuckY > 0.83)
                    {
                        Chat.SendChatMessage("ICING - BLUE");
                        currIcingState = icingState.None;
                    }
                    else
                    {
                        currIcingState = icingState.None;
                        Chat.SendChatMessage("GOOD GOAL - NO ICING");
                    }
                }
            }
            else if (currIcingState == icingState.None)
                hasWarned = false;
            previousPuckZ = Puck.Position.Z;
            if (isPuckTouched())
                lastTouchedPuck = teamTouchedPuck();
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
            Blue
        }
    }
}