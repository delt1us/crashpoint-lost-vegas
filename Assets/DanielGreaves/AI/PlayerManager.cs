using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // Start is called before the first frame update
    #region Singleton

    public static PlayerManager Instance;
    void Awake()
    {
        Instance = this;
    }

    #endregion

    [Header("Main References")]
    public GameObject Player;
}
