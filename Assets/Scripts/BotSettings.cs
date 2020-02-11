using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "BotSettings", menuName = "AI_API/New BotSettings")]
public class BotSettings : ScriptableObject
{ [FormerlySerializedAs("playerHealth")] [SerializeField]
    private int health = 10;

    public int Health => this.health;
    [SerializeField]
    private float playerVisionAngle = 75;
    public float PlayerVisionAngle => this.playerVisionAngle;
    [SerializeField]
    private float playerShootCooldown = 10;

    private float speedRotation = 3;
    public float SpeedRotation => this.speedRotation;
    
    public float PlayerShootCooldown => this.playerShootCooldown;
}