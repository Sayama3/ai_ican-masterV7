////////////////////////////////
/// Creation des bots
/// Creation des teams
/// Tire de rocket
/// Respawn des bots
/// Vision des bots
/// script qui gère la capture des drapeaux, 
/// le respawn des bots
///
///
/// To refactor :
///   - team + bot behaviour injection
///
///
/// 
////////////////////////////////

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;



public class GameMaster : MonoBehaviour
{
  public const int HASH_MULTIPLICATOR = 100;
  
  [SerializeField]
  private GameSettings gameSettings;
  [SerializeField]
  private TeamSettings teamSettings;
 
  
  // rockets pool to avoid instantiation in real time
  [SerializeField]
  private RocketSettings rocketSettings;
  
  private RocketPool rockets;
  public RocketPool Rockets => this.rockets;
  

  public bool finished = false;
  public bool gameInprogress = false;
  
  float time;


  private Team[] teams;
  public Bot[] bots;
  private Flag[] flags;
  
  // respawn loop

  public List<RespawnData> respawnList = new List<RespawnData>();
  
 
  // données de paramétrage de la partie
  public BattleGroundKeyPlaces[] teamPlaces;
  public Color[] team_color;
  public string[] layers;


  List<RespawnData> to_respawn = new List<RespawnData>();
  // delai de respawn quand un bot a été éliminé
  float respawn_delay = 6;
  float respawn_invincibility_time = 1.5f;
  int max_hp = 2;


  // variables remplies par le GameMaster
  // donant les informations sur l'état de la partie
  // (drapeaux, porteurs de drapeaux, score)
  public bool[] is_flag_home = new bool[2];
  public int[] flag_carriers = new int[2];
  public int[] score = new int[2];

  private RoundMaster master;
  public float TimeLeft => this.master.TimeLeft;
  public float TimeSinceStart => this.master.TimeSinceStart;
  public int RoundTurn => this.master.TurnNumber;

  
  public float FlagSpawnTime => this.gameSettings.FlagSpawnTimer;
  public float RocketRadius => this.rocketSettings.RocketBlastRadius;
  public float RocketDamage => this.rocketSettings.RocketDamage;
  public int GetTeamRoundScore(int teamId)
  {
    return this.master.GetScore(teamId);
  }
  public int GetTeamMatchScore(int teamId)
  {
    return this.master.GetMatchScore(teamId);
  }

