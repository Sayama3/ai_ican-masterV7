using UnityEngine;

[CreateAssetMenu(fileName = "TournamentSettings", menuName = "AI_API/New TournamentSettings")]
public class TournamentSettings : ScriptableObject
{
    [SerializeField]
    private string[] teamNames;
    public string[] TeamNames => this.teamNames;
    
    [SerializeField] private int matchNumber = 3;
    public int MatchNumber => this.matchNumber;
    
    
    [SerializeField] private bool automatized = false;
    public bool Automatized => this.automatized;
    
    [SerializeField] private float timeSpeed = 1.0f;
    public float TimeSpeed => this.timeSpeed;
    
    [SerializeField] private GameSettings gameSettings;
    public GameSettings GameSettings => this.gameSettings;
    
    [SerializeField] private BotSettings botSettings;
    public BotSettings BotSettings => this.botSettings;
    
    [SerializeField] private RocketSettings rocketSettings;
    public RocketSettings RocketSettings => this.rocketSettings;
    
    [SerializeField] private TeamSettings teamSettings;
    public TeamSettings TeamSettings => this.teamSettings;
    
}