using UnityEngine;

[CreateAssetMenu(fileName = "RoundSettings", menuName = "AI_API/New RoundSettings")]
public class RoundSettings : ScriptableObject
{ 
  
    [Range(60.0f, 600.0f)] [Tooltip("Time limit of a match in seconds.")] [SerializeField]
    private float timeLimit = 300;
    public float TimeLimit => this.timeLimit;
 
    [Range(0, 10)] [SerializeField] private int scoreLimit = 3;
    public int ScoreLimit => this.scoreLimit;
      
}