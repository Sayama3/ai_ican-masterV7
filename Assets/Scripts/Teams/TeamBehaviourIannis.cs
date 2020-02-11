using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamBehaviourIannis : TeamBehaviour
{
    //Tableaux de position
    public static Vector3[] positionMillieu = new Vector3[5];
    public static Vector3[] positionGauche = new Vector3[4];
    public static Vector3[] positionDroite = new Vector3[4];
    public static Vector3[] basePosition = new Vector3[4];
    public static Vector3 positionDeMonBotQuiALeDrapeau = new Vector3();


    //remplacer par le nom du script de vos bots
    public BotBehaviourIannis[] myBots; // pour éviter d'avoir à chercher les components qui ont le code spécifique de mes bots

    //ne pas modifier sauf pour remplacer les script par le nom du script de vos bots
    public override void RegisterBot(Bot[] bots) {
        this.myBots = new BotBehaviourIannis[bots.Length];
        for (int i = 0; i < this.myBots.Length; i++) {
            this.myBots[i] = bots[i].GetComponent<BotBehaviourIannis>();
        }
    }

    // call back automatiquement appelé au demarrage d'un round
    public override void OnMatchStart() {
        //attribution des valuer au tableau du milieu
        for (int i = 0; i < positionMillieu.Length; i++)
        {
            switch (i)
            {
                case 0:
                    positionMillieu[i] = this.team.Places.GetPlacePosition(KeyPlaces.FLAG);
                    break;
                case 1:
                    positionMillieu[i] = this.team.Places.GetPlacePosition(KeyPlaces.FRONT);
                    break;
                case 2:
                    positionMillieu[i] = this.team.Places.GetPlacePosition(KeyPlaces.CENTER);
                    break;
                case 3:
                    positionMillieu[i] = enemyTeam.Places.GetPlacePosition(KeyPlaces.FRONT);
                    break;
                case 4:
                    positionMillieu[i] = enemyTeam.Places.GetPlacePosition(KeyPlaces.FLAG);
                    break;
                default:
                    break;
            }
        }

        //attribution des valuer au tableau de gauche
        for (int i = 0; i < positionGauche.Length; i++)
        {
            switch (i)
            {
                case 0:
                    positionGauche[i] = this.team.Places.GetPlacePosition(KeyPlaces.SPAWN);
                    break;
                case 1:
                    positionGauche[i] = this.team.Places.GetPlacePosition(KeyPlaces.CAMPER);
                    break;
                case 2:
                    positionGauche[i] = enemyTeam.Places.GetPlacePosition(KeyPlaces.POWER_UP);
                    break;
                case 3:
                    positionGauche[i] = enemyTeam.Places.GetPlacePosition(KeyPlaces.PYLON);
                    break;
                default:
                    break;
            }
        }

        //attribution des valuer au tableau de droite
        for (int i = 0; i < positionDroite.Length; i++)
        {
            switch (i)
            {
                case 0:
                    positionDroite[i] = this.team.Places.GetPlacePosition(KeyPlaces.PYLON);
                    break;
                case 1:
                    positionDroite[i] = this.team.Places.GetPlacePosition(KeyPlaces.POWER_UP);
                    break;
                case 2:
                    positionDroite[i] = enemyTeam.Places.GetPlacePosition(KeyPlaces.CAMPER);
                    break;
                case 3:
                    positionDroite[i] = enemyTeam.Places.GetPlacePosition(KeyPlaces.SPAWN);
                    break;
                default:
                    break;
            }
        }

        //atribution des valeur au tableu de la base
        for (int i = 0; i < basePosition.Length; i++)
        {
            //float ajoutTemp = 1.5f;
            //basePosition[i]
            switch (i)
            {
                case 0:
                    basePosition[i] = this.team.Places.GetPlacePosition(KeyPlaces.FLAG);
                    break;
                case 1:
                    basePosition[i] = this.team.Places.GetPlacePosition(KeyPlaces.PYLON);
                    break;
                case 2:
                    basePosition[i] = this.team.Places.GetPlacePosition(KeyPlaces.FRONT);
                    break;
                case 3:
                    basePosition[i] = this.team.Places.GetPlacePosition(KeyPlaces.SPAWN)/* + new Vector3(0, 0, 14)*/;
                    break;
                
                case 4:
                    basePosition[i] = this.team.Places.GetPlacePosition(KeyPlaces.PYLON) + new Vector3(-10, 0, 8);
                    break;
                case 5:
                    basePosition[i] = this.team.Places.GetPlacePosition(KeyPlaces.SPAWN) + new Vector3(8, 0, 0);
                    break;
                

            }
        }

        //Atrivution des différents States aux bots
        for (int i = 0; i < myBots.Length; i++)
        {
            myBots[i].SwitchState(BotBehaviourIannis.BotState.IDLE);
            if (i < 2)
            {
                myBots[i].role = BotBehaviourIannis.BotRole.Attack;
            }
            else if (i < 4)
            {
                myBots[i].role = BotBehaviourIannis.BotRole.Defend;
            }
            else
            {
                myBots[i].role = BotBehaviourIannis.BotRole.Attack;
            }
            //switch (i)
            //{
            //    case 0:

            //        myBots[i].SwitchState(BotBehaviourIannis.BotState.IDLE);
            //        break;
            //    case 1:
            //        myBots[i].SwitchState(BotBehaviourIannis.BotState.IDLE);
            //        break;
            //    case 2:
            //        myBots[i].SwitchState(BotBehaviourIannis.BotState.IDLE);
            //        break;
            //    case 3:
            //        myBots[i].SwitchState(BotBehaviourIannis.BotState.IDLE);
            //        break;
            //    case 4:
            //        myBots[i].SwitchState(BotBehaviourIannis.BotState.IDLE);
            //        break;
            //    default:
            //        break;
            //}

        }

    }


    // call back automatiquement appelé quand un drapeau est volé (volé à la base ou rammasé au sol par l'équipe adverse)
    public override void OnFlagStolen(Team teamStolen)
    {
        
    }
    
    // call back automatiquement appelé quand un drapeau est sauvé 
    public override void OnFlagSaved(Team teamSaved)
    {
        
    }

    
    // call back automatiquement appelé quand un point est marqué
    public override void OnTeamScored(Team teamScored)
    {
        
    }

}
