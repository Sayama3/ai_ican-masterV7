using System;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine.Serialization;

public class Team : MonoBehaviour {

  
  // V2
  [SerializeField] private BattleGroundKeyPlaces places;
  public BattleGroundKeyPlaces Places => this.places;

  private TeamBehaviour behaviour;
  public TeamBehaviour Behaviour => this.behaviour;

  public Flag flag;
  
  
  // tableau contenant tous les bots de l'équippe
  public Bot[] bots;

  [SerializeField]
  private int id;
  public int Id => this.id;
  
  [SerializeField]
  private int enemyTeamId;
  public int EnemyTeamId => enemyTeamId;

  private Team enemyTeam;
  public Team EnemyTeam => this.enemyTeam;

  // couleur de l'équippe (rempli par le GameMaster)
  public Color team_color = Color.white;
  // layer utilisée pour les drapeaux de cette équippe
  public int layer;

  public int RoundScore => this.master.GetTeamRoundScore(this.id);
  public int MatchScore => this.master.GetTeamMatchScore(this.id);

  public void Init(GameMaster master, int id)
  {
    this.id = id;
    this.enemyTeamId = master.GetEnemyTeamIdFromTeamId(this.Id);
    this.enemyTeam = master.GetEnemyTeamFromTeamId(this.Id);
  }
  
  GameMaster master;
  void Awake()
  {
    master = FindObjectOfType<GameMaster>();
  }

//  void OnMatchStart()
//  {
//    print("I start the match first !");
//    for (int i = 0; i < bots.Length; i++)
//    {
//      bots[i].enemyTeam = enemy_team;
//    }
//  }

  // retourne le GameObject qui porte le drapeau de l'équipe
  // renvoie null si personne ne porte le drapeau
  // (le drapeau peut alors être à la base ou avoir été laché par le porteur s'il a été éliminé)
  public GameObject GetFlagCarrier()
  {
    int id = master.flag_carriers[Id];
    return master.GetBotGameObjectFromBotID(id);
  }

  public GameObject GetEnemyFlagCarrier()
  {
    int id = master.flag_carriers[EnemyTeamId];
    return master.GetBotGameObjectFromBotID(id);
  }
  
  // premet de savoir si on a le drapeau ennemi
  public bool HasEnemyFlag()
  {
    return EnemyTeam.GetFlagCarrier() != null;
  }

	// dit si le flag de l'équippe passée en paramètre
  // est dans sa base
  public bool IsTeamFlagHome()
  {
    return master.is_flag_home[Id];
  }

	// récupère le score de l'équippe passée en paramètre
  public int GetScore()
  {
    return master.score[Id];
  }




  // Cette fonction permet d'envoyer un message à tous les bots de l'équippe
  // Il suffit d'ajouter une fonction du type void NomFonction() dans 
  // le script de comportement des bots pour qu'elle soit appelée
  // exemple: SendMessageToTeam("SeenEnemyFlag"); 
  // permettra d'exécuter la fonction "SeenEnemyFlag" si elle est présente dans le script de comportement des bots
  public void SendMessageToTeam(string message)
  {
    BroadcastMessage(message, null, SendMessageOptions.DontRequireReceiver);
  }

  // Vector3 BaseRelativeToWorldPos(Vector3 positionRelativeToMyBase)
  // Permet d'exprimer des positions relatives à votre base (donc des positions 
  // qui vont fonctionner que vous soyez bleu ou rouge)
  // usage : prend en paramètre une position relative à la base de l'équipe (game object TEAM_FLAG_POS
  // et renvoie une position dans le repère du monde
  // exemple : 
  // GetWorldPosFromBaseRelativePos(new Vector3(0,0,0));
  // renvoie la position de la base
  // GetWorldPosFromBaseRelativePos(new Vector3(0,0,1));
  // renvoie une position qui se trouve à 1m de la base, vers le centre de l'arène (le transform de la base est dirigé vers le centre de l'arène)
  // GetWorldPosFromBaseRelativePos(new Vector3(1,0,0));
  // renvoie une position qui se trouve à 1m à droite de la base
//  public Vector3 GetWorldPosFromBasePos(Vector3 positionRelativeToMyBase)
//  {
//    return this.Places.GetPlacePosition(KeyPlaces.FLAG).TransformPoint(positionRelativeToMyBase);
//  }
//
//  // fonction qui permet de faire le contraire :
//  // à partir d'une position dans le repère du monde, renvoie une position équivalente 
//  // relative à la base de l'équipe
//  public Vector3 GetBaseRelativePosFromWorldPos(Vector3 worldPos)
//  {
//    // la position Y de la base de l'équipe étant fixe et le jeu se déroulant strictement sur le même plan, on doit s'assurer que le Y de la position donnée est le même que celui de la base
//    worldPos.y = team_base.position.y;
//    return team_base.InverseTransformPoint(worldPos);
//  }
  
  public void RegisterBehaviour(Action<GameMaster, Team> registerAction, TeamBehaviour behaviour) {
    this.behaviour = behaviour;
    registerAction.Invoke(this.master, this);
  }

  public void StartGame() {
    this.behaviour.OnMatchStart();
  }

  public void SetBots(Bot bots)
  {
    
  }
  public void SetFlag(Flag flag)
  {
    this.flag = flag;
    this.flag.transform.position = this.places.GetPlacePosition(KeyPlaces.FLAG);
  }

  public void SetPositions(BattleGroundKeyPlaces teamPlaces)
  {
    this.places = teamPlaces;
  }

  public TeamSide TeamSide => this.id == 0 ? TeamSide.Red : TeamSide.Blue;

}
public enum TeamSide
{
  Red = 0,
  Blue = 1
}