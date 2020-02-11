using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotBehaviourIannis : BotBehaviour
{
    //----------DEBUT GLOBALE----------
    private BotState previousState;
    private float distanceMinAvantDestination = 2f;
    bool isGoingToDestination = false;
    //private Vector3 ActualTarget = new Vector3();
    //----------FIN GLOBALE----------




    //----------DEBUT STATE ATTACK----------
    private string directionAttack = "none";
    int positionActuelAttack = -1;
    //----------FIN STATE ATTACK----------



    //----------DEBUT STATE GO_HOME----------
    private string directionHome = "none";
    int positionActuelHome = -1;
    //----------FIN STATE GO_HOME----------



    //----------DEBUT STATE SHOOT----------
    private GameObject targetShoot = null;
    //----------FIN STATE SHOOT----------
    public enum BotRole
    {
        Attack,
        Defend,
        Patrol
    }
    public enum BotState{ 
        IDLE,
        PATROL_ALL,
        PATROL_DEFEND,
        PATROL_ATTACK,
        GO_HOME,
        ATTACK,
        SHOOT,
        NONE
    };
  
    // état actuel du bot
    public BotState state = BotState.IDLE;
    public BotRole role = BotRole.Patrol;
    // remplacer TeamBehaviourIannis par le nom du script de votre team
    public TeamBehaviourIannis teamBehaviour;
    
    
    // ne pas modifier, seulement remplacer TeamBehaviourIannis par le nom du script de votre team
    public override void Init(GameMaster master, Bot bot)
    {
        base.Init(master, bot);
        this.teamBehaviour = FindObjectOfType<TeamBehaviourIannis>();
    }
  

    // ne pas modifier
    void Update()
    {
        if(!bot.agent.pathPending)
        {
            UpdateState();
        }
    }
    
  
    // fonction appelée pour changer d'état, ne pas modifier
    public void SwitchState(BotState newState) {
        this.OnExitState();
        //Debug.LogWarning(this.botId + " _ " + this.state);

        if (this.state != BotState.IDLE)
        {
            //Debug.LogWarning(this.state);
            previousState = this.state;
        }
        this.state = newState;
        this.OnEnterState();
    }

    // ajouter ici les actions à effectuer quand le bot change d'etat'
    protected void OnEnterState() {
        switch(state)
        {
            case BotState.SHOOT:
                {
                    if (targetShoot == null)
                    {
                        SwitchState(BotState.IDLE);
                    }
                    else
                    {
                        ShootInFeet(targetShoot);
                        SwitchState(BotState.IDLE);
                    }
                }
                break;
            case BotState.IDLE:
                {
                    //Debug.Log("Bot " + this.botId + " rentre en IDLE.");
                }
                break;
            case BotState.ATTACK:
                {
                    if (previousState != BotState.ATTACK && previousState != BotState.SHOOT)
                    {
                        isGoingToDestination = false;
                        int intTemp = Random.Range(0, 3);
                        switch (intTemp)
                        {
                            case 0:
                                {
                                    directionAttack = "gauche";
                                }
                                break;
                            case 1:
                                {
                                    directionAttack = "millieu";

                                }
                                break;
                            case 2:
                                {
                                    directionAttack = "droite";

                                }
                                break;
                            default:
                                break;
                        }
                    }
                    
                }
                break;
            case BotState.GO_HOME:
                {
                    if (previousState != BotState.GO_HOME && previousState != BotState.SHOOT)
                    {
                        isGoingToDestination = false;
                        int intTemp = Random.Range(0, 3);
                        switch (intTemp)
                        {
                            case 0:
                                {
                                    directionHome = "gauche";
                                }
                                break;
                            case 1:
                                {
                                    directionHome = "millieu";

                                }
                                break;
                            case 2:
                                {
                                    directionHome = "droite";

                                }
                                break;
                            default:
                                break;
                        }

                    }
                    positionActuelHome = TeamBehaviourIannis.positionDroite.Length - 1;

                }
                break;

        }
    }

    // ajouter ici les action a effecteur quand le bot est dans un etat
    protected void UpdateState() {

        if (bot.agent.remainingDistance <= distanceMinAvantDestination)
        {
            isGoingToDestination = false;
        }

        switch (state) {
            case BotState.PATROL_ALL:
                {
                    if (!isGoingToDestination)
                    {
                        Vector3 positionTemp = GoRandomPoint("all");
                        SetPosition(positionTemp);
                    }
                }
                break;
            case BotState.PATROL_DEFEND:
                {
                    if (!isGoingToDestination)
                    {
                        int randomPoint = Random.Range(0, TeamBehaviourIannis.basePosition.Length);
                        SetPosition(TeamBehaviourIannis.basePosition[randomPoint]);
                    }
                }
                break;
            case BotState.PATROL_ATTACK:
                {
                    if (!isGoingToDestination)
                    {
                        Vector3 positionTemp = GoRandomPoint("enemy");
                        SetPosition(positionTemp);
                    }
                }
                break;
            case BotState.GO_HOME:
                {
                    //int randomValue = Random.Range(0, 3); 
                    if (!isGoingToDestination)
                    {
                        SetDestination(botTeam.Places.GetPlacePosition(KeyPlaces.FLAG));
                        //Vector3 vectorTemp = new Vector3();

                        //switch (directionHome)
                        //{
                        //    case "gauche":
                        //        {
                                    
                        //            vectorTemp = GoToHome(TeamBehaviourIannis.positionGauche);
                        //        }
                        //        break;
                        //    case "millieu":
                        //        {

                        //            vectorTemp = GoToHome(TeamBehaviourIannis.positionMillieu);
                        //        }
                        //        break;
                        //    case "droite":
                        //        {
                        //            vectorTemp = GoToHome(TeamBehaviourIannis.positionDroite);
                        //        }
                        //        break;
                        //}
                        //Debug.LogWarning("Bot " + bot.ID + " va a " + vectorTemp);
                        //SetDestination(vectorTemp);
                    }


                    this.SwitchState(BotState.IDLE);
                }
                break;
            case BotState.SHOOT:
                {
                    if (bot.can_shoot)
                    {
                        ShootInFeet(targetShoot);
                        targetShoot = null;
                    }
                    SwitchState(BotState.IDLE);
                }
                break;
           case BotState.IDLE:
                {
                    //Debug.Log("Bot " + this.botId + " est en IDLE.");
                    switch (role)
                    {
                        case BotRole.Attack:
                            {
                                if (bot.hasFlag)
                                {
                                    SwitchState(BotState.GO_HOME);
                                }
                                else
                                {

                                    if (bot.visibleEnemyBots.Count > 0 && bot.can_shoot)
                                    {
                                        for (int i = 0; i < bot.visibleEnemyBots.Count; i++)
                                        {
                                            GameObject tempBot = bot.visibleEnemyBots[i];
                                            if (i == 0)
                                            {
                                                targetShoot = tempBot;
                                            }
                                            else
                                            {
                                                if ((tempBot.transform.position - transform.position).magnitude < (targetShoot.transform.position - transform.position).magnitude)
                                                {
                                                    targetShoot = tempBot;
                                                }
                                            }
                                        }
                                        SwitchState(BotState.SHOOT);
                                    }
                                    else if (!enemyTeam.flag.Stolen)
                                    {
                                        SwitchState(BotState.ATTACK);
                                    }
                                    else if (botTeam.flag.Stolen)
                                    {
                                        SwitchState(BotState.PATROL_ALL);
                                    }
                                    else
                                    {
                                        SwitchState(BotState.GO_HOME);
                                    }

                                }
                            }
                            break;
                        case BotRole.Defend:
                            {
                                if (bot.hasFlag)
                                {
                                    SwitchState(BotState.GO_HOME);
                                }
                                else
                                {
                                    if (bot.visibleEnemyBots.Count > 0 && bot.can_shoot)
                                    {
                                        for (int i = 0; i < bot.visibleEnemyBots.Count; i++)
                                        {
                                            GameObject tempBot = bot.visibleEnemyBots[i];
                                            if (i == 0)
                                            {
                                                targetShoot = tempBot;
                                            }
                                            else
                                            {
                                                if ((tempBot.transform.position - transform.position).magnitude < (targetShoot.transform.position - transform.position).magnitude)
                                                {
                                                    targetShoot = tempBot;
                                                }
                                            }
                                        }
                                        SwitchState(BotState.SHOOT);
                                    }
                                    else
                                    {
                                        if (botTeam.flag.Stolen)
                                        {
                                            SwitchState(BotState.PATROL_ATTACK);
                                        }
                                        else
                                        {
                                            SwitchState(BotState.PATROL_DEFEND);
                                        }
                                    }
                                }
                            }
                            break;
                        //case BotRole.Patrol:
                        //    break;
                        default:
                            role = BotRole.Attack;
                            break;
                    }
                }
                break;
            case BotState.ATTACK:
                {
                    //Debug.Log("Je suis bot " + this.botId + ", actuellemnet je vais vers " + directionAttack + " \r je vais vers quelque part : " + isGoingToDestination);
                    Vector3 objectifTemp = new Vector3(-1,-1,-1);
                    //Debug.Log(previousState);
                    //Debug.Log(positionActuelAttack);
                    if (!isGoingToDestination)
                    {
                        switch (directionAttack)
                        {
                            case "gauche":
                                {
                                    //Debug.Log("J'atteins le Switch");
                                    Vector3[] tableauxTemp = new Vector3[TeamBehaviourIannis.positionGauche.Length + 1];
                                    for (int i = 0; i < TeamBehaviourIannis.positionGauche.Length; i++)
                                    {
                                        tableauxTemp[i] = TeamBehaviourIannis.positionGauche[i];
                                    }
                                    tableauxTemp[tableauxTemp.Length - 1] = enemyTeam.Places.GetPlacePosition(KeyPlaces.FLAG);
                                    objectifTemp = GetDestinationAttack(tableauxTemp);
                                }
                                break;
                            case "millieu":
                                {
                                    objectifTemp = GetDestinationAttack(TeamBehaviourIannis.positionMillieu);

                                }
                                break;
                            case "droite":
                                {
                                    Vector3[] tableauxTemp = new Vector3[TeamBehaviourIannis.positionDroite.Length + 1];
                                    for (int i = 0; i < TeamBehaviourIannis.positionDroite.Length; i++)
                                    {
                                        tableauxTemp[i] = TeamBehaviourIannis.positionDroite[i];
                                    }
                                    tableauxTemp[tableauxTemp.Length - 1] = enemyTeam.Places.GetPlacePosition(KeyPlaces.FLAG);
                                    objectifTemp = GetDestinationAttack(tableauxTemp);
                                }
                                break;
                            default:
                                break;
                        }

                        if (objectifTemp != new Vector3(-1, -1, -1))
                        {
                            SetPosition(objectifTemp);
                        }
                    }
                    
                }
                this.SwitchState(BotState.IDLE);
                break;
        }
    }

    


    
    protected void OnExitState() {
        switch(state)
        {
            case BotState.SHOOT:
                {
                    targetShoot = null;
                }
                break;
            case BotState.IDLE:
                {
                    //Debug.Log("Bot " + this.botId + " sors de IDLE.");

                }
                break;

        }
    }

    public override void OnRespawn()
    {
        Debug.Log(" The bot " + this.botId + " respawns !");
        SwitchState(BotState.IDLE);
        positionActuelAttack = -1;
        positionActuelHome = -1;
        isGoingToDestination = false;
        previousState = BotState.NONE;
    }

    public override void TakeDamage()
    {
        Debug.Log(" The bot " + this.botId + " takes damage !");
    }

    public override void OnDeath()
    {
        Debug.Log(" The bot " + this.botId + " dies !");
    }


    //--------------------MES-FONCTIONS-A-MOI--------------------


    private Vector3 GetDestinationAttack(Vector3[] tableauChoisi)
    {
        Vector3 objectifTemp = new Vector3();

        if (previousState != BotState.ATTACK && previousState != BotState.SHOOT)
        {
            //Debug.Log("Je suis nouveaux en state attack, je suis le bot numero " + bot.ID);
            objectifTemp = NearestPlace(transform.position, tableauChoisi, true);
            positionActuelAttack = FindIndex(tableauChoisi, objectifTemp);

        }
        else
        {
            //Debug.Log("j'étais déjà en attack. Je suis le bot " + bot.ID + " et ma destination est d'ailleurs : " + positionActuelAttack);
            //Debug.Log("Je devrais allez vers la/le prochaine/prochain " + directionAttack);
            if (positionActuelAttack < 0)
            {
                objectifTemp = NearestPlace(transform.position, tableauChoisi, true);
                positionActuelAttack = FindIndex(tableauChoisi, objectifTemp);
            }
            else if (positionActuelAttack < tableauChoisi.Length - 1)
            {
                ++positionActuelAttack;
                objectifTemp = tableauChoisi[positionActuelAttack];
            }
            else
            {
                objectifTemp = NearestPlace(transform.position, tableauChoisi, true);
                positionActuelAttack = FindIndex(tableauChoisi, objectifTemp);
            }
        }
        //Debug.LogWarning("le tableau choisit est de la taille " + tableauChoisi.Length + ". le bot " + bot.ID + " se dirige vers " + objectifTemp + " au lieu de " + tableauChoisi[positionActuelAttack] + " ayant pour indice " + positionActuelAttack);
        return objectifTemp;
    }

    public Vector3 NearestPlace(Vector3 me, Vector3[] placeToSearch, bool setLaChose = false)
    {
        Vector3 nearestPosition = new Vector3();

        for (int i = 0; i < placeToSearch.Length; i++)
        {
            if (i == 0)
            {
                nearestPosition = placeToSearch[i];
                if (setLaChose)
                {
                    positionActuelAttack = i;
                }
            }
            else
            {
                if ((me - nearestPosition).magnitude >= (me - placeToSearch[i]).magnitude )
                {
                    nearestPosition = placeToSearch[i];
                    if (setLaChose)
                    {
                        positionActuelAttack = i;
                    }
                }
            }
        }

        return nearestPosition;
    }

    public void SetPosition(Vector3 position)
    {
        isGoingToDestination = true;
        bot.agent.SetDestination(position);
    }

    public void ShootInFeet(GameObject targetTemp)
    {
        if(targetTemp != null)
        {
            Vector3 targetPosition = targetTemp.transform.position;
            targetPosition = new Vector3(targetPosition.x, targetPosition.y - 1, targetPosition.z);
            Vector3 directionTemp = targetPosition - transform.position;
            bot.ShootInDirection(directionTemp);
        }
        else
        {
            Debug.Log("Il n'y a pas de cible sur qui tirer");
        }
        

    }

    private Vector3 GoRandomPoint(string camp) //fonction qui prends un point random allié, ennemie ou les deux et renvoie quel point a été choisi.
    {
        Vector3 positionChoose = new Vector3();
        
        if (camp == "allie")
        {
            int randomChoose = Random.Range(0, 7);

            switch (randomChoose)
            {
                case 0:
                    positionChoose = (botTeam.Places.GetPlacePosition(KeyPlaces.CAMPER));
                    break;
                case 1:
                    positionChoose = (botTeam.Places.GetPlacePosition(KeyPlaces.FLAG));
                    break;
                case 2:
                    positionChoose = (botTeam.Places.GetPlacePosition(KeyPlaces.FRONT));
                    break;
                case 3:
                    positionChoose = (botTeam.Places.GetPlacePosition(KeyPlaces.POWER_UP));
                    break;
                case 4:
                    positionChoose = (botTeam.Places.GetPlacePosition(KeyPlaces.PYLON));
                    break;
                case 5:
                    positionChoose = (botTeam.Places.GetPlacePosition(KeyPlaces.SPAWN));
                    break;
                case 6:
                    positionChoose = (botTeam.Places.GetPlacePosition(KeyPlaces.CENTER));
                    break;
                default:
                    positionChoose = (botTeam.Places.GetPlacePosition(KeyPlaces.SPAWN));
                    break;
            }
        }
        else if (camp == "enemy")
        {
            int randomChoose = Random.Range(0, 7);

            switch (randomChoose)
            {
                case 0:
                    positionChoose = (enemyTeam.Places.GetPlacePosition(KeyPlaces.CAMPER));
                    break;
                case 1:
                    positionChoose = (enemyTeam.Places.GetPlacePosition(KeyPlaces.FLAG));
                    break;
                case 2:
                    positionChoose = (enemyTeam.Places.GetPlacePosition(KeyPlaces.FRONT));
                    break;
                case 3:
                    positionChoose = (enemyTeam.Places.GetPlacePosition(KeyPlaces.POWER_UP));
                    break;
                case 4:
                    positionChoose = (enemyTeam.Places.GetPlacePosition(KeyPlaces.PYLON));
                    break;
                case 5:
                    positionChoose = (enemyTeam.Places.GetPlacePosition(KeyPlaces.SPAWN));
                    break;
                case 6:
                    positionChoose = (enemyTeam.Places.GetPlacePosition(KeyPlaces.CENTER));
                    break;
                default:
                    positionChoose = (enemyTeam.Places.GetPlacePosition(KeyPlaces.SPAWN));
                    break;
            }
        }
        else
        {
            int randomChoose = Random.Range(0, 2);

            switch (randomChoose)
            {
                case 0:
                    positionChoose = GoRandomPoint("allie");
                    break;
                case 1:
                    positionChoose = GoRandomPoint("enemy");
                    break;
                default:
                    positionChoose = GoRandomPoint("all");
                    break;
            }
        }
        //isGoingToDestination = true;
        //pointGoing = bot.agent.destination;
        //Debug.Log("je suis supposer avancer");
        return positionChoose;
    }

    //private Vector3 GoToHome(Vector3[] tableauARemonter)
    //{
    //    Vector3 objectifTemp = new Vector3(-1,-1,-1);
    //    if (previousState != BotState.GO_HOME && previousState != BotState.SHOOT)
    //    {
    //        objectifTemp = NearestPlace(transform.position, tableauARemonter, true);
    //        positionActuelHome = FindIndex(tableauARemonter, objectifTemp);
    //    }
    //    else
    //    {
    //        //Debug.Log("Je devrais allez vers la/le prochaine/prochain " + directionHome);
    //        if (positionActuelHome <= 0)
    //        {
    //            objectifTemp = botTeam.Places.GetPlacePosition(KeyPlaces.FLAG);
    //            Debug.Log("je vais a mon drapeau");
    //        }
    //        else if (positionActuelHome >= tableauARemonter.Length)
    //        {
    //            objectifTemp = NearestPlace(transform.position, tableauARemonter, true);
    //            positionActuelHome = FindIndex(tableauARemonter, objectifTemp);
    //            Debug.Log("je vais au point le plus proche");

    //        }
    //        else
    //        {
    //            positionActuelHome++;
    //            objectifTemp = tableauARemonter[positionActuelHome];
    //            Debug.Log("je vais au prochain point");
    //        }
    //    }

    //    //ActualTarget = objectifTemp;
    //    //Debug.LogWarning("le tableau choisit est de la taille " + tableauARemonter.Length + ". le bot " + bot.ID + " se dirige vers " + objectifTemp + " au lieu de " + tableauARemonter[positionActuelHome] + " ayant pour indice " + positionActuelHome);
    //    //SetPosition(objectifTemp);
    //    return objectifTemp;
    //}

    private int FindIndex(Vector3[] tableauARemonter, Vector3 vecteurATrouver)
    {
        int indiceARenvoyer = -1;
        for (int i = 0; i < tableauARemonter.Length; ++i)
        {
            if (vecteurATrouver == tableauARemonter[i])
            {
                indiceARenvoyer = i;
            }
        }
        return indiceARenvoyer;
    }
}
