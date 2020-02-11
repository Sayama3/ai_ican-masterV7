using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using System.Collections;

public abstract class BotBehaviour : MonoBehaviour {

  protected GameMaster master;
  protected Bot bot;
  protected int botId;
  protected Team botTeam;
  protected int teamId;
  protected Team enemyTeam;

  public virtual void Init(GameMaster master, Bot bot) {
    this.master = master;
    this.bot = bot;
    this.botId = this.bot.ID;
    this.botTeam = master.GetBotTeamFromBotId(this.botId);
    this.teamId = this.botTeam.Id;
    this.enemyTeam = master.GetEnemyTeamFromBotId(this.botId);
  }

  public virtual void SetDestination(Vector3 pos) {
    this.bot.agent.SetDestination(pos);
  }
  
  public virtual void OnRespawn()
  {
    //Debug.Log(this.botId + " just respawned !");
  }

  public virtual void TakeDamage()
  {
    //Debug.Log(this.botId + " I am attacked");
  }

  public virtual void OnDeath()
  {
    //Debug.Log(this.botId + " I am dead...");
  }
  
  
}
