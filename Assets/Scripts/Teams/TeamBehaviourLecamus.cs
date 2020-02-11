using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamBehaviourLecamus : TeamBehaviour
{
    public BotBehaviourLecamus[] myBots; // pour éviter d'avoir à chercher les components qui ont le code spécifique de mes bots
    public GameObject closestBot;
    public Vector3 cible;
    public bool enemyFlagIsAtOurBase;
    public Vector3 EnemyFlagPosition;
    public Vector3 OurFlagPosition;

    public override void RegisterBot(Bot[] bots)
    {
        this.myBots = new BotBehaviourLecamus[bots.Length];
        for (int i = 0; i < this.myBots.Length; i++)
        {
            this.myBots[i] = bots[i].GetComponent<BotBehaviourLecamus>();
        }
    }
    public override void OnMatchStart()
    {
        ObjectifsBots();
    }
    public void ObjectifsBots()
    {
        for (int i = 0; i < myBots.Length; i++) //chaque bot a un role différent au début de la partie
        {
            switch (i)
            {
                case 0:
                    myBots[i].SwitchState(BotBehaviourLecamus.BotState.POINT_BASE);
                    break;
                case 1:
                    myBots[i].SwitchState(BotBehaviourLecamus.BotState.PATROL);
                    break;
                case 2:
                    myBots[i].SwitchState(BotBehaviourLecamus.BotState.PATROL);
                    break;
                case 3:
                    myBots[i].SwitchState(BotBehaviourLecamus.BotState.PATROL);
                    break;
                case 4:
                    myBots[i].SwitchState(BotBehaviourLecamus.BotState.PATROL);
                    break;
            }
        }
    }
    public void FindClosestBot(Bot[] bots, Vector3 Place) //Fonction qui trouve et renvoie le bot le plus proche de l'endroit voulu
    {
        float distanceToClosestbot = Mathf.Infinity;
        closestBot = null;

        foreach (Bot b in bots)
        {
            float distanceToBot = (Place - b.transform.position).sqrMagnitude;
            if (distanceToBot < distanceToClosestbot)
            {
                distanceToClosestbot = distanceToBot;
                closestBot = b.gameObject;
            }
        }
    }

}