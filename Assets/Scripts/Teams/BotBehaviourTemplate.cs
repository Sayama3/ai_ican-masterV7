using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotBehaviourTemplate : BotBehaviour
{

    public static int valueIncremental = 0;
    public int myValue = 0;
    
    public enum BotState{ 
        IDLE,
    };
  
    // état actuel du bot
    public BotState state = BotState.IDLE;
    // remplacer TeamBehaviourTemplate par le nom du script de votre team
    public TeamBehaviourTemplate teamBehaviour;
    
    
    // ne pas modifier, seulement remplacer TeamBehaviourTemplate par le nom du script de votre team
    public override void Init(GameMaster master, Bot bot)
    {
        base.Init(master, bot);
        this.teamBehaviour = FindObjectOfType<TeamBehaviourTemplate>();
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
        this.state = newState;
        this.OnEnterState();
    }

    // ajouter ici les actions à effectuer quand le bot change d'etat'
    protected void OnEnterState() {
        switch(state)
        {
            case BotState.IDLE:
                break;

        }
    }

    // ajouter ici les action a effecteur quand le bot est dans un etat
    protected void UpdateState() {
        switch (state) {
           case BotState.IDLE:
                break;
        }
    }

    // ajouter ici les action a effecteur quand le bot sort d'un etat
    protected void OnExitState() {
        switch(state)
        {
            case BotState.IDLE:
                break;

        }
    }

    public override void OnRespawn()
    {
        Debug.Log(" The bot " + this.botId + " respawns !");
    }

    public override void TakeDamage()
    {
        Debug.Log(" The bot " + this.botId + " takes damage !");
    }

    public override void OnDeath()
    {
        Debug.Log(" The bot " + this.botId + " dies !");
    }
    
}
