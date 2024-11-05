//*************************************************************************************************************
/*  Capture Point
 *  A script to handle the capture point logic:
 *      Who the point is owned by
 *      What colour the point should be
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *      Armin - 05/07/23 - Added to main build
 *      Armin - 07/07/23 - Made team assignment server authoritative
 *      Armin - 09/07/23 - Control point logic working
 *      Armin - 18/07/23 - Support for revamped player script
 *      Armin - 08/08/23 - Fixed sync issue
 *      Armin - 10/08/23 - Fixed edge case of player dying and their car landing in the capture point
 *      Armin - 11/08/23 - Changed colours from blue/red to green/purple and removed relativity to current team
 *      Armin - 11/08/23 - Support for AI
 *      Armin - 11/08/23 - Support for point moving around
 *      Armin - 11/08/23 - Now resets when re enabled
 *      Dean             - Also changes the colour of the marker above the point
 */
//*************************************************************************************************************

using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CapturePoint : NetworkBehaviour
{
    public NetworkVariable<int> ownedByTeamId = new NetworkVariable<int>(0);
    public NetworkVariable<int> beingCapturedByTeamId = new NetworkVariable<int>(0);
    public NetworkVariable<bool> beingContested = new NetworkVariable<bool>(false);
    public static readonly float CaptureTime = 3f;
    public NetworkVariable<float> captureProgress = new NetworkVariable<float>(0f);

    public Material defaultMaterial;
    public Material teamOneMaterial;
    public Material teamTwoMaterial;
    
    private MeshRenderer _meshRenderer;
    private List<TeamMember> _teamMembersInArea;

    // Added an object marker to the point which changes colours depeneding on the team that holds it (Dean)
    private ObjMarker _objMarker;

    void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _objMarker = GetComponentInChildren<ObjMarker>();
    }

    private void OnEnable()
    {
        ownedByTeamId.Value = 0;
        beingCapturedByTeamId.Value = 0;
        beingContested.Value = false;
        captureProgress.Value = 0;
        _teamMembersInArea = new List<TeamMember>();
    }

    void Update()
    {
        // Return if game hasnt started
        if(GameMode.Instance) if (!GameMode.Instance.thisClientsPlayer) return;

        _UpdateMaterial();
        
        if (!IsServer) return;
        _UpdateCaptureProgress();
    }

    private void _UpdateCaptureProgress()
    {
        // If point is doing nothing
        if (beingCapturedByTeamId.Value == 0) return;
        if (beingContested.Value) return;
        
        // Add capture progress
        captureProgress.Value += Time.deltaTime;
        // If point has been captured this frame
        if (captureProgress.Value >= CaptureTime)
        {
            ownedByTeamId.Value = beingCapturedByTeamId.Value;
            beingCapturedByTeamId.Value = 0;
            captureProgress.Value = 0;
        }
    }

    // Called every frame
    private void _UpdateMaterial()
    {
        // Prevent server crashing if run without host
        if (!IsClient) return;
        
        if (ownedByTeamId.Value == 1)
        {
            _meshRenderer.material = teamOneMaterial;
            _objMarker.ChangeColour("purple");
            Globe.Instance.SetTeam(1);
        }
        else if (ownedByTeamId.Value == 2)
        {
            _meshRenderer.material = teamTwoMaterial;
            _objMarker.ChangeColour("green");
            Globe.Instance.SetTeam(2);
        }
        if (ownedByTeamId.Value == 0 || beingCapturedByTeamId.Value != 0)
        {
            _meshRenderer.material = defaultMaterial;
            _objMarker.ChangeColour("white");
            Globe.Instance.SetTeam(0);
        }
    }
    
    // Called whenever a player enters or leaves the point
    private void _UpdatePossession()
    {
        // Adds teams that are in the point to the list
        List<int> teamsInPoint = new List<int>();
        foreach (var teamMember in _teamMembersInArea)
        {
            if (!teamsInPoint.Contains(teamMember.teamId.Value))
            {
                teamsInPoint.Add(teamMember.teamId.Value);
            }
        }

        if (teamsInPoint.Count > 1)
        {
            beingContested.Value = true;
        }
        else
        {
            beingContested.Value = false;
        }

        // If only 1 team in point and they are not the ones capturing
        if (teamsInPoint.Count == 1 && ownedByTeamId.Value != teamsInPoint[0])
        {
            beingCapturedByTeamId.Value = teamsInPoint[0];
        }
        
        // If no one in the point
        if (teamsInPoint.Count == 0)
        {
            beingCapturedByTeamId.Value = 0;
            captureProgress.Value = 0f;
        }
    }

    public void RemovePlayerFromAreaList(TeamMember teamMember)
    {
        _teamMembersInArea.Remove(teamMember);
        _UpdatePossession();
    }
    
    // When something collides with the point
    private void OnTriggerEnter(Collider other)
    {
        print($"IsServer ontriggerenter?: {IsServer}");
        // This is a server side method
        if (!IsServer || !other.CompareTag("CanCapturePoint")) return;
        print("gothere2");

        TeamMember teamMember = other.transform.GetComponentInParent<Player>() ? 
            other.transform.GetComponentInParent<Player>() : other.transform.GetComponentInParent<NetworkAI>();
        _teamMembersInArea.Add(teamMember);
        _UpdatePossession();
    }
    private void OnTriggerExit(Collider other)
    {
        // This is a server side method
        if (!IsServer || !other.CompareTag("CanCapturePoint")) return;

        TeamMember teamMember = other.transform.GetComponentInParent<Player>() ? 
            other.transform.GetComponentInParent<Player>() : other.transform.GetComponentInParent<NetworkAI>();

        if (_teamMembersInArea.Contains(teamMember)) _teamMembersInArea.Remove(teamMember);

        _UpdatePossession();
    }
}
