//*************************************************************************************************************
/*  Globe
 *  A file to update the globe material
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *  
 */
//*************************************************************************************************************

using UnityEngine;

public class Globe : MonoBehaviour
{
    public static Globe Instance = null;
    
    [SerializeField] private MeshRenderer globe;
    [SerializeField] private Material neutralMaterial;
    [SerializeField] private Material teamOneMaterial;
    [SerializeField] private Material teamTwoMaterial;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void SetTeam(int teamId)
    {
        Material material = null;
        switch (teamId)
        {
            case 1:
                material = teamOneMaterial;
                break;
            case 2:
                material = teamTwoMaterial;
                break;
            default:
                material = neutralMaterial;
                break;
        }
        globe.material = material;
    }
}
