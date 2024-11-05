//*************************************************************************************************************
/*  Car Name Scriptable Object Info Data Manager
 *  A dictionary to enable cars to spawn given a name
 *  Cut content as I realised this was completely pointless
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *      Armin - unknown date - created and abandoned this idea
 */
//*************************************************************************************************************

using System;
using System.Collections.Generic;
using UnityEngine;

// This doesn't work how I want it to, so it is not being used
[Serializable]
public class CarNameAndScriptableObjectInfo
{
    public string name;
    public CarSelectDataScriptableObject carSelectDataScriptableObject;
}

// This is basically a dictionary
public class CarNameScriptableObjectInfoDataManager : MonoBehaviour
{
    [SerializeField] private List<CarNameAndScriptableObjectInfo> carNameAndScriptableObjectInfos;

    private bool TryGetInfo(string id, out CarNameAndScriptableObjectInfo value)
    {
        // Check each
        foreach (var info in carNameAndScriptableObjectInfos)
        {
            // If found
            if (info.name == id)
            {
                // Set value to the item sought after
                value = info;
                return true;
            }
        }

        value = default;
        return false;
    }

    public bool TryGetValue(string id, out CarNameAndScriptableObjectInfo value)
    {
        if (!TryGetInfo(id, out var info))
        {
            value = default;
            return false;
        }

        // can be null
        value = info;
        return true;
    }
}