  //Create bots
  public bool Init(string[] teamNames, RoundMaster round)
  {
    this.master = round;
    
    // create rockets
    this.rockets = new RocketPool();
    this.rockets.InitPool(this.rocketSettings.RockePoolNumber, this.rocketSettings.RocketPrefab);

    // create teams
    this.teams = new Team[teamNames.Length];
    for (int i = 0; i < this.teams.Length; i++)
    {
      GameObject team_root = Instantiate(this.gameSettings.Teamprefab, Vector3.zero, Quaternion.identity);
      team_root.name = "TEAM_" + teamNames[i];

      this.teams[i] = team_root.GetComponent<Team>();
      this.teams[i].team_color = team_color[i];
      this.teams[i].layer = LayerMask.NameToLayer(layers[i]);
    }
    
    for (int i = 0; i < this.teams.Length; i++)
    {
      this.teams[i].Init(this,i);
      this.teams[i].SetPositions(this.teamPlaces[i]);
    }


    // create bots
    bots = new Bot[this.teamSettings.BotNumber * 2];
    for (int i = 0; i < this.teams.Length; i++)
    {
      teams[i].bots = new Bot[this.teamSettings.BotNumber];
      for (int j = 0; j < this.teamSettings.BotNumber; j++)
      {
        GameObject bot_go = (GameObject) GameObject.Instantiate(this.gameSettings.BotPrefab, this.teams[i].Places.GetPlacePosition(KeyPlaces.SPAWN),
          Quaternion.identity);
        bot_go.GetComponent<Renderer>().material.color = team_color[i];
        bot_go.layer = teams[i].layer;
        bot_go.transform.parent = this.teams[i].transform;
        bot_go.name = "bot-" + j + "_team-" + i;

        Bot bot_infos = bot_go.GetComponent<Bot>();
        bot_infos.Init(j * HASH_MULTIPLICATOR + i, this);


        teams[i].bots[j] = bot_infos;
        bots[i * this.teamSettings.BotNumber + j] = bot_infos;
      }

    }

    // register behaviour
    for (int i = 0; i < this.teams.Length; i++)
    {

      // team behaviour
      string team_script = FindScriptContainingWords(teamNames[i], "team");
      if (team_script == "")
      {
        return false;
      }

      this.teams[i].gameObject.AddComponent(System.Type.GetType(team_script));
      var teamBehaviour = this.teams[i].GetComponent<TeamBehaviour>();
      this.teams[i].RegisterBehaviour(teamBehaviour.Init, teamBehaviour);

      // bots behaviour
      string bot_script = FindScriptContainingWords(teamNames[i], "bot");
      if (bot_script == "")
      {
        return false;
      }
      for (int j = 0; j < this.teams[i].bots.Length; j++)
      {
        this.teams[i].bots[j].gameObject.AddComponent(System.Type.GetType(bot_script));
        BotBehaviour behaviour = this.teams[i].bots[j].GetComponent<BotBehaviour>();
        this.teams[i].bots[j].RegisterBehaviour(behaviour.Init, behaviour);
      }

      teamBehaviour.RegisterBot(this.teams[i].bots);
    }

    // create flags
    this.flags = new Flag[2];
    for (int i = 0; i < this.teams.Length; i++)
    {
        var index = i;
        GameObject flagGo = Instantiate(this.gameSettings.FlagPrefab) as GameObject;
        flags[i] = flagGo.GetComponent<Flag>();
        flags[i].name = "flag_" + i;
        flags[i].tag = "Flag";
        flags[i].GetComponent<Renderer>().material.color = team_color[i];
        this.teams[index].SetFlag(flags[index]);
        flags[i].Init(this,this.teams[i]);
        flags[i].Disable();
    }
    
    respawnList = new List<RespawnData>();
    
    return true;
  }

// Tool for finding a script containing 2 given strings
string FindScriptContainingWords(string team_name, string script_role)
{
  string search_folder = Application.dataPath+"/Scripts/Teams/";
  // we don't care about case so let's convert all to lower
  team_name = team_name.ToLower();
  script_role = script_role.ToLower();
  // get all files in that folder
  
  string[] files = System.IO.Directory.GetFiles(search_folder);
  foreach(string file in files)
  {
    // now we have the file but we only need the name
    // we here assume the end is in the form name.extension
    string[] parts = file.Split('/');
    string name_wo_ext = parts[parts.Length-1].Split('.')[0];

    string lowered_filename = name_wo_ext.ToLower();
    if(!lowered_filename.Contains(team_name)) continue;
    if(!lowered_filename.Contains(script_role)) continue;

    return name_wo_ext;
  }
  return "";
}



  ///////////////////////////
  /// CODE DU GAME MASTER ///
  ///////////////////////////


  public void StartGame() {
    
    //init flag spawn timer
    
    foreach (var team in this.teams) {
      team.StartGame();
    }

    StartCoroutine(SpawnFlag());

    this.gameInprogress = true;
  }

  public void PauseGame() {
    this.gameInprogress = false;
  }
  
  public void ResumeGame() {
    this.gameInprogress = true;
  }

  public void StopGame()
  {
    Debug.Log("Stopping game");
//    for(int i = 0; i < every_bots.Count; i++)
//    {
//      Destroy(every_bots[i].gameObject);
//    }
//    every_bots.Clear();

    Destroy(teams[0].gameObject);
    Destroy(teams[1].gameObject);
    Destroy(flags[0]);
    Destroy(flags[1]);

    to_respawn.Clear();
    this.gameInprogress = false;
  }

  

