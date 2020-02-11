using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine.AI;

// - ajouter fonction rotate
// + équilibrage
// - nom des teams à côté du score
// + clean API
// + tool pour les position relatives à la base
// - fonction HasEnemyFlag
// - augmenter collider flag

public class Bot : MonoBehaviour
{
  // V2
  public bool DebugLine = true;
  
  private int id;
  public int ID => this.id;
  
  [SerializeField]
  private BotSettings settings;
  private GameMaster master;
  private BotBehaviour behaviour;
  public NavMeshAgent agent;
  public Collider botCollider;
  public Renderer botRenderer;
  
  private int health;
  public int Health => this.health;

  public bool hasFlag = false;

  public List<GameObject> visibleEnemyBots = new List<GameObject>();
  public List<GameObject> visibleAlliedBots = new List<GameObject>();
  public List<GameObject> visibleRockets = new List<GameObject>();
  public bool flagVisible;
  public bool enemyFlagVisible;
  
 
  void Awake()
  {
    this.botCollider = this.GetComponent<Collider>();
    this.botRenderer = this.GetComponent<Renderer>();
    this.agent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
  }
  
  public void Init(int id,  GameMaster gameMaster) {
    this.master = gameMaster;
    this.health = this.settings.Health;
    this.id = id;
    this.hasFlag = false;
  }

  public void Spawn(Vector3 pos)
  {
    this.health = this.settings.Health;
    this.is_dead = false;
    this.can_shoot = true;
    this.transform.position = pos;
    this.gameObject.SetActive(true);
    this.behaviour.OnRespawn();
  }
  
  public void Disable()
  {
    this.is_dead = true;
    this.gameObject.SetActive(false);
  }
  
  public void ShootInDirection(Vector3 dir)
  {
    if (this.can_shoot)
    {
      this.can_shoot = false;
      master.ShootRocketInDirection(this.id,dir);
      StartCoroutine(Reload());
    }
  }

  public void ShootAtObject(GameObject obj)
  {
    ShootInDirection(obj.transform.position - this.transform.position);
  }
  
  
  
  // V1

  // l'avatar est-il en cooldown ? (ne pas modifier)
  public bool can_shoot = true; 
  // permet de savoir si on est mort (géré par le game master)
  public bool is_dead = false;
  // permet de gérer l'orientation du bot soi-même, en écrasant celle du navmesh


  public bool CanSeeObject(GameObject obj)
  {
    //Direction dans laquelle se trouve l'objet
    Vector3 dir_to_obj = obj.transform.position - transform.position;
    if (Vector3.Angle(transform.forward, dir_to_obj) <= this.settings.PlayerVisionAngle)
    {
      int layer_mask = 1 << LayerMask.NameToLayer("Walls");
      if (Physics.Linecast(transform.position, obj.transform.position, layer_mask))
      {
        return false;
      }
      else
      {
        return true;
      }
    }
    return false;
  }
  public void ProcessEnemyVision(Bot[] enemyBots)
  {
    // check enemy bots
    visibleEnemyBots.Clear();
    for (int i = 0; i < enemyBots.Length; i++)
    {
      if (!enemyBots[i].is_dead && CanSeeObject(enemyBots[i].gameObject))
      {
        this.visibleEnemyBots.Add(enemyBots[i].gameObject);
      }
    }
  }
  public void ProcessAlliedVision(Bot[] alliedBots)
   {
     // check enemy bots
     visibleAlliedBots.Clear();
     for (int i = 0; i < alliedBots.Length; i++)
     {
       if (!alliedBots[i].is_dead && alliedBots[i] != this && CanSeeObject(alliedBots[i].gameObject))
       {
         this.visibleAlliedBots.Add(alliedBots[i].gameObject);
       }
     }
   }
  public void ProcessRocketVision(GameObject[] rockets)
  {
    // check enemy bots
    this.visibleRockets.Clear();
    for (int i = 0; i < rockets.Length; i++)
    {
      if (CanSeeObject(rockets[i]))
      {
        this.visibleRockets.Add(rockets[i]);
      }
    }
  }
  public void ProcessFlagVision(GameObject flag,GameObject enemyFlag)
  {
    this.flagVisible = CanSeeObject(flag);
    this.enemyFlagVisible = CanSeeObject(enemyFlag);
  }

  // tire une roquette dans la direction donnée en paramètre
  // (la direction est dans le repère du monde)
  // La roquette n'est pas tirée si on est en cooldown
  // elle n'est pas tirée non plus si on essaye de tirer en dehors du cone de vision
	


  public void RotateTowardsPoint(Vector3 point)
  {
    // first make sure the point is on the same plane than our center
    point.y = transform.position.y;
    Vector3 towardsPoint = point - transform.position;
    RotateTowardsDirection(towardsPoint);
  }


  private Vector3 currentRotation;
  // tourne vers la direction (world) passée en paramètre
  // la direction donnée doit être un vecteur sur le plan X-Z
  public void RotateTowardsDirection(Vector3 dir)
  {
    this.currentRotation = transform.rotation.eulerAngles;
    this.agent.isStopped = true;
    this.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(currentRotation, dir, 360/ this.settings.SpeedRotation * Time.deltaTime, 1));
  }

  //0 to left, 1 to right
  public void Rotate(bool direction)
  {
    this.agent.isStopped = true;
    float factor = direction ? 1 : -1;
    this.transform.Rotate(Vector3.up, 360/this.settings.SpeedRotation * Time.deltaTime * factor);
    // first make sure rotation step is regular
   // rotationStep = Mathf.Min(rotationStep, agent.angularSpeed * Time.deltaTime);
   // orientation =  Quaternion.AngleAxis(rotationStep, Vector3.up) * orientation;
  }

  // Gestion du reload, Ne pas appeler vous-même
  public IEnumerator Reload()
  {
    yield return new WaitForSeconds(this.settings.PlayerShootCooldown);
    can_shoot = true;
  }

  public void TakeDamage(int damage)
  {
    this.health -= damage;
    this.behaviour.TakeDamage();
    if (this.health < 0)
    {
      this.behaviour.OnDeath();
      this.master.OnBotDestroyed(this.id);
    }
  }

  public void RegisterBehaviour(Action<GameMaster, Bot> registerAction, BotBehaviour behaviour) {
    this.behaviour = behaviour;
    registerAction.Invoke(this.master, this);
  }

  public void OnDrawGizmos()
  {
    if(this.DebugLine)
    {
      foreach (var bot in this.visibleEnemyBots)
      {
        Debug.DrawLine(this.transform.position, bot.transform.position, Color.red);
      }

      foreach (var bot in this.visibleAlliedBots)
      {
        Debug.DrawLine(this.transform.position, bot.transform.position, Color.green);
      }

      foreach (var bot in this.visibleRockets)
      {
        Debug.DrawLine(this.transform.position, bot.transform.position, Color.yellow);
      }

      if (this.flagVisible)
      {
        Debug.DrawLine(this.transform.position, this.master.GetBotTeamFromBotId(this.id).flag.transform.position,
          Color.blue);
      }

      if (this.enemyFlagVisible)
      {
        Debug.DrawLine(this.transform.position, this.master.GetEnemyTeamFromBotId(this.id).flag.transform.position,
          Color.magenta);
      }
    }
  }
}
