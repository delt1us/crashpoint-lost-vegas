//*************************************************************************************************************
/*  Team Member
 *  This is a super class, it is used by the Player and NetworkAI classes. It:
 *      Defines common variables used by various things such as damage detection and weapon controlling
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *      Armin - 11/08/23 - Added support for AI
 *      Armin - 11/08/23 - Added support for checking AI teams
 */
//*************************************************************************************************************

using Unity.Netcode;

public enum ETeamMember
{
    Player,
    NetworkAI,
    None
}

public abstract class TeamMember : NetworkBehaviour
{
    public ETeamMember type; 
    public NetworkVariable<int> id = new NetworkVariable<int>();
    public NetworkVariable<int> teamId = new NetworkVariable<int>();
    public NetworkVariable<bool> isHunted = new NetworkVariable<bool>(false);
    public float huntedTimer = 0f;
    
    private void Awake()
    {
        type = ETeamMember.None;
    }
}