  const string TIME_LIMIT = "TIME ELAPSED";
  const string LOOP = "IN LOOP";
  const string WIN = "WIN";


  public void ShootRocketInDirection(int botId, Vector3 direction)
  {
    var rocket = this.rockets.GetAvailableRocket();
    rocket.Launch(GetBotFromBotId(botId),direction.normalized);
  }
  
  
  public void OnBotDestroyed(int botId)
  {
    Bot bot = GetBotFromBotId(botId);
    var d = new RespawnData();
    d.botId = bot.ID;
    d.deathTime = Time.time;
    this.respawnList.Add(d);
    if (GetEnemyTeamFromBotId(botId).flag.Stolen && GetEnemyTeamFromBotId(botId).flag.Carrier == botId)
    {
      DropFlag(botId);
    }
    bot.Disable();
    
    
//    if(bot_infos.ID == flag_carriers[0])
//    {
//      flag_carriers[0] = -1;
//    }
//    if(bot_infos.ID == flag_carriers[1])
//    {
//      flag_carriers[1] = -1;
//    }
//
//    bot_infos.is_dead = true;
//    bot_infos.BroadcastMessage("OnDeath", SendMessageOptions.DontRequireReceiver);
//
//    RespawnData rd = new RespawnData();
//    rd.timer = respawn_delay;
//    rd.bot = bot_infos;
//    rd.invincibility_left = respawn_invincibility_time;
//    rd.respawned = false;
//    to_respawn.Add(rd);
//
//    // give some invincibility by changing object's tag
//    rd.bot.gameObject.tag = "Untagged";
//    bot_infos.Disable();
  }

  void Respawn(Bot bot)
  {
  //  bot.transform.position = respawn_points[bot.team_ID].position;
    bot.Spawn(GetBotTeamFromBotId(bot.ID).Places.GetPlacePosition(KeyPlaces.SPAWN));
  }


	void Update () 
  {
    if(!this.gameInprogress) return;
    
    //update respawn
    var respawnID = new List<int>();
    foreach (var bot in this.respawnList)
    {
      if ((Time.time - bot.deathTime) >= this.gameSettings.RespawnTime)
      {
        respawnID.Add(bot.botId);
      }
    }

    foreach (var id in respawnID)
    {
      for (int i = 0; i < this.respawnList.Count; i++)
      {
        if (id == this.respawnList[i].botId)
        {
          Respawn(GetBotFromBotId(this.respawnList[i].botId));
          this.respawnList.RemoveAt(i);
          break;
        }
      }
    }
    
    //Update bot vision
    foreach (var bot in this.bots) {
      if(!bot.is_dead)
      {
        bot.ProcessEnemyVision(GetEnemyTeamFromBotId(bot.ID).bots);
        bot.ProcessAlliedVision(GetBotTeamFromBotId(bot.ID).bots);
        bot.ProcessRocketVision(this.rockets.GetAliveRocket());
        bot.ProcessFlagVision(GetBotTeamFromBotId(bot.ID).flag.gameObject,
          GetEnemyTeamFromBotId(bot.ID).flag.gameObject);
      }
    }
    
    //update flags statut
    
    /*
    

   
    time_since_last_score += Time.deltaTime;

    // CHECK PLAYERS TO RESPAWN
    int k = 0;
    while(k < to_respawn.Count)
    {
      RespawnData dat = to_respawn[k];
      if(dat.respawned)
      { 
        dat.invincibility_left -= Time.deltaTime;
        if(dat.invincibility_left <= 0)
        {
          // re-enable collider
          dat.bot.gameObject.tag = "Bot";
          to_respawn.RemoveAt(k);
          // don't move iterator
          continue;
        }
      }
      else
      {
        dat.timer -= Time.deltaTime;
        if(dat.timer < 0)
        {
          Respawn(dat.bot);
          dat.respawned = true;
        }
      }
      k++;*/

	}
  
  ///////////////////////////
  ///     Game mecanics
  ///////////////////////////
 

