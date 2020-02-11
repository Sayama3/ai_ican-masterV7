using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotBehaviourLecamus : BotBehaviour
{
    public float DistanceShow;
    private Team[] teams = new Team[2];
    float TimerRotatePointBase = 1;
    float TimerRotatePointBase2 = 1;
    bool sensRotationPointBase;
    bool sensRotationPointBase2;
    bool startRotation;
    bool startRotation2;
    bool Power_Up_Respawned;
    bool PatrollerBot;
    bool GuardianBot;
    bool botLaner;
    bool TopLaner;
    float TimerLookAround;

    // liste des states possibles pour ce comportement de bot
    public enum BotState
    {
        IDLE, 
        PATROL,
        SHOOT,
        DODGE,
        LOOK_AROUND,
        GET_BOOST,
        GO_TO_BOT_CARRIER,
        GO_TO_BASE_CARRIER,
        GO_TO_FLAG,
        POINT_TOP,
        POINT_BOTTOM,
        POINT_BASE
    };

    // état actuel du bot
    public BotState state = BotState.IDLE;
    public TeamBehaviourLecamus teamBehaviour;
    private float TimerDodge =1;
    public override void Init(GameMaster master, Bot bot)
    {
        base.Init(master, bot);
        this.teamBehaviour = FindObjectOfType<TeamBehaviourLecamus>();
        teams = new Team[]{botTeam,enemyTeam};
    }

    void Update()
    {
        if (!bot.agent.pathPending)
        {
            UpdateState();
        }
        TimerDodge -= Time.deltaTime;
    }
    // fonction appelée pour changer d'état
    public void SwitchState(BotState newState)
    {
        this.OnExitState();
        this.state = newState;
        this.OnEnterState();
    }
    public override void OnRespawn() //Lorsque le bot respawn, il va en IDLE pour décider quoi faire
    {
        SwitchState(BotState.IDLE);
    }
    protected void OnEnterState()
    {
        switch (state)
        {
            case BotState.GO_TO_FLAG:
                bot.agent.SetDestination(enemyTeam.Places.GetPlacePosition(KeyPlaces.FLAG)); //base ennemie
                break;

            case BotState.GO_TO_BASE_CARRIER:
                bot.agent.SetDestination(botTeam.Places.GetPlacePosition(KeyPlaces.FLAG)); //base alliée
                break;

            case BotState.POINT_BOTTOM:
                bot.agent.SetDestination(botTeam.Places.GetPlacePosition(KeyPlaces.POWER_UP));
                botLaner = true;
                break;

            case BotState.POINT_TOP:
                bot.agent.SetDestination(botTeam.Places.GetPlacePosition(KeyPlaces.CAMPER));
                TopLaner = true;
                break;

            case BotState.POINT_BASE: // point plus avantageux avec une meilleur vision que le point de la base
                GuardianBot = true;
                if (botTeam.Id == 0)
                {
                    bot.agent.SetDestination(botTeam.Places.GetPlacePosition(KeyPlaces.FLAG) + new Vector3(9, 0, 11)); // il faut ajouter une rotation pour ne pas qu'ils regardent le mur
                }
                else
                {
                    bot.agent.SetDestination(botTeam.Places.GetPlacePosition(KeyPlaces.FLAG) + new Vector3(-9, 0, -11)); // il faut ajouter une rotation pour ne pas qu'ils regardent le mur
                }
                break;

            case BotState.PATROL:
                PatrollerBot = true;
                var rand = Random.Range(0, 7);
                switch (rand)
                {
                    case 0:
                        bot.agent.SetDestination(enemyTeam.Places.GetPlacePosition(KeyPlaces.FLAG));
                        break;
                    case 1:
                        bot.agent.SetDestination(enemyTeam.Places.GetPlacePosition(KeyPlaces.FRONT));
                        break;
                    case 2:
                        bot.agent.SetDestination(enemyTeam.Places.GetPlacePosition(KeyPlaces.PYLON));
                        break;
                    case 3:
                        bot.agent.SetDestination(enemyTeam.Places.GetPlacePosition(KeyPlaces.SPAWN));
                        break;
                    case 4:
                        bot.agent.SetDestination(enemyTeam.Places.GetPlacePosition(KeyPlaces.CAMPER));
                        break;
                    case 5:
                        bot.agent.SetDestination(enemyTeam.Places.GetPlacePosition(KeyPlaces.CENTER));
                        break;
                    case 6:
                        bot.agent.SetDestination(enemyTeam.Places.GetPlacePosition(KeyPlaces.POWER_UP));
                        break;
                }
                break;

            case BotState.GET_BOOST:
                //teamBehaviour.FindClosestBot(botTeam, botTeam.Places.GetPlacePosition(KeyPlaces.POWER_UP));  //la fonction ne reconnait pas de tableau ou de listes contenant les bots alliés
                bot.agent.SetDestination(botTeam.Places.GetPlacePosition(KeyPlaces.POWER_UP));
                break;
        }
    }
    protected void UpdateState()
    {
        switch (state)
        {
            case BotState.IDLE:
                SHOOT();
                DODGE();
                if (!enemyTeam.flag.Stolen)
                {
                    teamBehaviour.enemyFlagIsAtOurBase = false;
                }
                if (Power_Up_Respawned)
                {
                    teamBehaviour.FindClosestBot(botTeam.bots, enemyTeam.Places.GetPlacePosition(KeyPlaces.FLAG));
                    if (teamBehaviour.closestBot == bot)
                    {
                        SwitchState(BotState.GET_BOOST);
                    }
                }
                if (!enemyTeam.flag.Stolen && !botTeam.flag.Stolen)
                {
                    if (enemyTeam.flag.AtBase)
                    {
                        if (botTeam.bots.Length == enemyTeam.bots.Length+2 | master.TimeLeft <= 30)
                        {
                            SwitchState(BotState.GO_TO_FLAG);
                        }
                    }
                }
                if ((!botTeam.flag.Stolen && !botTeam.flag.AtBase) | (!enemyTeam.flag.Stolen && !enemyTeam.flag.AtBase))
                {
                    SwitchState(BotState.PATROL);
                }
                if (PatrollerBot && !botTeam.flag.Stolen)
                {
                    SwitchState(BotState.PATROL);
                }
                if (botTeam.flag.Stolen && !enemyTeam.flag.Stolen)
                {
                    SwitchState(BotState.GO_TO_FLAG);
                }
                if (GuardianBot)
                {
                    SwitchState(BotState.POINT_BASE);
                }
                if (TopLaner && !enemyTeam.flag.Stolen)
                {
                    SwitchState(BotState.POINT_TOP);
                }
                else if (TopLaner && enemyTeam.flag.Stolen)
                {
                    SwitchState(BotState.GO_TO_BOT_CARRIER);
                }
                if (botLaner && !enemyTeam.flag.Stolen)
                {
                    SwitchState(BotState.POINT_BOTTOM);
                }
                else if (botLaner && enemyTeam.flag.Stolen)
                {
                    SwitchState(BotState.GO_TO_BOT_CARRIER);
                }
                if (bot.hasFlag && !teamBehaviour.enemyFlagIsAtOurBase && enemyTeam.flag.Stolen)
                {
                    SwitchState(BotState.GO_TO_BASE_CARRIER);
                }
                if (botTeam.flag.Stolen && enemyTeam.flag.Stolen && teamBehaviour.enemyFlagIsAtOurBase)
                {
                    if (!bot.hasFlag && !GuardianBot)
                    {
                        SwitchState(BotState.GO_TO_FLAG);
                    }
                }
                if (enemyTeam.flag.Stolen && !bot.hasFlag && !teamBehaviour.enemyFlagIsAtOurBase)
                {
                    SwitchState(BotState.GO_TO_BOT_CARRIER);
                }
                if (bot.enemyFlagVisible && !enemyTeam.flag.Stolen)
                {
                    SwitchState(BotState.GO_TO_FLAG);
                }
                if (bot.flagVisible && !botTeam.flag.AtBase && !botTeam.flag.Stolen)
                {
                    SwitchState(BotState.GO_TO_FLAG);
                }
                if (bot.hasFlag && teamBehaviour.enemyFlagIsAtOurBase)
                {
                    SwitchState(BotState.LOOK_AROUND);
                }
                break;

            case BotState.GO_TO_FLAG:
                SHOOT();
                DODGE();
                if (bot.enemyFlagVisible && !enemyTeam.flag.Stolen)
                {
                    bot.agent.SetDestination(teamBehaviour.EnemyFlagPosition);
                }
                if (bot.flagVisible && !botTeam.flag.AtBase && !botTeam.flag.Stolen)
                {
                    bot.agent.SetDestination(teamBehaviour.OurFlagPosition);
                }
                bot.agent.SetDestination(enemyTeam.Places.GetPlacePosition(KeyPlaces.FLAG)); //base ennemie
                SwitchState(BotState.IDLE);
                break;
            
            case BotState.POINT_BASE: // je dis au bot de tourner entre deux angles définis pur ne pas qu'il tourne a 360° et regarde le mur pendant que le drapeau se fait voler
                SHOOT();
                if (botTeam.Id == 0)
                {
                    if (bot.agent.remainingDistance <= 1)
                    {
                        float angle = Vector3.Angle(transform.position, transform.forward);
                        if ((angle >= 119)&&(angle<=120))
                        {
                            startRotation = true;
                        }
                        if (startRotation)
                        {
                            Debug.Log("Red Start Rotation");

                            TimerRotatePointBase -= Time.deltaTime;
                            if (TimerRotatePointBase <= 0)
                            {
                                sensRotationPointBase = !sensRotationPointBase;
                                TimerRotatePointBase = 1;
                            }
                            bot.Rotate(sensRotationPointBase);
                        }
                        else
                        {
                            Debug.Log("Red Startof");
                            bot.Rotate(sensRotationPointBase);
                        }
                        
                    }
                }
                else if(botTeam.Id == 1)
                {
                    if (bot.agent.remainingDistance <= 1)
                    {
                        float angle2 = Vector3.Angle(transform.position, transform.forward);
                        if ((angle2 >= 119) && (angle2 <= 120))
                        {
                            startRotation2 = true;
                        }
                        if (startRotation2)
                        {
                            Debug.Log("Blue Start Rotation");
                            TimerRotatePointBase2 -= Time.deltaTime;
                            if (TimerRotatePointBase2 <= 0)
                            {
                                sensRotationPointBase2 = !sensRotationPointBase2;
                                TimerRotatePointBase2 = 1;
                            }
                            bot.Rotate(sensRotationPointBase2);
                        }
                        else
                        {
                            Debug.Log("Blue Startof");
                            bot.Rotate(sensRotationPointBase2);
                        }
                    }
                }
                break;

            case BotState.PATROL:
                SHOOT();
                DODGE();
                if (bot.agent.remainingDistance <=3)
                {
                    var rand = Random.Range(0, 7);
                    switch (rand)
                    {
                        case 0:
                            bot.agent.SetDestination(enemyTeam.Places.GetPlacePosition(KeyPlaces.FLAG));
                            break;
                        case 1:
                            bot.agent.SetDestination(enemyTeam.Places.GetPlacePosition(KeyPlaces.FRONT));
                            break;
                        case 2:
                            bot.agent.SetDestination(enemyTeam.Places.GetPlacePosition(KeyPlaces.PYLON));
                            break;
                        case 3:
                            bot.agent.SetDestination(enemyTeam.Places.GetPlacePosition(KeyPlaces.SPAWN));
                            break;
                        case 4:
                            bot.agent.SetDestination(enemyTeam.Places.GetPlacePosition(KeyPlaces.CAMPER));
                            break;
                        case 5:
                            bot.agent.SetDestination(enemyTeam.Places.GetPlacePosition(KeyPlaces.CENTER));
                            break;
                        case 6:
                            bot.agent.SetDestination(enemyTeam.Places.GetPlacePosition(KeyPlaces.POWER_UP));
                            break;
                    }
                    SwitchState(BotState.IDLE);
                }
                if (bot.enemyFlagVisible)
                {
                    teamBehaviour.EnemyFlagPosition = enemyTeam.flag.transform.position;
                }
                if (bot.flagVisible)
                {
                    teamBehaviour.OurFlagPosition = botTeam.flag.transform.position;
                }
                if (bot.hasFlag)
                {
                    SwitchState(BotState.IDLE);
                }
                break;

            case BotState.GO_TO_BASE_CARRIER:
                SHOOT();
                DODGE();
                bot.agent.SetDestination(botTeam.Places.GetPlacePosition(KeyPlaces.FLAG));
                if (bot.agent.remainingDistance <= 1)
                {
                    SwitchState(BotState.IDLE);
                    teamBehaviour.enemyFlagIsAtOurBase = true;
                }
                else if (bot.agent.remainingDistance >1 | enemyTeam.flag.AtBase)
                {
                    teamBehaviour.enemyFlagIsAtOurBase = false;
                }
                if (!bot.hasFlag)
                {
                    SwitchState(BotState.IDLE);
                }
                break;

            case BotState.GO_TO_BOT_CARRIER:
                SHOOT();
                DODGE();
                for (int i = 0; i < botTeam.bots.Length; i++)
                {
                    if (botTeam.bots[i].ID == enemyTeam.flag.Carrier)
                    {
                        bot.agent.SetDestination(botTeam.bots[i].transform.position);
                    }
                }
                if (teamBehaviour.enemyFlagIsAtOurBase)
                {
                    SwitchState(BotState.IDLE);
                }
                if (!enemyTeam.flag.Stolen)
                {
                    SwitchState(BotState.IDLE);
                }
                if (bot.hasFlag && !teamBehaviour.enemyFlagIsAtOurBase && enemyTeam.flag.Stolen)
                {
                    SwitchState(BotState.GO_TO_BASE_CARRIER);
                }
                
                break;

            case BotState.POINT_BOTTOM:
                SHOOT();
                DODGE();
                if (!enemyTeam.flag.Stolen)
                {
                    bot.agent.SetDestination(botTeam.Places.GetPlacePosition(KeyPlaces.POWER_UP));
                    if (bot.agent.remainingDistance <= 1)
                    {
                        SwitchState(BotState.LOOK_AROUND);
                    }
                }
                else
                {
                    SwitchState(BotState.IDLE);
                }
                break;

            case BotState.POINT_TOP:
                SHOOT();
                DODGE();
                if (!enemyTeam.flag.Stolen)
                {
                    bot.agent.SetDestination(botTeam.Places.GetPlacePosition(KeyPlaces.CAMPER));
                    if (bot.agent.remainingDistance <= 1)
                    {
                        SwitchState(BotState.LOOK_AROUND);
                    }
                }
                else
                {
                    SwitchState(BotState.IDLE);
                }
                break;

            case BotState.LOOK_AROUND:
                SHOOT();
                DODGE();
                if (bot.hasFlag)
                {
                    SwitchState(BotState.IDLE);
                }
                bot.Rotate(true);
                TimerLookAround += Time.deltaTime;
                if (bot.enemyFlagVisible)
                {
                    teamBehaviour.EnemyFlagPosition = enemyTeam.flag.transform.position;
                }
                if (bot.flagVisible)
                {
                    teamBehaviour.OurFlagPosition = botTeam.flag.transform.position;
                }
                if (TimerLookAround >= 3.33f)
                {
                    if (enemyTeam.flag.Stolen)
                    {
                        SwitchState(BotState.GO_TO_BOT_CARRIER);
                    }
                    else
                    {
                        SwitchState(BotState.IDLE);
                    }
                    TimerLookAround = 0;
                }
                break;
        }
    }
    protected void OnExitState()
    {
        switch (state)
        {
            case BotState.IDLE:
                break;
        }
    }
    private void OnValidate()
    {
        this.OnEnterState();
    }
    public void SHOOT()
    {
        //for (int i = 0; i < enemyTeam.bots.Length; i++)
        //{
        //    if (bot.CanSeeObject(enemyTeam.bots[i].gameObject) && bot.can_shoot)
        //    {
        //        if (botTeam.flag.Stolen && enemyTeam.bots[i].ID == botTeam.flag.Carrier)
        //        {
        //            bot.ShootAtObject(enemyTeam.bots[i].gameObject);
        //        }
        //        else
        //        {

        //            bot.ShootAtObject(enemyTeam.bots[i].gameObject);
        //        }
        //    }
        //}
    }
    public void DODGE()
    {
        //TimerDodge = 1;
        //for (int j = 0; j < bot.visibleRockets.Count; j++)
        //{
        //    if (bot.visibleRockets.Count >= 1)
        //    {
        //        if (bot.can_shoot)
        //        {
        //            bot.ShootInDirection(bot.visibleRockets[j].transform.position);
        //            bot.agent.SetDestination(bot.transform.position + bot.transform.right);
        //        }
        //        if (TimerDodge <= 0)
        //        {
        //            break;
        //        }
        //    }
        //}
    }
}