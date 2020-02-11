using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotBehaviourSerera : BotBehaviour {

    struct SuperPos {
        public Vector3 posA;
    }
    
    
    private Vector3 mySuperPos;
    
    public enum BotType {
        Def,
        Atk,
        Gol
    }

    public BotType botMode = BotType.Gol;
    
    // liste des states possibles pour ce comportement de bot
    public enum BotState{ 
        IDLE,
        GO_PATROL
    };
  
    // état actuel du bot
    public BotState state = BotState.IDLE;
    public TeamBehaviourSerera teamBehaviour;
    
    public override void Init(GameMaster master, Bot bot)
    {
        base.Init(master, bot);
        this.teamBehaviour = FindObjectOfType<TeamBehaviourSerera>();

        this.mySuperPos = this.enemyTeam.Places.GetPlacePosition(KeyPlaces.CAMPER) + new Vector3(0, 0, -15);

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
            case BotState.GO_PATROL:
                this.SelectPAtrolRole();
                break;
            case BotState.IDLE:
                break;

        }
    }

    public void SelectPAtrolRole() {
        switch (this.botMode) {
            case BotType.Def:
                this.bot.agent.SetDestination(this.botTeam.Places.GetPlacePosition(KeyPlaces.FLAG));
                break;
            case BotType.Gol:
                this.bot.agent.SetDestination(this.mySuperPos);
                break;
            case BotType.Atk:
                this.bot.agent.SetDestination(this.enemyTeam.Places.GetPlacePosition(KeyPlaces.FLAG));
                break;
        }
    }

    protected void UpdateState() {
        switch (state) 
        {
            case BotState.GO_PATROL:
                if (this.bot.agent.remainingDistance < 0.5f) {
                    SwitchState(BotState.IDLE);
                    this.teamBehaviour.GoToEnemyBOtISee(this.transform.position);
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