  public void ScorePoint(int botId) {
    var team = GetBotTeamFromBotId(botId);
    var enemyTeam = GetEnemyTeamFromBotId(botId);
    GetBotFromBotId(botId).hasFlag = false;
    foreach (var t in this.teams)
    {
      t.Behaviour.OnTeamScored(GetBotTeamFromBotId(botId));
    }
    if (team.flag.Stolen == false && enemyTeam.flag.Carrier == botId)
    {
      var roundMaster = FindObjectOfType<RoundMaster>();
      roundMaster.TeamScores(team.Id);
      team.flag.Disable();
      enemyTeam.flag.Disable();
      StartCoroutine(SpawnFlag());
    }
  }

  public void DropFlag(int botId) {
    GetEnemyTeamFromBotId(botId).flag.Drop();
    GetBotFromBotId(botId).hasFlag = false;
    foreach (var t in this.teams)
    {
      t.Behaviour.OnFlagDropped(GetEnemyTeamFromBotId(botId));
    }
  }
  
  public void StealFlag(int botId)
  {
    var bot = GetBotFromBotId(botId);
    var flag = GetEnemyTeamFromBotId(botId).flag;
    flag.transform.parent = bot.transform;
    flag.Steal(botId);
    bot.hasFlag = true;
    foreach (var team in this.teams)
    {
      team.Behaviour.OnFlagStolen(GetBotTeamFromBotId(botId));
    }
  }

  public void SaveFlag(int botId) {
    GetBotTeamFromBotId(botId).flag.Spawn();
    foreach (var team in this.teams)
    {
      team.Behaviour.OnFlagSaved(GetBotTeamFromBotId(botId));
    }
  }
  
  public void ShootRocket() {
    
  }

  public IEnumerator SpawnFlag()
  {
    yield return new WaitForSeconds(this.gameSettings.FlagSpawnTimer);
    foreach (var team in this.teams)
    {
      team.flag.Spawn();
    }
  }


  #region tools


  public GameObject GetBotGameObjectFromBotID(int botId)
  {
    return GetBotFromBotId(botId).gameObject;
  }

  public Bot GetBotFromBotId(int botId)
  {
    var index = ((botId - botId % HASH_MULTIPLICATOR) / HASH_MULTIPLICATOR) + (this.teamSettings.BotNumber * (botId % HASH_MULTIPLICATOR));
    return this.bots[index];
  }

  public int GetBotTeamIdFromBotId(int botId)
  {
    var teamId = botId % HASH_MULTIPLICATOR;
    return teamId;
  }
  
  public int GetEnemyTeamIdFromBotId(int botId)
  {
    var teamId = botId % HASH_MULTIPLICATOR;
    return teamId == 1 ? 0 : 1;
  }
  

  public Team GetBotTeamFromBotId(int botId)
  {
    return this.teams[this.GetBotTeamIdFromBotId(botId)];
  }

  public Team GetTeamFromTeamId(int teamId)
  {
    return this.teams[teamId];
  }
  
  public Team GetEnemyTeamFromBotId(int botId) 
  {
    return this.GetEnemyTeamFromTeamId(this.GetBotTeamIdFromBotId(botId));
  }
  
  public Team GetEnemyTeamFromTeamId(int teamId)
  {
    for (int i = 0; i < this.teams.Length; i++)
    {
      if (i != teamId)
      {
        return  this.teams[i];
      }
    }
    return null;
  }
  
  public int GetEnemyTeamIdFromTeamId(int teamId)
  {
    return teamId == 1 ? 0 : 1;
  }
  
  #endregion


  public void EndGame()
  {
    Helper.GetInstance().ClearHelper();
    foreach (var bot in this.bots)
    {
      Destroy(bot.gameObject);
    }

    this.bots = null;
    foreach (var team in this.teams)
    {
      Destroy(team.gameObject);
    }

    this.teams = null;
    foreach (var flag in this.flags)
    {
      Destroy(flag.gameObject);
    }

    this.flags = null;
    this.rockets.Clear();
    
    
    
  }
}
