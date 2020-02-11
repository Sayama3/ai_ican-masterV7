using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine.TestTools;


public class BotBehaviourSegui : BotBehaviour 
{
  
    public float DistanceShow;
    
    // liste des states possibles pour ce comportement de bot
    public enum BotState{ 
        IDLE, RUN_TO_RANDOM_ALLIED_POSITION,
        HUNT_FLAG_CARRIER, SAVE_FLAG, FIND_FLAG_CARRIER
    };
  
    // état actuel du bot
    public BotState state = BotState.IDLE;
    public TeamBehaviourSegui teamBehaviour;
    
    public override void Init(GameMaster master, Bot bot)
    {
        base.Init(master, bot);
        this.teamBehaviour = FindObjectOfType<TeamBehaviourSegui>();
    }
    
    void Update()
    {
        Debug.Log("The round turn on my bot is : " +this.master.RoundTurn);
        this.bot.RotateTowardsPoint(this.botTeam.flag.transform.position);
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
        
        Debug.Log("time since start : " +master.TimeSinceStart);
        
        switch(state)
        {
            case BotState.RUN_TO_RANDOM_ALLIED_POSITION:
                var rand = Random.Range(0, 7);
                switch (rand)
                {
                    case 0:
                        bot.agent.SetDestination(this.enemyTeam.Places.GetPlacePosition(KeyPlaces.FLAG));
                        break;
                    case 1 :
                        bot.agent.SetDestination(this.botTeam.Places.GetPlacePosition(KeyPlaces.FRONT));
                        break;
                    case 2 :
                        bot.agent.SetDestination(this.botTeam.Places.GetPlacePosition(KeyPlaces.PYLON));
                        break;
                    case 3 :
                        bot.agent.SetDestination(this.botTeam.Places.GetPlacePosition(KeyPlaces.SPAWN));
                        break;
                    case 4 :
                        bot.agent.SetDestination(this.botTeam.Places.GetPlacePosition(KeyPlaces.CAMPER));
                        break;
                    case 5 :
                        bot.agent.SetDestination(this.botTeam.Places.GetPlacePosition(KeyPlaces.CENTER));
                        break;
                    case 6 :
                        bot.agent.SetDestination(this.botTeam.Places.GetPlacePosition(KeyPlaces.POWER_UP));
                        break;
                }
                break;
            case BotState.SAVE_FLAG:
            {
                if (this.bot.flagVisible)
                {
                    bot.agent.SetDestination(this.botTeam.flag.transform.position);
                }
            }
                break;
            case BotState.HUNT_FLAG_CARRIER:
            {
                foreach (var bot in this.bot.visibleEnemyBots)
                {
                    if (bot.GetComponent<Bot>().ID == this.botTeam.flag.Carrier)
                    {
                        this.bot.agent.SetDestination(bot.transform.position);
                    }
                }
            }
                break;
            case BotState.FIND_FLAG_CARRIER:
            {
                this.bot.agent.SetDestination(this.teamBehaviour.LastFlagCarrierPos);
            }
                break;
                
            case BotState.IDLE:
                break;

        }
    }

    protected void UpdateState() {
        switch (state) {
            case BotState.RUN_TO_RANDOM_ALLIED_POSITION:
            {
                if (this.bot.visibleEnemyBots.Count > 0)
                {
                    if (this.botTeam.flag.Stolen)
                    {
                        foreach (var bot in this.bot.visibleEnemyBots)
                        {
                            if (bot.GetComponent<Bot>().ID == this.botTeam.flag.Carrier)
                            {
                                this.teamBehaviour.HuntFlagCarrier(bot.transform.position);
                            }
                        }
                    }
                }
                DistanceShow = bot.agent.remainingDistance;
                if(bot.agent.remainingDistance < 1f)
                {
                  SwitchState(BotState.IDLE);
                }
            }
                break;
            case BotState.SAVE_FLAG:
            {
                if (!this.botTeam.flag.AtBase)
                {
                    this.SwitchState(BotState.RUN_TO_RANDOM_ALLIED_POSITION);
                }
                if (!this.botTeam.flag.Stolen)
                {
                    this.SwitchState(BotState.HUNT_FLAG_CARRIER);
                }
            }
                break;
            case BotState.HUNT_FLAG_CARRIER:
            {
                var detected = false;
                foreach (var bot in this.bot.visibleEnemyBots)
                {
                    if (bot.GetComponent<Bot>().ID == this.botTeam.flag.Carrier)
                    {
                        this.bot.agent.SetDestination(bot.transform.position);
                        this.teamBehaviour.UpdateFlagCarrierLastPos(bot.transform.position);
                        var direction = bot.transform.position - this.bot.transform.position;
                        this.bot.ShootInDirection(direction);
                        detected = true;
                    }
                }

                if (detected == false)
                {
                    SwitchState(BotState.FIND_FLAG_CARRIER);
                }

                if (!this.botTeam.flag.Stolen && !this.botTeam.flag.AtBase)
                {
                    SwitchState(BotState.SAVE_FLAG);
                }

                if (this.botTeam.flag.AtBase)
                {
                    SwitchState(BotState.RUN_TO_RANDOM_ALLIED_POSITION);
                }
            }
                break;
            case BotState.FIND_FLAG_CARRIER:
            {
                foreach (var bot in this.bot.visibleEnemyBots)
                {
                    if (bot.GetComponent<Bot>().ID == this.botTeam.flag.Carrier)
                    {
                        SwitchState(BotState.HUNT_FLAG_CARRIER);
                    }
                }

                if (this.bot.agent.destination != this.teamBehaviour.LastFlagCarrierPos)
                {
                    this.bot.agent.SetDestination(this.teamBehaviour.LastFlagCarrierPos);
                }
                if (!this.botTeam.flag.Stolen && !this.botTeam.flag.AtBase)
                {
                    SwitchState(BotState.SAVE_FLAG);
                }

                if (this.botTeam.flag.AtBase)
                {
                    SwitchState(BotState.RUN_TO_RANDOM_ALLIED_POSITION);
                }
            }
                break;
            case BotState.IDLE:
                SwitchState(BotState.RUN_TO_RANDOM_ALLIED_POSITION);
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

    public override void OnRespawn()
    {
        SwitchState(BotState.IDLE);
    }

    private void OnValidate() {
        this.OnEnterState();
    }

    
}
