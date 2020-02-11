using UnityEngine;

[CreateAssetMenu(fileName = "TeamSettings", menuName = "AI_API/New TeamSettings")]
public class TeamSettings : ScriptableObject
{
    [SerializeField] private int botNumber = 6;
    public int BotNumber => this.botNumber;
    
    
}