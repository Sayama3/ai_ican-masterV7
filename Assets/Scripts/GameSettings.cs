using UnityEngine;


[CreateAssetMenu(fileName = "GameSettings", menuName = "AI_API/New GameSettings")]
 public class GameSettings : ScriptableObject
 {

     [SerializeField] private GameObject teamprefab;
     public GameObject Teamprefab => this.teamprefab;

     [SerializeField] private GameObject botPrefab;
     public GameObject BotPrefab => this.botPrefab;
     
     [SerializeField] private GameObject flagPrefab;
     public GameObject FlagPrefab => this.flagPrefab;

     [SerializeField] private float flagSpawnTimer;
     public float FlagSpawnTimer => this.flagSpawnTimer;

     [SerializeField] public float respawnTime;
     public float RespawnTime => this.respawnTime;
 }