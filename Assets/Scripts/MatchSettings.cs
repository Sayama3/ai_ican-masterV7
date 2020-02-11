using UnityEngine;

[CreateAssetMenu(fileName = "MatchSettings", menuName = "AI_API/New MatchSettings")]
 public class MatchSettings : ScriptableObject
 { 
  
     [SerializeField] private int rounds = 3;
     public int Rounds => this.rounds;
     
     
     [SerializeField] private bool bestOf = false;
     public bool BestOf => this.bestOf;
      
 }