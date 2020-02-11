using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamBehaviourTemplate : TeamBehaviour
{
    
    //remplacer par le nom du script de vos bots
    public BotBehaviourTemplate[] myBots; // pour éviter d'avoir à chercher les components qui ont le code spécifique de mes bots

    //ne pas modifier sauf pour remplacer les script par le nom du script de vos bots
    public override void RegisterBot(Bot[] bots) {
        this.myBots = new BotBehaviourTemplate[bots.Length];
        for (int i = 0; i < this.myBots.Length; i++) {
            this.myBots[i] = bots[i].GetComponent<BotBehaviourTemplate>();
        }
    }

    // call back automatiquement appelé au demarrage d'un round
    public override void OnMatchStart() {
        
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
