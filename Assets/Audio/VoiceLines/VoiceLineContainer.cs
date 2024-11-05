/**************************************************************************************************************
* Voiceline Container
* A scriptable object that stores the corresponding voice lines. can be made for each character. The container is assigned to the AudioManager.
*
* Created by Dean Atkinson-Walker 2023
*
***************************************************************************************************************/

using UnityEngine;

[CreateAssetMenu(fileName = "NewVoicelineContainer", menuName = "Scriptable Objects/Audio/Voiceline Container")]
public class VoiceLineContainer : ScriptableObject
{
    public string Name;

    [Space] 
    public AudioClip[] Attacks;
    public AudioClip EnemyKill;
    public AudioClip ReceiveDamage;

    private void OnEnable()
    {
        Name = name;
    }
}