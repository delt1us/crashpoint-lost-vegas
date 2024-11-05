using UnityEngine;

[CreateAssetMenu(fileName = "NewGroundContainer", menuName = "Scriptable Objects/Audio/Ground SFX Container")]
public class GroundSfxContainer : ScriptableObject
{
    public string _name;

    public AudioClip Driving;
    public AudioClip Skidding;

    private void OnEnable()
    {
        _name = name;
    }
}
