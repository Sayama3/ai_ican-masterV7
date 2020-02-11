using UnityEngine;

[CreateAssetMenu(fileName = "RocketSettings", menuName = "AI_API/New RocketSettings")]
public class RocketSettings : ScriptableObject
{
    
    [SerializeField] private int rockePoolNumber = 40;
    public int RockePoolNumber => this.rockePoolNumber;
    
    [SerializeField] private float rocketSpeed = 100;
    public float RocketSpeed => this.rocketSpeed;
    
    [SerializeField] private int rocketDamage = 5;
    public int RocketDamage => this.rocketDamage;
    
    [SerializeField] private float rocketBlastRadius = 5;
    public float RocketBlastRadius => this.rocketBlastRadius;
    
    [SerializeField] private GameObject rocketPrefab;
    public GameObject RocketPrefab => this.rocketPrefab;
}