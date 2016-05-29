using HQMEditorDedicated;
using System;

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
        public icingState currIcingState;
        HQMTeam lastTouchedPuck = HQMTeam.NoTeam;
        HQMVector lastTouchedPuckAt = Puck.LastTouchedPosition;
        HQMVector currentPuckVector;
        HQMVector previousPuckVector = Puck.Position;
        bool hasWarned;
        bool isPuckTouched;

        public void checkForIcing()
        {
            currentPuckVector = Puck.Position;
            if (currentPuckVector == previousPuckVector)
                return;
            Player[] players = PlayerManager.Players;
            isPuckTouched = puckTouched();

            if (currIcingState == icingState.None)
                setIcingState();

            if (currIcingState != icingState.None)
            {
                detectIcingState();
                detectTouchIcingState();
            }
            
            if (hasWarned == true && currIcingState == icingState.None)
            {
                hasWarned = false;
            }
                
            previousPuckVector = currentPuckVector;
        }

        void setIcingState()
        {
            // detect red icing
            if (lastTouchedPuck == HQMTeam.Red &&
                !isPuckTouched &&                             // has puck been shot by red and no longer touched?
                previousPuckVector.Z > CENTER_ICE &&
                currentPuckVector.Z <= CENTER_ICE)
                currIcingState = icingState.Red;

            // detect blue icing
            else if (lastTouchedPuck == HQMTeam.Blue &&
                !isPuckTouched &&                             // has puck been shot by blue and no longer touched?
                previousPuckVector.Z < CENTER_ICE &&
                currentPuckVector.Z >= CENTER_ICE)
                currIcingState = icingState.Blue;
        }

        void detectIcingState()
        {
            if (currIcingState == icingState.Red)
            {
                if (currentPuckVector.Z > BLUE_GOALLINE_Z && isPuckTouched)
                    clearIcing();

                else if (currentPuckVector.Z < BLUE_WARNINGLINE && !hasWarned)
                    warnIcing("RED");

                else if (currentPuckVector.Z <= BLUE_GOALLINE_Z)
                {
                    if (currIcingType == icingType.Touch && !puckOnNet(currentPuckVector.X, currentPuckVector.Y))
                        currIcingState = icingState.RedCross;

                    else if (!puckOnNet(currentPuckVector.X, currentPuckVector.Y))
                        callIcing("RED");

                    else
                        clearIcing();
                }
            }

            else if (currIcingState == icingState.Blue)
            {
                if (currentPuckVector.Z < RED_GOALLINE_Z && isPuckTouched)
                    clearIcing();

                else if (currentPuckVector.Z > RED_WARNINGLINE && !hasWarned)
                    warnIcing("BLUE");

                else if (currentPuckVector.Z >= RED_GOALLINE_Z)
                {
                    if (currIcingType == icingType.Touch && !puckOnNet(currentPuckVector.X, currentPuckVector.Y))
                        currIcingState = icingState.BlueCross;

                    else if (!puckOnNet(currentPuckVector.X, currentPuckVector.Y))
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
                if (isPuckTouched)
                {
                    if (lastTouchedPuck == HQMTeam.Red)
                        clearIcing();

                    else if (lastTouchedPuck == HQMTeam.Blue)
                        callIcing("RED");
                }
            }

            else if (currIcingState == icingState.BlueCross)
            {
                if (isPuckTouched)
                {
                    if (lastTouchedPuck == HQMTeam.Blue)
                        clearIcing();

                    else if (lastTouchedPuck == HQMTeam.Red)
                        callIcing("BLUE");
                }
            }
        }

        void teamTouchedPuck()
        {
            foreach (Player p in PlayerManager.Players)
            {
                if (p.Team != HQMTeam.NoTeam && (p.StickPosition - lastTouchedPuckAt).Magnitude < 0.25f)
                {
                    lastTouchedPuck = p.Team;
                    return;
                }
            }
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
            setFaceoffPositions(team);
        }

        void setFaceoffPositions(string team = "")
        {
            float offsetX, offsetZ, teamOffsetZ, positionX, positionZ;
            HQMVector v;

            if (currentPuckVector.X <= 15)
                offsetX = 7.99f;
            else
                offsetX = 22.01f;

            if (team == "RED")
                offsetZ = 50.96f;
            else if (team == "BLUE")
                offsetZ = 10.04f;
            else
                offsetZ = 0;

            System.Threading.Thread.Sleep(15);
            Puck.Position = new HQMVector(offsetX, 2.5f, offsetZ);
            Puck.Velocity = new HQMVector(0, 0, 0);
            Puck.RotationalVelocity = new HQMVector(0, 0, 0);

            foreach (Player p in PlayerManager.Players)
            {
                if (p.Team == HQMTeam.NoTeam)
                    continue;
                else if (p.Team == HQMTeam.Blue)
                {
                    teamOffsetZ = -2.77f;

                    switch (p.Role)
                    {
                        case HQMRole.C:
                            positionX = 0f;
                            positionZ = 0f;
                            v = new HQMVector(offsetX + positionX, 4.5f, offsetZ + teamOffsetZ + positionZ);
                            p.Position = v;
                            break;
                        case HQMRole.LW:
                            positionX = -5f;
                            positionZ = -1.26f;
                            v = new HQMVector(offsetX + positionX, 4.5f, offsetZ + teamOffsetZ + positionZ);
                            p.Position = v;
                            break;
                        case HQMRole.RW:
                            positionX = 5f;
                            positionZ = -1.26f;
                            v = new HQMVector(offsetX + positionX, 4.5f, offsetZ + teamOffsetZ + positionZ);
                            p.Position = v;
                            break;
                        case HQMRole.LD:
                            positionX = -2f;
                            positionZ = -7.26f;
                            v = new HQMVector(offsetX + positionX, 4.5f, offsetZ + teamOffsetZ + positionZ);
                            p.Position = v;
                            break;
                        case HQMRole.RD:
                            positionX = 2f;
                            positionZ = -7.26f;
                            v = new HQMVector(offsetX + positionX, 4.5f, offsetZ + teamOffsetZ + positionZ);
                            p.Position = v;
                            break;
                        case HQMRole.G:
                        default:
                            break;
                    }
                }

                else
                {
                    teamOffsetZ = 2.77f;

                    switch (p.Role)
                    {
                        case HQMRole.C:
                            positionX = 0f;
                            positionZ = 0f;
                            v = new HQMVector(offsetX + positionX, 4.5f, offsetZ + teamOffsetZ + positionZ);
                            p.Position = v;
                            string s = v.ToString();
                            Chat.SendMessage("Position " + s);
                            break;
                        case HQMRole.LW:
                            positionX = 5f;
                            positionZ = 1.26f;
                            v = new HQMVector(offsetX + positionX, 4.5f, offsetZ + teamOffsetZ + positionZ);
                            p.Position = v;
                            break;
                        case HQMRole.RW:
                            positionX = -5f;
                            positionZ = 1.26f;
                            v = new HQMVector(offsetX + positionX, 4.5f, offsetZ + teamOffsetZ + positionZ);
                            p.Position = v;
                            break;
                        case HQMRole.LD:
                            positionX = 2f;
                            positionZ = 7.26f;
                            v = new HQMVector(offsetX + positionX, 4.5f, offsetZ + teamOffsetZ + positionZ);
                            p.Position = v;
                            break;
                        case HQMRole.RD:
                            positionX = -2f;
                            positionZ = 7.26f;
                            v = new HQMVector(offsetX + positionX, 4.5f, offsetZ + teamOffsetZ + positionZ);
                            p.Position = v;
                            break;
                        case HQMRole.G:
                        default:
                            break;
                    }
                }
            }
            return;
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

        bool puckTouched()
        {
            if (lastTouchedPuckAt != Puck.LastTouchedPosition)
            {
                lastTouchedPuckAt = Puck.LastTouchedPosition;
                teamTouchedPuck();      
                return true;
            }
            else
                return false;
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