using System;
using UnityEngine;
using System.Collections;



public class BotBehaviourLesur : BotBehaviour
{

    public float DistanceShow;
    
    // liste des states possibles pour ce comportement de bot
    public enum BotState{ 
        IDLE, RUN_TO_ALLIED_FLAG, RUN_TO_ENEMY_FLAG
    };
  
    // état actuel du bot
    public BotState state = BotState.IDLE;
    public TeamBehaviourLesur teamBehaviour;
    
    public override void Init(GameMaster master, Bot bot)
    {
        base.Init(master, bot);
        this.teamBehaviour = FindObjectOfType<TeamBehaviourLesur>();
    }
    
    void Update()
    {
        if(!bot.agent.pathPending)
        {
            UpdateState();
        }
    }
    
  
    // fonction appelée pour changer d'état
    public void SwitchState(BotState newState) {
        this.OnExitState();
        this.state = newState;
        this.OnEnterState();
    }

    protected void OnEnterState() {
        switch(state)
        {
            case BotState.RUN_TO_ALLIED_FLAG:
                 bot.agent.SetDestination(this.botTeam.Places.GetPlacePosition(KeyPlaces.FLAG));
                break;
            case BotState.IDLE:
                break;
            case BotState.RUN_TO_ENEMY_FLAG:
                bot.agent.SetDestination(this.enemyTeam.Places.GetPlacePosition(KeyPlaces.FLAG));
                break;

        }
    }

    protected void UpdateState() {
        switch (state) {
            case BotState.RUN_TO_ALLIED_FLAG:
            {
                if (this.bot.visibleEnemyBots.Count > 0)
                {
                    var direction = this.bot.visibleEnemyBots[0].transform.position - this.bot.transform.position;
                    this.bot.ShootInDirection(direction);
                }
                DistanceShow = bot.agent.remainingDistance;
                if(bot.agent.remainingDistance < 1f)
                {
                  SwitchState(BotState.RUN_TO_ENEMY_FLAG);
                }
            }
                break;
            case BotState.RUN_TO_ENEMY_FLAG: {
                if (this.bot.visibleEnemyBots.Count > 0)
                {
                    var direction = this.bot.visibleEnemyBots[0].transform.position - this.bot.transform.position;
                    this.bot.ShootInDirection(direction);
                }
                DistanceShow = bot.agent.remainingDistance;
                if(bot.agent.remainingDistance < 1f)
                {
                    SwitchState(BotState.RUN_TO_ALLIED_FLAG);
                }
            }
                break;
            case BotState.IDLE:

                break;
        }
    }

    protected void OnExitState() {
        switch(state)
        {
            case BotState.IDLE:
                break;

        }
    }
    
    private void OnValidate() {
        this.OnEnterState();
    }

    
}
